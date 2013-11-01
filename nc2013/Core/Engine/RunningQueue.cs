using System.Collections.Generic;

namespace Core.Engine
{
    public class RunningQueue : Queue<int>
    {
        public new void Enqueue(int x)
        {
            if (Count >= Parameters.MaxQueueSize)
                return;
            base.Enqueue(x);
        }
    }
}