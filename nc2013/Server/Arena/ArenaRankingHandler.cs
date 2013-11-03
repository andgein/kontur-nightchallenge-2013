using System;
using System.Linq;
using System.Threading;
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
			var tournamentHistoryItems = gamesRepo.GetAllTournamentIds().Select(id => new TournamentHistoryItem
			{
				TournamentId = id,
				CreationTimestamp = new DateTime(long.Parse(id), DateTimeKind.Utc),
			}).OrderByDescending(x => x.CreationTimestamp).ToArray();
			var response = new ArenaRankingResponse
			{
				Ranking = ranking,
				HistoryItems = tournamentHistoryItems,
			};
			context.SendResponse(response);
		}
	}
}