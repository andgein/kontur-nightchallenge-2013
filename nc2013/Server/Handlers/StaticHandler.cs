using System.IO;
using System.Net;

namespace Server.Handlers
{
	public class StaticHandler : HttpHandlerBase
	{
		public override bool Handle(HttpListenerContext context)
		{
			var localPath = TryGetLocalPath(context);
			if (localPath == null) return false;
			return
				TryHandleStatic("../../" + localPath, context)
					|| TryHandleStatic("../../StaticContent/" + localPath, context)
					|| TryHandleStatic(localPath, context)
					|| TryHandleStatic("StaticContent/" + localPath, context)
				;
		}

		private bool TryHandleStatic(string path, HttpListenerContext context)
		{
			if (!File.Exists(path)) return false;
			var contentType = GetContentType(path);
			SendResponseRaw(context, File.ReadAllBytes(path), contentType);
			return true;
		}

		private static string TryGetLocalPath(HttpListenerContext context)
		{
			var relPath = context.Request.Url.LocalPath;
			if (!relPath.Contains("..")
				&& relPath.StartsWith("/" + Program.CoreWarPrefix + "/"))
				return relPath.Substring(Program.CoreWarPrefix.Length + 2);
			return null;
		}

		private string GetContentType(string file)
		{
			file = file.ToLower();
			if (Path.GetExtension(file) == ".js")
				return "text/javascript; encoding=utf-8";
			if (Path.GetExtension(file) == ".html" || Path.GetExtension(file) == ".htm")
				return "text/html; encoding=utf-8";
			if (Path.GetExtension(file) == ".css")
				return "text/css; encoding=utf-8";
			return "text/plain; encoding=utf-8";
		}
	}
}