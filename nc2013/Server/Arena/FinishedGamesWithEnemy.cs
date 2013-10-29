using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class FinishedGamesWithEnemy
	{
		[JsonProperty] public string Enemy;
		[JsonProperty] public int EnemyVersion;
		[JsonProperty] public int Wins;
		[JsonProperty] public int Loses;
		[JsonProperty] public int Draws;

		[JsonProperty] public FinishedGameInfo[] LastGames;
	}
}