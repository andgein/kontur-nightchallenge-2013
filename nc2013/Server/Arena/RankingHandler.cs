using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class RankingHandler : StrictPathHttpHandlerBase
	{
		public RankingHandler()
			: base("arena/ranking") {}

		public override void Handle([NotNull] GameHttpContext context)
		{
			context.SendResponse(Ranking.CreateDummyRanking());
		}
	}
}