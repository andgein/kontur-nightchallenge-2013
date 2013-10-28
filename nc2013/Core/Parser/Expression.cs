namespace Core.Parser
{
    class Expression
    {
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
    }

    class NumberExpression : Expression
    {
        public int Value { get; private set; }

        public NumberExpression(int value)
        {
            Value = value;
        }
    }

    class VariableExpression : Expression
    {
        public string Name { get; private set; }

        public VariableExpression(string name)
        {
            Name = name;
        }
    }
}