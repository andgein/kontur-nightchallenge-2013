﻿using System;
using System.Linq;

namespace Core.Parser
{
    internal class WarriorParser : Parser
    {
        private readonly StatementFactory StatementFactory = new StatementFactory();
        private readonly ExpressionParser ExpressionParser = new ExpressionParser();

        public Warrior Parse(String text)
        {
            var warrior = new Warrior();
            foreach (var line in text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
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

            State = new ParserState(line);
            var token = ParseToken(IsIdentificatorChar);
            if (!IsCommandToken(token))
            {
                label = token;
                command = ParseToken(IsIdentificatorChar);
            }
            else
            {
                command = token;
            }

            if (!IsCommandToken(command))
                throw new CompilationException(String.Format("Expected command, but found '{0}'", command));

            var statement = StatementFactory.Create(command);
            statement.Label = label;

            if (!RestOnlyWhitespace())
            {
                statement.ModeA = ParseAddressingMode();
                statement.FieldA = ExpressionParser.Parse(State);
            }

            if (!RestOnlyWhitespace())
            {
                ParseComma();
                statement.ModeB = ParseAddressingMode();
                statement.FieldB = ExpressionParser.Parse(State);
            }

            return statement;
        }

        private void ParseComma()
        {
            SkipWhitespaces();
            if (State.Finished() || State.Current != ',')
                throw new CompilationException("Expected comma");
            State.Pos++;
        }

        private AddressingMode ParseAddressingMode()
        {
            SkipWhitespaces();
            AddressingMode mode;
            try
            {
                mode = (AddressingMode) Enum.ToObject(typeof (AddressingMode), @State.Current);
                if (! Enum.IsDefined(typeof (AddressingMode), mode))
                     throw new Exception();
            }
            catch (Exception)
            {
                return AddressingMode.Absolute;
            }
            State.Pos++;
            return mode;
        }

        private bool RestOnlyWhitespace()
        {
            return State.Str.Substring(State.Pos).All(Char.IsWhiteSpace);
        }

        private static bool IsCommandToken(string token)
        {
            return token == "MOV";
        }
    }
}