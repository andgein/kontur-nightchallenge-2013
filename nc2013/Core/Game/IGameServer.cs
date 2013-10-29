using JetBrains.Annotations;

namespace Core.Game
{
	public interface IGameServer
	{
		[NotNull]
		IGame StartNewGame([NotNull] ProgramStartInfo[] programStartInfos);

		[NotNull]
		IGame ResumeGame([NotNull] GameState gameState);
	}
}