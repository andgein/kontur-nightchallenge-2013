using JetBrains.Annotations;

namespace Core.Arena
{
	public class ArenaState
	{
		public ArenaState([NotNull] IPlayersRepo playersRepo, [NotNull] IGamesRepo gamesRepo, [NotNull] string godModeSecret, bool godAccessOnly)
		{
			PlayersRepo = playersRepo;
			GamesRepo = gamesRepo;
			GodModeSecret = godModeSecret;
			GodAccessOnly = godAccessOnly;
			SubmitIsAllowed = true;
		}

		[NotNull]
		public IPlayersRepo PlayersRepo { get; private set; }

		[NotNull]
		public IGamesRepo GamesRepo { get; private set; }

		[NotNull]
		public string GodModeSecret { get; private set; }

		public bool GodAccessOnly { get; private set; }

		public bool SubmitIsAllowed { get; set; }

		public bool TournamentIsRunning { get; set; }
	}
}