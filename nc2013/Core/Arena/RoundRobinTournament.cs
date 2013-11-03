using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public class RoundRobinTournament
	{
		private readonly int battlesPerPair;
		private readonly string tournamentId;
		private readonly TournamentPlayer[] players;
		private readonly AutoResetEvent botSubmissionSignal;
		private readonly Random rnd = new Random();

		public RoundRobinTournament(int battlesPerPair, [NotNull] string tournamentId, [NotNull] TournamentPlayer[] players, [CanBeNull] AutoResetEvent botSubmissionSignal)
		{
			this.battlesPerPair = battlesPerPair;
			this.tournamentId = tournamentId;
			this.players = players;
			this.botSubmissionSignal = botSubmissionSignal;
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
				if (botSubmissionSignal != null && botSubmissionSignal.WaitOne(0))
					yield break;
			}
		}

		[NotNull]
		private BattleResult RunBattle([NotNull] Battle battle)
		{
			try
			{
				var gameState = GetFinalGameStateForBattle(battle);
				var winner = gameState.Winner;
				return new BattleResult
				{
					RunToCompletion = true,
					Player1Result = new BattlePlayerResult
					{
						Player = battle.Player1,
						StartAddress = (int)gameState.ProgramStartInfos[0].StartAddress,
						ResultType = !winner.HasValue ? BattlePlayerResultType.Draw : (winner.Value == 0 ? BattlePlayerResultType.Win : BattlePlayerResultType.Loss),
					},
					Player2Result = new BattlePlayerResult
					{
						Player = battle.Player2,
						StartAddress = (int)gameState.ProgramStartInfos[1].StartAddress,
						ResultType = !winner.HasValue ? BattlePlayerResultType.Draw : (winner.Value == 1 ? BattlePlayerResultType.Win : BattlePlayerResultType.Loss),
					},
				};
			}
			catch (Exception e)
			{
				Log.For(this).Error(string.Format("Battle failed: {0}", battle), e);
				return new BattleResult { RunToCompletion = false };
			}
		}

		[NotNull]
		private static GameState GetFinalGameStateForBattle([NotNull] Battle battle)
		{
			var rules = new Rules
			{
				WarriorsCount = 2,
				Rounds = 1,
				MaxCycles = 80000,
				CoreSize = 8000,
				PSpaceSize = 500, // coreSize / 16 
				EnablePSpace = false,
				MaxProcesses = 1000,
				MaxLength = 100,
				MinDistance = 100,
				Version = 93,
				ScoreFormula = ScoreFormula.Standard,
				ICWSStandard = ICWStandard.ICWS88,
			};
			var game = new MarsGame(rules, battle.GetProgramStartInfos());
			game.StepToEnd();
			return game.GameState;
		}
	}
}