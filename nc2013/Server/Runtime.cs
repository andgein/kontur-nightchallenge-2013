using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Core;
using JetBrains.Annotations;
using log4net;

namespace Server
{
	public static class Runtime
	{
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
				Log.Perf.InfoFormat("{0} took {1}", actionName, sw.FormatElapsedTime());
			}
		}

		public static void SetStopHandler([NotNull] Action handler)
		{
			onCtrlBreak = sig =>
			{
				handler();
				return true;
			};
			var stopSignal = CreateStopSignal();
			RegisterStopSignalCallback(stopSignal, handler);
			WinApi.SetConsoleCtrlHandler(onCtrlBreak, true);
		}

		private static void RegisterStopSignalCallback([NotNull] WaitHandle stopSignal, [NotNull] Action onStopSignalCallback)
		{
			var state = new RegisteredWaitHandleState();
			state.Handle = ThreadPool.RegisterWaitForSingleObject(stopSignal, (o, timedOut) =>
			{
				onStopSignalCallback();
				var handle = ((RegisteredWaitHandleState)o).Handle;
				if (handle != null)
					handle.Unregister(null);
			}, state, Timeout.Infinite, true);
		}

		[NotNull]
		private static EventWaitHandle CreateStopSignal()
		{
			var stopSignalName = string.Format("Global\\{0}", Process.GetCurrentProcess().ProcessName);
			var everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
			var handleSecurity = new EventWaitHandleSecurity();
			handleSecurity.AddAccessRule(new EventWaitHandleAccessRule(everyoneSid, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow));
			bool createdNew;
			var stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset, stopSignalName, out createdNew, handleSecurity);
			if (!createdNew)
				stopSignal.Reset();
			return stopSignal;
		}

		private static void InitAppDomain([NotNull] ILog log)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				log.Fatal("Unhandled exception in current AppDomain", (Exception) args.ExceptionObject);
				Environment.Exit(-1);
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

		private class RegisteredWaitHandleState
		{
			public RegisteredWaitHandle Handle { get; set; }
		}

	}
}