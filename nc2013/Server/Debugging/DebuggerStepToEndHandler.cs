using System.Net;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStepToEndHandler : DebuggerHandlerBase
	{
		public DebuggerStepToEndHandler([NotNull] ISessionManager sessionManager) : base("debugger/step/end", sessionManager) {}

		protected override void DoHandle([NotNull] HttpListenerContext context, [NotNull] IDebugger debugger)
		{
			debugger.Play(game => game.StepToEnd());
			context.SendResponse(debugger.GameState);
		}
	}
}