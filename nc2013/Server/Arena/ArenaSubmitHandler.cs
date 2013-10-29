using System;
using System.Net;
using Core.Arena;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class ArenaSubmitHandler : StrictPathHttpHandlerBase
	{
		public ArenaSubmitHandler(GamesHistory arena)
			: base("arena/submit") {}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			var request = context.GetRequest<ArenaSubmitRequest>();
			if (request.Name == "foo")
				context.SendResponse("Неверный пароль", HttpStatusCode.Forbidden);
			context.SendResponse(42); // returns version of bot with specified name
		}
	}
}