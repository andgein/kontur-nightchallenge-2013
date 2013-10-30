using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface IHttpSessionManager
	{
		[NotNull]
		ISession GetSession([NotNull] GameHttpContext context);
	}
}