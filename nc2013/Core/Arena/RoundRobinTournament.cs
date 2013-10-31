using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using log4net;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RoundRobinTournament));
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
					var battle = new Battle
					{
						Player1 = pair.Item1,
						Player2 = pair.Item2,
					};
					var battleResult = RunBattle(battle);
					if (battleResult.RunToCompletion)
						yield return battleResult.Results;
				}
		}

		private BattleResult RunBattle([NotNull] Battle battle)
		{
			try
			{
				var marsGame = new MarsGame(battle.GetProgramStartInfos());
				marsGame.StepToEnd();
				var winner = marsGame.GameState.Winner;
				//TODO: set StartAddress after our Engine replace MarsEngine
				return new BattleResult
				{
					RunToCompletion = true,
					Player1Result = new BattlePlayerResult { Player = battle.Player1, Score = GetScore(0, winner), StartAddress = 42 },
					Player2Result = new BattlePlayerResult { Player = battle.Player2, Score = GetScore(1, winner), StartAddress = 42 },
				};
			}
			catch (Exception e)
			{
				log.Error(string.Format("Battle failed: {0}", battle), e);
				return new BattleResult { RunToCompletion = false };
			}
		}

		private static int GetScore(int player, int? winner)
		{
			return !winner.HasValue
				? 1
				: player == winner.Value ? 3 : 0;
		}
	}
}