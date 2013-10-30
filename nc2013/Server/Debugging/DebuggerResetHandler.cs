using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerResetHandler : DebuggerHandlerBase
	{
		public DebuggerResetHandler([NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base("debugger/reset", httpSessionManager, debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			debugger.Reset();
		}
	}
}