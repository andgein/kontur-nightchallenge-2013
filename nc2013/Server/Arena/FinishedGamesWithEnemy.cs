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

		public static FinishedGamesWithEnemy CreateDummy(int n = 0)
		{
			return new[]
			{
				new FinishedGamesWithEnemy {Enemy = "spaceorc", EnemyVersion = 3, Score = 100, EnemyScore = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
				new FinishedGamesWithEnemy {Enemy = "imp", EnemyVersion = 1, Score = 100, EnemyScore = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
			}[n];
		}
	}
}