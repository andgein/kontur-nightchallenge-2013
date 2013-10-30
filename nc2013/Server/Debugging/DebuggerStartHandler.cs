using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStartHandler : DebuggerHandlerBase
	{
		public DebuggerStartHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/start", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var programStartInfos = context.GetRequest<ProgramStartInfo[]>();
			debugger.StartNewGame(programStartInfos);
		}
	}
}