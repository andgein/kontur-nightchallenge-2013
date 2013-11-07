using Core.Arena;
using JetBrains.Annotations;

namespace Server.Handlers
{
	public class IndexHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public IndexHandler([NotNull] ArenaState arenaState)
			: base("index")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			var response = new IndexResponse
			{
				NavigationIsDisabled = arenaState.GodAccessOnly && !godMode,
			};
			context.SendResponse(response);
		}
	}
}