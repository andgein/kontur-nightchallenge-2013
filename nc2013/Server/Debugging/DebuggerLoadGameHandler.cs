using System;
using System.Linq;
using System.Net;
using Core.Arena;
using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerLoadGameHandler : DebuggerHandlerBase
	{
		private readonly IPlayersRepo playersRepo;
		private readonly Guid godModeSecret;

		public DebuggerLoadGameHandler([NotNull] IDebuggerManager debuggerManager, [NotNull] IPlayersRepo playersRepo, Guid godModeSecret)
			: base("debugger/load", debuggerManager)
		{
			this.playersRepo = playersRepo;
			this.godModeSecret = godModeSecret;
		}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			if (context.TryGetCookie<Guid>(GameHttpContext.GodModeSecretCookieName, Guid.TryParse) != godModeSecret)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			var name1 = context.GetStringParam("name1");
			var version1 = context.GetIntParam("version1");
			var address1 = context.GetIntParam("address1");
			var program1 = GetBotProgram(name1, version1);

			var name2 = context.GetStringParam("name2");
			var version2 = context.GetIntParam("version2");
			var address2 = context.GetIntParam("address2");
			var program2 = GetBotProgram(name2, version2);

			var programStartInfos = new[]
			{
				new ProgramStartInfo{ Program = program1, StartAddress = (uint)address1, },
				new ProgramStartInfo{ Program = program2, StartAddress = (uint)address2, },
			};

			debugger.StartNewGame(programStartInfos);
		}

		[NotNull]
		private string GetBotProgram([NotNull] string name, int version)
		{
			return playersRepo.LoadPlayerVersions(name).Single(p => p.Version == version).Program;
		}
	}
}