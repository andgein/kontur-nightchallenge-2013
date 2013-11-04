using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Game;
using Core.Game.MarsBased;
using NUnit.Framework;
using Newtonsoft.Json;
using nMars.RedCode;

namespace Tests.Core.Engine
{
	[TestFixture]
	public class CompareEngines_Test
	{
		public string[] BotsOk = TestWarriors.GetBotFiles("warriors-ok").ToArray();
//		public string[] BotsBad = TestWarriors.GetBotFiles("warriors-bad").ToArray();

		[Test]
		[TestCaseSource("BotsOk")]
		public void TestOk(string bot)
		{
			Compare(File.ReadAllText(bot));
		}

//		[Test]
//		[TestCaseSource("BotsBad")]
//		public void TestBad(string bot)
//		{
//			Compare(File.ReadAllText(bot));
//		}

		[Test]
//		[TestCase("0002-imp")]
//		[TestCase("0001-dwarf")]
//		[TestCase("0828-NULL")]
//		[TestCase("0646-elf")]
		public void TestOne(string name)
		{
			var program = File.ReadAllText(TestWarriors.GetBotFile(@"warriors-ok\" + name + ".red"));
			Compare(program);
		}

		[Test]
		public void TestSLT()
		{
			Compare(
@"DAT #0, #1
SLT #17, -1
MOV 1, 1
DAT #0, #0
");
		}

		public void Compare(string program)
		{
			Console.WriteLine(program);
			GameState our;
			GameState mars;
			Run(program, 79999, out our, out mars);
			if (!AreEqual(our, mars))
			{
				var step = FindFirstErrorStep(program, 0, 80000, out our, out mars);
				Assert.IsTrue(step < 79999);
				Console.WriteLine("Error on step " + step);
				File.WriteAllText(@"mars.txt", JsonConvert.SerializeObject(mars, Formatting.Indented));
				File.WriteAllText(@"our.txt", JsonConvert.SerializeObject(our, Formatting.Indented));
				Assert.AreEqual(Normalize(mars), Normalize(our));
			}
		}

		private int FindFirstErrorStep(string program, int minSteps, int maxSteps, out GameState our, out GameState mars)
		{
			our = null;
			mars = null;
			while (maxSteps - minSteps > 1)
			{
				int m = (minSteps + maxSteps)/2;
				Run(program, m, out our, out mars);
				if (AreEqual(mars, our)) 
					minSteps = m;
				else maxSteps = m;
			}
			return maxSteps;
		}

		private void Run(string program, int stepsCount, out GameState our, out GameState mars)
		{
			var programStartInfos = new[] { new ProgramStartInfo { Program = program, StartAddress = 0 } };
			var ourGame = new Game(programStartInfos);
			var marsGame = new MarsGame(new Rules() { MaxLength = 1000, WarriorsCount = 1 }, programStartInfos);
			ourGame.Step(stepsCount);
			marsGame.Step(stepsCount);
			our = ourGame.GameState;
			mars = marsGame.GameState;
		}

		private bool AreEqual(GameState mars, GameState our)
		{
			var m = Normalize(mars);
			var o = Normalize(our);
			return m == o;
		}

		private static Regex r = new Regex("LastModifiedByProgram\":.*?\\}", RegexOptions.Compiled);
		private string Normalize(GameState gs)
		{
			var s = JsonConvert.SerializeObject(gs);
			return Normalize(s);
		}

		private static string Normalize(string s)
		{
			foreach (var x in new[] {".AB", ".A", ".B", ".I", ".F", ".X", " ", "\t", ",", "\"Winner\":0", "\"Winner\":null",})
				s = s.Replace(x, "");
			s = r.Replace(s, "}");
			return s;
		}
	}
}
