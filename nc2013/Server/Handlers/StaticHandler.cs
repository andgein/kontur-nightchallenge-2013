using System;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public class StaticHandler : IHttpHandler
	{
		private readonly string staticContentPath;

		public StaticHandler(string staticContentPath)
		{
			this.staticContentPath = staticContentPath;
		}

		public bool CanHandle([NotNull] GameHttpContext context)
		{
			var contentType = GameHttpContextExtensions.TryGetContentType(context.Request.Url.AbsolutePath);
			return contentType != null;
		}

		public void Handle([NotNull] GameHttpContext context)
		{
			var localPath = TryGetLocalPath(context);
			if (localPath == null)
				throw new HttpException(HttpStatusCode.NotFound, string.Format("Static resource '{0}' is not found", context.Request.RawUrl));
			context.SendStaticFile(Path.Combine(staticContentPath, "StaticContent", localPath));
		}

		[CanBeNull]
		private static string TryGetLocalPath([NotNull] GameHttpContext context)
		{
			var relPath = context.Request.Url.LocalPath;
			if (!relPath.Contains("..")
				&& relPath.StartsWith(context.BasePath, StringComparison.OrdinalIgnoreCase))
				return relPath.Substring(context.BasePath.Length);
			return null;
		}
	}
}