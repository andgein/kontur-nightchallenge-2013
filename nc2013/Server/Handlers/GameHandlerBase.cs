using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Server.Handlers
{
	public abstract class GameHandlerBase
	{
		private readonly string path;

		protected GameHandlerBase(string path)
		{
			this.path = path;
		}

		public bool Handle(HttpListenerContext context)
		{
			if (context.Request.Url.AbsolutePath.Equals(path, StringComparison.OrdinalIgnoreCase))
			{
				DoHandle(context);
				context.Response.Close();
				return true;
			}
			return false;
		}

		protected T GetRequest<T>(HttpListenerContext context)
		{
			var reader = new StreamReader(context.Request.InputStream);
			var data = reader.ReadToEnd();
			var result = JsonConvert.DeserializeObject<T>(data);
			return result;
		}

		protected void SendResponse<T>(HttpListenerContext context, T value)
		{
			var result = JsonConvert.SerializeObject(value);
			using (var writer = new StreamWriter(context.Response.OutputStream))
				writer.Write(result);
			context.Response.Close();
		}

		protected abstract void DoHandle(HttpListenerContext context);
	}
}