using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly IPlayersRepo playersRepo;
		private readonly IGamesRepo gamesRepo;

		public ArenaPlayerHandler([NotNull] IPlayersRepo playersRepo, [NotNull] IGamesRepo gamesRepo)
			: base("arena/player")
		{
			this.playersRepo = playersRepo;
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			var playerName = context.GetStringParam("name");
			var version = context.GetOptionalIntParam("version");
			var playerVersions = playersRepo.LoadPlayerVersions(playerName);
			var botVersionInfos = playerVersions.Select(p => new BotVersionInfo
			{
				Name = p.Name,
				Version = p.Version,
				Timestamp = p.Timestamp
			})
			.OrderByDescending(x => x.Version)
			.ToArray();
			ArenaPlayer arenaPlayer;
			TournamentRanking ranking = null;
			var playerInfo = new PlayerInfo();
			var lastVersion = playerVersions.GetLastVersion();
			if (version.HasValue)
			{
				arenaPlayer = playerVersions.FirstOrDefault(p => p.Version == version.Value);
				if (arenaPlayer != null)
				{
					var tournamentId = "last";
					if (arenaPlayer.Version != lastVersion.Version)
					{
						var nextVersion = playerVersions.First(p => p.Version > version.Value);
						tournamentId = gamesRepo.GetAllTournamentIds()
							.Select(id => new DateTime(long.Parse(id), DateTimeKind.Utc))
							.OrderBy(ts => ts)
							.LastOrDefault(ts => ts < nextVersion.Timestamp)
							.Ticks.ToString();
					}
					ranking = gamesRepo.TryLoadRanking(tournamentId);
				}
			}
			else
			{
				arenaPlayer = lastVersion;
				ranking = gamesRepo.TryLoadRanking("last");
			}
			if (ranking != null)
				playerInfo = CreatePlayerInfo(arenaPlayer, ranking, botVersionInfos, godMode);
			context.SendResponse(playerInfo);
		}

		[NotNull]
		private PlayerInfo CreatePlayerInfo([NotNull] ArenaPlayer arenaPlayer, [NotNull] TournamentRanking ranking, [NotNull] BotVersionInfo[] botVersionInfos, bool godMode)
		{
			var rankingEntry = ranking.Places.FirstOrDefault(r => r.Name == arenaPlayer.Name && r.Version == arenaPlayer.Version) ?? new RankingEntry
			{
				Name = arenaPlayer.Name,
				Version = arenaPlayer.Version,
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
					Wins = g.Count(x => x.Item1.ResultType == BattlePlayerResultType.Win),
					Draws = g.Count(x => x.Item1.ResultType == BattlePlayerResultType.Draw),
					Loses = g.Count(x => x.Item1.ResultType == BattlePlayerResultType.Loss),
					GameInfos = godMode ? GetGameInfos(g) : null,
				})
				.OrderByDescending(x => x.Wins)
				.ToArray();
			var playerInfo = new PlayerInfo
			{
				RankingEntry = rankingEntry,
				Authors = arenaPlayer.Authors,
				SubmitTimestamp = arenaPlayer.Timestamp,
				GamesByEnemy = gamesByEnemy,
				BotVersionInfos = botVersionInfos,
				GodMode = godMode,
			};
			return playerInfo;
		}

		[NotNull]
		private static FinishedGameInfo[] GetGameInfos([NotNull] IEnumerable<Tuple<BattlePlayerResult, BattlePlayerResult>> games)
		{
			return games.Select(x => new FinishedGameInfo
			{
				Player1Result = x.Item1,
				Player2Result = x.Item2,
			}).ToArray();
		}
	}
}