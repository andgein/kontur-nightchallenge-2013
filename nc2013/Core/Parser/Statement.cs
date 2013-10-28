using System;
using System.Collections.Generic;

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
        public Dictionary<String, Type> Commands { get; private set; }

        public StatementFactory()
        {
            Commands = new Dictionary<string, Type>
                           {
                               {"MOV", typeof (MovStatement)},
                               {"ADD", typeof (AddStatement)},
                               {"SUB", typeof (SubStatement)},
                               {"CMP", typeof (CmpStatement)},
                               {"SLT", typeof (SltStatement)},
                               {"JMP", typeof (JmpStatement)},
                               {"JMZ", typeof (JmzStatement)},
                               {"JMN", typeof (JmnStatement)},
                               {"DJN", typeof (DjnStatement)},
                               {"SPL", typeof (SplStatement)},
                               {"DAT", typeof (DatStatement)}
                           };
        }

        public Statement Create(string command)
        {
            if (! Commands.ContainsKey(command.ToUpper()))
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

            var constructor = Commands[command.ToUpper()].GetConstructor(new Type[0]);
            if (constructor == null)
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

            return (Statement) constructor.Invoke(new object[0]);
        }
    }

    class MovStatement : Statement
    {
    }

    class AddStatement : Statement
    {
    }

    class SubStatement : Statement
    {
    }

    class CmpStatement : Statement
    {
    }

    class SltStatement : Statement
    {
    }

    class JmpStatement : Statement
    {
    }

    class JmzStatement : Statement
    {
    }

    class JmnStatement : Statement
    {
    }

    class DjnStatement : Statement
    {
    }

    class SplStatement : Statement
    {
    }

    class DatStatement : Statement
    {
    }
}