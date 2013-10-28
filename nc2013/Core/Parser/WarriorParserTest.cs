using System;
using System.Linq;
using NUnit.Framework;

namespace Core.Parser
{
    [TestFixture]
    class WarriorParserTest
    {
        private const string Imp = "MOV 0, 1";
        private WarriorParser Parser;

        [SetUp]
        public void SetUp()
        {
            Parser = new WarriorParser();
        }

        [Test]
        public void TestImp()
        {
            var warrior = Parser.Parse(Imp);
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithComment()
        {
            var warrior = Parser.Parse(Imp + "     ; any comment here");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithCommentsAndNewLines()
        {
            var warrior = Parser.Parse("  ; comment 1\n   \n" + Imp + "     ; any comment here\n\t  \t\n;comment 2");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithMultipleImps()
        {
            const int count = 4;
            var code = String.Concat(Enumerable.Repeat(Imp + " ; any comment \n", count));

            var warrior = Parser.Parse(code);
            Assert.AreEqual(warrior.Statements.Count, count);
            for (var i = 0; i < count; ++i)
            {
                Assert.AreEqual(warrior.Statements[i].HasLabel(), false);
                Assert.AreEqual(warrior.Statements[i].GetType(), typeof (MovStatement));
            }
        }

        [Test]
        public void TestImpWithLabels()
        {
            const string code = "label1 " + Imp + "\n" +
                                "label2 " + Imp + "\n" +
                                "label3 " + Imp + "\n";

            var warrior = Parser.Parse(code);
            Assert.AreEqual(warrior.Statements.Count, 3);
            for (var i = 0; i < 3; ++i)
            {
                Assert.AreEqual(warrior.Statements[i].HasLabel(), true);
                Assert.AreEqual(warrior.Statements[i].GetType(), typeof(MovStatement));
                Assert.AreEqual(warrior.Statements[i].Label, String.Format("label{0}", i + 1));
            }
        }

        [Test]
        [ExpectedException(typeof(CompilationException))]
        public void TestSameLabels()
        {
            const string code = "label1 " + Imp + "\n" +
                                "label1 " + Imp + "\n";

            Parser.Parse(code);
        }

        [Test]
        [ExpectedException(typeof(CompilationException))]
        public void TestIncompleteCommand()
        {
            Parser.Parse("MOV 0, ; comment");
        }

        [Test]
        [ExpectedException(typeof(CompilationException))]
        public void TestInvalidCommand()
        {
            Parser.Parse("INV 0, 1");
        }

        [Test]
        public void TestDifferentAddressingModes()
        {
            var warrior = Parser.Parse("JMZ 0, #1");
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.Direct);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.Immediate);

            warrior = Parser.Parse("ADD @0, @1");
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.Indirect);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.Indirect);

            warrior = Parser.Parse("DJN <0, <1");
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.PredecrementIndirect);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.PredecrementIndirect);
        }
    }
}
