using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Parser
{
    [TestFixture]
    class ExpressionParserTest
    {
        private ExpressionParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new ExpressionParser();
        }

        [Test]
        public void TestSimpleExpression()
        {
            var expr = parser.Parse("1 + 2");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression) expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression) expr).Left;
            var right = ((BinaryExpression) expr).Right;

            Assert.AreEqual(left.GetType(), typeof (NumberExpression));
            Assert.AreEqual(right.GetType(), typeof(NumberExpression));

            Assert.AreEqual(((NumberExpression) left).Value, 1);
            Assert.AreEqual(((NumberExpression) right).Value, 2);
        }

        [Test]
        public void TestSimpleTreeSum()
        {
            var expr = parser.Parse("1 + 2 + 3");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression) expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression) expr).Left;
            var right = ((BinaryExpression) expr).Right;

            Assert.AreEqual(left.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(right.GetType(), typeof(NumberExpression));

            Assert.AreEqual(((NumberExpression) right).Value, 3);
        }

        [Test]
        public void TestBrackets()
        {
            var expr = parser.Parse("1 + (2 + 3)");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression)expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression) expr).Left;
            var right = ((BinaryExpression) expr).Right;

            Assert.AreEqual(left.GetType(), typeof(NumberExpression));
            Assert.AreEqual(right.GetType(), typeof(BinaryExpression));

            Assert.AreEqual(((NumberExpression) left).Value, 1);
        }

        [Test]
        public void TestBracketsWithSub()
        {
            var expr = parser.Parse("1 - (2 - 3)");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression) expr).Op, BinaryOperation.Sub);
            var left = ((BinaryExpression) expr).Left;
            var right = ((BinaryExpression) expr).Right;

            Assert.AreEqual(left.GetType(), typeof(NumberExpression));
            Assert.AreEqual(right.GetType(), typeof(BinaryExpression));
        }

        [Test]
        public void TestPriority()
        {
            var expr = parser.Parse("1 + 2 * 10");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression) expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression) expr).Left;
            var right = ((BinaryExpression) expr).Right;

            Assert.AreEqual(left.GetType(), typeof(NumberExpression));
            Assert.AreEqual(right.GetType(), typeof(BinaryExpression));

            var rightLeft = ((BinaryExpression) right).Left;
            var rightRight = ((BinaryExpression) right).Right;
            Assert.AreEqual(rightLeft.GetType(), typeof(NumberExpression));
            Assert.AreEqual(rightRight.GetType(), typeof(NumberExpression));

            Assert.AreEqual(((NumberExpression) rightLeft).Value, 2);
            Assert.AreEqual(((NumberExpression) rightRight).Value, 10);
        }

        [Test]
        public void TestVariables()
        {
            var expr = parser.Parse("1 + a_1b3");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression)expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression)expr).Left;
            var right = ((BinaryExpression)expr).Right;

            Assert.AreEqual(left.GetType(), typeof(NumberExpression));
            Assert.AreEqual(right.GetType(), typeof(VariableExpression));

            Assert.AreEqual(((NumberExpression)left).Value, 1);
            Assert.AreEqual(((VariableExpression)right).Name, "a_1b3");
        }

        [Test]
        public void TestWhitespaces()
        {
            var expr = parser.Parse("       1 \t + \t\t\t    2  \t");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
        }
    }
}
