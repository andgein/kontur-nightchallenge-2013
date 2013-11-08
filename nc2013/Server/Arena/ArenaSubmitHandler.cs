using System.Net;
using Core;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaSubmitHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;
		private readonly ITournamentRunner tournamentRunner;

		public ArenaSubmitHandler([NotNull] ArenaState arenaState, [NotNull] ITournamentRunner tournamentRunner)
			: base("arena/submit")
		{
			this.arenaState = arenaState;
			this.tournamentRunner = tournamentRunner;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			if (!arenaState.SubmitIsAllowed)
				throw new HttpException(HttpStatusCode.Forbidden, "Bot submission is disabled");

			var arenaPlayer = context.GetRequest<ArenaPlayer>();
			try
			{
				if (arenaState.PlayersRepo.CreateOrUpdate(arenaPlayer))
				{
					Log.For(this).Info(string.Format("New bot submitted: {0}", arenaPlayer));
					tournamentRunner.SignalBotSubmission();
				}
			}
			catch (BadBotException e)
			{
				Log.For(this).Warn(string.Format("Bot submission failed: {0}", arenaPlayer), e);
				throw new HttpException(HttpStatusCode.BadRequest, e.Message, e);
			}
		}
	}
}