using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class BattlePlayerResult
	{
		[JsonProperty]
		public TournamentPlayer Player;
		[JsonProperty]
		public int StartAddress;
		[JsonProperty]
		public int Score;
	}
}