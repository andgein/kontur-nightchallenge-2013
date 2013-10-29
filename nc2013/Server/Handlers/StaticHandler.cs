using System;
using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public class StaticHandler : HttpHandlerBase
	{
		public override bool CanHandle([NotNull] HttpListenerContext context)
		{
			var contentType = HttpListenerContextExtensions.TryGetContentType(context.Request.Url.AbsolutePath);
			return contentType != null;
		}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			var localPath = TryGetLocalPath(context);
			if (localPath == null)
				throw new HttpException(HttpStatusCode.NotFound, string.Format("Static resource '{0}' is not found", context.Request.RawUrl));
			context.SendStaticFile(localPath);
		}

		[CanBeNull]
		private static string TryGetLocalPath([NotNull] HttpListenerContext context)
		{
			var relPath = context.Request.Url.LocalPath;
			if (!relPath.Contains("..")
				&& relPath.StartsWith(Program.CoreWarPrefix, StringComparison.OrdinalIgnoreCase))
				return relPath.Substring(Program.CoreWarPrefix.Length);
			return null;
		}
	}
}