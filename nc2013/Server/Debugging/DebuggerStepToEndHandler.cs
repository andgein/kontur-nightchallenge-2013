using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStepToEndHandler : DebuggerHandlerBase
	{
		public DebuggerStepToEndHandler([NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base("debugger/step/end", httpSessionManager, debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			debugger.Play(game => game.StepToEnd());
			context.SendResponse(debugger.GameState);
		}
	}
}