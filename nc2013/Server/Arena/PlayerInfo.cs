using System;
using Core.Arena;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty]
		public RankingEntry RankingEntry;

		[JsonProperty]
		public string Authors;

		[JsonProperty]
		public DateTime SubmitTimestamp;

		[JsonProperty]
		public FinishedGamesWithEnemy[] GamesByEnemy;

		[JsonProperty]
		public BotVersionInfo[] BotVersionInfos;
	}
}