using System;

namespace Core.Arena
{
	public class CountdownProvider
	{
		private readonly DateTime? contestStartTimestamp;
		private readonly DateTime? contestEndTimestamp;

		public CountdownProvider(DateTime? contestStartTimestamp, TimeSpan contestDuration)
		{
			this.contestStartTimestamp = contestStartTimestamp;
			if (contestStartTimestamp.HasValue)
				contestEndTimestamp = contestStartTimestamp.Value + contestDuration;
		}

		public TimeSpan? GetTimeToContestStart()
		{
			var now = DateTime.UtcNow;
			if (!contestStartTimestamp.HasValue || now > contestStartTimestamp)
				return null;
			return contestStartTimestamp.Value - now;
		}

		public TimeSpan? GetContestTimeLeft()
		{
			var now = DateTime.UtcNow;
			if (contestStartTimestamp.HasValue && now < contestStartTimestamp)
				return null;
			if (!contestEndTimestamp.HasValue || now > contestEndTimestamp)
				return null;
			return contestEndTimestamp.Value - now;
		}
	}
}