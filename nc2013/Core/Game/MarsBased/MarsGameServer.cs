using JetBrains.Annotations;

namespace Core.Game.MarsBased
{
	public class MarsGameServer : IGameServer
	{
		[NotNull]
		public IGame StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			return new MarsGame(programStartInfos);
		}

		[NotNull]
		public IGame ResumeGame([NotNull] GameState gameState)
		{
			return new MarsGame(gameState);
		}
	}
}