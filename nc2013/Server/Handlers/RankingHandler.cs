using System.Net;
using Core;
using Server.DataContracts;

namespace Server.Handlers
{
	public class RankingHandler : StrictPathHttpHandlerBase
	{
		public RankingHandler(Arena arena)
			: base("arena/ranking")
		{
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			SendResponse(context, Ranking.CreateDummyRanking());
		}

	}
}