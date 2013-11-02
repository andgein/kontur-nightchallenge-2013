using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class GamesRepo : IGamesRepo
	{
		private readonly DirectoryInfo gamesDir;

		public GamesRepo([NotNull] DirectoryInfo gamesDir)
		{
			this.gamesDir = gamesDir;
			if (!gamesDir.Exists)
				gamesDir.Create();
		}

		public bool TryStartTournament([NotNull] string tournamentId)
		{
			lock (gamesDir)
			{
				var gamesFile = GetGamesFile(tournamentId);
				if (File.Exists(gamesFile))
					return false;
				File.WriteAllText(gamesFile, string.Empty);
				return true;
			}
		}

		public void SaveTournamentResult([NotNull] string tournamentId, [NotNull] RoundRobinTournamentResult result)
		{
			SaveGames(tournamentId, result.BattleResults);
			SaveRanking(tournamentId, result.TournamentRanking);
		}

		[NotNull]
		public List<BattleResult> LoadGames([NotNull] string tournamentId = "last")
		{
			lock (gamesDir)
				return File
					.ReadAllLines(GetGamesFile(tournamentId))
					.Select(JsonConvert.DeserializeObject<BattleResult>)
					.ToList();
		}

		private void SaveGames([NotNull] string tournamentId, [NotNull] List<BattleResult> battleResults)
		{
			lock (gamesDir)
			{
				var gamesFile = GetGamesFile(tournamentId);
				File.WriteAllLines(gamesFile, battleResults.Select(JsonConvert.SerializeObject));
				File.Copy(gamesFile, GetGamesFile("last"), true);
			}
		}

		[CanBeNull]
		public TournamentRanking TryLoadRanking([NotNull] string tournamentId)
		{
			lock (gamesDir)
			{
				var rankingFile = GetRankingFile(tournamentId);
				if (!File.Exists(rankingFile))
					return null;
				return JsonConvert.DeserializeObject<TournamentRanking>(File.ReadAllText(rankingFile));
			}
		}

		private void SaveRanking([NotNull] string tournamentId, [NotNull] TournamentRanking ranking)
		{
			lock (gamesDir)
			{
				var rankingFile = GetRankingFile(tournamentId);
				File.WriteAllText(rankingFile, JsonConvert.SerializeObject(ranking, Formatting.Indented));
				File.Copy(rankingFile, GetRankingFile("last"), true);
			}
		}

		[NotNull]
		private string GetGamesFile([NotNull] string tournamentId)
		{
			return Path.Combine(gamesDir.FullName, "games-" + tournamentId + ".json");
		}

		[NotNull]
		private string GetRankingFile([NotNull] string tournamentId)
		{
			return Path.Combine(gamesDir.FullName, "ranking-" + tournamentId + ".json");
		}
	}
}