using System.Net;
using Server.DataContracts;

namespace Server.Handlers
{
	public class StepToEndHandler : StrictPathHttpHandlerBase
	{
		private readonly GameHttpServer gameHttpServer;

		public StepToEndHandler(GameHttpServer gameHttpServer) : base("step/end")
		{
			this.gameHttpServer = gameHttpServer;
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var gameId = GetGameId(context);
			var game = gameHttpServer.GetGame(gameId);
			game.StepToEnd();
			var gameState = GameState.FromCore(gameId, game.GameState);
			SendResponse(context, gameState);
		}
	}
}