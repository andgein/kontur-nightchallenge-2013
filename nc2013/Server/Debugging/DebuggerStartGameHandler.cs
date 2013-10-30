using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStartGameHandler : DebuggerHandlerBase
	{
		public DebuggerStartGameHandler([NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base("debugger/start", httpSessionManager, debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var programStartInfos = context.GetRequest<ProgramStartInfo[]>();
			debugger.StartNewGame(programStartInfos);
			context.SendResponse(debugger.GameState);
		}
	}
}