using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStepHandler : DebuggerHandlerBase
	{
		private const int memoryDiffsLimit = 1000;
		private const int programStateDiffsLimit = 1000;

		public DebuggerStepHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/step", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var stepCount = context.GetOptionalIntParam("count") ?? 1;
			var currentStep = context.GetOptionalIntParam("currentStep");
			var gameStepResult = debugger.Step(stepCount, currentStep);
			var response = new DebuggerStepResponse {StoppedOnBreakpoint = gameStepResult.StoppedInBreakpoint};
			if (gameStepResult.Diff == null || DiffIsTooBig(gameStepResult.Diff))
				response.GameState = debugger.State.GameState;
			else
				response.Diff = gameStepResult.Diff;
			context.SendResponse(response);
		}

		private static bool DiffIsTooBig([NotNull] Diff diff)
		{
			return (diff.MemoryDiffs != null && diff.MemoryDiffs.Length > memoryDiffsLimit)
				|| (diff.ProgramStateDiffs != null && diff.ProgramStateDiffs.Length > programStateDiffsLimit);
		}
	}
}