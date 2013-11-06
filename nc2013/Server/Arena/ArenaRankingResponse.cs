using Core.Arena;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class ArenaRankingResponse
	{
		[JsonProperty]
		public TournamentRanking Ranking;

		[JsonProperty]
		public TournamentHistoryItem[] HistoryItems;

		[JsonProperty]
		public bool GodMode;
	}
}