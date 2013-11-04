using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Arena;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using Newtonsoft.Json;
using nMars.RedCode;
using NUnit.Framework;

namespace Tests.Touranment
{
	public class DobleCheckedBattleRunner : BattleRunner
	{
		private static readonly Regex r = new Regex("LastModifiedByProgram\":.*?\\}", RegexOptions.Compiled);

		public DobleCheckedBattleRunner()
		{
			BattlesWithDifferentResults = new List<Battle>();
		}

		public List<Battle> BattlesWithDifferentResults { get; private set; }

		protected override void PostProcessBattle([NotNull] Rules rules, [NotNull] Battle battle, [NotNull] GameState finalGameState)
		{
			var marsFinalGameState = GetFinalGameStateByMars(rules, battle);
			Assert.That(finalGameState.ProgramStartInfos[0].StartAddress, Is.EqualTo(marsFinalGameState.ProgramStartInfos[0].StartAddress));
			Assert.That(finalGameState.ProgramStartInfos[1].StartAddress, Is.EqualTo(marsFinalGameState.ProgramStartInfos[1].StartAddress));
			var m = Normalize(marsFinalGameState);
			var o = Normalize(finalGameState);
			if (o != m)
			{
				BattlesWithDifferentResults.Add(battle);
			}
			//Assert.That(finalGameState.GameOver, Is.EqualTo(marsFinalGameState.GameOver));
			//Assert.That(finalGameState.Winner, Is.EqualTo(marsFinalGameState.Winner));
			//Assert.That(o, Is.EqualTo(m));
		}

		[NotNull]
		private static GameState GetFinalGameStateByMars([NotNull] Rules rules, [NotNull] Battle battle)
		{
			var programStartInfos = battle.GetProgramStartInfos();
			var game = new MarsGame(rules, programStartInfos);
			game.StepToEnd();
			return game.GameState;
		}

		private static string Normalize(GameState gs)
		{
			var s = JsonConvert.SerializeObject(gs);
			return Normalize(s);
		}

		private static string Normalize(string s)
		{
			foreach (var x in new[] { ".AB", ".A", ".B", ".I", ".F", ".X", " ", "\t", ",", "\"Winner\":0", "\"Winner\":null", })
				s = s.Replace(x, "");
			s = r.Replace(s, "}");
			return s;
		}
	}
}