using NUnit.Framework;

namespace Core.Game
{
	[TestFixture]
	public class Game_Test
	{
		[Test]
		public void Test()
		{
			var game = new Game(new[] {new ProgramStartInfo
			{
				Program = "SPL 0\r\nMOV 0, 1",
				StartAddress = 0
			}});
			var diff = game.Step(1);
			Assert.IsNotNull(diff);
			Assert.That(diff.CurrentStep, Is.EqualTo(1));
			Assert.That(diff.CurrentProgram, Is.EqualTo(0));
			Assert.That(diff.GameOver, Is.False);
			Assert.That(diff.Winner, Is.Null);
			Assert.That(diff.MemoryDiffs, Is.Empty);
			Assert.That(diff.ProgramStateDiffs, Is.EqualTo(new[]
			{
				new ProgramStateDiff{ChangeType = ProcessStateChangeType.Executed, NextPointer = 1, Program = 0},
				new ProgramStateDiff{ChangeType = ProcessStateChangeType.Splitted, NextPointer = 0, Program = 0}
			}));
		}
	}
}