using System;
using System.Collections.Generic;
using System.IO;
using Core;
using nMars.Engine;
using nMars.Parser;
using nMars.RedCode;
using nMars.RedCode.Modules;
using NUnit.Framework;
using Tests.Mars;

namespace Tests
{
	[TestFixture]
	public class MarsEngineRunner
	{
		private WrappedConsole wrappedConsole;
		private ComponentLoader components;
		private string basePath;
		private Rules rules;

		[SetUp]
		public void SetUp()
		{
			wrappedConsole = new WrappedConsole();
			components = new ComponentLoader();
			components.Parser = new WarriorParser();
			components.Engine = new EngineSteps();
			basePath = SettingsFileHelper.PatchDirectoryName("warriors", Directory.GetCurrentDirectory());
			rules = Rules.DefaultRules;
		}

		[Test]
		public void Single()
		{
			Console.WriteLine("Single");
			rules.Rounds = 3;
			rules.WarriorsCount = 1;
			//var fileOne = Path.Combine(basePath, "maniacs/5/coleman5.red");
			var fileOne = Path.Combine(basePath, @"pycorewar\Koenigstuhl\94\aggression.red");
			var midleOne = fileOne.Substring(basePath.Length);
			Console.Write("Fighting {0}      \r", midleOne);
			var pproject = new Project(rules, fileOne);
			pproject.ParserOptions.Instructions = false;
			pproject.ParserOptions.StatusLine = false;
			var pr = components.Parser.Parse(pproject, wrappedConsole);
			if (pr.Succesfull)
				components.Engine.Run(pproject, wrappedConsole);
			Console.WriteLine("\nDone");
		}

		[Test]
		public void Pair()
		{
			Console.WriteLine("Pair");
			LoadRunPair(
				Path.Combine(basePath, @"imp.red"),
				Path.Combine(basePath, @"dwarf.red"),
				0);
			Console.WriteLine("\nDone");
		}

		[Test]
		public void Full()
		{
			Console.WriteLine("Full");

			List<string> files = new List<string>(Directory.GetFiles(basePath, "*.rc", SearchOption.AllDirectories));
			files.AddRange(Directory.GetFiles(basePath, "*.red", SearchOption.AllDirectories));
			files.Sort();
			bool allOK = true;
			int c = 0;
			for (int i = 0; i < files.Count; i++)
			{
				for (int j = 0; j < files.Count; j++)
				{
					try
					{
						LoadRunPair(files[i], files[j], c);
						c++;
					}
					catch (EngineDifferException)
					{
						Console.WriteLine("\nFailed");
						allOK = false;
					}
					catch (ParserException)
					{
						//swallow
					}
				}
			}
			Console.WriteLine("\nDone");
			if (!allOK)
				throw new EngineDifferException("Some warriors failed.", null);
		}

		[Test]
		public void Random(int count)
		{
			Console.WriteLine("Random");

			List<string> files = new List<string>(Directory.GetFiles(basePath, "*.rc", SearchOption.AllDirectories));
			files.AddRange(Directory.GetFiles(basePath, "*.red", SearchOption.AllDirectories));
			files.Sort();
			Random r = new Random();

			bool allOK = true;
			for (int i = 0; i < count; i++)
			{
				try
				{
					LoadRunPair(files[r.Next(files.Count)], files[r.Next(files.Count)], i);
				}
				catch (EngineDifferException)
				{
					Console.WriteLine("\nFailed");
					allOK = false;
				}
				catch (ParserException)
				{
					//swallow
				}
			}
			Console.WriteLine("\nDone");
			if (!allOK)
				throw new EngineDifferException("Some warriors failed.", null);
		}

		private void LoadRunPair(string fileOne, string fileTwo, int counter)
		{
			var pproject = new Project(rules);
			pproject.ParserOptions.Instructions = false;
			pproject.ParserOptions.StatusLine = false;
			pproject.WarriorFiles.Add(fileOne);
			pproject.WarriorFiles.Add(fileTwo);

			var pr = components.Parser.Parse(pproject, wrappedConsole);
			if (!pr.Succesfull)
				return;

			var name1 = pproject.Warriors[0].Name;
			var name2 = pproject.Warriors[1].Name;
			name1 = name1.Substring(0, Math.Min(name1.Length, 20));
			name2 = name2.Substring(0, Math.Min(name2.Length, 20));
			Console.Write("{2} Fighting {0} and {1}         \r", name1, name2, counter);
			try
			{
				components.Engine.Run(pproject, wrappedConsole);
			}
			catch (EngineDifferException)
			{
				Console.WriteLine();
				Console.WriteLine(pproject.Warriors[0].FileName);
				Console.WriteLine(pproject.Warriors[1].FileName);
				throw;
			}
		}
	}
}