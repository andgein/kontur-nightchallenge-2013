using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public interface IHttpHandler
	{
		bool CanHandle([NotNull] HttpListenerContext context);
		void Handle([NotNull] HttpListenerContext context);
	}
}