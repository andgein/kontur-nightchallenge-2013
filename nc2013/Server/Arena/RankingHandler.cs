using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class RankingHandler : StrictPathHttpHandlerBase
	{
		public RankingHandler(GamesHistory arena)
			: base("arena/ranking") {}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			context.SendResponse(Ranking.CreateDummyRanking());
		}
	}
}