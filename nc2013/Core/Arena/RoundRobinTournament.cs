using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Core.Game;
using Core.Game.MarsBased;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private static readonly Random random = new Random();
		private readonly int battlesPerPair;
		private readonly GamesRepo gamesRepo;
		private readonly TournamentPlayer[] players;
		private readonly string tournamentId;

		public RoundRobinTournament(int battlesPerPair, string tournamentId, GamesRepo gamesRepo, TournamentPlayer[] players)
		{
			this.battlesPerPair = battlesPerPair;
			this.tournamentId = tournamentId;
			this.gamesRepo = gamesRepo;
			this.players = players;
		}

		public void Run()
		{
			var res = RunTournament(GenerateAllPairs()).ToList();
			gamesRepo.SaveGames(tournamentId, res);
			MakeRankingTable(res.SelectMany(r => r).ToList());
		}

		private IEnumerable<Tuple<TournamentPlayer, TournamentPlayer>> GenerateAllPairs()
		{
			return players.Join(players, p => 1, p => 1, Tuple.Create).Where(t => t.Item1 != t.Item2);
		}

		private void MakeRankingTable(List<BattlePlayerResult> res)
		{
			var ranking = res
				.GroupBy(g => g.Player)
				.Select(
					g => new RankingEntry
					{
						Name = g.Key.Name,
						Version = g.Key.Version,
						Score = g.Sum(r => r.Score),
						Games = g.Count()
					}
				).OrderByDescending(t => t.Score);
			gamesRepo.SaveRanking(
				new TournamentRanking
				{
					TournamentId = tournamentId,
					Places = ranking.ToArray(),
					Time = DateTime.Now,
				});
		}

		private IEnumerable<BattlePlayerResult[]> RunTournament(IEnumerable<Tuple<TournamentPlayer, TournamentPlayer>> pairs)
		{
			for (int i = 0; i < battlesPerPair; i++)
				foreach (var pair in pairs)
				{
					var battlePlayers = new[] {pair.Item1, pair.Item2};
					var res = RunEngine(battlePlayers);
					yield return res;
				}
		}

		private BattlePlayerResult[] RunEngine(TournamentPlayer[] battlePlayers)
		{
			//TODO fix this dummy Code. Use Engine to run battle
			var programStartInfos = battlePlayers.Select(p => new ProgramStartInfo { Program = p.Program }).ToArray();
			var marsGame = new MarsGame(programStartInfos);
			marsGame.StepToEnd();
			return battlePlayers.Select((p, idx) => new BattlePlayerResult {Player = p, Score = GetScore(idx, marsGame.GameState.Winner)}).ToArray();
		}

		private static int GetScore(int player, int? winner)
		{
			return !winner.HasValue
				? 1
				: player == winner.Value ? 3 : 0;
		}
	}
}