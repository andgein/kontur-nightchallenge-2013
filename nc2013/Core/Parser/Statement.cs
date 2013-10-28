using System;

namespace Core.Parser
{
    class Statement
    {
        public string Label;
        public AddressingMode ModeA;
        public Expression FieldA;
        public AddressingMode ModeB;
        public Expression FieldB;

        public bool HasLabel()
        {
            return Label != "";
        }
    }

    class StatementFactory
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

    class MovStatement : Statement
    {
    }
}