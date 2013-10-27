using System.Net;
using Server.DataContracts;

namespace Server.Handlers
{
	public class StepHandler : GameHandlerBase
	{
		private readonly GameHttpServer gameHttpServer;
		private const int memoryDiffsLimit = 1000;
		private const int programStateDiffsLimit = 1000;

		public StepHandler(GameHttpServer gameHttpServer) : base("step")
		{
			this.gameHttpServer = gameHttpServer;
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var gameId = GetGameId(context);
			var stepCount = GetOptionalIntParam(context, "count") ?? 1;
			var game = gameHttpServer.GetGame(gameId);
			var diff = game.Step(stepCount);
			Diff response;
			if (diff.MemoryDiffs.Length > memoryDiffsLimit || diff.ProgramStateDiffs.Length > programStateDiffsLimit)
				response = new Diff {GameState = GameState.FromCore(gameId, game.GameState)};
			else
				response = Diff.FromCore(diff);
			SendResponse(context, response);
		}
	}
}