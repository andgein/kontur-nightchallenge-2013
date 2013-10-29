using JetBrains.Annotations;

namespace Core.Game
{
	public interface IGame
	{
		[NotNull]
		GameState GameState { get; }

		[CanBeNull]
		Diff Step(int stepCount);

		void StepToEnd();
	}
}