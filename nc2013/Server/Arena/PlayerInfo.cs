using System;
using Core.Arena;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty] public string Authors;
		[JsonProperty] public FinishedGamesWithEnemy[] GamesByEnemy;
		[JsonProperty] public RankingEntry Info;
		[JsonProperty] public DateTime SubmitTime;
		[JsonProperty] public int Version;
	}
}