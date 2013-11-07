using System;
using System.Collections.Generic;
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
		private readonly HashSet<Breakpoint> breakpoints = new HashSet<Breakpoint>();

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
					}
					catch (Exception e)
					{
						Log.For(this).Error("Resume game failed", e);
					}
				if (state.Breakpoints != null)
					breakpoints.UnionWith(state.Breakpoints);
			}
			if (lastProgramStartInfos == null)
				lastProgramStartInfos = defaultProgramStartInfos;
		}

		public void StartNewGame([NotNull] DebuggerProgramStartInfo[] programStartInfos)
		{
			lastProgramStartInfos = programStartInfos;
			game = gameServer.StartNewGame(programStartInfos.Where(x => !x.Disabled).Select(x => new ProgramStartInfo { Program = x.Program, StartAddress = x.StartAddress }).ToArray());
			session.Save(debuggerStateKey, State);
		}

		public void Reset()
		{
			game = null;
			SaveState();
		}

		[NotNull]
		public GameStepResult Step(int stepCount, int? currentStep)
		{
			return Play(() =>
			{
				var clientStateWasActual = currentStep == game.GameState.CurrentStep;
				var gameStepResult = game.Step(stepCount, breakpoints);
				return clientStateWasActual ? gameStepResult : new GameStepResult { StoppedInBreakpoint = gameStepResult.StoppedInBreakpoint };
			});

		}

		[NotNull]
		public GameStepResult StepToEnd()
		{
			return Play(() => game.StepToEnd(breakpoints));
		}

		[NotNull]
		public GameStepResult Restart()
		{
			return Play(() => game.Step(-game.GameState.CurrentStep, breakpoints));
		}

		public void AddBreakpoint([NotNull] Breakpoint breakpoint)
		{
			breakpoints.Add(breakpoint);
		}

		public void RemoveBreakpoint([NotNull] Breakpoint breakpoint)
		{
			breakpoints.Remove(breakpoint);
		}

		public void ClearBreakpoints()
		{
			breakpoints.Clear();
		}

		private T Play<T>([NotNull] Func<T> action)
		{
			if (game == null)
				throw new HttpException(HttpStatusCode.Conflict, "Debugger is not started yet");
			var result = action();
			SaveState();
			return result;
		}

		private void SaveState()
		{
			session.Save(debuggerStateKey, State);
		}

		[NotNull]
		public DebuggerState State
		{
			get
			{
				return new DebuggerState
				{
					GameState = game == null ? null : game.GameState,
					ProgramStartInfos = lastProgramStartInfos,
					Breakpoints = breakpoints
				};
			}
		}
	}
}