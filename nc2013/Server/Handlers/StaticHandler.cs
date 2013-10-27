using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Server.Handlers
{
	public class StaticHandler : HttpHandlerBase
	{
		private const string serverStaticContentPrefix = "Server.StaticContent.";

		private readonly ConcurrentDictionary<string, string> resources = new ConcurrentDictionary<string, string>();
		private readonly string[] resourceNames;

		public StaticHandler()
		{
			resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith(serverStaticContentPrefix)).ToArray();
		}

		public override bool Handle(HttpListenerContext context)
		{
			var absolutePath = context.Request.Url.AbsolutePath;
			foreach (var resourceName in resourceNames)
			{
				var file = Path.GetFileName(absolutePath);
				if (string.Equals(resourceName, serverStaticContentPrefix + file))
				{
					var content = resources.GetOrAdd(file, LoadResource);
					var contentType = GetContentType(file);
					SendResponseRaw(context, content, contentType);
					return true;
				}
			}
			return false;
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
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(serverStaticContentPrefix + name))
			{
				if (stream == null)
					throw new InvalidOperationException(string.Format("Static resource '{0}' not found", name));
				var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}
	}
}