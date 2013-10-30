using System;
using System.IO;
using System.Linq;
using System.Threading;
using Core.Arena;
using Core.Parser;
using log4net;

namespace Server.Arena
{
	public class TournamentRunner
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TournamentRunner));
		private readonly PlayersRepo playersRepo;
		private readonly DirectoryInfo gameLogsDir;
		private readonly FileInfo rankingFile;
		private readonly int battlesPerPair;
		private readonly WarriorParser parser = new WarriorParser();

		public TournamentRunner(PlayersRepo playersRepo, DirectoryInfo gameLogsDir, FileInfo rankingFile, int battlesPerPair)
		{
			this.playersRepo = playersRepo;
			this.gameLogsDir = gameLogsDir;
			this.rankingFile = rankingFile;
			this.battlesPerPair = battlesPerPair;
			if (!gameLogsDir.Exists) gameLogsDir.Create();
		}

		public void Start()
		{
			var thread = new Thread(TournamentCycle);
			thread.IsBackground = true;
			thread.Name = "TournamentCycle";
			thread.Start();
		}

		private void TournamentCycle()
		{
			int i = 0;
			int lastPlayersHash = 0;
			while (true)
			{
				var players = playersRepo.LoadLastVersions();
				var playersHash = string.Join(" ", players.Select(p => p.Name + " " + p.Version)).GetHashCode();
				if (playersHash != lastPlayersHash)
				{
					log.InfoFormat("Warriors changed! Tournament {0}. {1} warriors", i, players.Length);
					var tournament = new RoundRobinTournament(
						battlesPerPair,
						new FileInfo(gameLogsDir.FullName + "\\" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".json"),
						rankingFile,
						players.Select(p => new TournamentPlayer {Name = p.Name, Version = p.Version, Warrior = parser.Parse(p.Program)}).ToArray()
						);
					tournament.Run();
					lastPlayersHash = playersHash;
					log.InfoFormat("Tournament {0} finished.", i);
					i++;
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}
	}
}