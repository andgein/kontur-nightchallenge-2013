using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class RankingEntry
	{
		[JsonProperty]
		public string Name;

		[JsonProperty]
		public int Version;

		[JsonProperty]
		public int Score;

		[JsonProperty]
		public int Games;
	}
}