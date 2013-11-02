using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Core;
using JetBrains.Annotations;
using log4net;

namespace Server
{
	public static class Runtime
	{
		private static readonly ILog perfLog = LogManager.GetLogger("Perf");
		private static WinApi.HandlerRoutine onCtrlBreak;

		public static void Init([NotNull] ILog log)
		{
			InitAppDomain(log);
			InitTaskScheduler(log);
		}

		public static void DoWithPerfMeasurement([NotNull] string actionName, [NotNull] Action action)
		{
			var sw = Stopwatch.StartNew();
			try
			{
				action();
			}
			finally
			{
				perfLog.InfoFormat("{0} took {1}", actionName, sw.FormatElapsedTime());
			}
		}

		public static void SetConsoleCtrlHandler([NotNull] Action handler)
		{
			onCtrlBreak = sig =>
			{
				handler();
				return true;
			};
			WinApi.SetConsoleCtrlHandler(onCtrlBreak, true);
		}

		private static void InitAppDomain([NotNull] ILog log)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				log.Fatal("Unhandled exception in current AppDomain", (Exception) args.ExceptionObject);
				Environment.ExitCode = -1;
			};
		}

		private static void InitTaskScheduler([NotNull] ILog log)
		{
			TaskScheduler.UnobservedTaskException += (sender, args) =>
			{
				log.Fatal("Unobserved TaskException", args.Exception);
				args.SetObserved();
			};
		}

		private static class WinApi
		{
			public delegate bool HandlerRoutine(uint dwCtrlType);

			[DllImport("kernel32.dll")]
			public static extern bool SetConsoleCtrlHandler([NotNull] HandlerRoutine handler, bool add);
		}
	}
}