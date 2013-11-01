using System;
using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface ISessionManager
	{
		[NotNull]
		ISession GetSession(Guid sessionId);
	}
}