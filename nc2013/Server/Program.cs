﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Arena;
using Core.Game;
using Core.Parser;
using JetBrains.Annotations;
using nMars.RedCode;
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
			var baseRules = new Rules
			{
				WarriorsCount = 2,
				Rounds = 1,
				MaxCycles = 80000,
				CoreSize = 8000,
				PSpaceSize = 500, // coreSize / 16 
				EnablePSpace = false,
				MaxProcesses = 1000,
				MaxLength = 100,
				MinDistance = 100,
				Version = 93,
				ScoreFormula = ScoreFormula.Standard,
				ICWSStandard = ICWStandard.ICWS88,
			};
			var prefix = GetPrefix(args);
			var godModeSecret = Guid.NewGuid();
			Log.For<GameHttpServer>().Warn(string.Format("GodModeSecret: {0}", godModeSecret));
			var playersRepo = new PlayersRepo(new DirectoryInfo("../players"), new WarriorParser());
			var gamesRepo = new GamesRepo(new DirectoryInfo("../games"));
			var sessionManager = new SessionManager("../sessions");
			var gameServer = new GameServer();
//			var gameServer = new MarsGameServer(baseRules);
			var debuggerManager = new DebuggerManager(gameServer);
			var battleRunner = new BattleRunner();
			var tournamentRunner = new TournamentRunner(playersRepo, gamesRepo, battleRunner, 10);
			var httpServer = new GameHttpServer(
				prefix, playersRepo, gamesRepo, sessionManager, debuggerManager, tournamentRunner, 
				GetStaticContentDir(), godModeSecret);
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