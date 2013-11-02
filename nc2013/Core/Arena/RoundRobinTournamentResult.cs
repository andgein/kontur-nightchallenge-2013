using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.Arena
{
	public class RoundRobinTournamentResult
	{
		[NotNull]
		public List<BattleResult> BattleResults;

		[NotNull]
		public TournamentRanking TournamentRanking;
	}
}