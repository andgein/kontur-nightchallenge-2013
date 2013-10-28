using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Parser
{
    class ExpressionParser : Parser
    {
        private Lexem CurrentLexem;
        private ParserState State;

        public Expression Parse(ParserState state)
        {
            State = state;
            NextLexem();
            return ParseExpression();
        }

        public Expression Parse(string str)
        {
            return Parse(new ParserState(str));
        }

        private Expression ParseExpression()
        {
            var left = ParseSummand();
            while (CurrentLexem.Type == LexemType.Plus || CurrentLexem.Type == LexemType.Minus)
            {
                var op = CurrentLexem.Type;
                NextLexem();
                var right = ParseSummand();
                left = (op == LexemType.Plus)
                           ? new BinaryExpression(BinaryOperation.Sum, left, right)
                           : new BinaryExpression(BinaryOperation.Sub, left, right);
            }
            return left;
        }

        private Expression ParseSummand()
        {
            var left = ParseFactor();
            while (CurrentLexem.Type == LexemType.Multiply || CurrentLexem.Type == LexemType.Divide)
            {
                var op = CurrentLexem.Type;
                NextLexem();
                var right = ParseFactor();
                left = (op == LexemType.Multiply)
                           ? new BinaryExpression(BinaryOperation.Mul, left, right)
                           : new BinaryExpression(BinaryOperation.Div, left, right);
            }
            return left;
        }

        private Expression ParseFactor()
        {
            if (CurrentLexem.Type == LexemType.Number)
            {
                var intValue = CurrentLexem.IntValue;
                NextLexem();
                return new NumberExpression(intValue);
            }
            if (CurrentLexem.Type == LexemType.Variable)
            {
                var name = CurrentLexem.StrValue;
                NextLexem();
                return new VariableExpression(name);
            }
            if (CurrentLexem.Type == LexemType.Minus)
            {
                NextLexem();
                var expr = ParseExpression();
                return new UnaryExpression(UnaryOperation.Negate, expr);
            }
            throw new CompilationException(String.Format("Invalid token: '{0}'", CurrentLexem));
        }

        private void NextLexem()
        {
            while (State.Pos < State.Str.Length && Char.IsWhiteSpace(State.Str, State.Pos))
                State.Pos++;

            if (State.Pos >= State.Str.Length)
            {
                CurrentLexem = new Lexem(LexemType.End);
                return;
            }

            /* Numbers */
            if (Char.IsDigit(State.Str, State.Pos))
            {
                var token = ParseToken(State, Char.IsDigit);
                CurrentLexem = new Lexem(LexemType.Number, Int32.Parse(token));
                return;
            }

            /* Variables */
            if (IsIdentificatorChar(State.Str, State.Pos))
            {
                var token = ParseToken(State, IsIdentificatorChar);
                CurrentLexem = new Lexem(LexemType.Variable, token);
                return;
            }

            LexemType lexemType;
            try
            {
                lexemType = (LexemType) Enum.ToObject(typeof (LexemType), @State.Str[State.Pos]);
            }
            catch (Exception)
            {
                CurrentLexem = new Lexem(LexemType.End);
                return;
            }
            State.Pos++;

            CurrentLexem = new Lexem(lexemType);
        }
    }

    class Lexem
    {
        public LexemType Type;
        public int IntValue;
        public string StrValue;

        public Lexem(LexemType type)
        {
            Type = type;
        }
        
        public Lexem(LexemType type, int intValue)
        {
            Type = type;
            IntValue = intValue;
        }

        public Lexem(LexemType type, string strValue)
        {
            Type = type;
            StrValue = strValue;
        }
    }

    enum LexemType
    {
        Number = '1', Variable = 'a', OpenBracket = '(', CloseBracket = ')', Plus = '+', Minus = '-', Multiply = '*', Divide = '/', Mod = '%', End
    }

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

    public enum BinaryOperation
    {
        Sum, Sub, Div, Mul, Mod
    }

    public enum UnaryOperation
    {
        Negate
    }
}
