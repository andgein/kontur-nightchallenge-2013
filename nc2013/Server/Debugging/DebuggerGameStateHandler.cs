using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerGameStateHandler : DebuggerHandlerBase
	{
		public DebuggerGameStateHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/state", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			context.SendResponse(debugger.GameState);
		}
	}
}