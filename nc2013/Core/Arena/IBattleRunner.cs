using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public interface IBattleRunner
	{
		int? RunBattle([NotNull] Rules rules, [NotNull] Battle battle);
	}
}