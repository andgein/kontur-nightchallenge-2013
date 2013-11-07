using JetBrains.Annotations;

namespace Core.Game
{
	public class GameStepResult
	{
		[CanBeNull]
		public Breakpoint StoppedInBreakpoint { get; set; }

		[CanBeNull]
		public Diff Diff { get; set; }
	}
}