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
			};
		}

		[NotNull]
		public GameState GameState
		{
			get { return gameState ?? (gameState = GetGameState(0)); }
		}

		[NotNull]
		public Diff Step(int stepCount)
		{
			currentTurn += stepCount;
			if (currentTurn < 0)
				throw new InvalidOperationException(string.Format("Cannot rewind game state behind 0 turn. StepCount: {0}", stepCount));
			gameState = GetGameState(currentTurn);
			return null;
		}

		[NotNull]
		private GameState GetGameState(int turnsToMake)
		{
			//todo !!! account for start pi.StartAddress
			var warriors = programStartInfos.Select((pi, idx) => ParseWarrior(pi.Program, string.Format("w{0}", idx))).ToArray();
			var project = new MarsProject(rules, warriors);
			var engine = new MarsEngine(project);
			engine.Run(turnsToMake);
			var currentProgram = turnsToMake % project.Warriors.Count;
			return new GameState
			{
				CurrentProgram = currentProgram,
				MemoryState = new CellState[0],
				ProgramStates = new ProgramState[0],
			};
		}

		[NotNull]
		private ExtendedWarrior ParseWarrior([NotNull] string source, [NotNull] string filename)
		{
			var warriorParser = new MarsWarriorParser(rules);
			var implicitName = Path.GetFileNameWithoutExtension(filename);
			var warrior = warriorParser.Parse(source, implicitName);
			if (warrior == null)
				throw new InvalidOperationException(string.Format("Failed to parse warrior {0}: {1}", implicitName, source));
			warrior.FileName = filename;
			return warrior;
		}
	}
}