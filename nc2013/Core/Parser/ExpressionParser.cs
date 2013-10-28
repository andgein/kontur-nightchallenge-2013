using System;

namespace Core.Parser
{
    class ExpressionParser : Parser
    {
        private Lexem CurrentLexem;

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
                return new NumberExpression(intValue.GetValueOrDefault());
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
            if (CurrentLexem.Type == LexemType.OpenBracket)
            {
                NextLexem();
                var expr = ParseExpression();
                if (CurrentLexem.Type != LexemType.CloseBracket)
                    throw new CompilationException(String.Format("Can't find close bracket"));
                NextLexem();
                return expr;
            }
            throw new CompilationException(String.Format("Invalid token in expression: '{0}'", CurrentLexem));
        }

        private void NextLexem()
        {
            SkipWhitespaces();

            if (State.Finished())
            {
                CurrentLexem = new Lexem(LexemType.End);
                return;
            }

            /* Numbers */
            if (Char.IsDigit(State.Current))
            {
                var token = ParseToken(Char.IsDigit);
                CurrentLexem = new Lexem(LexemType.Number, Int32.Parse(token));
                return;
            }

            /* Variables */
            if (IsIdentificatorChar(State.Current))
            {
                var token = ParseToken(IsIdentificatorChar);
                CurrentLexem = new Lexem(LexemType.Variable, token);
                return;
            }

            LexemType lexemType;
            try
            {
                lexemType = (LexemType) Enum.ToObject(typeof (LexemType), @State.Current);
                if (! Enum.IsDefined(typeof (LexemType), lexemType))
                    throw new Exception();
            }
            catch (Exception)
            {
                CurrentLexem = new Lexem(LexemType.End);
                return;
            }
            State.Next();

            CurrentLexem = new Lexem(lexemType);
        }
    }

    class Lexem
    {
        public readonly LexemType Type;
        public int? IntValue;
        public readonly string StrValue;

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

        public override string ToString()
        {
            if (IntValue != null)
                return String.Format("{0}: {1}", Type, IntValue);
            if (StrValue != null)
                return String.Format("{0}: {1}", Type, StrValue);
            return String.Format("{0}", Type);
        }
    }

    internal enum LexemType
    {
        Number = '1',
        Variable = 'a',
        OpenBracket = '(',
        CloseBracket = ')',
        Plus = '+',
        Minus = '-',
        Multiply = '*',
        Divide = '/',
        End
    }
}
