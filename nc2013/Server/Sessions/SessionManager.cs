using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Server.Sessions
{
	public class SessionManager : ISessionManager
	{
		private readonly string sessionStorageFolder;
		private readonly ConcurrentDictionary<Guid, ISession> sessions = new ConcurrentDictionary<Guid, ISession>();

		public SessionManager([NotNull] string sessionStorageFolder)
		{
			this.sessionStorageFolder = sessionStorageFolder;
		}

		[NotNull]
		public ISession GetSession(Guid sessionId)
		{
			return sessions.GetOrAdd(sessionId, CreateSession);
		}

		[NotNull]
		private Session CreateSession(Guid sessionId)
		{
			return new Session(sessionId, sessionStorageFolder);
		}
	}
}