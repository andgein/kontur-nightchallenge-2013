using NUnit.Framework;

namespace Core.Game
{
	[TestFixture]
	public class Game_Test
	{
		[Test]
		public void ProgramStateDiff_Splitted()
		{
			var game = new Game(new[]
			{
				new ProgramStartInfo
				{
					Program = "SPL 0",
					StartAddress = 0
				}
			});
			var gameStepResult = game.Step(1);
			Assert.IsNotNull(gameStepResult.Diff);
			Assert.That(gameStepResult.Diff.CurrentStep, Is.EqualTo(1));
			Assert.That(gameStepResult.Diff.CurrentProgram, Is.EqualTo(0));
			Assert.That(gameStepResult.Diff.GameOver, Is.False);
			Assert.That(gameStepResult.Diff.Winner, Is.Null);
			Assert.That(gameStepResult.Diff.MemoryDiffs, Is.Empty);
			Assert.That(gameStepResult.Diff.ProgramStateDiffs, Is.EqualTo(new[]
			{
				new ProgramStateDiff {ChangeType = ProcessStateChangeType.Executed, NextPointer = 1, Program = 0},
				new ProgramStateDiff {ChangeType = ProcessStateChangeType.Splitted, NextPointer = 0, Program = 0}
			}));
		}

		[Test]
		public void ProgramStateDiff_Executed()
		{
			var game = new Game(new[]
			{
				new ProgramStartInfo
				{
					Program = "MOV 0, 0",
					StartAddress = 0
				}
			});
			var gameStepResult = game.Step(1);
			Assert.IsNotNull(gameStepResult.Diff);
			Assert.That(gameStepResult.Diff.CurrentStep, Is.EqualTo(1));
			Assert.That(gameStepResult.Diff.CurrentProgram, Is.EqualTo(0));
			Assert.That(gameStepResult.Diff.GameOver, Is.False);
			Assert.That(gameStepResult.Diff.Winner, Is.Null);
			Assert.That(gameStepResult.Diff.MemoryDiffs, Is.EqualTo(new[]
			{
				new MemoryDiff
				{
					Address = 0,
					CellState = new CellState
					{
						CellType = CellType.Command,
						Instruction = "MOV $0, $0",
						LastModifiedByProgram = 0
					}
				}
			}));
			Assert.That(gameStepResult.Diff.ProgramStateDiffs, Is.EqualTo(new[]
			{
				new ProgramStateDiff {ChangeType = ProcessStateChangeType.Executed, NextPointer = 1, Program = 0}
			}));
		}

		[Test]
		public void ProgramStateDiff_Killed()
		{
			var game = new Game(new[]
			{
				new ProgramStartInfo
				{
					Program = "DAT #0, #0",
					StartAddress = 0
				}
			});
			var gameStepResult = game.Step(1);
			Assert.IsNotNull(gameStepResult.Diff);
			Assert.That(gameStepResult.Diff.CurrentStep, Is.EqualTo(1));
			Assert.That(gameStepResult.Diff.CurrentProgram, Is.EqualTo(0));
			Assert.That(gameStepResult.Diff.GameOver, Is.True);
			Assert.That(gameStepResult.Diff.Winner, Is.Null);
			Assert.That(gameStepResult.Diff.MemoryDiffs, Is.Empty);
			Assert.That(gameStepResult.Diff.ProgramStateDiffs, Is.EqualTo(new[]
			{
				new ProgramStateDiff {ChangeType = ProcessStateChangeType.Killed, NextPointer = null, Program = 0}
			}));
		}
	}
}