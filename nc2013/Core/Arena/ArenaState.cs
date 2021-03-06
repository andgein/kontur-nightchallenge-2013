﻿using JetBrains.Annotations;

namespace Core.Arena
{
	public class ArenaState
	{
		public ArenaState([NotNull] IPlayersRepo playersRepo, [NotNull] IGamesRepo gamesRepo, [NotNull] CountdownProvider countdownProvider, [NotNull] string godModeSecret, bool godAccessOnly, bool submitIsAllowed, bool enableDeepNavigation)
		{
			PlayersRepo = playersRepo;
			GamesRepo = gamesRepo;
			CountdownProvider = countdownProvider;
			GodModeSecret = godModeSecret;
			GodAccessOnly = godAccessOnly;
			SubmitIsAllowed = submitIsAllowed;
			EnableDeepNavigation = enableDeepNavigation;
		}

		[NotNull]
		public IPlayersRepo PlayersRepo { get; private set; }

		[NotNull]
		public IGamesRepo GamesRepo { get; private set; }

		[NotNull]
		public CountdownProvider CountdownProvider { get; private set; }

		[NotNull]
		public string GodModeSecret { get; private set; }

		public bool GodAccessOnly { get; private set; }

		public bool SubmitIsAllowed { get; set; }
		public bool EnableDeepNavigation { get; set; }

		public bool TournamentIsRunning { get; set; }
	}
}