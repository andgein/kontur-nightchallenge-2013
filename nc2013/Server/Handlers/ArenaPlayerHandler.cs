using System;
using System.Net;
using Core;
using Core.Game;
using Server.DataContracts;

namespace Server.Handlers
{
	public class ArenaPlayerHandler : StrictPathHttpHandlerBase
	{
		public ArenaPlayerHandler(Arena arena) : base("arena/player")
		{
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var programName = GetStringParam(context, "name");
			var programVersion = GetOptionalIntParam(context, "version");
			SendResponse(context, CreateDummyPlayerInfo());
		}

		private PlayerInfo CreateDummyPlayerInfo()
		{
			return
				new PlayerInfo
				{
					Info = ProgramRankInfo.CreateDummy(0),
					SubmitTime = DateTime.Now - TimeSpan.FromHours(1),
					GamesByEnemy = new []
						{
							new FinishedGamesWithEnemy{Enemy="spaceorc", EnemyVersion = 3, Wins=100, Loses=20, Draws=80, LastGames = new []{new FinishedGameInfo()}},
							new FinishedGamesWithEnemy{Enemy="imp", EnemyVersion = 1, Wins=100, Loses=20, Draws=80, LastGames = new []{new FinishedGameInfo()}},
						}
				};
		}
	}
}