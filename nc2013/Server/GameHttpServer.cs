using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Arena;
using Core.Game;
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

		public GameHttpServer([NotNull] string prefix)
		{
			var baseUri = new Uri(prefix.Replace("*", "localhost").Replace("+", "localhost"));
			DefaultUrl = new Uri(baseUri, "index.html").AbsoluteUri;
			basePath = baseUri.AbsolutePath;

			listener = new HttpListener();
			listener.Prefixes.Add(prefix);
			var gameServer = new StupidGameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var httpSessionManager = new HttpSessionManager(new SessionManager("sessions"));
			var playersRepo = new PlayersRepo(new DirectoryInfo("players"));
			handlers = new IHttpHandler[]
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
				log.InfoFormat("Incoming request: {0}", httpListenerContext.Request.RawUrl);
				var context = new GameHttpContext(httpListenerContext, basePath);
				var handlersThatCanHandle = handlers.Where(h => h.CanHandle(context)).ToArray();
				if (handlersThatCanHandle.Length == 1)
				{
					log.InfoFormat("Handing request with {0}", handlersThatCanHandle[0].GetType().Name);
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