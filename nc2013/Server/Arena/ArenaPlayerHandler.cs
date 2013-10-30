using System.Collections.Generic;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;
using System.Linq;

namespace Server.Arena
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly PlayersRepo players;
		private readonly GamesRepo gamesRepo;

		public ArenaPlayerHandler(PlayersRepo players, GamesRepo gamesRepo) : base("arena/player")
		{
			this.players = players;
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var programName = context.GetStringParam("name");
			var programVersion = context.GetOptionalIntParam("version");
			var arenaPlayer = players.LoadPlayer(programName, programVersion);
			context.SendResponse(CreatePlayerInfo(arenaPlayer));
		}

		private PlayerInfo CreatePlayerInfo(ArenaPlayer arenaPlayer)
		{
			var ranking = gamesRepo.LoadRanking();
			var games = gamesRepo.LoadGames(ranking.TournamentId);
			var allGames = games.Concat(games.Select(g => g.Reverse().ToArray()));
			var byEnemy = allGames.Where(g => g[0].Player.Name == arenaPlayer.Name && g[0].Player.Version == arenaPlayer.Version)
								.GroupBy(g => g[1].Player).ToList();
			var rankingEntry = ranking.Places.FirstOrDefault(r => r.Name == arenaPlayer.Name && r.Version == arenaPlayer.Version) ?? new RankingEntry {Games = 0, Wins = 0};
			return
				new PlayerInfo
				{
					Info = new RankingEntry
					{
						Name = arenaPlayer.Name,
						Wins = rankingEntry.Wins,
						Games = rankingEntry.Games
					},
					Authors = arenaPlayer.Authors,
					Version = arenaPlayer.Version,
					SubmitTime = arenaPlayer.Timestamp,
					GamesByEnemy =
						byEnemy.Select(
							e => new FinishedGamesWithEnemy
							{
								Enemy = e.Key.Name,
								EnemyVersion = e.Key.Version,
								Wins = e.Sum(r => r[0].Score),
								Loses = e.Sum(r => r[1].Score),
							}).ToArray()
				};
		}
	}
}