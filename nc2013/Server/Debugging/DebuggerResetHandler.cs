using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerResetHandler : DebuggerHandlerBase
	{
		public DebuggerResetHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/reset", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode)
		{
			debugger.Reset();
		}
	}
}