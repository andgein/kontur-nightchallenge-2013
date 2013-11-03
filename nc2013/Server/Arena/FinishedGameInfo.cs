using Core.Arena;
using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class FinishedGameInfo
	{
		[JsonProperty]
		public BattlePlayerResult Player1Result;

		[JsonProperty]
		public BattlePlayerResult Player2Result;
	}
}