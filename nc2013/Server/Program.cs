using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Arena;
using Core.Game;
using Core.Parser;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using Server.Arena;
using Server.Debugging;
using Server.Sessions;
using Debugger = System.Diagnostics.Debugger;

namespace Server
{
	public static class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (Program));

		public static void Main([NotNull] string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			Runtime.Init(log);
			RunServer(args);
		}

		private static void RunServer([NotNull] IEnumerable<string> args)
		{
			var staticContentPath = GetStaticContentDir();
			var settingsFile = GetSettingsFile(args);
			var prefix = settingsFile.HttpListenerPrefix;
			var battlesPerPair = settingsFile.BattlesPerPair;
			var productionMode = settingsFile.ProductionMode;
			var godAccessOnly = settingsFile.GodAccessOnly;
			var godModeSecret = settingsFile.GodModeSecret;
			var warriorProgramParser = new WarriorParser();
			var playersRepo = new PlayersRepo(new DirectoryInfo("../players"), warriorProgramParser);
			var gamesRepo = new CachingGamesRepo(new GamesRepo(new DirectoryInfo("../games")));
			var sessionManager = new SessionManager("../sessions");
			var gameServer = new GameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var battleRunner = new BattleRunner();
			var arenaState = new ArenaState(playersRepo, gamesRepo, godModeSecret, godAccessOnly);
			var tournamentRunner = new TournamentRunner(arenaState, battleRunner, battlesPerPair);
			var httpServer = new GameHttpServer(prefix, arenaState, sessionManager, debuggerManager, tournamentRunner, staticContentPath);
			Runtime.SetConsoleCtrlHandler(() =>
			{
				log.InfoFormat("Stopping...");
				httpServer.Stop();
				tournamentRunner.Stop();
			});
			tournamentRunner.Start();
			httpServer.Run();
			log.InfoFormat("Listening on: {0}", prefix);
			if (!productionMode)
				Process.Start(httpServer.DefaultUrl);
			httpServer.WaitForTermination();
			tournamentRunner.WaitForTermination();
			log.InfoFormat("Stopped");
		}

		[NotNull]
		private static SettingsFile GetSettingsFile([NotNull] IEnumerable<string> args)
		{
			var filename = args.FirstOrDefault();
			var settingsFile = new SettingsFile(filename);
			log.Info(settingsFile.ToString());
			return settingsFile;
		}

		private static string GetStaticContentDir()
		{
			return Debugger.IsAttached ? "../../nc2013/Server" : ".";
		}
	}
}