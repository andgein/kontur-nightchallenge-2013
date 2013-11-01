using System;
using System.Linq;
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
            Assert.AreEqual(false, warrior.Statements[0].HasLabel());
            Assert.AreEqual(StatementType.Mov, warrior.Statements[0].Type);
        }

        [Test]
        public void TestImpWithComment()
        {
            var warrior = parser.Parse(imp + "     ; any comment here");
            Assert.AreEqual(1, warrior.Statements.Count);
            Assert.AreEqual(false, warrior.Statements[0].HasLabel());
            Assert.AreEqual(StatementType.Mov, warrior.Statements[0].Type);
        }

        [Test]
        public void TestImpWithCommentsAndNewLines()
        {
            var warrior = parser.Parse("  ; comment 1\n   \n" + imp + "     ; any comment here\n\t  \t\n;comment 2");
            Assert.AreEqual(1, warrior.Statements.Count);
            Assert.AreEqual(false, warrior.Statements[0].HasLabel());
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
                Assert.AreEqual(false, warrior.Statements[i].HasLabel());
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
                Assert.AreEqual(true, warrior.Statements[i].HasLabel());
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
            var warrior = parser.Parse(imp + "\n" + imp + "\n" + "END -1\nline will be ignored");
            Assert.AreEqual(2, warrior.Statements.Count);
            Assert.AreEqual(1, warrior.StartAddress);
        }
    }
}
