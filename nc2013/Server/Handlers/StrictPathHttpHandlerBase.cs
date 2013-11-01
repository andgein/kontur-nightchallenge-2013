using System;
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

		public bool CanHandle([NotNull] GameHttpContext context)
		{
			return context.Request.Url.AbsolutePath.Equals(context.BasePath + path, StringComparison.OrdinalIgnoreCase);
		}

		public abstract void Handle([NotNull] GameHttpContext context);
	}
}