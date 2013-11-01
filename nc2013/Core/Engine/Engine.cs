using System.Collections.Generic;
using System.Linq;
using Core.Parser;

namespace Core.Engine
{
    public class Engine
    {
        private readonly List<RunningWarrior> warriors;
        public readonly Memory Memory;
        private int currentWarrior;
        private int currentStep;
        public int CurrentIp { get; private set; }
        private StepResult stepResult;

        public Engine(IEnumerable<Warrior> warriors)
        {
            Memory = new Memory(Parameters.CORESIZE);
            // TODO 0 -> loadAddress
            this.warriors = warriors.Select((w, idx) => new RunningWarrior(w, idx, 0)).ToList();
            PlaceWarrior(this.warriors[0], 0);
            currentWarrior = 0;
            currentStep = 0;
        }

        private void PlaceWarrior(RunningWarrior warrior, int address)
        {
            var statements = warrior.Warrior.Statements;
            for (var i = 0; i < statements.Count; ++i)
                Memory[address + i] = new Instruction(statements[i], ModularArith.Mod(address + i), warrior.Index);
        }

        public StepResult Step()
        {
            if (warriors[currentWarrior].Queue.Count == 0)
            {
                var startWarrior = currentWarrior;
                currentWarrior = (currentWarrior + 1)%warriors.Count;
                while (currentWarrior != startWarrior && warriors[currentWarrior].Queue.Count == 0)
                    currentWarrior = (currentWarrior + 1)%warriors.Count;
                if (currentWarrior == startWarrior)
                    return new StepResult {GameFinished = true};
            }
            CurrentIp = warriors[currentWarrior].Queue.Dequeue();
            var instruction = Memory[CurrentIp];

            stepResult = new StepResult();

            ExecuteInstruction(instruction);
            
            if (! stepResult.KilledInInstruction)
                warriors[currentWarrior].Queue.Enqueue(stepResult.SetNextIP.HasValue ? stepResult.SetNextIP.GetValueOrDefault() : CurrentIp + 1);

            if (stepResult.SplittedInInstruction.HasValue)
                warriors[currentWarrior].Queue.Enqueue(stepResult.SplittedInInstruction.GetValueOrDefault());

            currentWarrior = (currentWarrior + 1) % warriors.Count;
            currentStep++;

            return stepResult;
        }

        private void ExecuteInstruction(Instruction instruction)
        {
            new InstructionExecutor(instruction).Execute(this);
        }

        public void WriteToMemory(int address, Statement statement)
        {
            Memory[address].Statement = statement;
            Memory[address].LastModifiedByProgram = currentWarrior;
        }

        public void KillCurrentProcess()
        {
            stepResult.KilledInInstruction = true;
        }

        public void JumpTo(int address)
        {
            stepResult.SetNextIP = address;
        }

        public void SplitAt(int address)
        {
            stepResult.SplittedInInstruction = address;
        }
    }
}
