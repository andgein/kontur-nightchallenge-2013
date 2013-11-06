using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.Game
{
	public interface IGame
	{
		[NotNull]
		GameState GameState { get; }

		[NotNull]
		GameStepResult Step(int stepCount, [CanBeNull] HashSet<Breakpoint> breakpoints = null);

		[NotNull]
		GameStepResult StepToEnd([CanBeNull] HashSet<Breakpoint> breakpoints = null);
	}
}