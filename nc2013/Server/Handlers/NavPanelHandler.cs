using Core;
using Core.Arena;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public class NavPanelHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public NavPanelHandler([NotNull] ArenaState arenaState)
			: base("nav")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var timeToContestStart = arenaState.CountdownProvider.GetTimeToContestStart();
			var contestTimeLeft = arenaState.CountdownProvider.GetContestTimeLeft();
			var response = new NavPanelResponse
			{
				NavigationIsDisabled = arenaState.GodAccessOnly && !context.GodMode,
				TimeToContestStart = !timeToContestStart.HasValue ? null : timeToContestStart.Value.DropMillis().ToString("c"),
				ContestTimeLeft = !contestTimeLeft.HasValue ? null : contestTimeLeft.Value.DropMillis().ToString("c"),
			};
			context.SendResponse(response);
		}
	}
}