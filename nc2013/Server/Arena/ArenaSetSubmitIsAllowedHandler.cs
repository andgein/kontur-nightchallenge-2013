using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaSetSubmitIsAllowedHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public ArenaSetSubmitIsAllowedHandler([NotNull] ArenaState arenaState)
			: base("arena/submit/allowed/set")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			if (!godMode)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			arenaState.SubmitIsAllowed = context.GetBoolParam("value");
			context.Redirect(context.BasePath + "submit.html");
		}
	}
}