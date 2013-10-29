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
		[JsonProperty] public int Loses;
		[JsonProperty] public int Wins;

		public static FinishedGamesWithEnemy CreateDummy(int n = 0)
		{
			return new[]
			{
				new FinishedGamesWithEnemy {Enemy = "spaceorc", EnemyVersion = 3, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
				new FinishedGamesWithEnemy {Enemy = "imp", EnemyVersion = 1, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
			}[n];
		}
	}
}