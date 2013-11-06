using System;
using System.Linq;
using System.Net;
using Core;
using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class Debugger : IDebugger
	{
		private const string debuggerStateKey = "debuggerState";
		private readonly IGameServer gameServer;
		private readonly ISession session;
		private IGame game;

		private DebuggerProgramStartInfo[] lastProgramStartInfos;

		private readonly DebuggerProgramStartInfo[] defaultProgramStartInfos =
		{
			new DebuggerProgramStartInfo {Program = @";imp strategy
MOV 0, 1"},
			new DebuggerProgramStartInfo {Program = @";dwarf strategy
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
						Log.For(this).Error("Resume game failed", e);
					}
			}
			if (lastProgramStartInfos == null)
				lastProgramStartInfos = defaultProgramStartInfos;
		}

		public void StartNewGame([NotNull] DebuggerProgramStartInfo[] programStartInfos)
		{
			lastProgramStartInfos = programStartInfos;
			game = gameServer.StartNewGame(programStartInfos.Where(x => !x.Disabled).Select(x => new ProgramStartInfo {Program = x.Program, StartAddress = x.StartAddress}).ToArray());
			session.Save(debuggerStateKey, State);
		}

		public void Reset()
		{
			game = null;
			SaveState();
		}

		public T Play<T>([NotNull] Func<IGame, T> action)
		{
			if (game == null)
				throw new HttpException(HttpStatusCode.Conflict, "Debugger is not started yet");
			var result = action(game);
			SaveState();
			return result;
		}

		private void SaveState()
		{
			session.Save(debuggerStateKey, State);
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