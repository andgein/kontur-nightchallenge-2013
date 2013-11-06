using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStepToEndHandler : DebuggerHandlerBase
	{
		public DebuggerStepToEndHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/step/end", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode)
		{
			var gameStepResult = debugger.StepToEnd();
			var response = new DebuggerStepResponse
			{
				StoppedOnBreakpoint = gameStepResult.StoppedInBreakpoint,
				GameState = debugger.State.GameState
			};
			context.SendResponse(response);
		}
	}
}