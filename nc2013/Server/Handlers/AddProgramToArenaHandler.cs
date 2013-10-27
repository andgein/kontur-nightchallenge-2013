using System;
using System.Net;
using Core;
using Server.DataContracts;

namespace Server.Handlers
{
	public class AddProgramToArenaHandler : GameHandlerBase
	{
		public AddProgramToArenaHandler(Arena arena)
			: base("/corewars/add")
		{
		}

		protected override void DoHandle(HttpListenerContext context)
		{
			var request = GetRequest<AddProgramToArenaRequest>(context);
			if (request.Name == "foo") throw new Exception("wrong password for player " + request.Name);
			SendResponse(context, 42); // returns version of bot with specified name
		}
	}
}