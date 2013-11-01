using System;
using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;
using log4net;

namespace Server.Arena
{
	public class ArenaSubmitHandler : StrictPathHttpHandlerBase
	{
		private readonly PlayersRepo players;

		public ArenaSubmitHandler(PlayersRepo players)
			: base("arena/submit")
		{
			this.players = players;
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			try
			{
				var request = context.GetRequest<ArenaPlayer>();
				players.CreateOrUpdate(request);
				context.SendResponse("OK");
			}
			catch (Exception e)
			{
				log.Error("Submit failed!", e);
				context.SendResponse(e.Message, HttpStatusCode.BadRequest);
			}
		}

		private static readonly ILog log = LogManager.GetLogger(typeof (ArenaSubmitHandler));

	}
}