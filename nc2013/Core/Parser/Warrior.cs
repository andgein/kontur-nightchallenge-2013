using System;
using System.Collections.Generic;

namespace Core.Parser
{
	public class Warrior
    {
        public List<Statement> Statements { get; private set; }
        public Dictionary<String, uint> LabelsAddresses { get; private set; }
	    public int StartAddress;

        public Warrior(List<Statement> statements)
        {
            Statements = statements;
            LabelsAddresses = new Dictionary<string, uint>();
        }

        public Warrior() : this(new List<Statement>())
        {
        }

        public void AddStatement(Statement statement)
        {
            if (statement.HasLabel() && LabelsAddresses.ContainsKey(statement.Label))
                throw new CompilationException(String.Format("Statement with same label '{0}' already exists", statement.Label));

            Statements.Add(statement);

            if (statement.HasLabel())
                LabelsAddresses[statement.Label] = (uint) Statements.Count - 1;
        }
    }
}
