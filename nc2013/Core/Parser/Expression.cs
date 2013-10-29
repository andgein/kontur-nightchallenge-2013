using System;
using Core.Engine;

namespace Core.Parser
{
    abstract class Expression
    {
        public abstract int Calculate();

        public Expression Decremented()
        {
            return new NumberExpression((Calculate() - 1) % Parameters.CORESIZE);
        }
    }

    class BinaryExpression : Expression
    {
        public BinaryOperation Op { get; private set; }
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public BinaryExpression(BinaryOperation op, Expression left, Expression right)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override int Calculate()
        {
            int answer;
            switch (Op)
            {
                case BinaryOperation.Sum:
                    answer = Left.Calculate() + Right.Calculate();
                    break;
                case BinaryOperation.Sub:
                    answer = Left.Calculate() - Right.Calculate();
                    break;
                case BinaryOperation.Mul:
                    answer = Left.Calculate() * Right.Calculate();
                    break;
                case BinaryOperation.Div:
                    answer = Left.Calculate() / Right.Calculate();
                    break;
                default:
                    throw new InvalidOperationException("Invalid operation to calculate: " + Op);
            }
            return answer % Parameters.CORESIZE;
        }
    }

    class UnaryExpression : Expression
    {
        public UnaryOperation Op { get; private set; }
        public Expression Sub { get; private set; }

        public UnaryExpression(UnaryOperation op, Expression sub)
        {
            Op = op;
            Sub = sub;
        }

        public override int Calculate()
        {
            switch (Op)
            {
                case UnaryOperation.Negate:
                    return -Sub.Calculate();
            }
            throw new InvalidOperationException("Internal error. Unknown unary operation.");
        }
    }

    class NumberExpression : Expression
    {
        public int Value { get; private set; }

        public NumberExpression(int value)
        {
            Value = value;
        }

        public override int Calculate()
        {
            return Value;
        }
    }

    class VariableExpression : Expression
    {
        public string Name { get; private set; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override int Calculate()
        {
            throw new System.NotImplementedException();
        }
    }
}