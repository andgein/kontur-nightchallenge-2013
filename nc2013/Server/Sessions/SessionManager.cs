using System;
using System.Collections.Concurrent;
using Core.Game;
using JetBrains.Annotations;

namespace Server.Sessions
{
	public class SessionManager : ISessionManager
	{
		private readonly string sessionStorageFolder;
		private readonly IGameServer gameServer;
		private readonly ConcurrentDictionary<Guid, ISession> sessions = new ConcurrentDictionary<Guid, ISession>();

		public SessionManager([NotNull] string sessionStorageFolder, [NotNull] IGameServer gameServer)
		{
			this.sessionStorageFolder = sessionStorageFolder;
			this.gameServer = gameServer;
		}

		[NotNull]
		public ISession CreateSession()
		{
			var session = CreateSession(Guid.NewGuid());
			sessions[session.SessionId] = session;
			return session;
		}

		[NotNull]
		public ISession GetSession(Guid sessionId)
		{
			return sessions.GetOrAdd(sessionId, CreateSession);
		}

		[NotNull]
		private Session CreateSession(Guid sessionId)
		{
			return new Session(sessionId, sessionStorageFolder, gameServer);
		}
	}
}