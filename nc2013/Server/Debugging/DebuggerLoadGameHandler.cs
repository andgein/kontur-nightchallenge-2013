using System.Linq;
using System.Net;
using Core.Arena;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public class DebuggerLoadGameHandler : DebuggerHandlerBase
	{
		private readonly ArenaState arenaState;

		public DebuggerLoadGameHandler([NotNull] IDebuggerManager debuggerManager, [NotNull] ArenaState arenaState)
			: base("debugger/load", debuggerManager)
		{
			this.arenaState = arenaState;
		}

		protected override void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger)
		{
			if (!context.GodMode && !arenaState.EnableDeepNavigation)
				throw new HttpException(HttpStatusCode.Forbidden, "This operation is only allowed in god mode :-)");

			var playerName1 = context.GetStringParam("player1Result[player][name]");
			var playerVersion1 = context.GetIntParam("player1Result[player][version]");
			var playerName2 = context.GetStringParam("player2Result[player][name]");
			var playerVersion2 = context.GetIntParam("player2Result[player][version]");
			var programStartInfos = new[]
			{
				new DebuggerProgramStartInfo
				{
					Program = GetBotProgram(playerName1, playerVersion1),
					StartAddress = context.GetIntParam("player1Result[startAddress]"),
				},
				new DebuggerProgramStartInfo
				{
					Program = GetBotProgram(playerName2, playerVersion2),
					StartAddress = context.GetIntParam("player2Result[startAddress]"),
				}
			};

			debugger.StartNewGame(programStartInfos);
			context.Redirect(context.BasePath + "debugger.html");
		}

		[NotNull]
		private string GetBotProgram([NotNull] string playerName, int playerVersion)
		{
			return arenaState.PlayersRepo.LoadPlayerVersions(playerName).Single(p => p.Version == playerVersion).Program;
		}
	}
}