using System;
using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public abstract class StrictPathHttpHandlerBase : IHttpHandler
	{
		private readonly string path;

		protected StrictPathHttpHandlerBase([NotNull] string path)
		{
			this.path = path;
		}

		public bool CanHandle([NotNull] HttpListenerContext context)
		{
			return context.Request.Url.AbsolutePath.Equals(Program.CoreWarPrefix + path, StringComparison.OrdinalIgnoreCase);
		}

		public abstract void Handle([NotNull] HttpListenerContext context);
	}
}