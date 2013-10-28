using System;
using System.Linq;

namespace Core.Parser
{
    class WarriorParser : Parser
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
            var token = ParseToken(state, IsIdentificatorChar);
            if (! IsCommandToken(token))
            {
                label = token;
                command = ParseToken(state, IsIdentificatorChar);
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
