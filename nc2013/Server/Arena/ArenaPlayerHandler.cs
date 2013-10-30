using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly PlayersRepo players;

		public ArenaPlayerHandler(PlayersRepo players) : base("arena/player")
		{
			this.players = players;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var programName = context.GetStringParam("name");
			var programVersion = context.GetOptionalIntParam("version");
			var arenaPlayer = players.LoadPlayer(programName, programVersion);
			context.SendResponse(CreateDummyPlayerInfo(arenaPlayer));
		}

		private static PlayerInfo CreateDummyPlayerInfo(ArenaPlayer arenaPlayer)
		{
			return
				new PlayerInfo
				{
					Info = new ProgramRankInfo
					{
						Name = arenaPlayer.Name,
						Loses = 10,
						Wins = 100500,
						TotalGames = 100510
					},
					Authors = arenaPlayer.Authors,
					Version = arenaPlayer.Version,
					SubmitTime = arenaPlayer.Timestamp,
					GamesByEnemy = new[]
					{
						new FinishedGamesWithEnemy {Enemy = "spaceorc", EnemyVersion = 3, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
						new FinishedGamesWithEnemy {Enemy = "imp", EnemyVersion = 1, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}}
					}
				};
		}
	}
}