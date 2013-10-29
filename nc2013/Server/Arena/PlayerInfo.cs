using System;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty] public ProgramRankInfo Info;
		[JsonProperty] public DateTime SubmitTime;
		[JsonProperty] public FinishedGamesWithEnemy[] GamesByEnemy;
	}
}