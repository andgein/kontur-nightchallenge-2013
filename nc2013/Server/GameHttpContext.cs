using System.Net;
using JetBrains.Annotations;

namespace Server
{
	public class GameHttpContext
	{
		private readonly HttpListenerContext httpListenerContext;

		public GameHttpContext([NotNull] HttpListenerContext httpListenerContext, [NotNull] string basePath)
		{
			BasePath = basePath;
			this.httpListenerContext = httpListenerContext;
		}

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