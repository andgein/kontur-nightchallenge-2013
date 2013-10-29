using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using nMars.Parser.Warrior;
using nMars.RedCode;

namespace Core.Game.MarsBased
{
	public class MarsGame : IGame
	{
		private readonly ProgramStartInfo[] programStartInfos;
		private readonly Rules rules;
		private GameState gameState;
		private int currentTurn;

		public MarsGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			this.programStartInfos = programStartInfos;
			rules = new Rules
			{
				WarriorsCount = programStartInfos.Length,
				Rounds = 1,
				MaxCycles = 80000,
				CoreSize = 8000,
				PSpaceSize = 0,
				EnablePSpace = false,
				MaxProcesses = 1000,
				MaxLength = 100,
				MinDistance = 100,
				Version = 93,
				ScoreFormula = ScoreFormula.Standard,
				ICWSStandard = ICWStandard.ICWS88,
			};
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
				throw new InvalidOperationException(string.Format("Cannot rewind game state behind 0 turn. StepCount: {0}", stepCount));
			gameState = GetGameState(currentTurn);
			return null; // todo !!!
		}

		public void StepToEnd()
		{
			var stepsToEnd = rules.MaxCycles - currentTurn;
			if (stepsToEnd > 0)
				Step(stepsToEnd);
		}

		[NotNull]
		private GameState GetGameState(int turnsToMake)
		{
			var engine = CreateEngine();
			var finished = engine.Run(turnsToMake);

			var startInfos = programStartInfos.Select((pi, idx) => new ProgramStartInfo
			{
				Program = pi.Program,
				StartAddress = (uint)engine.warriors[idx].LoadAddress,
			}).ToArray();

			var currentProgram = turnsToMake % engine.WarriorsCount;

			var winner = !finished || engine.LiveWarriorsCount != 1 ? (int?)null : engine.LiveWarriors.Single().WarriorIndex;

			var memoryState = new CellState[engine.CoreSize];
			for (var addr = 0; addr < memoryState.Length; addr++)
			{
				memoryState[addr] = new CellState
				{
					Instruction = engine.core[addr].ToString(),
					CellType = engine.core[addr].Operation == Operation.DAT ? CellType.Data : CellType.Command,
					LastModifiedByProgram = engine.core[addr].OriginalOwner.WarriorIndex,
					LastModifiedStep = null, // todo !!!
				};
			}

			var programStates = new ProgramState[engine.WarriorsCount];
			for (var idx = 0; idx < programStates.Length; idx++)
			{
				var w = engine.warriors[idx];
				programStates[idx] = new ProgramState
				{
					LastPointer = currentTurn <= idx ? (uint?)null : (uint)w.PrevInstruction.Address,
					ProcessPointers = w.Tasks.Select(ip => (uint)ip).ToArray(),
				};
			}

			return new GameState
			{
				ProgramStartInfos = startInfos,
				CurrentStep = currentTurn,
				CurrentProgram = currentProgram,
				Winner = winner,
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
			var warrior = warriorParser.Parse(programStartInfo.Program, implicitName);
			if (warrior == null)
				throw new InvalidOperationException(string.Format("Failed to parse warrior {0}: {1}", implicitName, programStartInfo));
			warrior.FileName = filename;
			warrior.PredefinedLoadAddress = (int?)programStartInfo.StartAddress;
			return warrior;
		}
	}
}