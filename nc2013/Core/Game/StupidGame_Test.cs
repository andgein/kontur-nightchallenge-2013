using NUnit.Framework;

namespace Core.Game
{
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