using System;
using System.Net;
using System.Text.RegularExpressions;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

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

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			try
			{
				var request = context.GetRequest<ArenaPlayer>();
				players.CreateOrUpdate(request);
				context.SendResponse("OK");
			}
			catch (Exception e)
			{
				context.SendResponse(e.Message, HttpStatusCode.BadRequest);
			}
		}
	}
}