using System;
using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty] public ProgramRankInfo Info;
		[JsonProperty] public DateTime SubmitTime;
		[JsonProperty] public FinishedGamesWithEnemy[] GamesByEnemy;
	}
}