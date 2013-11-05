using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
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

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public static void Main([NotNull] string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			Runtime.Init(log);
			RunServer(args);
		}

		private static void RunServer([NotNull] string[] args)
		{
			var prefix = GetPrefix(args);
			var godModeSecret = Guid.NewGuid();
			Log.For<GameHttpServer>().Warn(string.Format("GodModeSecret: {0}", godModeSecret));
			var warriorProgramParser = new WarriorParser();
			var playersRepo = new PlayersRepo(new DirectoryInfo("../players"), warriorProgramParser);
			var gamesRepo = new GamesRepo(new DirectoryInfo("../games"));
			var sessionManager = new SessionManager("../sessions");
			var gameServer = new GameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var battleRunner = new BattleRunner();
			var tournamentRunner = new TournamentRunner(playersRepo, gamesRepo, battleRunner, 5);
			var godAccessOnly = GodAccessOnly(args);
			var httpServer = new GameHttpServer(prefix, playersRepo, gamesRepo, sessionManager, debuggerManager, tournamentRunner, GetStaticContentDir(), godModeSecret, godAccessOnly);
			Runtime.SetConsoleCtrlHandler(() =>
			{
				log.InfoFormat("Stopping...");
				httpServer.Stop();
				tournamentRunner.Stop();
			});
			tournamentRunner.Start();
			httpServer.Run();
			log.InfoFormat("Listening {0}", prefix);
			if (!ProductionMode(args))
				Process.Start(httpServer.DefaultUrl);
			httpServer.WaitForTermination();
			tournamentRunner.WaitForTermination();
			log.InfoFormat("Stopped");
		}

		private static bool ProductionMode([NotNull] IEnumerable<string> args)
		{
			return args.Any(x => string.Equals(x, "-production", StringComparison.OrdinalIgnoreCase));
		}

		private static bool GodAccessOnly([NotNull] IEnumerable<string> args)
		{
			return args.Any(x => string.Equals(x, "-godAccessOnly", StringComparison.OrdinalIgnoreCase));
		}

		private static string GetPrefix([NotNull] IEnumerable<string> args)
		{
			var prefix = args.FirstOrDefault();
			return string.IsNullOrEmpty(prefix) ? defaultPrefix : prefix;
		}

		private static string GetStaticContentDir()
		{
			return Debugger.IsAttached ? "../../nc2013/Server" : ".";
		}
	}
}