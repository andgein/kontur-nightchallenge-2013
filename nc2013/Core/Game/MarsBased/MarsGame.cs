using System.IO;
using System.Linq;
using JetBrains.Annotations;
using nMars.Parser.Warrior;
using nMars.RedCode;

namespace Core.Game.MarsBased
{
	public class MarsGame : IGame
	{
		private readonly Rules rules;
		private readonly ProgramStartInfo[] programStartInfos;
		private GameState gameState;
		private int currentTurn;

		public MarsGame([NotNull] Rules rules, [NotNull] ProgramStartInfo[] programStartInfos)
		{
			this.rules = rules;
			this.programStartInfos = programStartInfos;
			var engine = CreateEngine();
			engine.Run(0, out currentTurn);
			this.programStartInfos = programStartInfos.Select((pi, idx) => new ProgramStartInfo
			{
				Program = pi.Program,
				StartAddress = pi.StartAddress.HasValue ? pi.StartAddress.Value : (uint)engine.warriors[idx].LoadAddress,
			}).ToArray();
		}

		[NotNull]
		public GameState GameState
		{
			get { return gameState ?? (gameState = GetGameState(0)); }
		}

		[CanBeNull]
		public Diff Step(int stepCount)
		{
			currentTurn += stepCount;
			if (currentTurn < 0)
				currentTurn = 0;
			gameState = GetGameState(currentTurn);
			return null; // todo !!!
		}

		public void StepToEnd()
		{
			var stepsToEnd = rules.MaxCycles * 2 - currentTurn;
			if (stepsToEnd > 0)
				Step(stepsToEnd);
		}

		[NotNull]
		private GameState GetGameState(int turnsToMake)
		{
			var engine = CreateEngine();
			var finished = engine.Run(turnsToMake, out currentTurn);

			var currentProgram = turnsToMake % engine.WarriorsCount;

			var winner = !finished || engine.LiveWarriorsCount != 1 ? (int?)null : engine.LiveWarriors.Single().WarriorIndex;

			var memoryState = new CellState[engine.CoreSize];
			for (var addr = 0; addr < memoryState.Length; addr++)
			{
				var instruction = engine.core[addr].ToString();
				var cellType = engine.core[addr].Operation == Operation.DAT ? CellType.Data : CellType.Command;
				var lastModifiedByProgram = engine.core[addr].OriginalOwner == null ? (int?)null : engine.core[addr].OriginalOwner.WarriorIndex;
				memoryState[addr] = new CellState
				{
					Instruction = instruction,
					CellType = cellType,
					LastModifiedByProgram = lastModifiedByProgram,
				};
			}

			var programStates = new ProgramState[engine.WarriorsCount];
			for (var idx = 0; idx < programStates.Length; idx++)
			{
				var w = engine.warriors[idx];
				var lastPointer = currentTurn <= idx ? (uint?)null : (uint)w.PrevInstruction.Address;
				var processPointers = w.Tasks.Select(ip => (uint)ip).ToArray();
				programStates[idx] = new ProgramState
				{
					LastPointer = lastPointer,
					ProcessPointers = processPointers,
				};
			}

			return new GameState
			{
				ProgramStartInfos = programStartInfos,
				CurrentStep = currentTurn,
				CurrentProgram = currentProgram,
				Winner = winner,
				GameOver = finished,
				MemoryState = memoryState,
				ProgramStates = programStates,
			};
		}

		[NotNull]
		private MarsEngine CreateEngine()
		{
			var warriors = programStartInfos.Select((pi, idx) => ParseWarrior(pi, string.Format("w{0}", idx))).ToArray();
			var project = new MarsProject(rules, warriors);
			return new MarsEngine(project);
		}

		[NotNull]
		private ExtendedWarrior ParseWarrior([NotNull] ProgramStartInfo programStartInfo, [NotNull] string filename)
		{
			var warriorParser = new MarsWarriorParser(rules);
			var implicitName = Path.GetFileNameWithoutExtension(filename);
			var warrior = warriorParser.TryParse(programStartInfo.Program, implicitName);
			if (warrior == null)
				throw new WarriorProgramParserException(string.Format("Failed to parse warrior {0} [{1}]: {2}", implicitName, warriorParser.GetErrorMessages(), programStartInfo));
			warrior.FileName = filename;
			warrior.PredefinedLoadAddress = (int?)programStartInfo.StartAddress;
			return warrior;
		}
	}
}