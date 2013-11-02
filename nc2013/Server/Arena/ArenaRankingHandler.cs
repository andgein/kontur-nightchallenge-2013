using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaRankingHandler : StrictPathHttpHandlerBase
	{
		private readonly GamesRepo gamesRepo;

		public ArenaRankingHandler(GamesRepo gamesRepo)
			: base("arena/ranking")
		{
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var ranking = gamesRepo.TryLoadRanking();
			context.SendResponse(ranking);
		}
	}
}