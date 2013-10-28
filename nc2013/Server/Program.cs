using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Game;
using Server.Handlers;

namespace Server
{
	public class Program
	{
		public const string CoreWarPrefix = "corewar";

		public static void Main()
		{
			var listener = new HttpListener();
			listener.Prefixes.Add("http://*/" + CoreWarPrefix + "/");
			listener.Start();
			var gameHttpServer = new GameHttpServer();
			var arena = new Arena();
			var handlers = new IHttpHandler[]
			{
				new StartGameHandler(gameHttpServer),
				new GetGameStateHandler(gameHttpServer),
				new StepHandler(gameHttpServer),
				new StepToEndHandler(gameHttpServer),
				new StaticHandler(),
				new RankingHandler(arena),
				new AddProgramToArenaHandler(arena),
				new ArenaPlayerHandler(arena),
			};
			Process.Start("http://localhost/corewar/index.html");
			while (true)
			{
				var context = listener.GetContext();
				Task.Factory.StartNew(() => HandleRequest(context, handlers));
			}
		}

		private static void HandleRequest(HttpListenerContext context, IEnumerable<IHttpHandler> handlers)
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
					context.Response.ContentType = "text/plain; charset: utf-8";
					e.WriteToResponse(context.Response);
				}
				catch (Exception e)
				{
					context.Response.ContentType = "text/plain; charset: utf-8";
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
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