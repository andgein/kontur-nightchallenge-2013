using System;
using System.Net;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server
{
	public class GameHttpContext
	{
		private readonly HttpListenerContext httpListenerContext;
		private const string sessionIdCookieName = "coreWarSessionId";
		private const string basePathCookieName = "basePath";

		public GameHttpContext([NotNull] HttpListenerContext httpListenerContext, [NotNull] string basePath, [NotNull] ISessionManager sessionManager)
		{
			BasePath = basePath;
			this.httpListenerContext = httpListenerContext;
			this.SetCookie(basePathCookieName, basePath, persistent: false, httpOnly: false);
			var sessionId = this.TryGetCookie<Guid>(sessionIdCookieName, Guid.TryParse) ?? Guid.NewGuid();
			this.SetCookie(sessionIdCookieName, sessionId.ToString(), persistent: true, httpOnly: true);
			Session = sessionManager.GetSession(sessionId);
		}

		[NotNull]
		public ISession Session { get; private set; }

		[NotNull]
		public HttpListenerRequest Request
		{
			get { return httpListenerContext.Request; }
		}

		[NotNull]
		public HttpListenerResponse Response
		{
			get { return httpListenerContext.Response; }
		}

		[NotNull]
		public string BasePath { get; private set; }
	}
}