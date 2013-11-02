using System;
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
			while (true)
			{
				var tournamentWasRun = false;
				try
				{
					tournamentWasRun = TryRunTournament();
				}
				catch (Exception e)
				{
					log.Error("Tournament failed!", e);
				}
				if (!tournamentWasRun)
					Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}

		private bool TryRunTournament()
		{
			var players = playersRepo.LoadLastVersions();
			var mostRecentPlayer = players.OrderByDescending(p => p.Timestamp).FirstOrDefault();
			if (mostRecentPlayer == null)
				return false;
			var lastTournamentId = mostRecentPlayer.Timestamp.Ticks.ToString();
			if (!gamesRepo.TryStartTournament(lastTournamentId))
				return false;

			log.InfoFormat("Warriors changed! Tournament {0}: {1} warriors", lastTournamentId, players.Length);
			var tournamentPlayers = players.Select(p => new TournamentPlayer
			{
				Name = p.Name,
				Version = p.Version,
				Program = p.Program,
				//Warrior = parser.Parse(p.Program),
			}).ToArray();
			var tournament = new RoundRobinTournament(battlesPerPair, lastTournamentId, tournamentPlayers);
			var result = tournament.Run();
			gamesRepo.SaveTournamentResult(lastTournamentId, result);
			log.InfoFormat("Tournament {0} finished.", lastTournamentId);
			return true;
		}
	}
}