using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaRankingHandler : StrictPathHttpHandlerBase
	{
		private readonly IGamesRepo gamesRepo;

		public ArenaRankingHandler([NotNull] IGamesRepo gamesRepo)
			: base("arena/ranking")
		{
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			var tournamentId = context.GetOptionalStringParam("tournamentId");
			var ranking = gamesRepo.TryLoadRanking(tournamentId ?? "last");
			context.SendResponse(ranking);
		}
	}
}