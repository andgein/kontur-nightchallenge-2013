using System.Collections.Generic;
using Core.Game;
using Core.Parser;

namespace Core.Engine
{
    public class StepResult
    {
        public bool KilledInInstruction;
        public int? SetNextIP;
        public int? SplittedInInstruction;
        public readonly Dictionary<int, CellState> MemoryDiff;

        public StepResult()
        {
            KilledInInstruction = false;
            SetNextIP = null;
            SplittedInInstruction = null;
            MemoryDiff = new Dictionary<int, CellState>();
        }

        public void ChangeMemory(int address, Statement statement, int modifiedBy)
        {
            MemoryDiff[address] = new CellState
            {
                CellType = statement.CellType,
                Instruction = statement.ToString(),
                LastModifiedByProgram = modifiedBy
            };
        }
    }
}