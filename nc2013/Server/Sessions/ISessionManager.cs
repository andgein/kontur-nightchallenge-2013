using System;
using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface ISessionManager
	{
		[NotNull]
		ISession CreateSession();

		[NotNull]
		ISession GetSession(Guid sessionId);
	}
}