using System;
using Newtonsoft.Json;
using Server.Handlers;

namespace Server.DataContracts
{
	[JsonObject]
	public class PlayerInfo
	{
		[JsonProperty]
		public ProgramRankInfo Info;
		[JsonProperty]
		public DateTime SubmitTime;
		[JsonProperty]
		public FinishedGamesWithEnemy[] GamesByEnemy;
	}

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
		public int Loses;
		[JsonProperty]
		public int Draws;

		[JsonProperty]
		public FinishedGameInfo[] LastGames;
	}
}