using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Engine;
using Core.Parser;
using JetBrains.Annotations;
using nMars.RedCode;

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
		private readonly Rules rules;
		private readonly Random rnd = new Random();
		private readonly WarriorParser warriorParser = new WarriorParser();
		private readonly RandomAllocator randomAllocator;

		public RoundRobinTournament([NotNull] IBattleRunner battleRunner, int battlesPerPair, [NotNull] string tournamentId, [NotNull] TournamentPlayer[] players, [CanBeNull] AutoResetEvent botSubmissionSignal, [CanBeNull]ManualResetEvent stopSignal, bool suppressBattleErrors = true)
		{
			this.battleRunner = battleRunner;
			this.battlesPerPair = battlesPerPair;
			this.tournamentId = tournamentId;
			this.players = players;
			this.botSubmissionSignal = botSubmissionSignal;
			this.stopSignal = stopSignal;
			this.suppressBattleErrors = suppressBattleErrors;
			rules = new Rules
			{
				WarriorsCount = 2,
				Rounds = 1,
				MaxCycles = Parameters.MaxStepsPerWarrior,
				CoreSize = Parameters.CoreSize,
				PSpaceSize = 500, // coreSize / 16 
				EnablePSpace = false,
				MaxProcesses = Parameters.MaxQueueSize,
				MaxLength = Parameters.MaxWarriorLength,
				MinDistance = Parameters.MinWarriorsDistance,
				Version = 93,
				ScoreFormula = ScoreFormula.Standard,
				ICWSStandard = ICWStandard.ICWS88,
			};
			this.randomAllocator = new RandomAllocator(rules.CoreSize, rules.MinDistance);
		}

		[NotNull]
		public RoundRobinTournamentResult Run()
		{
			ParseWarriors();
			var pairs = GenerateAllPairs();
			var battleResults = RunTournament(pairs).ToList();
			var ranking = Ranking.MakeRankingTable(tournamentId, battleResults);
			return new RoundRobinTournamentResult
			{
				BattleResults = battleResults,
				TournamentRanking = ranking,
			};
		}

		private void ParseWarriors()
		{
			foreach (var player in players)
				player.Warrior = warriorParser.Parse(player.Program);
		}

		[NotNull]
		private List<Tuple<TournamentPlayer, TournamentPlayer>> GenerateAllPairs()
		{
			var result = new List<Tuple<TournamentPlayer, TournamentPlayer>>();
			for (var i = 0; i < players.Length; i++)
				for (var j = 0; j < players.Length; j++)
					if (i != j)
						result.Add(Tuple.Create(players[i], players[j]));
			return result;
		}

		[NotNull]
		private IEnumerable<BattleResult> RunTournament([NotNull] List<Tuple<TournamentPlayer, TournamentPlayer>> pairs)
		{
			var battleCount = 0;
			if (botSubmissionSignal != null)
				botSubmissionSignal.WaitOne(0);
			for (var i = 0; i < battlesPerPair; i++)
			{
				foreach (var pair in pairs)
				{
					var battle = new Battle
					{
						Number = ++battleCount,
						Player1 = pair.Item1,
						StartAddress1 = 0,
						Player2 = pair.Item2,
						StartAddress2 = randomAllocator.NextLoadAddress(pair.Item1.Warrior.Length, pair.Item2.Warrior.Length),
					};
					var battleResult = RunBattle(battle);
					if (battleResult.RunToCompletion)
						yield return battleResult;
					if (battleCount % 500 == 1)
						Log.Perf.InfoFormat("Battles performed: {0}", battleCount);
				}
				if (botSubmissionSignal != null && botSubmissionSignal.WaitOne(0) || stopSignal != null && stopSignal.WaitOne(0))
					yield break;
				rnd.Shuffle(pairs);
			}
		}

		private static BattlePlayerResultType GetResultForPlayer(int player, int? winner)
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
				var winner = battleRunner.RunBattle(rules, battle);
				var p1 = new BattlePlayerResult
				{
					Player = battle.Player1,
					StartAddress = battle.StartAddress1,
					ResultType = GetResultForPlayer(0, winner),
				};
				var p2 = new BattlePlayerResult
				{
					Player = battle.Player2,
					StartAddress = battle.StartAddress2,
					ResultType = GetResultForPlayer(1, winner),
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