using System;
using System.Net;
using Core;
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
					LastFinishedGames = new FinishedGameInfo[0]
				};
		}
	}
}