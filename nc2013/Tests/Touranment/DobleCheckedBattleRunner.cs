using System;
using System.Text.RegularExpressions;
using Core.Arena;
using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Touranment
{
	public class DobleCheckedBattleRunner : BattleRunner
	{
		private static readonly Regex r = new Regex("LastModifiedByProgram\":.*?\\}", RegexOptions.Compiled);

		protected override void PostProcessBattle(Battle battle, [NotNull] ProgramStartInfo[] programStartInfos, [NotNull] GameState finalGameState)
		{
			var marsFinalGameState = GetFinalGameStateByMars(programStartInfos);
			var m = Normalize(marsFinalGameState);
			var o = Normalize(finalGameState);
			if (o != m)
			{
				Console.WriteLine("Different battle results for:");
				Console.WriteLine("Player1: {0}", battle.Player1);
				Console.WriteLine("Player2: {0}", battle.Player2);
			}
			Assert.That(o, Is.EqualTo(m));
			Assert.That(finalGameState.GameOver, Is.EqualTo(marsFinalGameState.GameOver));
			Assert.That(finalGameState.Winner, Is.EqualTo(marsFinalGameState.Winner));
		}

		[NotNull]
		private GameState GetFinalGameStateByMars([NotNull] ProgramStartInfo[] programStartInfos)
		{
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