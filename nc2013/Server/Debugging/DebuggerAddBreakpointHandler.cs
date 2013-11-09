using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerAddBreakpointHandler : DebuggerHandlerBase
	{
		public DebuggerAddBreakpointHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/breakpoints/add", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			debugger.AddBreakpoint(new Breakpoint(context.GetUIntParam("address"), context.GetIntParam("program"), context.GetEnumParam<BreakpointType>("breakpointType")));
		}
	}
}