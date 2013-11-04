using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class FinishedGamesWithEnemy
	{
		[JsonProperty]
		public string Enemy;

		[JsonProperty]
		public int EnemyVersion;

		[JsonProperty]
		public int Wins;

		[JsonProperty]
		public int Draws;

		[JsonProperty]
		public int Loses;

		[JsonProperty]
		public int Games { get { return Wins + Draws + Loses; } }

		[JsonProperty]
		public FinishedGameInfo[] GameInfos;
	}
}