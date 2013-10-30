using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private static readonly Random random = new Random();
		private readonly int battlesPerPair;
		private readonly FileInfo gamesLog;
		private readonly TournamentPlayer[] players;
		private readonly FileInfo rankingResultFile;

		public RoundRobinTournament(int battlesPerPair, FileInfo gamesLog, FileInfo rankingResultFile, TournamentPlayer[] players)
		{
			this.battlesPerPair = battlesPerPair;
			this.gamesLog = gamesLog;
			this.rankingResultFile = rankingResultFile;
			this.players = players;
		}

		public void Run()
		{
			File.WriteAllText(gamesLog.FullName, "");
			List<BattlePlayerResult> res = RunTournament(GenerateAllPairs()).SelectMany(r => r).ToList();
			MakeRankingTable(res);
		}

		private IEnumerable<Tuple<TournamentPlayer, TournamentPlayer>> GenerateAllPairs()
		{
			return players.Join(players, p => 1, p => 1, Tuple.Create).Where(t => t.Item1 != t.Item2);
		}

		private void MakeRankingTable(List<BattlePlayerResult> res)
		{
			var ranking = res.GroupBy(g => g.Player).Select(g => new {name=g.Key.Name, version=g.Key.Version, score=g.Sum(r => r.Score)}).OrderByDescending(t => t.score);
			File.WriteAllLines(rankingResultFile.FullName, ranking.Select(JsonConvert.SerializeObject));
		}

		private IEnumerable<BattlePlayerResult[]> RunTournament(IEnumerable<Tuple<TournamentPlayer, TournamentPlayer>> pairs)
		{
			for (int i = 0; i < battlesPerPair; i++)
				foreach (var pair in pairs)
				{
					var battlePlayers = new[] {pair.Item1, pair.Item2};
					BattlePlayerResult[] res = RunEngine(battlePlayers);
					File.AppendAllText(gamesLog.FullName, JsonConvert.SerializeObject(res) + Environment.NewLine);
					yield return res;
				}
		}

		private BattlePlayerResult[] RunEngine(TournamentPlayer[] battlePlayers)
		{
			//TODO fix this dummy Code. Use Engine to run battle
			return battlePlayers.Select(p => new BattlePlayerResult {Player = p, Score = random.Next(2)}).ToArray();
		}
	}
}