using Core.Game;
using JetBrains.Annotations;

namespace Core.Arena
{
	public interface IBattleRunner
	{
		[NotNull]
		GameState RunBattle([NotNull] Battle battle);
	}
}