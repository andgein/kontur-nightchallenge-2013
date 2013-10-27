using System.Net;

namespace Server.Handlers
{
	public interface IHttpHandler
	{
		bool Handle(HttpListenerContext context);
	}
}