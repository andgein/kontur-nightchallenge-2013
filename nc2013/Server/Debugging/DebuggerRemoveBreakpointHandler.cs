using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerRemoveBreakpointHandler : DebuggerHandlerBase
	{
		public DebuggerRemoveBreakpointHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/breakpoints/remove", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode)
		{
			debugger.RemoveBreakpoint(new Breakpoint(context.GetUIntParam("address"), context.GetIntParam("program"), context.GetEnumParam<BreakpointType>("breakpointType")));
		}
	}
}