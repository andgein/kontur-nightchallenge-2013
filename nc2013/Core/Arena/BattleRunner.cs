using System;
using Core.Engine;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public class BattleRunner : IBattleRunner
	{
		private readonly Rules rules;
		private readonly Random rnd = new Random();

		public BattleRunner()
		{
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
		}

		[NotNull]
		public GameState RunBattle([NotNull] Battle battle)
		{
			var programStartInfos = battle.GetProgramStartInfos();
			programStartInfos[0].StartAddress = 0;
			programStartInfos[1].StartAddress = NextLoadAddress(0);
			var game = new MarsGame(rules, programStartInfos);
			game.StepToEnd();
			return game.GameState;
		}

		private int NextLoadAddress(int baseAddress)
		{
			var positions = rules.CoreSize + 1 - (rules.MinDistance << 1);
			var nextLoadAddress = ModularArith.Mod(baseAddress + rules.MinDistance + rnd.Next() % positions);
			return nextLoadAddress;
		}
	}
}