using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerStepHandler : DebuggerHandlerBase
	{
		private const int memoryDiffsLimit = 1000;
		private const int programStateDiffsLimit = 1000;

		public DebuggerStepHandler([NotNull] IDebuggerManager debuggerManager) : base("debugger/step", debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode)
		{
			var stepCount = context.GetOptionalIntParam("count") ?? 1;
			//HandleWithDiffs(context, debugger, stepCount);
			HandleWithoutDiffs(context, debugger, stepCount);
		}

		private static void HandleWithoutDiffs([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, int stepCount)
		{
			debugger.Play(game => game.Step(stepCount));
			var response = new DebuggerStepResponse
			{
				GameState = debugger.State.GameState
			};
			context.SendResponse(response);
		}

		private static void HandleWithDiffs([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, int stepCount)
		{
			var currentStep = context.GetOptionalIntParam("currentStep");
			var diff = debugger.Play(game =>
			{
				if (currentStep != game.GameState.CurrentStep)
				{
					game.Step(stepCount);
					return null;
				}
				return game.Step(stepCount);
			});
			var response = new DebuggerStepResponse();
			if (diff == null || DiffIsTooBig(diff))
				response.GameState = debugger.State.GameState;
			else
				response.Diff = diff;
			context.SendResponse(response);
		}

		private static bool DiffIsTooBig([NotNull] Diff diff)
		{
			return (diff.MemoryDiffs != null && diff.MemoryDiffs.Length > memoryDiffsLimit)
				|| (diff.ProgramStateDiffs != null && diff.ProgramStateDiffs.Length > programStateDiffsLimit);
		}
	}
}