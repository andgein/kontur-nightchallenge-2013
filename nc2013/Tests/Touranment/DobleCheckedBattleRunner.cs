using System;
using System.Collections.Generic;
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

		public DobleCheckedBattleRunner()
		{
			DifferentResults = new List<string>();
		}

		public List<string> DifferentResults { get; private set; }

		protected override void PostProcessBattle(Battle battle, [NotNull] ProgramStartInfo[] programStartInfos, [NotNull] GameState finalGameState)
		{
			var marsFinalGameState = GetFinalGameStateByMars(programStartInfos);
			Assert.That(finalGameState.ProgramStartInfos[0].StartAddress, Is.EqualTo(marsFinalGameState.ProgramStartInfos[0].StartAddress));
			Assert.That(finalGameState.ProgramStartInfos[1].StartAddress, Is.EqualTo(marsFinalGameState.ProgramStartInfos[1].StartAddress));
			var m = Normalize(marsFinalGameState);
			var o = Normalize(finalGameState);
			if (o != m)
			{
				DifferentResults.Add(string.Format("StartAddress1: {0} Player1: {1}\r\nStartAddress2: {2} Player2: {3}", finalGameState.ProgramStartInfos[0].StartAddress, battle.Player1, finalGameState.ProgramStartInfos[1].StartAddress, battle.Player2));
			}
			//Assert.That(finalGameState.GameOver, Is.EqualTo(marsFinalGameState.GameOver));
			//Assert.That(finalGameState.Winner, Is.EqualTo(marsFinalGameState.Winner));
			//Assert.That(o, Is.EqualTo(m));
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