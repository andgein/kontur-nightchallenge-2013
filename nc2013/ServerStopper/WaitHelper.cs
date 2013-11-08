using System;
using System.Threading;

namespace ServerStopper
{
	public static class WaitHelper
	{
		public static bool Wait(TimeSpan timeout, Func<WaitAction> func)
		{
			var waitStartTime = DateTime.Now;
			while (true)
			{
				var result = func();
				if (result == WaitAction.StopWating)
					return true;
				if (DateTime.Now - waitStartTime > timeout)
					return false;
				Thread.Sleep(500);
			}
		}
	}
}