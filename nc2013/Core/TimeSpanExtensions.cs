using System;

namespace Core
{
	public static class TimeSpanExtensions
	{
		public static TimeSpan DropMillis(this TimeSpan timeSpan)
		{
			return new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
	}
}