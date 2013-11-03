using System.Linq;
using System.Net;
using Core.Arena;
using Core.Game;
using JetBrains.Annotations;
using Server.Arena;

namespace Server.Debugging
{
	public class DebuggerLoadGameHandler : DebuggerHandlerBase
	{
		private readonly IPlayersRepo playersRepo;

		public DebuggerLoadGameHandler([NotNull] IDebuggerManager debuggerManager, [NotNull] IPlayersRepo playersRepo)
			: base("debugger/load", debuggerManager)
		{
			this.playersRepo = playersRepo;
		}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode)
		{
			if (!godMode)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			var gameInfo = context.GetRequest<FinishedGameInfo>();
			var program1 = GetBotProgram(gameInfo.Player1Result.Player);
			var program2 = GetBotProgram(gameInfo.Player2Result.Player);
			var programStartInfos = new[]
			{
				new ProgramStartInfo{ Program = program1, StartAddress = (uint)gameInfo.Player1Result.StartAddress, },
				new ProgramStartInfo{ Program = program2, StartAddress = (uint)gameInfo.Player2Result.StartAddress, }
			};

			debugger.StartNewGame(programStartInfos);
		}

		[NotNull]
		private string GetBotProgram([NotNull] TournamentPlayer player)
		{
			return playersRepo.LoadPlayerVersions(player.Name).Single(p => p.Version == player.Version).Program;
		}
	}
}