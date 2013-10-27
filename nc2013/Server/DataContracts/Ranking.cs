using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class Ranking
	{
		[JsonProperty] public ProgramRankInfo[] Programs;

		public static Ranking CreateDummyRanking()
		{
			return new Ranking
				{
					Programs = new[]
						{
							ProgramRankInfo.CreateDummy(0),
							ProgramRankInfo.CreateDummy(1),
							ProgramRankInfo.CreateDummy(2),
						}
				};
		}
	}

	[JsonObject]
	public class ProgramRankInfo
	{
		[JsonProperty] public string Author;
		[JsonProperty] public int Loses;
		[JsonProperty] public string Name;
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
								Author = "Pavel Egorov",
								Loses = 10,
								Wins = 100500,
								TotalGames = 100510
							},
						new ProgramRankInfo
							{
								Name = "spaceorc",
								Author = "Ivan Dashkevich",
								Loses = 100500,
								Wins = 10,
								TotalGames = 100510
							},
						new ProgramRankInfo
							{
								Name = "imp",
								Author = "Andrey {Kostousov, Gein}",
								Loses = 0,
								Wins = 0,
								TotalGames = 300
							}
					}[i%3];
		}
	}
}