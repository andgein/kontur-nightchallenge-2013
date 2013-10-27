using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Server.Handlers;

namespace Server
{
	public class Program
	{
		public static void Main()
		{
			var listener = new HttpListener();
			listener.Prefixes.Add("http://*/corewars/");
			listener.Start();
			var gameHttpServer = new GameHttpServer();
			var handlers = new GameHandlerBase[]
			{
				new StartGameHandler(gameHttpServer)
			};
			while (true)
			{
				var context = listener.GetContext();
				Task.Factory.StartNew(() => HandleRequest(context, handlers));
			}
		}

		private static void HandleRequest(HttpListenerContext context, GameHandlerBase[] handlers)
		{
			try
			{
				try
				{
					if (!handlers.Any(h => h.Handle(context)))
					{
						context.Response.StatusCode = (int) HttpStatusCode.NotImplemented;
						using (var writer = new StreamWriter(context.Response.OutputStream))
							writer.Write("Method '{0}' is not implemented", context.Request.RawUrl);
						context.Response.Close();
					}
				}
				catch (HttpException e)
				{
					e.WriteToResponse(context.Response);
				}
				catch (Exception e)
				{
					context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
					using (var writer = new StreamWriter(context.Response.OutputStream))
						writer.Write(e.ToString());
					context.Response.Close();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("FATAL ERROR: {0}", e);
			}
		}
	}
}