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

		public MarsGame([NotNull] GameState gameState)
		{
			this.gameState = gameState;
			// todo - resume game
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
			return null;
		}

		public void StepToEnd()
		{
			if (currentTurn < 80000) // todo !!! winner detection and this limit
				Step(80000 - currentTurn);
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

			var memoryState = new CellState[engine.CoreSize];
			for (var p = 0; p < memoryState.Length; p++)
			{
				memoryState[p] = new CellState
				{
					Command = engine.core[p].ToString(),
					ArgA = engine.core[p].ValueA.ToString(),
					ArgB = engine.core[p].ValueB.ToString(),
					LastModifiedByProgram = engine.core[p].OriginalOwner.WarriorIndex,
					LastModifiedStep = -1, // todo !!!
				};
			}

			var programStates = new ProgramState[0]; // todo !!!

			return new GameState
			{
				CurrentProgram = currentProgram,
				MemoryState = memoryState,
				ProgramStates = programStates,
				CurrentStep = currentTurn,
				Winner = null, // todo !!!
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