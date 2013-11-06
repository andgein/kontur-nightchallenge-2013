using JetBrains.Annotations;

namespace Core.Game
{
	public class GameStepResult
	{
		public bool StoppedInBreakpoint { get; set; }

		[CanBeNull]
		public Diff Diff { get; set; }
	}
}