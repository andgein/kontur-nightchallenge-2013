using System;
using System.Linq;

namespace Core.Parser
{
    class WarriorParser
    {
        private readonly StatementFactory StatementFactory = new StatementFactory();

        public Warrior Parse(String text)
        {
            var warrior = new Warrior();
            foreach (var line in text.Split(new[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var stLine = TrimComment(line);
                if (stLine.All(Char.IsWhiteSpace))
                    continue;
                warrior.AddStatement(ParseLine(stLine));
            }
            return warrior;
        }

        private static string TrimComment(string line)
        {
            var commentPos = line.IndexOf(';');
            return commentPos != -1 ? line.Substring(0, commentPos) : line;
        }

        private Statement ParseLine(String line)
        {
            var label = "";
            string command;

            var state = new ParserState(line);
            var token = ParseToken(state);
            if (! IsCommandToken(token))
            {
                label = token;
                command = ParseToken(state);
            }
            else
            {
                command = token;
            }

            if (! IsCommandToken(command))
                throw new CompilationException(String.Format("Expected command, but found '{0}'", command));

            var statement = StatementFactory.Create(command);
            statement.Label = label;

            return statement;
        }

        private static bool IsCommandToken(string token)
        {
            return token == "MOV";
        }

        private static String ParseToken(ParserState state)
        {
            while (state.Pos < state.Str.Length && Char.IsWhiteSpace(state.Str[state.Pos]))
                state.Pos++;

            if (state.Pos >= state.Str.Length)
                throw new CompilationException(String.Format("Can't parse token at position {0}: end of line", state.Pos));

            var startPos = state.Pos;
            while (state.Pos < state.Str.Length && IsTokenChar(state.Str[state.Pos]))
                state.Pos++;

            return state.Str.Substring(startPos, state.Pos - startPos);
        }

        private static bool IsTokenChar(char c)
        {
            return Char.IsLetter(c) || Char.IsNumber(c) || c == '_';
        }
    }

    internal class StatementFactory
    {
        public Statement Create(string command)
        {
            switch (command.ToUpper())
            {
                case "MOV":
                    return new MovStatement();
            }
            throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));
        }
    }

    internal class MovStatement : Statement
    {
    }

    internal enum Command
    {
        Mov, Jmp
    }

    internal class CompilationException : Exception
    {
        public CompilationException(string message) : base(message)
        {
        }
    }

    internal class ParserState
    {
        public String Str { get; private set; }
        public int Pos;

        public ParserState(string str, int pos = 0)
        {
            Str = str;
            Pos = pos;
        }
    }
}
