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
		const string warriorsOk = basePath + "warriors-ok\\";
		[Test]
		public void Try_parse_all_hill88_bots()
		{
			if (Directory.Exists(warriorsOk))
				Directory.Delete(warriorsOk, true);
			Directory.CreateDirectory(warriorsOk);

			var exceptions = new List<Tuple<string, string>>();
			int botIndex = 0;
			foreach (var botFile in allBots)
			{
				var bot = File.ReadAllText(botFile);
				try
				{
					new WarriorParser().Parse(bot);
					Console.WriteLine(botIndex + "  " + Path.GetFileName(botFile));
					File.Copy(botFile, warriorsOk + (botIndex++).ToString("0000") + "-" + Path.GetFileName(botFile), true);
				}
				catch (CompilationException e)
				{
//					Console.WriteLine(e.Message);
					exceptions.Add(Tuple.Create(e.Error, e.Line));
				}
				catch (Exception e)
				{
//					Console.WriteLine(e.Message);
					exceptions.Add(Tuple.Create(e.Message, ""));
				}
			}
			Console.WriteLine("Exception statistics:");
			foreach (var ex in exceptions.GroupBy(e => e.Item1, (exception, group) => new { exception, sampleLine = group.First().Item2, count = group.Count() }).OrderByDescending(p => p.count))
			{
				Console.WriteLine(ex.count.ToString().PadLeft(10) + "  " + ex.exception + "  " + ex.sampleLine);
			}
		}

		private const string basePath = @"..\..\..\..\";
		private static IEnumerable<string> hill88Bots
		{
			get
			{
				const string path = basePath + @"warriors\88-HILL";
				Console.WriteLine(Path.GetFullPath(path));
				return Directory.EnumerateFiles(path, "*.red");
			}
		}
		private static IEnumerable<string> allBots
		{
			get
			{
				const string path = basePath + @"warriors";
				Console.WriteLine(Path.GetFullPath(path));
				return Directory.EnumerateFiles(path, "*.red", SearchOption.AllDirectories);
			}
		}
	}
}