using System;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class TournamentHistoryItem
	{
		[JsonProperty]
		public string TournamentId;

		[JsonProperty]
		public DateTime CreationTimestamp;
	}
}