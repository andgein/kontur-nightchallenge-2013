using System;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Core.Game
{
	public class StupidGame : IGame
	{
		private readonly Random r = new Random(12344);
		private readonly GameState gameState;

		public StupidGame([NotNull] GameState gameState)
		{
			this.gameState = gameState;
		}

		public StupidGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			gameState = new GameState
			{
				ProgramStartInfos = programStartInfos,
				CurrentStep = 0,
				CurrentProgram = 0,
				ProgramStates = programStartInfos.Select((p, i) => new ProgramState {ProcessPointers = new[] {(uint) (i*1000), (uint) (i*1000 + 100)}}).ToArray(),
			};
			gameState.MemoryState = Enumerable.Range(0, 8000).Select(i => CreateRandomCommand()).ToArray();
		}

		[NotNull]
		public GameState GameState
		{
			get { return gameState; }
		}

		[CanBeNull]
		public Diff Step(int stepCount)
		{
			var res = new Diff();
			gameState.CurrentStep += stepCount;
			stepCount = Math.Abs(stepCount);
			res.MemoryDiffs = Enumerable.Range(0, stepCount).Select(i => RandomMemDiff()).ToArray();
			res.ProgramStateDiffs = Enumerable.Range(0, stepCount).Select(RandomProgramStateDiff).ToArray();
			res.CurrentStep = gameState.CurrentStep;
			if (gameState.CurrentStep >= 80000)
			{
				gameState.Winner = r.Next(gameState.ProgramStates.Length);
				res.Winner = gameState.Winner;
			}
			return res;
		}

		public void StepToEnd()
		{
			Step(80000);
		}

		private ProgramStateDiff RandomProgramStateDiff(int i)
		{
			return new ProgramStateDiff
			{
				ChangeType = ProcessStateChangeType.Executed,
				Program = r.Next(gameState.ProgramStates.Length)
			};
		}

		private MemoryDiff RandomMemDiff()
		{
			return new MemoryDiff {Address = (uint) r.Next(8000), CellState = CreateRandomCommand()};
		}

		private CellState CreateRandomCommand()
		{
			return new CellState
			{
				Instruction = "nope 12 23",
				CellType = r.Next() % 2 == 0 ? CellType.Data : CellType.Command,
				LastModifiedByProgram = r.Next(gameState.ProgramStates.Length),
				LastModifiedStep = r.Next(gameState.CurrentStep),
			};
		}
	}

	[TestFixture]
	public class StupidGame_Test
	{
		[Test]
		public void Test()
		{
			var game = new StupidGame(new[] {new ProgramStartInfo {Program = "JMP 0 0", StartAddress = 0}});
			game.Step(1);
			game.Step(-1);
			game.Step(2);
			game.Step(-2);
			game.Step(80000);
			game.Step(-80000);
		}
	}
}