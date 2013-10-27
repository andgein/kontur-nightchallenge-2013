using System.Net;
using Server.DataContracts;

namespace Server.Handlers
{
	public class GetGameStateHandler : StrictPathHttpHandlerBase
	{
		private readonly GameHttpServer gameHttpServer;

		public GetGameStateHandler(GameHttpServer gameHttpServer) : base("state")
		{
			this.gameHttpServer = gameHttpServer;
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var gameId = GetGameId(context);
			var game = gameHttpServer.GetGame(gameId);
			var gameState = GameState.FromCore(gameId, game.GameState);
			SendResponse(context, gameState);
		}
	}
}