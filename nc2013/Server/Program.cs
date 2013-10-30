using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Arena;
using Core.Game;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using Server.Arena;
using Server.Debugging;
using Server.Handlers;
using Server.Sessions;

namespace Server
{
	public static class Program
	{
		public const string CoreWarPrefix = "/corewar/";
		public const string DefaultUrl = CoreWarPrefix + "index.html";

		private static readonly ILog mainLog = LogManager.GetLogger("main");
		private static readonly ILog log = LogManager.GetLogger(typeof (Program));

		public static void Main()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			var listener = new HttpListener();
			const string prefix = "http://*" + CoreWarPrefix;
			listener.Prefixes.Add(prefix);
			listener.Start();
			var gameServer = new StupidGameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var httpSessionManager = new HttpSessionManager(new SessionManager("sessions"));
			var playersRepo = new PlayersRepo(new DirectoryInfo("players"));
			var handlers = new IHttpHandler[]
			{
				new DebuggerHandler(),
				new DebuggerStartGameHandler(httpSessionManager, debuggerManager),
				new DebuggerGameStateHandler(httpSessionManager, debuggerManager),
				new DebuggerStepHandler(httpSessionManager, debuggerManager),
				new DebuggerStepToEndHandler(httpSessionManager, debuggerManager),
				new StaticHandler(),
				new RankingHandler(),
				new ArenaSubmitHandler(playersRepo),
				new ArenaPlayerHandler(playersRepo)
			};
			mainLog.InfoFormat("Listening {0}", prefix);
			Process.Start("http://localhost" + DefaultUrl);
			while (true)
			{
				var context = listener.GetContext();
				Task.Factory.StartNew(() => HandleRequest(context, handlers));
			}
		}

		private static void HandleRequest([NotNull] HttpListenerContext context, [NotNull] IEnumerable<IHttpHandler> handlers)
		{
			try
			{
				try
				{
					log.InfoFormat("Incoming request: {0}", context.Request.RawUrl);
					var handlersThatCanHandle = handlers.Where(h => h.CanHandle(context)).ToArray();
					if (handlersThatCanHandle.Length == 1)
					{
						log.InfoFormat("Handing request with {0}", handlersThatCanHandle[0].GetType().Name);
						handlersThatCanHandle[0].Handle(context);
					}
					else if (handlersThatCanHandle.Length == 0)
						throw new HttpException(HttpStatusCode.NotImplemented, string.Format("Method '{0}' is not implemented", context.Request.RawUrl));
					else
						throw new HttpException(HttpStatusCode.InternalServerError, string.Format("Method '{0}' can be handled with many handlers: {1}", context.Request.RawUrl, string.Join(", ", handlersThatCanHandle.Select(h => h.GetType().Name))));
				}
				catch (HttpException e)
				{
					context.Response.ContentType = "text/plain; charset: utf-8";
					e.WriteToResponse(context.Response);
				}
				catch (Exception e)
				{
					log.Error("Request failed", e);
					context.Response.ContentType = "text/plain; charset: utf-8";
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