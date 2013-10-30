using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class RankingHandler : StrictPathHttpHandlerBase
	{
		private readonly GamesRepo gamesRepo;

		public RankingHandler(GamesRepo gamesRepo)
			: base("arena/ranking")
		{
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			context.SendResponse(gamesRepo.LoadRanking());
		}
	}
}