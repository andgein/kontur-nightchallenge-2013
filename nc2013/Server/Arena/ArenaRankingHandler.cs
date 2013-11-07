using System;
using System.Linq;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaRankingHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public ArenaRankingHandler([NotNull] ArenaState arenaState)
			: base("arena/ranking")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			var tournamentId = context.GetOptionalStringParam("tournamentId");
			var ranking = arenaState.GamesRepo.TryLoadRanking(tournamentId ?? "last");
			var tournamentHistoryItems = arenaState.GamesRepo.GetAllTournamentIds().Select(id => new TournamentHistoryItem
			{
				TournamentId = id,
				CreationTimestamp = new DateTime(long.Parse(id), DateTimeKind.Utc),
			}).OrderByDescending(x => x.CreationTimestamp).ToArray();
			var response = new ArenaRankingResponse
			{
				Ranking = ranking,
				HistoryItems = tournamentHistoryItems,
				TournamentIsRunning = arenaState.TournamentIsRunning,
				GodMode = godMode,
			};
			context.SendResponse(response);
		}
	}
}