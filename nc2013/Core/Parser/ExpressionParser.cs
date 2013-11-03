using System;

namespace Core.Parser
{
    public class ExpressionParser : Parser
    {
        private Lexem currentLexem;

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
            while (currentLexem.Type == LexemType.Plus || currentLexem.Type == LexemType.Minus)
            {
                var op = currentLexem.Type;
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
            while (currentLexem.Type == LexemType.Multiply || currentLexem.Type == LexemType.Divide)
            {
                var op = currentLexem.Type;
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
            if (currentLexem.Type == LexemType.Number)
            {
                var intValue = currentLexem.IntValue;
                NextLexem();
                return new NumberExpression(intValue.GetValueOrDefault());
            }
            if (currentLexem.Type == LexemType.Variable)
            {
                var name = currentLexem.StrValue;
                NextLexem();
                return new VariableExpression(name);
            }
            if (currentLexem.Type == LexemType.Minus)
            {
                NextLexem();
                var expr = ParseFactor();
                return new UnaryExpression(UnaryOperation.Negate, expr);
            }
			if (currentLexem.Type == LexemType.Plus)
			{
				NextLexem();
				var expr = ParseFactor();
				return new UnaryExpression(UnaryOperation.Positive, expr);
			}
			if (currentLexem.Type == LexemType.OpenBracket)
            {
                NextLexem();
                var expr = ParseExpression();
                if (currentLexem.Type != LexemType.CloseBracket)
                    throw new CompilationException("Can't find close bracket", State);
                NextLexem();
                return expr;
            }
            throw new CompilationException("Invalid token in expression", State);
        }

        private void NextLexem()
        {
            SkipWhitespaces();

            if (State.Finished())
            {
                currentLexem = new Lexem(LexemType.End);
                return;
            }

            /* Numbers */
            if (Char.IsDigit(State.Current))
            {
                var token = ParseToken(Char.IsDigit);
                currentLexem = new Lexem(LexemType.Number, Int32.Parse(token));
                return;
            }

            /* Variables */
            if (IsIdentificatorChar(State.Current))
            {
                var token = ParseToken(IsIdentificatorChar);
                currentLexem = new Lexem(LexemType.Variable, token);
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
                currentLexem = new Lexem(LexemType.End);
                return;
            }
            State.Next();

            currentLexem = new Lexem(lexemType);
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
