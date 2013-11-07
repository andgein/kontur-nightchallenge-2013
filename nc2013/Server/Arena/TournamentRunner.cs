using System;
using System.Linq;
using System.Threading;
using Core;
using Core.Arena;
using JetBrains.Annotations;

namespace Server.Arena
{
	public class TournamentRunner : ITournamentRunner
	{
		private readonly ArenaState arenaState;
		private readonly IBattleRunner battleRunner;
		private readonly int battlesPerPair;
		private readonly ManualResetEvent stopSignal = new ManualResetEvent(false);
		private readonly AutoResetEvent botSubmissionSignal = new AutoResetEvent(false);
		private readonly Thread thread;

		public TournamentRunner([NotNull] ArenaState arenaState, [NotNull] IBattleRunner battleRunner, int battlesPerPair)
		{
			this.arenaState = arenaState;
			this.battleRunner = battleRunner;
			this.battlesPerPair = battlesPerPair;
			thread = new Thread(TournamentCycle)
			{
				IsBackground = true,
				Name = "tournament",
			};
		}

		public void Start()
		{
			thread.Start();
		}

		public void Stop()
		{
			stopSignal.Set();
		}

		public void WaitForTermination()
		{
			thread.Join();
		}

		public void SignalBotSubmission()
		{
			botSubmissionSignal.Set();
		}

		private void TournamentCycle()
		{
			while (!stopSignal.WaitOne(TimeSpan.FromSeconds(1)))
			{
				try
				{
					string tournamentId;
					var players = TryStartTournament(out tournamentId);
					if (players != null)
					{
						Log.For(this).InfoFormat("Warriors changed! Tournament {0}: {1} warriors", tournamentId, players.Length);
						arenaState.TournamentIsRunning = true;
						Runtime.DoWithPerfMeasurement(string.Format("RunTournament({0})", tournamentId), () => RunTournament(players, tournamentId));
						arenaState.TournamentIsRunning = false;
					}
				}
				catch (Exception e)
				{
					Log.For(this).Error("Tournament failed!", e);
				}
			}
		}

		[CanBeNull]
		private ArenaPlayer[] TryStartTournament(out string tournamentId)
		{
			tournamentId = null;
			var players = arenaState.PlayersRepo.LoadLastVersions();
			if (players.Length == 0) return null;
			tournamentId = players.Select(p => p.Timestamp).Max().Ticks.ToString();
			if (!arenaState.GamesRepo.TryStartTournament(tournamentId))
				return null;
			return players;
		}

		private void RunTournament([NotNull] ArenaPlayer[] players, [NotNull] string tournamentId)
		{
			var tournamentPlayers = players.Select(p => new TournamentPlayer
			{
				Name = p.Name,
				Version = p.Version,
				Program = p.Program,
			}).ToArray();
			var tournament = new RoundRobinTournament(battleRunner, battlesPerPair, tournamentId, tournamentPlayers, botSubmissionSignal, stopSignal);
			var result = tournament.Run();
			arenaState.GamesRepo.SaveTournamentResult(tournamentId, result);
		}
	}
}