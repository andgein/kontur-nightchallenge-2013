﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Arena;
using Server.Arena;
using log4net;
using log4net.Config;

namespace Server
{
	public static class Program
	{
		private const string defaultPrefix = "http://*/corewar/";

		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		public static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			Runtime.Init(log);
			try
			{
				RunServer(args);
			}
			catch (Exception e)
			{
				log.Fatal("Unhandled exception:", e);
			}
		}

		private static void RunServer(IEnumerable<string> args)
		{
			var prefix = GetPrefix(args);
			var playersRepo = new PlayersRepo(new DirectoryInfo("players"));
			var gamesRepo = new GamesRepo(new DirectoryInfo("games"));
			var httpServer = new GameHttpServer(prefix, playersRepo, gamesRepo);
			Runtime.SetConsoleCtrlHandler(() =>
			{
				log.InfoFormat("Stopping...");
				httpServer.Stop();
			});
			httpServer.Run();
			log.InfoFormat("Listening {0}", prefix);
			Process.Start(httpServer.DefaultUrl);
			var tournamentRunner = new TournamentRunner(playersRepo, gamesRepo, 10);
			tournamentRunner.Start();
			httpServer.WaitForTermination();
			log.InfoFormat("Stopped");
		}

		private static string GetPrefix(IEnumerable<string> args)
		{
			var prefix = args.FirstOrDefault();
			return string.IsNullOrEmpty(prefix) ? defaultPrefix : prefix;
		}
	}
}