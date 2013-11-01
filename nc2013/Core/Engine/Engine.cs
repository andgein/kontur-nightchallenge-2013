using System.Collections.Generic;
using Core.Parser;

namespace Core.Engine
{
    public class Engine
    {
        private readonly List<RunningWarrior> warriors = new List<RunningWarrior>();
        public readonly Memory Memory;
        public int CurrentWarrior { get; private set; }
        public int CurrentStep { get; private set; }
        public int CurrentIp { get; private set; }
        private StepResult stepResult;

        public Engine(IEnumerable<WarriorStartInfo> warriorsStartInfos)
        {
            Memory = new Memory(Parameters.CORESIZE);
            var idx = 0;
            foreach (var wsi in warriorsStartInfos)
            {
                var warrior = new RunningWarrior(wsi.Warrior, idx++, wsi.LoadAddress);
                warriors.Add(warrior);
                PlaceWarrior(warrior, wsi.LoadAddress);
            }
            CurrentWarrior = 0;
            CurrentStep = 0;
        }

        private void PlaceWarrior(RunningWarrior warrior, int address)
        {
            var statements = warrior.Warrior.Statements;
            for (var i = 0; i < statements.Count; ++i)
                Memory[address + i] = new Instruction(statements[i], ModularArith.Mod(address + i), warrior.Index);
        }

        public StepResult Step()
        {
            if (warriors[CurrentWarrior].Queue.Count == 0)
            {
                var startWarrior = CurrentWarrior;
                CurrentWarrior = (CurrentWarrior + 1)%warriors.Count;
                while (CurrentWarrior != startWarrior && warriors[CurrentWarrior].Queue.Count == 0)
                    CurrentWarrior = (CurrentWarrior + 1)%warriors.Count;
                if (CurrentWarrior == startWarrior)
                    return new StepResult {GameFinished = true};
            }
            CurrentIp = warriors[CurrentWarrior].Queue.Dequeue();
            var instruction = Memory[CurrentIp];

            stepResult = new StepResult();

            ExecuteInstruction(instruction);
            
            if (! stepResult.KilledInInstruction)
                warriors[CurrentWarrior].Queue.Enqueue(stepResult.SetNextIP.HasValue ? stepResult.SetNextIP.GetValueOrDefault() : CurrentIp + 1);

            if (stepResult.SplittedInInstruction.HasValue)
                warriors[CurrentWarrior].Queue.Enqueue(stepResult.SplittedInInstruction.GetValueOrDefault());

            CurrentWarrior = (CurrentWarrior + 1) % warriors.Count;
            CurrentStep++;

            return stepResult;
        }

        private void ExecuteInstruction(Instruction instruction)
        {
            new InstructionExecutor(instruction).Execute(this);
        }

        public void WriteToMemory(int address, Statement statement)
        {
            Memory[address].Statement = statement;
            Memory[address].LastModifiedByProgram = CurrentWarrior;
            stepResult.ChangeMemory(address, statement, CurrentWarrior);
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
