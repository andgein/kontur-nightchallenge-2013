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

		[JsonProperty]
		public string Label;

		public override string ToString()
		{
			return string.Format("Label: {0}, Player1Result: {1}, Player2Result: {2}", Label, Player1Result, Player2Result);
		}
	}
}