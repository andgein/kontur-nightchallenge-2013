using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Parser
{
	public class StatementFactory
    {
        public Dictionary<string, StatementType> Commands { get; private set; }

        public StatementFactory()
        {
            Commands = new Dictionary<string, StatementType>
            {
                {"MOV", StatementType.Mov},
                {"ADD", StatementType.Add},
                {"SUB", StatementType.Sub},
                {"MOV.4", StatementType.Mov4},
                {"ADD.4", StatementType.Add4},
                {"SUB.4", StatementType.Sub4},
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

        public Statement Create(Warrior warrior, string command)
        {
            if (! Commands.ContainsKey(command.ToUpper()))
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

        	return new Statement(warrior, Commands[command.ToUpper()]);
        }

		public String GetStatementMnemonic(StatementType type)
		{
			return Commands.FirstOrDefault(kvp => kvp.Value == type).Key;
		}
    }
}