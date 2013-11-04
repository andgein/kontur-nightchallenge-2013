using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private readonly IBattleRunner battleRunner;
		private readonly int battlesPerPair;
		private readonly string tournamentId;
		private readonly TournamentPlayer[] players;
		private readonly AutoResetEvent botSubmissionSignal;
		private readonly ManualResetEvent stopSignal;
		private readonly bool suppressBattleErrors;
		private readonly Random rnd = new Random();

		public RoundRobinTournament([NotNull] IBattleRunner battleRunner, int battlesPerPair, [NotNull] string tournamentId, [NotNull] TournamentPlayer[] players, [CanBeNull] AutoResetEvent botSubmissionSignal, [CanBeNull]ManualResetEvent stopSignal, bool suppressBattleErrors = true)
		{
			this.battleRunner = battleRunner;
			this.battlesPerPair = battlesPerPair;
			this.tournamentId = tournamentId;
			this.players = players;
			this.botSubmissionSignal = botSubmissionSignal;
			this.stopSignal = stopSignal;
			this.suppressBattleErrors = suppressBattleErrors;
		}

		[NotNull]
		public RoundRobinTournamentResult Run()
		{
			var battleResults = RunTournament(GenerateAllPairs()).ToList();
			var ranking = MakeRankingTable(battleResults.SelectMany(r => r.Results).ToList());
			return new RoundRobinTournamentResult
			{
				BattleResults = battleResults,
				TournamentRanking = ranking,
			};
		}

		[NotNull]
		private List<Tuple<TournamentPlayer, TournamentPlayer>> GenerateAllPairs()
		{
			var result = new List<Tuple<TournamentPlayer, TournamentPlayer>>();
			for (var i = 0; i < players.Length; i++)
				for (var j = i + 1; j < players.Length; j++)
					result.Add(Tuple.Create(players[i], players[j]));
			return result;
		}

		[NotNull]
		private TournamentRanking MakeRankingTable([NotNull] List<BattlePlayerResult> results)
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

		[NotNull]
		private IEnumerable<BattleResult> RunTournament([NotNull] List<Tuple<TournamentPlayer, TournamentPlayer>> pairs)
		{
			if (botSubmissionSignal != null)
				botSubmissionSignal.WaitOne(0);
			for (var i = 0; i < battlesPerPair; i++)
			{
				rnd.Shuffle(pairs);
				foreach (var pair in pairs)
				{
					var battle = new Battle
					{
						Player1 = pair.Item1,
						Player2 = pair.Item2,
					};
					var battleResult = RunBattle(battle);
					if (battleResult.RunToCompletion)
						yield return battleResult;
				}
				if (botSubmissionSignal != null && botSubmissionSignal.WaitOne(0) || stopSignal != null && stopSignal.WaitOne(0))
					yield break;
			}
		}
		private BattlePlayerResultType GetResultForPlayer(int player, int? winner)
		{
			if (!winner.HasValue) return BattlePlayerResultType.Draw;
			if (winner == player) return BattlePlayerResultType.Win;
			return BattlePlayerResultType.Loss;
		}

		[NotNull]
		private BattleResult RunBattle([NotNull] Battle battle)
		{
			try
			{
				var gameState = battleRunner.RunBattle(battle);
				var winner = gameState.Winner;
				var res1 = GetResultForPlayer(0, winner);
				var p1 = new BattlePlayerResult
				{
					Player = battle.Player1, 
//					StartAddress = gameState.ProgramStartInfos[0].StartAddress ?? 0, 
					ResultType = res1,
				};
				var res2 = GetResultForPlayer(1, winner);
				var p2 = new BattlePlayerResult
				{
					Player = battle.Player2, 
//					StartAddress = (int) gameState.ProgramStartInfos[1].StartAddress, 
					ResultType = res2,
				};
				return new BattleResult
				{
					RunToCompletion = true,
					Player1Result = p1,
					Player2Result = p2,
				};
			}
			catch (Exception e)
			{
				if (!suppressBattleErrors)
					throw;
				Log.For(this).Error(string.Format("Battle failed: {0}", battle), e);
				return new BattleResult { RunToCompletion = false };
			}
		}
	}
}