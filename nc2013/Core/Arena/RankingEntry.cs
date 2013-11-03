using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class RankingEntry
	{
		[JsonProperty] public int Draws;
		[JsonProperty] public int Games;
		[JsonProperty] public int Loses;
		[JsonProperty] public string Name;
		[JsonProperty] public int Score;
		[JsonProperty] public int Version;
		[JsonProperty] public int Wins;
	}
}