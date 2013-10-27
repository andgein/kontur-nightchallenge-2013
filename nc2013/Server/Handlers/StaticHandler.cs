using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Reflection;

namespace Server.Handlers
{
	public class StaticHandler : GameHandlerBase
	{
		private readonly ConcurrentDictionary<string, string> resources = new ConcurrentDictionary<string, string>();

		public StaticHandler() : base("static")
		{
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var file = GetStringParam(context, "file");
			var content = resources.GetOrAdd(file, LoadResource);
			var contentType = GetContentType(file);
			SendResponseRaw(context, content, contentType);
		}

		private string GetContentType(string file)
		{
			file = file.ToLower();
			if (Path.GetExtension(file) == ".js")
				return "text/javascript; encoding=utf-8";
			if (Path.GetExtension(file) == ".html" || Path.GetExtension(file) == ".htm")
				return "text/html; encoding=utf-8";
			return "text/plain; encoding=utf-8";
		}

		private string LoadResource(string name)
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Server.StaticContent." + name))
			{
				if (stream == null)
					throw new HttpException(HttpStatusCode.NotFound, string.Format("Static resource '{0}' not found", name));
				var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}
	}
}