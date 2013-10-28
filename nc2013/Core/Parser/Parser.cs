using System;

namespace Core.Parser
{
    class Parser
    {
        protected static String ParseToken(ParserState state, Func<char, bool> tokenValidator)
        {
            while (state.Pos < state.Str.Length && Char.IsWhiteSpace(state.Str[state.Pos]))
                state.Pos++;

            if (state.Pos >= state.Str.Length)
                throw new CompilationException(String.Format("Can't parse token at position {0}: end of line", state.Pos));

            var startPos = state.Pos;
            while (state.Pos < state.Str.Length && tokenValidator(state.Str[state.Pos]))
                state.Pos++;

            return state.Str.Substring(startPos, state.Pos - startPos);
        }

        protected static bool IsIdentificatorChar(char c)
        {
            return Char.IsLetter(c) || Char.IsNumber(c) || c == '_';
        }

        protected static bool IsIdentificatorChar(string s, int idx)
        {
            return IsIdentificatorChar(s[idx]);
        }
    }
}
