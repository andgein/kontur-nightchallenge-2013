using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaSubmitFormHandler : StrictPathHttpHandlerBase
	{
		private readonly ArenaState arenaState;

		public ArenaSubmitFormHandler([NotNull] ArenaState arenaState)
			: base("arena/submit/form")
		{
			this.arenaState = arenaState;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var response = new ArenaSubmitFormResponse
			{
				SubmitIsAllowed = arenaState.SubmitIsAllowed,
				GodMode = context.GodMode,
			};
			context.SendResponse(response);
		}
	}
}