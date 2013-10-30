using JetBrains.Annotations;

namespace Server.Sessions
{
	public class HttpSessionManager : IHttpSessionManager
	{
		private readonly ISessionManager sessionManager;

		public HttpSessionManager([NotNull] ISessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		[NotNull]
		public ISession GetSession([NotNull] GameHttpContext context)
		{
			var sessionId = context.TryGetSessionId();
			if (sessionId.HasValue)
				return sessionManager.GetSession(sessionId.Value);
			var session = sessionManager.CreateSession();
			context.SetSessionId(session.SessionId);
			return session;
		}
	}
}