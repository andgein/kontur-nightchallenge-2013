using Core.Parser;

namespace Core.Engine
{
    public class Instruction
    {
        public Statement Statement { get; set; }

        public int Address { get; private set; }
        public int? LastModifiedByProgram;

        public Instruction(int address) : this(new Statement(), address, null)
        {
        }

        public Instruction(Statement statement, int address, int? owner)
        {
            Statement = statement;
            Address = address;
            LastModifiedByProgram = owner;
        }
    }
}