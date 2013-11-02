using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Core
{
	public static class StopwatchExtensions
	{
		public static string FormatElapsedTime([NotNull] this Stopwatch watch)
		{
			if (watch == null)
				throw new ArgumentNullException("watch");
			var days = string.Empty;
			if (watch.Elapsed.Days > 0)
				days = string.Format("{0}:", watch.Elapsed.Days.ToString("D2"));
			return string.Format(
				"{0}{1}:{2}:{3}.{4}",
				days,
				watch.Elapsed.Hours.ToString("D2"),
				watch.Elapsed.Minutes.ToString("D2"),
				watch.Elapsed.Seconds.ToString("D2"),
				watch.Elapsed.Milliseconds.ToString("D3")
			);
		}
	}
}