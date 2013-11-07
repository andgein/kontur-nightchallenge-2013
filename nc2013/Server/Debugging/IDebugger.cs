using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public interface IDebugger
	{
		void StartNewGame([NotNull] DebuggerProgramStartInfo[] programStartInfos);
		void Reset();

		[NotNull]
		GameStepResult Step(int stepCount, int? currentStep);

		[NotNull]
		GameStepResult StepToEnd();

		[NotNull]
		GameStepResult Restart();

		void AddBreakpoint([NotNull] Breakpoint breakpoint);
		void RemoveBreakpoint([NotNull] Breakpoint breakpoint);
		void ClearBreakpoints();

		[NotNull]
		DebuggerState State { get; }
	}
}