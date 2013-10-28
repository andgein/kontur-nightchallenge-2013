using JetBrains.Annotations;

namespace Core.Game
{
	public interface IGame
	{
		[NotNull]
		Diff Step(int stepCount);

		[NotNull]
		GameState GameState { get; }
	}
}