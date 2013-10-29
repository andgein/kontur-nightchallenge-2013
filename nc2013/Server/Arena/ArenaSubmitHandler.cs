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
		private const string nameValidationRegex = @"^[0-9A-Za-z_-]+$";
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
				if (!Regex.IsMatch(request.Name, nameValidationRegex))
					throw new Exception(string.Format("Имя должно быть {0}! ;-)", nameValidationRegex));
				if (string.IsNullOrEmpty(request.Password))
					throw new Exception(@"Пароль должен быть непустым");
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