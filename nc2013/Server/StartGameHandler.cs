namespace Server
{
	public class StartGameHandler : GameFuncHandlerBase<ProgramStartInfo[], GameState>
	{
		private readonly GameHttpServer gameHttpServer;

		public StartGameHandler(GameHttpServer gameHttpServer) : base("/corewars/start")
		{
			this.gameHttpServer = gameHttpServer;
		}

		protected override GameState Handle(ProgramStartInfo[] request)
		{
			var gameId = gameHttpServer.StartNewGame(request);
			return GameState.FromCore(gameId, gameHttpServer.GetGame(gameId).GameState);
		}
	}
}