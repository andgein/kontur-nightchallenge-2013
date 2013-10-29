using System.Net;
using JetBrains.Annotations;
using Server.Handlers;
using Server.Sessions;

namespace Server.Debugging
{
	[RequireAuthorization]
	public abstract class DebuggerHandlerBase : StrictPathHttpHandlerBase
	{
		private readonly ISessionManager sessionManager;

		protected DebuggerHandlerBase([NotNull] string path, [NotNull] ISessionManager sessionManager) : base(path)
		{
			this.sessionManager = sessionManager;
		}

		public override sealed void DoHandle([NotNull] HttpListenerContext context)
		{
			var session = sessionManager.GetSession(context.GetSessionId());
			DoHandle(context, session.Debugger);
		}

		protected abstract void DoHandle([NotNull] HttpListenerContext context, [NotNull] IDebugger debugger);
	}
}