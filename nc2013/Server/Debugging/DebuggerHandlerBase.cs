using System.Net;
using JetBrains.Annotations;
using Server.Handlers;
using Server.Sessions;

namespace Server.Debugging
{
	public abstract class DebuggerHandlerBase : StrictPathHttpHandlerBase
	{
		private readonly IHttpSessionManager httpSessionManager;
		private readonly IDebuggerManager debuggerManager;

		protected DebuggerHandlerBase([NotNull] string path, [NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base(path)
		{
			this.httpSessionManager = httpSessionManager;
			this.debuggerManager = debuggerManager;
		}

		public override sealed void Handle([NotNull] HttpListenerContext context)
		{
			var session = httpSessionManager.GetSession(context);
			lock (session)
			{
				var debugger = debuggerManager.GetDebugger(session);
				DoHandle(context, debugger);
			}
		}

		protected abstract void DoHandle([NotNull] HttpListenerContext context, [NotNull] IDebugger debugger);
	}
}