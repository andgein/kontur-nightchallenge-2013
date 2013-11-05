using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Arena;
using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;
using nMars.RedCode;
using NUnit.Framework;

namespace Tests.Touranment
{
	public class DobleCheckedBattleRunner : BattleRunner
	{
		private static readonly Regex r1 = new Regex("LastModifiedByProgram\":.*?\\}", RegexOptions.Compiled);
		private static readonly Regex r2 = new Regex("LastPointer\":.*?\"", RegexOptions.Compiled);
		private static readonly Regex r3 = new Regex("CurrentInstruction\":.*?\\}", RegexOptions.Compiled);
		private static readonly Regex r4 = new Regex("CurrentProgram\":.*?\"", RegexOptions.Compiled);

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
			if (finalGameState.GameOver != marsFinalGameState.GameOver || finalGameState.Winner != marsFinalGameState.Winner)
			{
				BattlesWithDifferentResults.Add(battle);
			}
			var m = Normalize(marsFinalGameState);
			var o = Normalize(finalGameState);
			Assert.That(o, Is.EqualTo(m));
		}

		private static string Normalize(GameState gs)
		{
			var s = JsonConvert.SerializeObject(gs);
			return Normalize(s);
		}

		private static string Normalize(string s)
		{
			foreach (var x in new[] { ".AB", ".A", ".B", ".I", ".F", ".X", " ", "\t", ",", })
				s = s.Replace(x, "");
			s = r1.Replace(s, "}");
			s = r2.Replace(s, "\"");
			s = r3.Replace(s, "}");
			s = r4.Replace(s, "\"");
			return s;
		}
	}
}