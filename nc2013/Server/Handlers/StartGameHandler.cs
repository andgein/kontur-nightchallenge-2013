using System.Net;
using Server.DataContracts;

namespace Server.Handlers
{
	public class StartGameHandler : StrictPathHttpHandlerBase
	{
		private readonly GameHttpServer gameHttpServer;

		public StartGameHandler(GameHttpServer gameHttpServer) : base("start")
		{
			this.gameHttpServer = gameHttpServer;
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var programStartInfos = GetRequest<ProgramStartInfo[]>(context);
			var gameId = gameHttpServer.StartNewGame(programStartInfos);
			SendResponseRaw(context, gameId);
		}
	}
}