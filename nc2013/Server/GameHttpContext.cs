using System;
using System.Net;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server
{
	public class GameHttpContext
	{
		private const string sessionIdCookieName = "coreWarSessionId";
		private const string basePathCookieName = "basePath";
		private const string godModeSecretCookieName = "godModeSecret";
		private const string godModeCookieName = "godMode"; 
		private readonly HttpListenerContext httpListenerContext;

		public GameHttpContext([NotNull] HttpListenerContext httpListenerContext, [NotNull] string basePath, [NotNull] ISessionManager sessionManager, [NotNull] string godModeSecret)
		{
			BasePath = basePath;
			this.httpListenerContext = httpListenerContext;
			this.SetCookie(basePathCookieName, basePath, persistent: false, httpOnly: false);
			var sessionId = this.TryGetCookie<Guid>(sessionIdCookieName, Guid.TryParse) ?? Guid.NewGuid();
			this.SetCookie(sessionIdCookieName, sessionId.ToString(), persistent: true, httpOnly: true);
			Session = sessionManager.GetSession(sessionId);
			var secretValue = this.GetOptionalStringParam(godModeSecretCookieName) ?? this.TryGetCookie(godModeSecretCookieName);
			if (secretValue == godModeSecret)
			{
				this.SetCookie(godModeSecretCookieName, godModeSecret, persistent: false, httpOnly: false);
				this.SetCookie(godModeCookieName, "true", persistent: false, httpOnly: false);
				GodMode = true;
			}
		}

		public bool GodMode { get; private set; }

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