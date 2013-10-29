using System.Net;
using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStartGameHandler : DebuggerHandlerBase
	{
		public DebuggerStartGameHandler([NotNull] ISessionManager sessionManager) : base("debugger/start", sessionManager) {}

		protected override void DoHandle([NotNull] HttpListenerContext context, [NotNull] IDebugger debugger)
		{
			var programStartInfos = context.GetRequest<ProgramStartInfo[]>();
			debugger.StartNewGame(programStartInfos);
			context.SendResponse(debugger.GameState);
		}
	}
}