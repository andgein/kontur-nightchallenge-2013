using System;
using System.IO;
using Core;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using nMars.Engine;
using nMars.Parser;
using nMars.Parser.Warrior;
using nMars.RedCode;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class MarsEngineRunner
	{
		private WrappedConsole wrappedConsole;
		private string basePath;

		[SetUp]
		public void SetUp()
		{
			wrappedConsole = new WrappedConsole();
			basePath = SettingsFileHelper.PatchDirectoryName("warriors", Directory.GetCurrentDirectory());
		}

		[Test]
		public void Single()
		{
			var rules = Rules.DefaultRules;
			rules.WarriorsCount = 1;

			//var fileOne = Path.Combine(basePath, "maniacs/5/coleman5.red");
			//var fileOne = Path.Combine(basePath, @"pycorewar\Koenigstuhl\94\aggression.red");
			var fileOne = Path.Combine(basePath, @"imp.red");

			var pproject = new Project(rules, fileOne);
			pproject.ParserOptions.Instructions = true;
			pproject.ParserOptions.StatusLine = false;

			var pr = new WarriorParser().Parse(pproject, wrappedConsole);
			if (pr.Succesfull)
				new EngineSteps().Run(pproject, wrappedConsole);
		}

		[Test]
		public void Pair()
		{
			Console.WriteLine("Pair");
			LoadRunPair(
				Path.Combine(basePath, @"imp.red"),
				Path.Combine(basePath, @"dwarf.red")
				);
			Console.WriteLine("\nDone");
		}

		private void LoadRunPair(string fileOne, string fileTwo)
		{
			var rules = Rules.DefaultRules;
			var pproject = new Project(rules);
			pproject.ParserOptions.Instructions = false;
			pproject.ParserOptions.StatusLine = false;
			pproject.WarriorFiles.Add(fileOne);
			pproject.WarriorFiles.Add(fileTwo);

			var pr = new WarriorParser().Parse(pproject, wrappedConsole);
			if (!pr.Succesfull)
				return;

			var name1 = pproject.Warriors[0].Name;
			var name2 = pproject.Warriors[1].Name;
			name1 = name1.Substring(0, Math.Min(name1.Length, 20));
			name2 = name2.Substring(0, Math.Min(name2.Length, 20));
			Console.Write("Fighting {0} and {1}         \r", name1, name2);
			new EngineSteps().Run(pproject, wrappedConsole);
		}

		[Test]
		public void Run()
		{
			var rules = Rules.DefaultRules;
			rules.WarriorsCount = 2;
			var f1 = Path.Combine(basePath, @"imp.red");
			var f2 = Path.Combine(basePath, @"dwarf.red");
			var w1 = ParseWarrior(rules, File.ReadAllText(f1), f1);
			var w2 = ParseWarrior(rules, File.ReadAllText(f2), f2);
			var project = new MarsProject(rules, w1, w2);
			var engine = new MarsEngine(project);
			var mr = engine.Run();
			mr.Dump(wrappedConsole, project);
		}

		[NotNull]
		private static ExtendedWarrior ParseWarrior([NotNull] Rules rules, [NotNull] string source, [NotNull] string filename)
		{
			var warriorParser = new MarsWarriorParser(rules);
			var implicitName = Path.GetFileNameWithoutExtension(filename);
			var warrior = warriorParser.TryParse(source, implicitName);
			if (warrior == null)
				throw new InvalidOperationException(string.Format("Failed to parse warrior {0}: {1}", implicitName, source));
			warrior.FileName = filename;
			return warrior;
		}
	}
}