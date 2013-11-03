using System.Collections.Generic;

namespace Core.Engine
{
	public class RunningQueue : Queue<int>
	{
		private readonly int coresize;

		public RunningQueue(int coresize)
		{
			this.coresize = coresize;
		}

		public new void Enqueue(int x)
		{
			if (Count >= Parameters.MaxQueueSize)
				return;
			base.Enqueue(ModularArith.Mod(x, coresize));
		}
	}
}