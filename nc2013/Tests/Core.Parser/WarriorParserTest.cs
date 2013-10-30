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
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithComment()
        {
            var warrior = parser.Parse(imp + "     ; any comment here");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithCommentsAndNewLines()
        {
            var warrior = parser.Parse("  ; comment 1\n   \n" + imp + "     ; any comment here\n\t  \t\n;comment 2");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof (MovStatement));
        }

        [Test]
        public void TestImpWithMultipleImps()
        {
            const int count = 4;
            var code = String.Concat(Enumerable.Repeat(imp + " ; any comment \n", count));

            var warrior = parser.Parse(code);
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
            const string code = "label1 " + imp + "\n" +
                                "label2 " + imp + "\n" +
                                "label3 " + imp + "\n";

            var warrior = parser.Parse(code);
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
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.Direct);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.Immediate);

            warrior = parser.Parse("ADD @0, @1");
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.Indirect);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.Indirect);

            warrior = parser.Parse("DJN <0, <1");
            Assert.AreEqual(warrior.Statements[0].ModeA, AddressingMode.PredecrementIndirect);
            Assert.AreEqual(warrior.Statements[0].ModeB, AddressingMode.PredecrementIndirect);
        }
    }
}
