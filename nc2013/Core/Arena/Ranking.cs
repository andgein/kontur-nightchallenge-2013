using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Core.Arena
{
	public static class Ranking
	{
		[NotNull]
		public static TournamentRanking MakeRankingTable([NotNull] string tournamentId, [NotNull] List<BattleResult> battleResults)
		{
			var battlePlayerResults = battleResults.SelectMany(r => r.Results).ToList();
			return MakeRankingTable(tournamentId, battlePlayerResults);
		}

		[NotNull]
		private static TournamentRanking MakeRankingTable([NotNull] string tournamentId, [NotNull] List<BattlePlayerResult> results)
		{
			var rankingEntries = results
				.GroupBy(g => g.Player)
				.Select(g => new RankingEntry
				{
					Name = g.Key.Name,
					Version = g.Key.Version,
					Score = g.Sum(res => res.Score()),
					Wins = g.Count(res => res.ResultType == BattlePlayerResultType.Win),
					Loses = g.Count(res => res.ResultType == BattlePlayerResultType.Loss),
					Draws = g.Count(res => res.ResultType == BattlePlayerResultType.Draw),
					Games = g.Count(),
				})
				.OrderByDescending(t => t.Score)
				.ToArray();
			var ranking = new TournamentRanking
			{
				TournamentId = tournamentId,
				Timestamp = DateTime.UtcNow,
				Places = rankingEntries,
			};
			return ranking;
		}
	}
}