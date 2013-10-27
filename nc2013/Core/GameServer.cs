namespace Core
{
	public class GameServer
	{
		public Game StartNewGame(ProgramStartInfo[] programStartInfos)
		{
			return new StupidGame(programStartInfos);
		}
	}

}