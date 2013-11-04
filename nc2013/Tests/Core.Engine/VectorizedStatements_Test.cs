using System;
using System.Linq;
using Core.Engine;
using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Engine
{
	[TestFixture]
	public class VectorizedStatements_Test
	{
		private readonly WarriorParser parser = new WarriorParser();

		[Test]
		public void AddImmediateToDirect()
		{
			Run("ADD.4 #1, 1",
				@"ADD.4 #1, $1
				DAT $0, $1
				DAT $0, $1
				DAT $0, $1
				DAT $0, $1
				DAT $0, $0");
		}

		[Test]
		public void AddImmediateToIndirect()
		{
			Run("ADD.4 #1, @0",
				@"ADD.4 #1, @1
				DAT $0, $1
				DAT $0, $1
				DAT $0, $1
				DAT $0, $0");
		}

		[Test]
		public void AddImmediateToIndirect2()
		{
			// destinations = (1, 3, 4, 5)
			Run("ADD.4 #1, @1   \nDAT #1, #0 \nDAT #1, #1 \nDAT #1, #1 \nDAT #1, #1",
				@"ADD.4 #1, @1
				DAT #1, #1
				DAT #1, #1
				DAT #1, #2
				DAT #1, #2
				DAT $0, $1
				DAT $0, $0");
		}

		[Test]
		public void AddImmediateToPredecrement()
		{
			Run(
@"DAT #1, #1
DAT #1, #1
DAT #1, #1
DAT #1, #1
start ADD.4 #2, <-4
END start",

@"DAT #1, #2
DAT #1, #2
DAT #1, #2
DAT #1, #2
ADD.4 #2, <7996");
		}

		[Test]
		public void AddDirectToDirect()
		{
			Run(
@"DAT #0, #4
DAT #1, #5
DAT #2, #6
DAT #3, #7
start ADD.4 -4, -4
END start",

@"DAT #0, #8
DAT #2, #10
DAT #4, #12
DAT #6, #14
ADD.4 $7996, $7996");
		}

		[Test]
		public void AddIndirectToDirect()
		{
			Run(
@"DAT #0, #0
DAT #1, #0
DAT #2, #0
DAT #3, #0
start ADD.4 @-4, @-4
END start",

@"DAT #0, #0
DAT #2, #0
DAT #4, #0
DAT #6, #0
ADD.4 @7996, @7996");
		}
		[Test]
		public void AddIndirectToDirect2()
		{
			Run(
@"DAT #0, #1
DAT #1, #-1
DAT #2, #1
DAT #3, #-2
DAT #0, #0
DAT #0, #0
DAT #0, #0
DAT #0, #0
start ADD.4 @-8, @-4
END start",

@"DAT #0, #1
DAT #1, #7999
DAT #2, #1
DAT #3, #7998
DAT #1, #7999
DAT #0, #1
DAT #3, #7998
DAT #1, #7999
ADD.4 @7992, @7996");
		}

		private void Run(string program, string expectedResultMem)
		{
			Console.WriteLine();
			Console.WriteLine(program);
			Console.WriteLine("result:");
			string[] lines = expectedResultMem.Trim().Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
			string expectedDump = string.Join("\n", lines);
			Warrior warrior = parser.Parse(program);
			var engine = new GameEngine(new WarriorStartInfo(warrior, 0));
			engine.Step();
			string dump = engine.Memory.Dump(0, lines.Count());
			Console.WriteLine(dump);
			Assert.AreEqual(expectedDump, dump);
		}
	}
}