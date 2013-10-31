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
		private const string debuggerStateKey = "debuggerState";
		private readonly IGameServer gameServer;
		private readonly ISession session;
		private IGame game;

		private static readonly ILog log = LogManager.GetLogger(typeof (Debugger));
		private ProgramStartInfo[] lastProgramStartInfos;
		private readonly ProgramStartInfo[] defaultProgramStartInfos =
		{
			new ProgramStartInfo{Program = @";imp strategy
MOV 0, 1"}, 
			new ProgramStartInfo{Program = @";dwarf strategy
ADD #4, 3
MOV 2, @2
JMP -2
DAT #0, #0"}
		};


		public Debugger([NotNull] IGameServer gameServer, [NotNull] ISession session)
		{
			this.gameServer = gameServer;
			this.session = session;
			var state = session.Load<DebuggerState>(debuggerStateKey);
			if (state != null)
			{
				lastProgramStartInfos = state.ProgramStartInfos;
				if (state.GameState != null)
					try
					{
						game = gameServer.ResumeGame(state.GameState);
						State.GameState = game.GameState;
					}
					catch (Exception e)
					{
						log.Error("Resume game failed", e);
					}
			}
			if (lastProgramStartInfos == null)
				lastProgramStartInfos = defaultProgramStartInfos;
		}

		public void StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			lastProgramStartInfos = programStartInfos;
			game = gameServer.StartNewGame(programStartInfos);
			session.Save(debuggerStateKey, State);
		}

		public void Reset()
		{
			game = null;
			session.Save(debuggerStateKey, (DebuggerState) null);
		}

		public T Play<T>([NotNull] Func<IGame, T> action)
		{
			if (game == null)
				throw new HttpException(HttpStatusCode.Conflict, "Debugger is not started yet");
			var result = action(game);
			session.Save(debuggerStateKey, State);
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

		[NotNull]
		public DebuggerState State
		{
			get
			{
				return new DebuggerState
				{
					GameState = game == null ? null : game.GameState,
					ProgramStartInfos = lastProgramStartInfos
				};
			}
		}
	}
}