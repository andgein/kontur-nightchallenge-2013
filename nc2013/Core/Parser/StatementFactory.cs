using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Parser
{
    class StatementFactory
    {
        public Dictionary<string, StatementType> Commands { get; private set; }

        public StatementFactory()
        {
            Commands = new Dictionary<string, StatementType>
            {
                {"MOV", StatementType.Mov},
                {"ADD", StatementType.Add},
                {"SUB", StatementType.Sub},
                {"CMP", StatementType.Cmp},
                {"SLT", StatementType.Slt},
                {"JMP", StatementType.Jmp},
                {"JMZ", StatementType.Jmz},
                {"JMN", StatementType.Jmn},
                {"DJN", StatementType.Djn},
                {"SPL", StatementType.Spl},
                {"DAT", StatementType.Dat},

                {"END", StatementType.End},
                {"EQU", StatementType.Equ},
            };
        }

        public Statement Create(string command)
        {
            if (! Commands.ContainsKey(command.ToUpper()))
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

        	return new Statement(Commands[command.ToUpper()]);
        }

		public String GetStatementMnemonic(StatementType type)
		{
			return Commands.FirstOrDefault(kvp => kvp.Value == type).Key;
		}
    }
}