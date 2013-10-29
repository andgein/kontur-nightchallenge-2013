using System.Net;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerGameStateHandler : DebuggerHandlerBase
	{
		public DebuggerGameStateHandler([NotNull] ISessionManager sessionManager) : base("debugger/state", sessionManager) {}

		protected override void DoHandle([NotNull] HttpListenerContext context, [NotNull] IDebugger debugger)
		{
			context.SendResponse(debugger.GameState);
		}
	}
}