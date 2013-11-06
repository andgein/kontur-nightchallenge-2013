using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaRemovePlayerHandler : StrictPathHttpHandlerBase
	{
		private readonly IPlayersRepo playersRepo;
		private readonly IGamesRepo gamesRepo;

		public ArenaRemovePlayerHandler([NotNull] IPlayersRepo playersRepo, [NotNull] IGamesRepo gamesRepo)
			: base("arena/player/remove")
		{
			this.playersRepo = playersRepo;
			this.gamesRepo = gamesRepo;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			if (!godMode)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			var playerName = context.GetOptionalStringParam("name");
			gamesRepo.RemovePlayer(playerName);
			playersRepo.Remove(playerName);

			context.Redirect(context.BasePath + "ranking.html");
		}
	}
}