using NUnit.Framework;

namespace Core.Parser
{
    [TestFixture]
    class WarriorParserTest
    {
        private string Imp = "MOV 0, 1";
        private WarriorParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new WarriorParser();
        }

        [Test]
        public void TestImp()
        {
            var warrior = parser.Parse(Imp);
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof(MovStatement));
        }

        [Test]
        public void TestImpWithComment()
        {
            var warrior = parser.Parse(Imp + "     ; any comment here");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof(MovStatement));
        }

        [Test]
        public void TestImpWithCommentsAndNewLines()
        {
            var warrior = parser.Parse("  ; comment 1\n   \n" + Imp + "     ; any comment here\n\t  \t\n;comment 2");
            Assert.AreEqual(warrior.Statements.Count, 1);
            Assert.AreEqual(warrior.Statements[0].HasLabel(), false);
            Assert.AreEqual(warrior.Statements[0].GetType(), typeof(MovStatement));
        }

    }
}
