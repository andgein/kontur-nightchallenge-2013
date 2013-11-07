using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arena;
using Core.Engine;
using Core.Parser;
using JetBrains.Annotations;

namespace Core.Game
{
	public class Game : IGame
	{
		private GameEngine engine;
		private readonly ProgramStartInfo[] programStartInfos;
		private readonly List<WarriorStartInfo> warriors;

		public Game([NotNull] ProgramStartInfo[] programStartInfos)
		{
			this.programStartInfos = programStartInfos;

			var r = new RandomAllocator(Parameters.CoreSize, Parameters.MinWarriorsDistance);
			var parser = new WarriorParser();

			var lastAddress = 0;
			warriors = new List<WarriorStartInfo>();
			foreach (var psi in programStartInfos)
			{
				var warrior = parser.Parse(psi.Program);
				warriors.Add(new WarriorStartInfo(
					warrior,
					psi.StartAddress.HasValue ? (int) psi.StartAddress : r.NextLoadAddress(lastAddress, warrior.Length)
					));
				lastAddress = warriors.Last().LoadAddress + warriors.Last().Warrior.Length;
			}

			Init();
		}

		private void Init()
		{
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
			if (stepCount < 0)
			{
				var currentStep = engine.CurrentStep;
				currentStep += stepCount;
				if (currentStep < 0)
					currentStep = 0;
				Init();
				Step(currentStep, breakpoints);
				return new GameStepResult();
			}

			var programStateDiffs = new List<ProgramStateDiff>();

			var memoryDiffs = new HashSet<int>();
			Breakpoint stoppedOnBreakpoint = null;
			for (var i = 0; i < stepCount; ++i)
			{
				var stepResult = engine.Step();
				memoryDiffs.UnionWith(stepResult.MemoryDiffs);
				programStateDiffs.AddRange(stepResult.ProgramStateDiffs);
				stoppedOnBreakpoint = StoppedOnBreakpoint(stepResult, breakpoints);
				if (stoppedOnBreakpoint != null)
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

		[CanBeNull]
		private Breakpoint StoppedOnBreakpoint([CanBeNull] StepResult stepResult, [CanBeNull] HashSet<Breakpoint> breakpoints)
		{
			if (breakpoints == null || breakpoints.Count <= 0)
				return null;
			return IsMemoryBreakpoint(stepResult, breakpoints) ?? IsExecutionBreakpoint(breakpoints);
		}

		[CanBeNull]
		private Breakpoint IsExecutionBreakpoint([NotNull] HashSet<Breakpoint> breakpoints)
		{
			var warrior = engine.CurrentWarrior;
			var newCurrentWarriorInstructionPointer = engine.Warriors[warrior].Queue.PeekOrNull();
			if (newCurrentWarriorInstructionPointer.HasValue)
			{
				var breakpoint = new Breakpoint((uint) newCurrentWarriorInstructionPointer.Value, warrior, BreakpointType.Execution);
				if (breakpoints.Contains(breakpoint))
					return breakpoint;
			}
			return null;
		}

		[CanBeNull]
		private Breakpoint IsMemoryBreakpoint([CanBeNull] StepResult stepResult, [NotNull] HashSet<Breakpoint> breakpoints)
		{
			if (stepResult == null || !engine.LastExecutedWarrior.HasValue)
				return null;
			foreach (var address in stepResult.MemoryDiffs)
			{
				var breakpoint = new Breakpoint((uint) address, engine.LastExecutedWarrior.Value, BreakpointType.MemoryChange);
				if (breakpoints.Contains(breakpoint))
					return breakpoint;
			}
			return null;
		}

		[NotNull]
		public GameStepResult StepToEnd([CanBeNull] HashSet<Breakpoint> breakpoints = null)
		{
			Breakpoint stoppedOnBreakpoint = null;
			while (!engine.GameOver)
			{
				var stepResult = engine.Step();
				stoppedOnBreakpoint = StoppedOnBreakpoint(stepResult, breakpoints);
				if (stoppedOnBreakpoint != null)
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