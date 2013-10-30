using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Arena;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using log4net;
using Server.Arena;
using Server.Debugging;
using Server.Handlers;
using Server.Sessions;

namespace Server
{
	public class GameHttpServer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (GameHttpServer));

		private readonly HttpListener listener;
		private readonly IHttpHandler[] handlers;
		private Task listenerTask;
		private readonly string basePath;
		private readonly ManualResetEvent stopEvent;
		private readonly SessionManager sessionManager;

		public GameHttpServer([NotNull] string prefix, PlayersRepo playersRepo, GamesRepo gamesRepo)
		{
			var baseUri = new Uri(prefix.Replace("*", "localhost").Replace("+", "localhost"));
			DefaultUrl = new Uri(baseUri, "index.html").AbsoluteUri;
			basePath = baseUri.AbsolutePath;

			listener = new HttpListener();
			listener.Prefixes.Add(prefix);
			sessionManager = new SessionManager("sessions");
			var gameServer = new MarsGameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			handlers = new IHttpHandler[]
			{
				new DebuggerStartHandler(debuggerManager),
				new DebuggerGameStateHandler(debuggerManager),
				new DebuggerStepHandler(debuggerManager),
				new DebuggerStepToEndHandler(debuggerManager),
				new DebuggerResetHandler(debuggerManager),
				new StaticHandler(),
				new RankingHandler(gamesRepo),
				new ArenaSubmitHandler(playersRepo),
				new ArenaPlayerHandler(playersRepo, gamesRepo)
			};
			stopEvent = new ManualResetEvent(false);
		}

		[NotNull]
		public string DefaultUrl { get; private set; }

		public void Run()
		{
			listener.Start();
			listenerTask = Task.Factory.StartNew(() =>
			{
				while (true)
				{
					var asyncResult = listener.BeginGetContext(null, null);
					if (WaitHandle.WaitAny(new[] {asyncResult.AsyncWaitHandle, stopEvent}) == 1)
						break;
					var httpListenerContext = listener.EndGetContext(asyncResult);
					Task.Factory.StartNew(() => HandleRequest(httpListenerContext));
				}
			});
		}

		public void WaitForTermination()
		{
			listenerTask.Wait();
		}

		public void Stop()
		{
			stopEvent.Set();
		}

		private void HandleRequest([NotNull] HttpListenerContext httpListenerContext)
		{
			try
			{
				log.DebugFormat("Incoming request: {0}", httpListenerContext.Request.RawUrl);
				var handlersThatCanHandle = handlers.Where(h => h.CanHandle(context)).ToArray();
				if (handlersThatCanHandle.Length == 1)
				var context = new GameHttpContext(httpListenerContext, basePath, sessionManager);
				{
					log.DebugFormat("Handling request with {0}", handlersThatCanHandle[0].GetType().Name);
					handlersThatCanHandle[0].Handle(context);
				}
				else if (handlersThatCanHandle.Length == 0)
					throw new HttpException(HttpStatusCode.NotImplemented, string.Format("Method '{0}' is not implemented", httpListenerContext.Request.RawUrl));
				else
					throw new HttpException(HttpStatusCode.InternalServerError, string.Format("Method '{0}' can be handled with many handlers: {1}", httpListenerContext.Request.RawUrl, string.Join(", ", handlersThatCanHandle.Select(h => h.GetType().Name))));
			}
			catch (HttpException e)
			{
				httpListenerContext.Response.ContentType = "text/plain; charset: utf-8";
				e.WriteToResponse(httpListenerContext.Response);
			}
			catch (Exception e)
			{
				log.Error("Request failed", e);
				httpListenerContext.Response.ContentType = "text/plain; charset: utf-8";
				httpListenerContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
				using (var writer = new StreamWriter(httpListenerContext.Response.OutputStream))
					writer.Write(e.ToString());
				httpListenerContext.Response.Close();
			}
		}
	}
}