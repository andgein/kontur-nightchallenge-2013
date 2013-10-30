using System;
using System.Net;
using Core.Game;
using JetBrains.Annotations;
using log4net;
using Server.Sessions;

namespace Server.Debugging
{
	public class Debugger : IDebugger
	{
		private const string debuggerGameStateKey = "debuggerGameState";
		private readonly IGameServer gameServer;
		private readonly ISession session;
		private IGame game;

		private static readonly ILog log = LogManager.GetLogger(typeof (Debugger));

		public Debugger([NotNull] IGameServer gameServer, [NotNull] ISession session)
		{
			this.gameServer = gameServer;
			this.session = session;
			var persistedGameState = session.Load<GameState>(debuggerGameStateKey);
			if (persistedGameState != null)
				try
				{
					game = gameServer.ResumeGame(persistedGameState);
				}
				catch (Exception e)
				{
					log.Error("Resume game failed", e);
				}
		}

		public void StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			game = gameServer.StartNewGame(programStartInfos);
			session.Save(debuggerGameStateKey, game.GameState);
		}

		public void Reset()
		{
			game = null;
			session.Save(debuggerGameStateKey, (GameState) null);
		}

		public T Play<T>([NotNull] Func<IGame, T> action)
		{
			if (game == null)
				throw new HttpException(HttpStatusCode.Conflict, "Debugger is not started yet");
			var result = action(game);
			session.Save(debuggerGameStateKey, game.GameState);
			return result;
		}

		public void Play([NotNull] Action<IGame> action)
		{
			Play(g =>
			{
				action(g);
				return 0;
			});
		}

		[CanBeNull]
		public GameState GameState
		{
			get { return game == null ? null : game.GameState; }
		}
	}
}