using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStartHandler : DebuggerHandlerBase
	{
		public DebuggerStartHandler([NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base("debugger/start", httpSessionManager, debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var programStartInfos = context.GetRequest<ProgramStartInfo[]>();
			debugger.StartNewGame(programStartInfos);
		}
	}
}