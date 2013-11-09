using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerRestartHandler : DebuggerHandlerBase
	{
		public DebuggerRestartHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/restart", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var gameStepResult = debugger.Restart();
			var response = new DebuggerStepResponse
			{
				StoppedOnBreakpoint = gameStepResult.StoppedInBreakpoint,
				GameState = debugger.State.GameState
			};
			context.SendResponse(response);
		}
	}
}