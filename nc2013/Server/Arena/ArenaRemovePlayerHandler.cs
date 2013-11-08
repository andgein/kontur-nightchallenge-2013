using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaRemovePlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public ArenaRemovePlayerHandler([NotNull] ArenaState arenaState)
			: base("arena/player/remove")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			if (!context.GodMode)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			var playerName = context.GetStringParam("name");
			arenaState.GamesRepo.RemovePlayer(playerName);
			arenaState.PlayersRepo.Remove(playerName);

			context.Redirect(context.BasePath + "ranking.html");
		}
	}
}