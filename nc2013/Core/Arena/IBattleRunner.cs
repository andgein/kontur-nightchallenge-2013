using Core.Game;
using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public interface IBattleRunner
	{
		[NotNull]
		GameState RunBattle([NotNull] Rules rules, [NotNull] Battle battle);
	}
}