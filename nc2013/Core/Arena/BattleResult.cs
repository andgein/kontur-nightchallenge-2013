using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class BattleResult
	{
		[JsonProperty]
		public bool RunToCompletion;

		[JsonProperty]
		public BattlePlayerResult Player1Result;

		[JsonProperty]
		public BattlePlayerResult Player2Result;

		[JsonIgnore]
		public BattlePlayerResult[] Results
		{
			get { return new[] { Player1Result, Player2Result }; }
		}
	}
}