using System;
using System.Collections.Generic;
using System.Linq;
using Core.Engine;
using Core.Parser;
using JetBrains.Annotations;

namespace Core.Game
{
	public class Game : IGame
	{
		private readonly GameEngine engine;
		private readonly ProgramStartInfo[] programStartInfos;

		public Game([NotNull] ProgramStartInfo[] programStartInfos)
		{
			this.programStartInfos = programStartInfos;
			var r = new Random();
			var parser = new WarriorParser();
			var warriors = programStartInfos.Select(
				psi => new WarriorStartInfo(
					parser.Parse(psi.Program),
					psi.StartAddress.HasValue ? (int) psi.StartAddress : r.Next(Parameters.CoreSize)
					));
			engine = new GameEngine(warriors);
		}

		public Game([NotNull] ProgramStartInfo[] programStartInfos, [NotNull] WarriorStartInfo[] warriorStartInfos)
		{
			this.programStartInfos = programStartInfos;
			engine = new GameEngine(warriorStartInfos);
		}

		[NotNull]
		public GameState GameState
		{
			get
			{
				return new GameState
				{
					CurrentProgram = engine.CurrentWarrior,
					CurrentStep = engine.CurrentStep,
					ProgramStartInfos = programStartInfos,
					GameOver = engine.GameOver,
					Winner = engine.Winner,
					MemoryState = engine.Memory.ToMemoryState(),
					ProgramStates = engine.Warriors.Select(w => new ProgramState
					{
						LastPointer = w.LastPointer,
						ProcessPointers = w.Queue.ToArray().Select(x => (uint) x).ToArray()
					}).ToArray(),
				};
			}
		}

		[NotNull]
		public GameState GameStateFast
		{
			get
			{
				return new GameState
				{
					CurrentProgram = engine.CurrentWarrior,
					CurrentStep = engine.CurrentStep,
					ProgramStartInfos = programStartInfos,
					GameOver = engine.GameOver,
					Winner = engine.Winner,
				};
			}
		}

		[NotNull]
		public GameStepResult Step(int stepCount, [CanBeNull] HashSet<Breakpoint> breakpoints = null)
		{
			if (stepCount == 0)
				return new GameStepResult
				{
					StoppedInBreakpoint = StoppedOnBreakpoint(null, breakpoints),
					Diff = new Diff
					{
						CurrentProgram = engine.CurrentWarrior,
						CurrentStep = engine.CurrentStep,
						GameOver = engine.GameOver,
						Winner = engine.Winner,
					}
				};

			var programStateDiffs = new List<ProgramStateDiff>();

			var memoryDiffs = new HashSet<int>();
			var stoppedOnBreakpoint = false;
			for (var i = 0; i < stepCount; ++i)
			{
				var stepResult = engine.Step();
				memoryDiffs.UnionWith(stepResult.MemoryDiffs);
				programStateDiffs.AddRange(stepResult.ProgramStateDiffs);
				stoppedOnBreakpoint = StoppedOnBreakpoint(stepResult, breakpoints);
				if (stoppedOnBreakpoint)
					break;
			}

			return new GameStepResult
			{
				StoppedInBreakpoint = stoppedOnBreakpoint,
				Diff = new Diff
				{
					CurrentProgram = engine.CurrentWarrior,
					CurrentStep = engine.CurrentStep,
					GameOver = engine.GameOver,
					Winner = engine.Winner,
					MemoryDiffs = memoryDiffs.Select(address => new MemoryDiff
					{
						Address = (uint) address,
						CellState = new CellState
						{
							Instruction = engine.Memory[address].Statement.ToString(),
							CellType = engine.Memory[address].Statement.CellType,
							LastModifiedByProgram = engine.Memory[address].LastModifiedByProgram
						}
					}).ToArray(),
					ProgramStateDiffs = programStateDiffs.ToArray()
				},
			};
		}

		private bool StoppedOnBreakpoint([CanBeNull] StepResult stepResult, [CanBeNull] HashSet<Breakpoint> breakpoints)
		{
			if (breakpoints == null || breakpoints.Count <= 0)
				return false;
			return IsMemoryBreakpoint(stepResult, breakpoints) || IsExecutionBreakpoint(breakpoints);
		}

		private bool IsExecutionBreakpoint([NotNull] HashSet<Breakpoint> breakpoints)
		{
			var warrior = engine.CurrentWarrior;
			var newCurrentWarriorInstructionPointer = engine.Warriors[warrior].Queue.PeekOrNull();
			if (newCurrentWarriorInstructionPointer.HasValue)
			{
				var breakpoint = new Breakpoint((uint) newCurrentWarriorInstructionPointer.Value, warrior, BreakpointType.Execution);
				if (breakpoints.Contains(breakpoint))
					return true;
			}
			return false;
		}

		private bool IsMemoryBreakpoint([CanBeNull] StepResult stepResult, [NotNull] HashSet<Breakpoint> breakpoints)
		{
			if (stepResult == null || !engine.LastExecutedWarrior.HasValue)
				return false;
			foreach (var address in stepResult.MemoryDiffs)
			{
				var breakpoint = new Breakpoint((uint) address, engine.LastExecutedWarrior.Value, BreakpointType.MemoryChange);
				if (breakpoints.Contains(breakpoint))
					return true;
			}
			return false;
		}

		[NotNull]
		public GameStepResult StepToEnd([CanBeNull] HashSet<Breakpoint> breakpoints = null)
		{
			var stoppedOnBreakpoint = false;
			while (!engine.GameOver)
			{
				var stepResult = engine.Step();
				stoppedOnBreakpoint = StoppedOnBreakpoint(stepResult, breakpoints);
				if (stoppedOnBreakpoint)
					break;
			}
			return new GameStepResult
			{
				StoppedInBreakpoint = stoppedOnBreakpoint,
				Diff = null,
			};
		}
	}
}