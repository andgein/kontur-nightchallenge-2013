using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Core.Parser
{
    [TestFixture]
    class ExpressionParserTest
    {
        private ExpressionParser Parser;

        [SetUp]
        public void SetUp()
        {
            Parser = new ExpressionParser();
        }

        [Test]
        public void TestSimpleExpression()
        {
            var expr = Parser.Parse("1 + 2");
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
            var expr = Parser.Parse("1 + 2 + 3");
            Assert.AreEqual(expr.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(((BinaryExpression)expr).Op, BinaryOperation.Sum);
            var left = ((BinaryExpression)expr).Left;
            var right = ((BinaryExpression)expr).Right;

            Assert.AreEqual(left.GetType(), typeof(BinaryExpression));
            Assert.AreEqual(right.GetType(), typeof(NumberExpression));

            Assert.AreEqual(((NumberExpression)right).Value, 3);
        }
    }
}
