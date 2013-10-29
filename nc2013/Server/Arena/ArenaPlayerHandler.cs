using System;
using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		public ArenaPlayerHandler(GamesHistory arena) : base("arena/player") {}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			var programName = context.GetStringParam("name");
			var programVersion = context.GetOptionalIntParam("version");
			context.SendResponse(CreateDummyPlayerInfo());
		}

		private PlayerInfo CreateDummyPlayerInfo()
		{
			return
				new PlayerInfo
				{
					Info = ProgramRankInfo.CreateDummy(0),
					SubmitTime = DateTime.Now - TimeSpan.FromHours(1),
					GamesByEnemy = new[]
					{
						new FinishedGamesWithEnemy {Enemy = "spaceorc", EnemyVersion = 3, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
						new FinishedGamesWithEnemy {Enemy = "imp", EnemyVersion = 1, Wins = 100, Loses = 20, Draws = 80, LastGames = new[] {new FinishedGameInfo()}},
					}
				};
		}
	}
}