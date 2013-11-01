using System.Collections.Generic;
using System.Linq;

namespace Core.Engine
{
    public class Memory
    {
        private readonly int coresize;
        private readonly List<Instruction> memory;

        public Memory(int coresize)
        {
            this.coresize = coresize;
            memory = Enumerable.Range(0, coresize).Select(x => new Instruction(x)).ToList();
        }

        public Instruction this[int index]
        {
            get { return memory[ModularArith.Mod(index, coresize)]; }
            set { memory[ModularArith.Mod(index, coresize)] = value; }
        }
    }
}