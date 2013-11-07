using System;

namespace Core.Parser
{
	public class CompilationException : Exception
	{
		public readonly string Line;
		public readonly string Error;

		public CompilationException(string message, ParserState parserState)
			: this(message, parserState.Str, parserState.Pos)
		{
		}

		public CompilationException(string message, string line = null, int pos = 0)
			: base(FormatMessage(message, line, pos))
		{
			Line = line;
			Error = message;
		}

		private static string FormatMessage(string message, string line, int pos)
		{
			return line == null
				? string.Format("Error: {0}", message)
				: string.Format("Line [{0}], Pos: {1}, Error: {2}", line, pos, message);
		}
	}
}