using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.Arena
{
	public interface IGamesRepo
	{
		bool TryStartTournament([NotNull] string tournamentId);

		void SaveTournamentResult([NotNull] string tournamentId, [NotNull] RoundRobinTournamentResult result);

		[NotNull]
		List<BattleResult> LoadGames([NotNull] string tournamentId = "last");

		[CanBeNull]
		TournamentRanking TryLoadRanking([NotNull] string tournamentId);

		[NotNull]
		string[] GetAllTournamentIds();

		void RemovePlayer([NotNull] string playerName);
	}
}