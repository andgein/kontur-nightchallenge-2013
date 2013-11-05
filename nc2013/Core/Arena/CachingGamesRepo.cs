using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.Arena
{
	public class CachingGamesRepo : IGamesRepo
	{
		private readonly ConcurrentDictionary<string, List<BattleResult>> gamesCache = new ConcurrentDictionary<string, List<BattleResult>>();
		private readonly ConcurrentDictionary<string, TournamentRanking> rankingCache = new ConcurrentDictionary<string, TournamentRanking>();
		private readonly IGamesRepo gamesRepo;

		public CachingGamesRepo([NotNull] IGamesRepo gamesRepo)
		{
			this.gamesRepo = gamesRepo;
		}

		public bool TryStartTournament([NotNull] string tournamentId)
		{
			var result = gamesRepo.TryStartTournament(tournamentId);
			if (result)
				InvalidateCache(tournamentId);
			return result;
		}

		public void SaveTournamentResult([NotNull] string tournamentId, [NotNull] RoundRobinTournamentResult result)
		{
			gamesRepo.SaveTournamentResult(tournamentId, result);
			InvalidateCache(tournamentId);
		}

		[NotNull]
		public List<BattleResult> LoadGames([NotNull] string tournamentId = "last")
		{
			return gamesCache.GetOrAdd(tournamentId, x => gamesRepo.LoadGames(x));
		}

		[NotNull]
		public TournamentRanking TryLoadRanking([NotNull] string tournamentId)
		{
			return rankingCache.GetOrAdd(tournamentId, x => gamesRepo.TryLoadRanking(x));
		}

		[NotNull]
		public string[] GetAllTournamentIds()
		{
			return gamesRepo.GetAllTournamentIds();
		}

		private void InvalidateCache([NotNull] string tournamentId)
		{
			List<BattleResult> dummy;
			gamesCache.TryRemove(tournamentId, out dummy);
			gamesCache.TryRemove("last", out dummy);
			TournamentRanking dummy2;
			rankingCache.TryRemove(tournamentId, out dummy2);
			rankingCache.TryRemove("last", out dummy2);
		}
	}
}