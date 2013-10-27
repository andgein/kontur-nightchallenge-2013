using System;
using Newtonsoft.Json;
using Server.Handlers;

namespace Server.DataContracts
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty]
		public ProgramRankInfo Info;
		[JsonProperty]
		public DateTime SubmitTime;
		[JsonProperty]
		public FinishedGameInfo[] LastFinishedGames;
	}
}