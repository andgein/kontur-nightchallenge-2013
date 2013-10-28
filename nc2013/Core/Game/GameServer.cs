using JetBrains.Annotations;

namespace Core.Game
{
	public class GameServer
	{
		[NotNull]
		public IGame StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			return new StupidGame(programStartInfos);
		}
	}

}