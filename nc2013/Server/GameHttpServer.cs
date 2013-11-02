using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Arena;
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
		private static readonly ILog log = LogManager.GetLogger(typeof(GameHttpServer));
		private static readonly ILog httpListenerExceptionLog = LogManager.GetLogger("network");

		private readonly HttpListener listener;
		private readonly IHttpHandler[] handlers;
		private Task listenerTask;
		private readonly string basePath;
		private readonly ManualResetEvent stopEvent;
		private readonly SessionManager sessionManager;
		private readonly ConcurrentDictionary<int, Tuple<string, Stopwatch>> activeRequests = new ConcurrentDictionary<int, Tuple<string, Stopwatch>>();
		private int requestId;

		public GameHttpServer([NotNull] string prefix, PlayersRepo playersRepo, GamesRepo gamesRepo, SessionManager sessionManager, DebuggerManager debuggerManager, string staticContentPath)
		{
			this.sessionManager = sessionManager;
			var baseUri = new Uri(prefix.Replace("*", "localhost").Replace("+", "localhost"));
			DefaultUrl = new Uri(baseUri, "index.html").AbsoluteUri;
			basePath = baseUri.AbsolutePath;
			listener = new HttpListener();
			listener.Prefixes.Add(prefix);
			handlers = new IHttpHandler[]
			{
				new DebuggerStartHandler(debuggerManager),
				new DebuggerStateHandler(debuggerManager),
				new DebuggerStepHandler(debuggerManager),
				new DebuggerStepToEndHandler(debuggerManager),
				new DebuggerResetHandler(debuggerManager),
				new StaticHandler(staticContentPath),
				new CommandDescribeHandler(),
				new ArenaRankingHandler(gamesRepo),
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
					if (WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, stopEvent }) == 1)
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
			var currentRequestId = Interlocked.Increment(ref requestId);
			try
			{
				var requestUrl = httpListenerContext.Request.RawUrl;
				log.DebugFormat("Incoming request: {0}", requestUrl);
				var handleTime = Stopwatch.StartNew();
				activeRequests[currentRequestId] = Tuple.Create(requestUrl, handleTime);
				var context = new GameHttpContext(httpListenerContext, basePath, sessionManager);
				if (TryHandleActivity(context))
					return;
				try
				{
					lock (context.Session)
					{
						var handlersThatCanHandle = handlers.Where(h => h.CanHandle(context)).ToArray();
						if (handlersThatCanHandle.Length == 1)
						{
							log.DebugFormat("Handling request with {0}: {1}", handlersThatCanHandle[0].GetType().Name, requestUrl);
							handlersThatCanHandle[0].Handle(context);
							context.Response.Close();
							log.DebugFormat("Request handled in {0} ms: {1}", handleTime.ElapsedMilliseconds, requestUrl);
						}
						else if (handlersThatCanHandle.Length == 0)
							throw new HttpException(HttpStatusCode.NotImplemented, string.Format("Method '{0}' is not implemented", requestUrl));
						else
							throw new HttpException(HttpStatusCode.InternalServerError, string.Format("Method '{0}' can be handled with many handlers: {1}", requestUrl, string.Join(", ", handlersThatCanHandle.Select(h => h.GetType().Name))));
					}
				}
				catch (HttpListenerException)
				{
					throw;
				}
				catch (HttpException e)
				{
					context.Response.Headers.Clear();
					context.Response.ContentType = "text/plain; charset: utf-8";
					e.WriteToResponse(context.Response);
					context.Response.Close();
				}
				catch (Exception e)
				{
					log.Error("Request failed", e);
					httpListenerContext.Response.Headers.Clear();
					httpListenerContext.Response.ContentType = "text/plain; charset: utf-8";
					httpListenerContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					using (var writer = new StreamWriter(httpListenerContext.Response.OutputStream))
						writer.Write(e.ToString());
					httpListenerContext.Response.Close();
				}
			}
			catch (HttpListenerException e)
			{
				httpListenerExceptionLog.Debug("HttpListener failure", e);
			}
			finally
			{
				Tuple<string, Stopwatch> dummy;
				activeRequests.TryRemove(currentRequestId, out dummy);
			}
		}

		private bool TryHandleActivity([NotNull] GameHttpContext context)
		{
			if (!string.Equals(context.Request.Url.AbsolutePath, basePath + "activity", StringComparison.OrdinalIgnoreCase))
				return false;
			using (var writer = new StreamWriter(context.Response.OutputStream))
			{
				foreach (var activeRequest in activeRequests.Values.OrderBy(x => x.Item1, StringComparer.OrdinalIgnoreCase))
					writer.WriteLine("{0} running for {1} ms", activeRequest.Item1, activeRequest.Item2.ElapsedMilliseconds);
			}
			context.Response.Close();
			return true;
		}
	}
}