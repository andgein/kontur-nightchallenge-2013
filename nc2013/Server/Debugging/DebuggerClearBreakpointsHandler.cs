using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerClearBreakpointsHandler : DebuggerHandlerBase
	{
		public DebuggerClearBreakpointsHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/breakpoints/clear", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			debugger.ClearBreakpoints();
		}
	}
}