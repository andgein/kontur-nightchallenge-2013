using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStepToEndHandler : DebuggerHandlerBase
	{
		public DebuggerStepToEndHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/step/end", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			debugger.Play(game => game.StepToEnd());
			context.SendResponse(debugger.GameState);
		}
	}
}