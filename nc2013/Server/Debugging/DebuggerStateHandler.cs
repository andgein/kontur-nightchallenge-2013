using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStateHandler : DebuggerHandlerBase
	{
		public DebuggerStateHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/state", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			context.SendResponse(debugger.State);
		}
	}
}