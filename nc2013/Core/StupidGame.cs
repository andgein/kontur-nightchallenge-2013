﻿using System;
using System.Linq;
using NUnit.Framework;

namespace Core
{
	public class StupidGame : Game
	{
		private readonly Random r = new Random(12344);
		private GameState gameState;

		public StupidGame(ProgramStartInfo[] programStartInfos) : base(programStartInfos)
		{
			gameState = new GameState();
			gameState.CurrentProgram = 0;
			gameState.MemoryState = Enumerable.Range(0, 8000).Select(i => CreateRandomCommand()).ToArray();
			gameState.ProgramStates = programStartInfos.Select((p, i) => new ProgramState { ProcessPointers = new[] { (uint)(i * 1000), (uint)(i * 1000 + 100) } }).ToArray();
		}

		public override Diff Step(int stepCount)
		{
			var res = new Diff();
			currentStep = Math.Max(0, Math.Min(80000, currentStep + stepCount));
			stepCount = Math.Abs(stepCount);
			res.MemoryDiffs = Enumerable.Range(0, stepCount).Select(i => RandomMemDiff()).ToArray();
			res.ProgramStateDiffs = Enumerable.Range(0, stepCount).Select(RandomProgramStateDiff).ToArray();
			return res;

		}

		public override GameState GameState { get { return gameState; } }

		private ProgramStateDiff RandomProgramStateDiff(int i)
		{
			return new ProgramStateDiff
				{
					ChangeType = ProcessStateChangeType.Executed,
					Program = i
				};
		}

		private MemoryDiff RandomMemDiff()
		{
			return new MemoryDiff{Address = (uint)r.Next(8000), CellState = CreateRandomCommand()};
		}

		private CellState CreateRandomCommand()
		{
			return new CellState
				{
					Command = "nope",
					ArgA = "12",
					ArgB = "23",
					LastModifiedByProgram = r.Next(programStartInfos.Length),
					LastModifiedStep = r.Next(currentStep)
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