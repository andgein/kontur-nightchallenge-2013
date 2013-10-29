using System.Net;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Sessions
{
	public class LoginHandler : StrictPathHttpHandlerBase
	{
		private readonly ISessionManager sessionManager;

		public LoginHandler([NotNull] ISessionManager sessionManager) : base("login")
		{
			this.sessionManager = sessionManager;
		}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			var session = sessionManager.CreateSession();
			context.SetSessionId(session.SessionId);
			var backUrl = context.GetOptionalStringParam("back") ?? Program.DefaultUrl;
			context.Redirect(backUrl);
		}
	}
}