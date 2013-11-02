using System;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class TournamentRanking
	{
		[JsonProperty]
		public string TournamentId;

		[JsonProperty]
		public DateTime Timestamp;

		[JsonProperty]
		public RankingEntry[] Places;
	}
}