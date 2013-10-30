using JetBrains.Annotations;

namespace Core.Game
{
	public class StupidGameServer : IGameServer
	{
		[NotNull]
		public IGame StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			return new StupidGame(programStartInfos);
		}

		[NotNull]
		public IGame ResumeGame([NotNull] GameState gameState)
		{
			return new StupidGame(gameState);
		}
	}
}