using System;

namespace Core.Parser
{
	public class CompilationException : Exception
	{
		public readonly string Line;
		public readonly int Pos;
		public readonly string Error;

		public CompilationException(string message, string line, int pos)
			: base(FormatMessage(message, line, pos))
		{
			Pos = pos;
			Line = line;
			Error = message;
		}

		private static string FormatMessage(string message, string line, int pos)
		{
			return string.Format("Line [{0}], Pos: {1}, Error: {2}", line, pos, message);
		}

		public CompilationException(string message, ParserState parserState)
			: this(message, parserState.Str, parserState.Pos)
		{
		}

		public CompilationException(string message, string line, int pos, Exception innerException) 
			: base(FormatMessage(message, line, pos), innerException)
		{
			Pos = pos;
			Line = line;
			Error = message;
		}

		public CompilationException(string message, ParserState parserState, Exception innerException)
			: this(message, parserState.Str, parserState.Pos, innerException)
		{
		}

	}
}