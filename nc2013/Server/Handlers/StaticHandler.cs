using System;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public class StaticHandler : IHttpHandler
	{
		private readonly string staticContentPath;
		private readonly Guid godModeSecret;

		public StaticHandler(string staticContentPath, Guid godModeSecret)
		{
			this.staticContentPath = staticContentPath;
			this.godModeSecret = godModeSecret;
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
			TryTurnGodModeOn(context);
			context.SendStaticFile(Path.Combine(staticContentPath, "StaticContent", localPath));
		}

		private void TryTurnGodModeOn([NotNull] GameHttpContext context)
		{
			var secretValue = context.GetOptionalGuidParam(GameHttpContext.GodModeSecretCookieName);
			if (secretValue == godModeSecret)
				context.SetCookie(GameHttpContext.GodModeSecretCookieName, godModeSecret.ToString(), persistent: false, httpOnly: true);
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