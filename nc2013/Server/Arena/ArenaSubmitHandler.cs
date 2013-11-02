using System;
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
			try
			{
				var request = context.GetRequest<ArenaPlayer>();
				playersRepo.CreateOrUpdate(request);
				context.SendResponse("OK");
			}
			catch (Exception e)
			{
				Log.For(this).Error("Submit failed!", e);
				context.SendResponse(e.Message, HttpStatusCode.BadRequest);
			}
		}
	}
}