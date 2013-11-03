using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Parser
{
	[TestFixture]
	public class WarriorParser_BigTest
	{
		private readonly List<Tuple<string, string>> exceptions = new List<Tuple<string, string>>();

		[Test]
		[TestCaseSource("hill88Bots")]
		public void Try_parse_all_hill88_bots(string botFile)
		{
			var bot = File.ReadAllText(botFile);
			Console.WriteLine(bot);
			try
			{
				new WarriorParser().Parse(bot);
			}
			catch (CompilationException e)
			{
				exceptions.Add(Tuple.Create(e.Error, e.Line));
				throw;
			}
			catch (Exception e)
			{
				exceptions.Add(Tuple.Create(e.Message, ""));
				throw;
			}
		}

		[TestFixtureSetUp]
		public void SetUp()
		{
			Console.WriteLine("Exception statistics will be shown in the last test output");
			exceptions.Clear();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			Console.WriteLine("Exception statistics:");
			foreach (var ex in exceptions.GroupBy(e => e.Item1, (exception, group) => new {exception, sampleLine = group.First().Item2, count = group.Count()}).OrderByDescending(p => p.count))
			{
				Console.WriteLine(ex.count.ToString().PadLeft(10) + "  " + ex.exception + "  " + ex.sampleLine);
			}
		}

		private static IEnumerable<string> hill88Bots
		{
			get
			{
				const string path = @"..\..\..\..\warriors\88-HILL";
				Console.WriteLine(Path.GetFullPath(path));
				return Directory.EnumerateFiles(path, "*.red");
			}
		}
	}
}