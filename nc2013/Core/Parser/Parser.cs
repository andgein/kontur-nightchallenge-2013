﻿using System;
using System.Linq;

namespace Core.Parser
{
	public class Parser
	{
		protected ParserState State;

		protected String ParseToken(Func<char, bool> tokenValidator)
		{
			SkipWhitespaces();

			if (State.Pos >= State.Str.Length)
				throw new CompilationException("Unexpected end of line", State);

			var startPos = State.Pos;
			while (!State.Finished() && tokenValidator(State.Current))
				State.Next();

			return State.Str.Substring(startPos, State.Pos - startPos);
		}

		protected void SkipWhitespaces()
		{
			while (!State.Finished() && Char.IsWhiteSpace(State.Current))
				State.Next();
		}

		protected static bool IsIdentificatorChar(char c)
		{
			return Char.IsLetter(c) || Char.IsNumber(c) || c == '_' || c == '.';
		}

		protected static bool IsIdentificatorChar(string s, int idx)
		{
			return IsIdentificatorChar(s[idx]);
		}

		protected static bool IsIdentificator(string s)
		{
			return s.All(IsIdentificatorChar) && !Char.IsNumber(s[0]);
		}
	}
}
