using System.Collections.Generic;
using Core.Game;

namespace Core.Engine
{
    public class StepResult
    {
        public bool KilledInInstruction;
        public int? SetNextIP;
        public int? SplittedInInstruction;
    	public readonly HashSet<int> MemoryDiffs;
    	public readonly List<ProgramStateDiff> ProgramStateDiffs;

        public StepResult()
        {
            KilledInInstruction = false;
            SetNextIP = null;
            SplittedInInstruction = null;
			MemoryDiffs = new HashSet<int>();
        	ProgramStateDiffs = new List<ProgramStateDiff>();
        }

        public void ChangeMemory(int address)
        {
        	MemoryDiffs.Add(ModularArith.Mod(address));
        }
    }
}