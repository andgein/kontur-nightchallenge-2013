using System;
using System.Net;
using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Arena
{
	public class AddProgramToArenaHandler : StrictPathHttpHandlerBase
	{
		public AddProgramToArenaHandler(Core.Game.Arena arena)
			: base("arena/add") {}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			var request = context.GetRequest<AddProgramToArenaRequest>();
			if (request.Name == "foo") throw new Exception("wrong password for player " + request.Name);
			context.SendResponse(42); // returns version of bot with specified name
		}
	}
}