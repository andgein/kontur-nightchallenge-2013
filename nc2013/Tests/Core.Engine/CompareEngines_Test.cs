using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Game;
using Core.Game.MarsBased;
using Core.Parser;
using NUnit.Framework;
using Core.Engine;
using Newtonsoft.Json;
using nMars.RedCode;

namespace Tests.Core.Engine
{
	[TestFixture]
	public class CompareEngines_Test
	{
		public string[] BotsOk = TestWarriors.GetBotFiles("warriors-ok").ToArray();
		public string[] BotsBad = TestWarriors.GetBotFiles("warriors-bad").ToArray();

		[Test]
		[TestCaseSource("BotsOk")]
		public void TestOk(string bot)
		{
			Compare(File.ReadAllText(bot));
		}

		[Test]
		[TestCaseSource("BotsBad")]
		public void TestBad(string bot)
		{
			Compare(File.ReadAllText(bot));
		}

		[Test]
//		[TestCase("0002-imp")]
//		[TestCase("0001-dwarf")]
//		[TestCase("0828-NULL")]
//		[TestCase("0646-elf")]
		[TestCase("0015-backimp")]
		public void TestOne(string name)
		{
			var program = File.ReadAllText(TestWarriors.GetBotFile(@"warriors-ok\" + name + ".red"));
			Compare(program);
		}

		[Test]
		public void TestDJN()
		{
			Compare("DJN 0, 1");
		}

		public void Compare(string program)
		{
			Console.WriteLine(program);
			var programStartInfos = new[] { new ProgramStartInfo { Program = program, StartAddress = 0 } };
			var ourGame = new Game(programStartInfos);
			var marsGame = new MarsGame(new Rules() {WarriorsCount = 1}, programStartInfos);
			for (int i = 0; i < 30; i++)
			{
				ourGame.Step(1);
				marsGame.Step(1);
				CompareStates(ourGame.GameState, marsGame.GameState, i);
			}
//			ourGame.Step(100);
//			marsGame.Step(100);
			CompareStates(ourGame.GameState, marsGame.GameState, 100);
		}

		private void CompareStates(GameState our, GameState mars, int i)
		{
			var m = Normalize(mars);
			var o = Normalize(our);
			Assert.AreEqual(m, o, "Step: " + i);
		}

		private string Normalize(GameState gs)
		{
			var s = JsonConvert.SerializeObject(gs);
			foreach (var x in new[] {".AB", ".A", ".B", ".I", ".F", ".X", " ", "\t", ","})
				s = s.Replace(x, "");
			s = Regex.Replace(s, "LastModifiedByProgram\":.*\\}", "}");
			return s;
		}
	}
}
