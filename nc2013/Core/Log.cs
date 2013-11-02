using log4net;

namespace Core
{
	public static class Log
	{
		public static readonly ILog Perf = LogManager.GetLogger("Perf");
		public static readonly ILog Network = LogManager.GetLogger("Network");

		private static class LogContainer<T>
		{
			public static readonly ILog Logger = LogManager.GetLogger(typeof(T));
		}

		public static ILog For<T>()
		{
			return LogContainer<T>.Logger;
		}

		public static ILog For<T>(T instance)
		{
			return For<T>();
		}
	}
}