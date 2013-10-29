using System.Net;
using Core.Game;
using JetBrains.Annotations;
using Server.DataContracts;

namespace Server.Handlers
{
	public class StepHandler : StrictPathHttpHandlerBase
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
			var response = new StepResponse();
			if (diff == null || DiffIsTooBig(diff))
				response.GameState = game.GameState;
			else
				response.Diff = diff;
			SendResponse(context, response);
		}

		private bool DiffIsTooBig([NotNull] Diff diff)
		{
			return (diff.MemoryDiffs != null && diff.MemoryDiffs.Length > memoryDiffsLimit)
				|| (diff.ProgramStateDiffs != null && diff.ProgramStateDiffs.Length > programStateDiffsLimit);
		}
	}
}