using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerStepHandler : DebuggerHandlerBase
	{
		private const int memoryDiffsLimit = 1000;
		private const int programStateDiffsLimit = 1000;

		public DebuggerStepHandler([NotNull] IHttpSessionManager httpSessionManager, [NotNull] IDebuggerManager debuggerManager) : base("debugger/step", httpSessionManager, debuggerManager) {}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			var stepCount = context.GetOptionalIntParam("count") ?? 1;
			var diff = debugger.Play(game => game.Step(stepCount));
			var response = new DebuggerStepResponse();
			if (diff == null || DiffIsTooBig(diff))
				response.GameState = debugger.GameState;
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