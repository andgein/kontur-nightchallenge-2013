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

		public override string ToString()
		{
			return string.Format("Enemy: {0}, EnemyVersion: {1}, Wins: {2}, Draws: {3}, Loses: {4}", Enemy, EnemyVersion, Wins, Draws, Loses);
		}
	}
}