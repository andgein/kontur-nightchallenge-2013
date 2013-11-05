using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Arena;
using Core.Game;
using Core.Parser;
using JetBrains.Annotations;
using Server.Arena;
using log4net;
using log4net.Config;
using Server.Debugging;
using Server.Sessions;
using Debugger = System.Diagnostics.Debugger;

namespace Server
{
	public static class Program
	{
		private const string defaultPrefix = "http://*/corewar/";
		private const int defaultBattlesPerPair = 5;
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public static void Main([NotNull] string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			Runtime.Init(log);
			RunServer(args);
		}

		private static void RunServer([NotNull] string[] args)
		{
			var staticContentPath = GetStaticContentDir();
			var prefix = GetPrefix(args);
			var battlesPerPair = GetBattlesPerPair(args);
			var productionMode = ProductionMode(args);
			var godAccessOnly = GodAccessOnly(args);
			var godModeSecret = GenerateGodModeSecret();
			var warriorProgramParser = new WarriorParser();
			var playersRepo = new PlayersRepo(new DirectoryInfo("../players"), warriorProgramParser);
			var gamesRepo = new CachingGamesRepo(new GamesRepo(new DirectoryInfo("../games")));
			var sessionManager = new SessionManager("../sessions");
			var gameServer = new GameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var battleRunner = new BattleRunner();
			var tournamentRunner = new TournamentRunner(playersRepo, gamesRepo, battleRunner, battlesPerPair);
			var httpServer = new GameHttpServer(prefix, playersRepo, gamesRepo, sessionManager, debuggerManager, tournamentRunner, staticContentPath, godModeSecret, godAccessOnly);
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

		private static Guid GenerateGodModeSecret()
		{
			var godModeSecret = Guid.NewGuid();
			log.Warn(string.Format("GodModeSecret: {0}", godModeSecret));
			return godModeSecret;
		}

		private static int GetBattlesPerPair([NotNull] IEnumerable<string> args)
		{
			var battlesPerPairArg = args.FirstOrDefault(x => x.StartsWith("-battlesPerPair:", StringComparison.OrdinalIgnoreCase));
			var battlesPerPair = battlesPerPairArg == null ? defaultBattlesPerPair : int.Parse(battlesPerPairArg.Split(':')[1]);
			log.InfoFormat("BattlesPerPair: {0}", battlesPerPair);
			return battlesPerPair;
		}

		private static bool ProductionMode([NotNull] IEnumerable<string> args)
		{
			var productionMode = args.Any(x => string.Equals(x, "-production", StringComparison.OrdinalIgnoreCase));
			log.InfoFormat("ProductionMode: {0}", productionMode);
			return productionMode;
		}

		private static bool GodAccessOnly([NotNull] IEnumerable<string> args)
		{
			var godAccessOnly = args.Any(x => string.Equals(x, "-godAccessOnly", StringComparison.OrdinalIgnoreCase));
			log.InfoFormat("GodAccessOnly: {0}", godAccessOnly);
			return godAccessOnly;
		}

		private static string GetPrefix([NotNull] IEnumerable<string> args)
		{
			var prefix = args.FirstOrDefault();
			var result = string.IsNullOrEmpty(prefix) ? defaultPrefix : prefix;
			log.InfoFormat("HttpListenerPrefix: {0}", result);
			return result;
		}

		private static string GetStaticContentDir()
		{
			return Debugger.IsAttached ? "../../nc2013/Server" : ".";
		}
	}
}