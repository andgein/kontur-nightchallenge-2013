using System;
using System.Linq;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly PlayersRepo players;
		private readonly GamesRepo gamesRepo;

		public ArenaPlayerHandler(PlayersRepo players, GamesRepo gamesRepo)
			: base("arena/player")
		{
			this.players = players;
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var programName = context.GetStringParam("name");
			var programVersion = context.GetOptionalIntParam("version");
			var arenaPlayer = players.LoadPlayer(programName, programVersion);
			var playerInfo = CreatePlayerInfo(arenaPlayer);
			context.SendResponse(playerInfo);
		}

		[NotNull]
		private PlayerInfo CreatePlayerInfo([NotNull] ArenaPlayer arenaPlayer)
		{
			var ranking = gamesRepo.LoadRanking();
			var rankingEntry = ranking.Places.FirstOrDefault(r => r.Name == arenaPlayer.Name && r.Version == arenaPlayer.Version) ?? new RankingEntry
			{
				Name = arenaPlayer.Name,
				Version = arenaPlayer.Version,
				Games = 0,
				Score = 0,
			};
			var games = gamesRepo.LoadGames(ranking.TournamentId);
			var gamesByEnemy = games
				.Select(x => Tuple.Create(x.Player1Result, x.Player2Result)).Concat(games.Select(x => Tuple.Create(x.Player2Result, x.Player1Result)))
				.Where(x => x.Item1.Player.Name == arenaPlayer.Name && x.Item1.Player.Version == arenaPlayer.Version)
				.GroupBy(x => x.Item2.Player)
				.Select(g => new FinishedGamesWithEnemy
				{
					Enemy = g.Key.Name,
					EnemyVersion = g.Key.Version,
					Wins = g.Count(x => x.Item2.ResultType == BattlePlayerResultType.Win),
					Draws = g.Count(x => x.Item2.ResultType == BattlePlayerResultType.Draw),
					Losses = g.Count(x => x.Item2.ResultType == BattlePlayerResultType.Loss),
					GameInfos = null,
				})
				.ToArray();
			return new PlayerInfo
			{
				RankingEntry = rankingEntry,
				Authors = arenaPlayer.Authors,
				SubmitTimestamp = arenaPlayer.Timestamp,
				GamesByEnemy = gamesByEnemy,
			};
		}
	}
}