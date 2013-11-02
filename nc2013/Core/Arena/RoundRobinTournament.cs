using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using log4net;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RoundRobinTournament));
		private readonly int battlesPerPair;
		private readonly TournamentPlayer[] players;
		private readonly string tournamentId;

		public RoundRobinTournament(int battlesPerPair, [NotNull] string tournamentId, [NotNull] TournamentPlayer[] players)
		{
			this.battlesPerPair = battlesPerPair;
			this.tournamentId = tournamentId;
			this.players = players;
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
					Score = g.Sum(r => r.Score()),
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
			for (var i = 0; i < battlesPerPair; i++)
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
		}

		[NotNull]
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
					Player1Result = new BattlePlayerResult
					{
						Player = battle.Player1,
						StartAddress = 42,
						ResultType = !winner.HasValue ? BattlePlayerResultType.Draw : (winner.Value == 0 ? BattlePlayerResultType.Win : BattlePlayerResultType.Loss),
					},
					Player2Result = new BattlePlayerResult
					{
						Player = battle.Player2,
						StartAddress = 42,
						ResultType = !winner.HasValue ? BattlePlayerResultType.Draw : (winner.Value == 1 ? BattlePlayerResultType.Win : BattlePlayerResultType.Loss),
					},
				};
			}
			catch (Exception e)
			{
				log.Error(string.Format("Battle failed: {0}", battle), e);
				return new BattleResult { RunToCompletion = false };
			}
		}
	}
}