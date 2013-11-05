using System;
using Core.Arena;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty] public string Authors;
		[JsonProperty] public BotVersionInfo[] BotVersionInfos;
		[JsonProperty] public FinishedGamesWithEnemy[] GamesByEnemy;
		[JsonProperty] public bool GodMode;
		[JsonProperty] public string Program;
		[JsonProperty] public RankingEntry RankingEntry;
		[JsonProperty] public DateTime SubmitTimestamp;
	}
}