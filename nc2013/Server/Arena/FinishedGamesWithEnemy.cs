using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class FinishedGamesWithEnemy
	{
		[JsonProperty] public int Draws;
		[JsonProperty] public string Enemy;
		[JsonProperty] public int EnemyVersion;
		[JsonProperty] public FinishedGameInfo[] LastGames;
		[JsonProperty] public int EnemyScore;
		[JsonProperty] public int Score;
	}
}