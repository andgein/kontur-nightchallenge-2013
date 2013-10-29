using System;
using System.Net;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Handlers
{
	public abstract class HttpHandlerBase : IHttpHandler
	{
		public void Handle([NotNull] HttpListenerContext context)
		{
			if (GetType().IsDefined(typeof (RequireAuthorizationAttribute), true))
			{
				var sessionId = context.TryGetSessionId();
				if (!sessionId.HasValue)
				{
					if (context.IsAjax())
						throw new HttpException(HttpStatusCode.Forbidden, "User is not logged in");
					context.Redirect(Program.LoginUrl + "?back=" + Uri.EscapeDataString(context.Request.RawUrl));
					return;
				}
			}
			DoHandle(context);
		}

		public abstract void DoHandle([NotNull] HttpListenerContext context);
		public abstract bool CanHandle([NotNull] HttpListenerContext context);
	}
}