using System.Collections.Generic;
using System.Linq;
using Core.Parser;
using NUnit.Framework;

namespace Core.Engine
{
    public class Engine
    {
        private readonly List<RunningWarrior> warriors;
        private int currentWarrior;
        public readonly List<Instruction> Memory;
        private int currentStep;
        private bool killedInInstruction;

        public Engine(IEnumerable<Warrior> warriors)
        {
            Memory = Enumerable.Range(0, Parameters.CORESIZE).Select(x => new Instruction()).ToList();
            // TODO 0 -> loadAddress
            this.warriors = warriors.Select((w, idx) => new RunningWarrior(w, idx, 0)).ToList();
            PlaceWarrior(this.warriors[0], 0);
            currentWarrior = 0;
            currentStep = 0;
        }

        public int CurrentIp { get; private set; }

        private void PlaceWarrior(RunningWarrior warrior, int address)
        {
            var statements = warrior.Warrior.Statements;
            for (var i = 0; i < statements.Count; ++i)
                Memory[(address + i)%Parameters.CORESIZE] = new Instruction(statements[i], warrior.Index);
        }

        public StepResult Step()
        {
            CurrentIp = warriors[currentWarrior].Queue.Dequeue();
            var instruction = Memory[CurrentIp];

            ExecuteInstruction(instruction);
            
            if (! killedInInstruction)
                warriors[currentWarrior].Queue.Enqueue(CurrentIp + 1);
            currentWarrior = (currentWarrior + 1) % warriors.Count;
            currentStep++;

            return new StepResult();
        }

        private void ExecuteInstruction(Instruction instruction)
        {
            var stmt = instruction.Statement;
            stmt.Execute(this);
        }

        public void WriteToMemory(int address, Statement statement)
        {
            Memory[address].Statement = statement;
            Memory[address].LastModifiedByProgram = currentWarrior;
            Memory[address].LastModifiedStep = currentStep;
        }

        public void KillCurrentProcess()
        {
            killedInInstruction = true;
        }
    }

    public class Instruction
    {
        public Statement Statement { get; set; }

        public int? LastModifiedByProgram = null;
        public int? LastModifiedStep = null;

        public Instruction() : this(new DatStatement
            {
                FieldA = new NumberExpression(0),
                ModeA = AddressingMode.Direct,
                FieldB = new NumberExpression(0),
                ModeB = AddressingMode.Direct
            },
            null)
        {
        }

        public Instruction(Statement statement, int? owner)
        {
            Statement = statement;
            LastModifiedByProgram = owner;
        }
    }

    public class StepResult
    {
        
    }
}
