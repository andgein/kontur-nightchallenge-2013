using System;
using Core.Engine;

namespace Core.Arena
{
	public class RandomAllocator
	{
		private readonly int coresize;
		private readonly int minDistance;
		private readonly Random rnd;

		public RandomAllocator(int coresize, int minDistance)
		{
			this.coresize = coresize;
			this.minDistance = minDistance;
			rnd = new Random();
		}

		public int NextLoadAddress(int baseAddress, int length)
		{
			var positions = coresize + 1 - (minDistance * 2) - length;
			var nextLoadAddress = ModularArith.Mod(baseAddress + minDistance + rnd.Next() % positions);
			return nextLoadAddress;
		}
	}
}
