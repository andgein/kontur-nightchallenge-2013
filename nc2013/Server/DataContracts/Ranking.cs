using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class Ranking
	{
		[JsonProperty]
		public ProgramRankInfo[] Programs;
	}

	[JsonObject]
	public class ProgramRankInfo
	{
		[JsonProperty]
		public string Name;
		[JsonProperty]
		public string Author;
		[JsonProperty]
		public int Wins;
		[JsonProperty]
		public int Loses;
		[JsonProperty]
		public int TotalGames;
	}
}