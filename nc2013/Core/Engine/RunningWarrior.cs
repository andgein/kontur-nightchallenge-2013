using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Parser;

namespace Core.Engine
{
    class RunningWarrior
    {
        public Warrior Warrior { get; private set; }

        public int Index { get; private set; }

        public readonly Queue<int> Queue;

        public RunningWarrior(Warrior warrior, int index, int loadAddress)
        {
            Warrior = warrior;
            Index = index;
            Queue = new Queue<int>();
            Queue.Enqueue(loadAddress);
        }
    }
}
