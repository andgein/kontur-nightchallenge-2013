using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;

namespace ServerStopper
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var timeoutInSeconds = 30;
			if (args.Length > 0)
			{
				int parsedTimeout;
				if (int.TryParse(args[0], out parsedTimeout))
					timeoutInSeconds = parsedTimeout;
				else Console.Out.WriteLine("Can't parse timeout from '{0}'", args[0]);
			}
			KillCoreWarProcesses(TimeSpan.FromSeconds(timeoutInSeconds));
		}

		public static void KillCoreWarProcesses(TimeSpan waitForTerminationTimeout)
		{
			if (SignalTermination())
			{
				var allProcessesTerminated = WaitHelper.Wait(waitForTerminationTimeout, () => GetCoreWarProcesses().Any() ? WaitAction.ContinueWaiting : WaitAction.StopWating);
				if (!allProcessesTerminated)
					KillCoreWarProcesses();
			}
		}

		private static void KillCoreWarProcesses()
		{
			foreach (var process in GetCoreWarProcesses())
			{
				process.Kill();
				Console.Out.WriteLine("Process killed: {0}", process.ProcessName);
			}
		}

		private static bool SignalTermination()
		{
			var diadocProcesses = GetCoreWarProcesses();
			foreach (var process in diadocProcesses)
			{
				var eventName = string.Format("Global\\{0}", process.ProcessName);
				try
				{
					using (var stopSignal = EventWaitHandle.OpenExisting(eventName, EventWaitHandleRights.Modify))
						stopSignal.Set();
					Console.Out.WriteLine("Event signaled: {0}", eventName);
				}
				catch (UnauthorizedAccessException e)
				{
					Console.Out.WriteLine("Event not signaled: {0}. Exception: {1}", eventName, e);
				}
				catch (IOException e)
				{
					Console.Out.WriteLine("Event not signaled: {0}. Exception: {1}", eventName, e);
				}
				catch (WaitHandleCannotBeOpenedException e)
				{
					Console.Out.WriteLine("Event not signaled: {0}. Exception: {1}", eventName, e);
				}
			}
			return diadocProcesses.Any();
		}

		public static List<Process> GetCoreWarProcesses()
		{
			return Process.GetProcesses()
				.Where(p => p.ProcessName == "corewar")
				.ToList();
		}
	}
}