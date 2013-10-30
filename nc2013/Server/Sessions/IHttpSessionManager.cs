using System.Net;
using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface IHttpSessionManager
	{
		ISession GetSession([NotNull] HttpListenerContext context);
	}
}