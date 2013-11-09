using System;
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
			var settings = GetSettingsFile(args);
			var httpListenerPrefix = settings.HttpListenerPrefix;
			var warriorProgramParser = new WarriorParser();
			var playersRepo = new PlayersRepo(new DirectoryInfo("../players"), warriorProgramParser);
			var gamesRepo = new CachingGamesRepo(new GamesRepo(new DirectoryInfo("../games")));
			var sessionManager = new SessionManager("../sessions");
			var gameServer = new GameServer();
			var debuggerManager = new DebuggerManager(gameServer);
			var battleRunner = new BattleRunner();
			var countdownProvider = new CountdownProvider(settings.ContestStartTimestamp, TimeSpan.FromHours(settings.ContestDurationInHours));
			var arenaState = new ArenaState(playersRepo, gamesRepo, countdownProvider, settings.GodModeSecret, settings.GodAccessOnly, settings.SubmitIsAllowed);
			var tournamentRunner = new TournamentRunner(arenaState, battleRunner, settings.BattlesPerPair);
			var httpServer = new GameHttpServer(httpListenerPrefix, arenaState, sessionManager, debuggerManager, tournamentRunner, staticContentPath);
			Runtime.SetStopHandler(() =>
			{
				log.InfoFormat("Stopping...");
				httpServer.Stop();
				tournamentRunner.Stop();
			});
			tournamentRunner.Start();
			httpServer.Run();
			log.InfoFormat("Listening on: {0}", httpListenerPrefix);
			if (!settings.ProductionMode)
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