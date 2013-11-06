using System;
using System.Linq;
using System.Text;
using Core.Engine;
using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Parser
{
	[TestFixture]
	class WarriorParserTest
	{
		private const string imp = "MOV 0, 1";
		private WarriorParser parser;

		[SetUp]
		public void SetUp()
		{
			parser = new WarriorParser();
		}

		[Test]
		public void TestImp()
		{
			var warrior = parser.Parse(imp);
			Assert.AreEqual(1, warrior.Statements.Count);
			Assert.AreEqual(false, warrior.Statements[0].HasLabel);
			Assert.AreEqual(StatementType.Mov, warrior.Statements[0].Type);
		}

		[Test]
		public void HugeImp()
		{
			var w = new StringBuilder();
			for (int i = 0; i < Parameters.MaxWarriorLength; i++)
				w.AppendLine(imp);
			var warrior = parser.Parse(w.ToString());
			Assert.AreEqual(Parameters.MaxWarriorLength, warrior.Statements.Count);
			w.AppendLine(imp);
			Assert.Throws<CompilationException>(() => parser.Parse(w.ToString()));
		}

		[Test]
		public void TestImpWithComment()
		{
			var warrior = parser.Parse(imp + "     ; any comment here");
			Assert.AreEqual(1, warrior.Statements.Count);
			Assert.AreEqual(false, warrior.Statements[0].HasLabel);
			Assert.AreEqual(StatementType.Mov, warrior.Statements[0].Type);
		}

		[Test]
		public void TestImpWithCommentsAndNewLines()
		{
			var warrior = parser.Parse("  ; comment 1\n   \n" + imp + "     ; any comment here\n\t  \t\n;comment 2");
			Assert.AreEqual(1, warrior.Statements.Count);
			Assert.AreEqual(false, warrior.Statements[0].HasLabel);
			Assert.AreEqual(StatementType.Mov, warrior.Statements[0].Type);
		}

		[Test]
		public void TestImpWithMultipleImps()
		{
			const int count = 4;
			var code = String.Concat(Enumerable.Repeat(imp + " ; any comment \n", count));

			var warrior = parser.Parse(code);
			Assert.AreEqual(count, warrior.Statements.Count);
			for (var i = 0; i < count; ++i)
			{
				Assert.AreEqual(false, warrior.Statements[i].HasLabel);
				Assert.AreEqual(StatementType.Mov, warrior.Statements[i].Type);
			}
		}

		[Test]
		public void TestImpWithLabels()
		{
			const string code = "label1 " + imp + "\n" +
								"label2 " + imp + "\n" +
								"label3 " + imp + "\n";

			var warrior = parser.Parse(code);
			Assert.AreEqual(3, warrior.Statements.Count);
			for (var i = 0; i < 3; ++i)
			{
				Assert.AreEqual(true, warrior.Statements[i].HasLabel);
				Assert.AreEqual(String.Format("label{0}", i + 1), warrior.Statements[i].Label);
				Assert.AreEqual(StatementType.Mov, warrior.Statements[i].Type);
			}
		}

		[Test]
		[ExpectedException(typeof(CompilationException))]
		public void TestSameLabels()
		{
			const string code = "label1 " + imp + "\n" +
								"label1 " + imp + "\n";

			parser.Parse(code);
		}

		[Test]
		[ExpectedException(typeof(CompilationException))]
		public void TestIncompleteCommand()
		{
			parser.Parse("MOV 0, ; comment");
		}

		[Test]
		[ExpectedException(typeof(CompilationException))]
		public void TestInvalidCommand()
		{
			parser.Parse("INV 0, 1");
		}

		[Test]
		[ExpectedException(typeof(CompilationException))]
		public void TestInvalidAddressingMode()
		{
			parser.Parse("ADD #1, #2");
		}

		[Test]
		public void TestDifferentAddressingModes()
		{
			var warrior = parser.Parse("JMZ 0, #1");
			Assert.AreEqual(AddressingMode.Direct, warrior.Statements[0].ModeA);
			Assert.AreEqual(AddressingMode.Immediate, warrior.Statements[0].ModeB);

			warrior = parser.Parse("ADD @0, @1");
			Assert.AreEqual(AddressingMode.Indirect, warrior.Statements[0].ModeA);
			Assert.AreEqual(AddressingMode.Indirect, warrior.Statements[0].ModeB);

			warrior = parser.Parse("DJN <0, <1");
			Assert.AreEqual(AddressingMode.PredecrementIndirect, warrior.Statements[0].ModeA);
			Assert.AreEqual(AddressingMode.PredecrementIndirect, warrior.Statements[0].ModeB);
		}

		[Test]
		public void TestEndWithoutAddress()
		{
			var warrior = parser.Parse(imp + "\n" + imp + "\n" + "END\nline will be ignored");
			Assert.AreEqual(2, warrior.Statements.Count);
			Assert.AreEqual(0, warrior.StartAddress);
		}

		[Test]
		public void TestEnd()
		{
			var warrior = parser.Parse(imp + "\n" + imp + "\n" + "END 1\nline will be ignored");
			Assert.AreEqual(2, warrior.Statements.Count);
			Assert.AreEqual(1, warrior.StartAddress);
		}

		[Test]
		public void TestEndWithLabel()
		{
			var warrior = parser.Parse(imp + "\nstart " + imp + "\n" + "END start\nline will be ignored");
			Assert.AreEqual(2, warrior.Statements.Count);
			Assert.AreEqual(1, warrior.StartAddress);
		}

		[Test]
		public void TestEndWithConstant()
		{
			var warrior = parser.Parse("a EQU -1\n" + imp + "\n" + imp + "\n" + "END a\nline will be ignored");
			Assert.AreEqual(2, warrior.Statements.Count);
			Assert.AreEqual(1, warrior.StartAddress);
		}

		[Test]
		public void TestLabels()
		{
			var warrior = parser.Parse("imp " + imp + "\nJMP imp");
			Assert.AreEqual(2, warrior.Statements.Count);
			Assert.AreEqual(typeof(NumberExpression), warrior.Statements[1].FieldA.GetType());
			Assert.AreEqual(ModularArith.Mod(-1), warrior.Statements[1].FieldA.Calculate());
		}

		[Test]
		public void TestConstants()
		{
			var warrior = parser.Parse("a EQU b\nb EQU c + 10\nc EQU d + 2 * 4\nd EQU link - 1\nlink " + imp);
			Assert.AreEqual(4, warrior.Constants.Count);
			Assert.AreEqual(typeof(NumberExpression), warrior.Constants["a"].GetType());
			Assert.AreEqual(typeof(NumberExpression), warrior.Constants["b"].GetType());
			Assert.AreEqual(typeof(NumberExpression), warrior.Constants["c"].GetType());
			Assert.AreEqual(typeof(NumberExpression), warrior.Constants["d"].GetType());
			Assert.AreEqual(17, warrior.Constants["a"].Calculate());
			Assert.AreEqual(17, warrior.Constants["b"].Calculate());
			Assert.AreEqual(7, warrior.Constants["c"].Calculate());
			Assert.AreEqual(ModularArith.Mod(-1), warrior.Constants["d"].Calculate());
		}

		[Test]
		public void TestExpressionWithMultipleLabels()
		{
			var warrior = parser.Parse(imp + "\nl1 " + imp + "\nl2 " + imp + "\nl3 MOV l1, l1 + l2");
			Assert.AreEqual(ModularArith.Mod(-3), warrior.Statements[3].FieldB.Calculate());
		}

		[Test]
		[ExpectedException(typeof(CompilationException))]
		public void TestCyclicConstants()
		{
			parser.Parse("a EQU b + 10\nb EQU 10 + c\nc EQU a - 20\n" + imp);
		}

		[Test]
		public void TestComplexEqu()
		{
			parser.Parse(
@"
CORESIZE equ 8000
gap1st	equ	100
gap1	equ 10
multipl equ ((CORESIZE-gap1st)-((CORESIZE-gap1st)/gap1))
MOV 0, multipl");
		}
	}
}
