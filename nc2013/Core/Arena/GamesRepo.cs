using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class GamesRepo
	{
		private readonly DirectoryInfo gamesDir;

		public GamesRepo(DirectoryInfo gamesDir)
		{
			this.gamesDir = gamesDir;
			if (!gamesDir.Exists) gamesDir.Create();
		}

		public List<BattlePlayerResult[]> LoadGames(string tournamentId = "last")
		{
			lock (gamesDir)
				return File
					.ReadAllLines(GetGamesFile(tournamentId))
					.Select(JsonConvert.DeserializeObject<BattlePlayerResult[]>)
					.ToList();
		}

		public void SaveGames(string tournamentId, List<BattlePlayerResult[]> games)
		{
			lock (gamesDir)
			{
				var gamesFile = GetGamesFile(tournamentId);
				File.WriteAllLines(gamesFile, games.Select(JsonConvert.SerializeObject));
				File.Copy(gamesFile, GetGamesFile("last"), true);
			}
		}

		public TournamentRanking LoadRanking(string tournamentId = "last")
		{
			lock (gamesDir)
				return JsonConvert.DeserializeObject<TournamentRanking>(File.ReadAllText(GetRankingFile(tournamentId)));
		}

		public void SaveRanking(TournamentRanking ranking)
		{
			lock (gamesDir)
			{
				var rankingFile = GetRankingFile(ranking.TournamentId);
				File.WriteAllText(rankingFile, JsonConvert.SerializeObject(ranking, Formatting.Indented));
				File.Copy(rankingFile, GetRankingFile("last"), true);
			}
		}

		private string GetGamesFile(string tournamentId)
		{
			return Path.Combine(gamesDir.FullName, "games-" + tournamentId + ".json");
		}

		private string GetRankingFile(string tournamentId)
		{
			return Path.Combine(gamesDir.FullName, "ranking-" + tournamentId + ".json");
		}

	}
}