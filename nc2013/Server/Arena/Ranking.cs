using Newtonsoft.Json;

namespace Server.Arena
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
}