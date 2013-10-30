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
		private readonly GamesRepo gamesRepo;
		private readonly int battlesPerPair;
		private readonly WarriorParser parser = new WarriorParser();

		public TournamentRunner(PlayersRepo playersRepo, GamesRepo gamesRepo, int battlesPerPair)
		{
			this.playersRepo = playersRepo;
			this.gamesRepo = gamesRepo;
			this.battlesPerPair = battlesPerPair;
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
				try
				{
					var newPlayersHash = RunTournament(lastPlayersHash, i);
					if (newPlayersHash != lastPlayersHash)
					{
						lastPlayersHash = newPlayersHash;
						i++;
					}
				}
				catch (Exception e)
				{
					log.Error("Tournament failed!", e);
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}

		private int RunTournament(int lastPlayersHash, int i)
		{
			var players = playersRepo.LoadLastVersions();
			var playersHash = string.Join(" ", players.Select(p => p.Name + " " + p.Version)).GetHashCode();
			if (playersHash != lastPlayersHash)
			{
				log.InfoFormat("Warriors changed! Tournament {0}. {1} warriors", i, players.Length);
				var tournament = new RoundRobinTournament(
					battlesPerPair,
					i.ToString(),
					gamesRepo,
					players.Select(p => new TournamentPlayer
					{
						Name = p.Name,
						Version = p.Version,
						Program = p.Program,
						//Warrior = parser.Parse(p.Program),
					}).ToArray()
					);
				tournament.Run();
				log.InfoFormat("Tournament {0} finished.", i);
			}
			return playersHash;
		}
	}
}