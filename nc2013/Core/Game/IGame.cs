using JetBrains.Annotations;

namespace Core.Game
{
	public interface IGame
	{
		[NotNull]
		GameState GameState { get; }

		[NotNull]
		Diff Step(int stepCount);
	}
}