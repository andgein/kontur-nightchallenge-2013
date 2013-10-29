using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class ProgramRankInfo
	{
		[JsonProperty] public string Name;
		[JsonProperty] public int Loses;
		[JsonProperty] public int TotalGames;
		[JsonProperty] public int Wins;

		public static ProgramRankInfo CreateDummy(int i = 0)
		{
			return
				new[]
				{
					new ProgramRankInfo
					{
						Name = "xoposhiy",
						Loses = 10,
						Wins = 100500,
						TotalGames = 100510
					},
					new ProgramRankInfo
					{
						Name = "spaceorc",
						Loses = 100500,
						Wins = 10,
						TotalGames = 100510
					},
					new ProgramRankInfo
					{
						Name = "imp",
						Loses = 0,
						Wins = 0,
						TotalGames = 300
					}
				}[i%3];
		}
	}
}