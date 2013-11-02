using System.Net;
using Core;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaSubmitHandler : StrictPathHttpHandlerBase
	{
		private readonly IPlayersRepo playersRepo;

		public ArenaSubmitHandler([NotNull] IPlayersRepo playersRepo)
			: base("arena/submit")
		{
			this.playersRepo = playersRepo;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			var arenaPlayer = context.GetRequest<ArenaPlayer>();
			try
			{
				playersRepo.CreateOrUpdate(arenaPlayer);
				Log.For(this).Warn(string.Format("Bot submitted: {0}", arenaPlayer));
			}
			catch (BadBotExcpetion e)
			{
				Log.For(this).Warn(string.Format("Bot submission failed: {0}", arenaPlayer), e);
				throw new HttpException(HttpStatusCode.BadRequest, e.Message, e);
			}
		}
	}
}