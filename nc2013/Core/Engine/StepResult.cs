using System.Collections.Generic;

namespace Core.Engine
{
    public class StepResult
    {
        public bool KilledInInstruction;
        public int? SetNextIP;
        public int? SplittedInInstruction;
    	public readonly HashSet<int> MemoryDiffs;

        public StepResult()
        {
            KilledInInstruction = false;
            SetNextIP = null;
            SplittedInInstruction = null;
			MemoryDiffs = new HashSet<int>();
        }

        public void ChangeMemory(int address)
        {
        	MemoryDiffs.Add(ModularArith.Mod(address));
        }
    }
}