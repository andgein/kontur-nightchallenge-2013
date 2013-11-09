using System.Net;
using Core.Parser;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStartHandler : DebuggerHandlerBase
	{
		public DebuggerStartHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/start", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var programStartInfos = context.GetRequest<DebuggerProgramStartInfo[]>();
			try
			{
				debugger.StartNewGame(programStartInfos);
			}
			catch (CompilationException e)
			{
				throw new HttpException(HttpStatusCode.BadRequest, string.Format("В программе есть ошибки:\r\n{0}", e.Message), e);
			}
			context.SendResponse(debugger.State.GameState);
		}
	}
}