using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		public string[] Bots = TestWarriors.GetBotFiles("warriors-ok").ToArray();

		[Test]
		[TestCaseSource("Bots")]
		public void Test(string bot)
		{
			Compare(bot);
		}

		[Test]
		[TestCase("0002-imp")]
		[TestCase("0001-dwarf")]
		[TestCase("0828-NULL")]
		[TestCase("0646-elf")]
		public void TestOne(string name)
		{
			Compare(TestWarriors.GetBotFile(@"warriors-ok\" + name + ".red"));
		}

		public void Compare(string botFile)
		{
			var programStartInfos = new[] {new ProgramStartInfo {Program = File.ReadAllText(botFile), StartAddress = 0}};
			var ourGame = new Game(programStartInfos);
			var marsGame = new MarsGame(new Rules() {WarriorsCount = 1}, programStartInfos);
			for (int i = 0; i < 20; i++)
			{
				ourGame.Step(1);
				marsGame.Step(1);
				CompareStates(ourGame.GameState, marsGame.GameState, i);
			}
			ourGame.Step(1000);
			marsGame.Step(1000);
			CompareStates(ourGame.GameState, marsGame.GameState, 1000);
		}

		private void CompareStates(GameState ourState, GameState marsState, int i)
		{
			var our = JsonConvert.SerializeObject(ourState);
			var mars = JsonConvert.SerializeObject(marsState);
			Assert.AreEqual(Normalize(mars), Normalize(our), "Step: " + i);
		}

		private string Normalize(string s)
		{
			foreach (var x in new[] {".AB", ".A", ".B", ".I", ".F", ".X", " ", "\t", ","})
				s = s.Replace(x, "");
			return s;
		}
	}
}
