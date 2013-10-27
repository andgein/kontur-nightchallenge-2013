using System.Net;
using Core;
using Server.DataContracts;

namespace Server.Handlers
{
	public class RankingHandler : GameHandlerBase
	{
		public RankingHandler(Arena arena)
			: base("/corewars/ranking")
		{
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var ranking = CreateDummyRanking();
			SendResponse(context, ranking);
		}

		private static Ranking CreateDummyRanking()
		{
			return new Ranking
				{
					Programs = new[]
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
						}
				};
		}
	}
}