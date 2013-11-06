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
				return DoLoadGames(tournamentId);
		}

		[NotNull]
		private List<BattleResult> DoLoadGames([NotNull] string tournamentId)
		{
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

		[NotNull]
		public string[] GetAllTournamentIds()
		{
			lock (gamesDir)
			{
				return gamesDir
					.GetFiles("ranking-*.json")
					.Select(file => Path.GetFileNameWithoutExtension(file.Name).Split('-')[1])
					.Where(id => id != "last")
					.ToArray();
			}
		}

		public void RemovePlayer([NotNull] string playerName)
		{
			lock (gamesDir)
			{
				var gamesFiles = gamesDir.GetFiles("games-*.json");
				foreach (var gamesFile in gamesFiles)
				{
					var tournamentId = Path.GetFileNameWithoutExtension(gamesFile.Name).Split('-')[1];
					var games = DoLoadGames(tournamentId)
						.Where(x => x.Player1Result.Player.Name != playerName && x.Player2Result.Player.Name != playerName)
						.ToList();
					var ranking = Ranking.MakeRankingTable(tournamentId, games);
					var rankingFile = GetRankingFile(tournamentId);
					File.WriteAllLines(gamesFile.FullName, games.Select(JsonConvert.SerializeObject));
					File.WriteAllText(rankingFile, JsonConvert.SerializeObject(ranking, Formatting.Indented));
				}
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