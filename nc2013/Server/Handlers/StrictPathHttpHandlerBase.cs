using System;
using System.Net;

namespace Server.Handlers
{
	public abstract class StrictPathHttpHandlerBase : HttpHandlerBase
	{
		private readonly string path;

		protected StrictPathHttpHandlerBase(string path)
		{
			this.path = path;
		}

		public override bool Handle(HttpListenerContext context)
		{
			if (context.Request.Url.AbsolutePath.Equals("/" + Program.CoreWarPrefix + "/" + path, StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine(context.Request.Url);
				DoHandle(context);
				context.Response.Close();
				return true;
			}
			return false;
		}

		protected abstract void DoHandle(HttpListenerContext context);
	}
}