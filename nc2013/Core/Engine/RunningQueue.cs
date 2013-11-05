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

		public new bool Enqueue(int x)
		{
			if (Count >= Parameters.MaxQueueSize)
				return false;
			base.Enqueue(ModularArith.Mod(x, coresize));
			return true;
		}

		public int? PeekOrNull()
		{
			return Count > 0 ? (int?) Peek() : null;
		}
	}
}