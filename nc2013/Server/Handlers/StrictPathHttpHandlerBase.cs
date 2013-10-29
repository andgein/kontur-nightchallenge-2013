using System;
using System.Net;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public abstract class StrictPathHttpHandlerBase : HttpHandlerBase
	{
		private readonly string path;

		protected StrictPathHttpHandlerBase([NotNull] string path)
		{
			this.path = path;
		}

		public override bool CanHandle([NotNull] HttpListenerContext context)
		{
			if (context.Request.Url.AbsolutePath.Equals(Program.CoreWarPrefix + path, StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine(context.Request.Url);
				return true;
			}
			return false;
		}
	}
}