using System.Linq;
using JetBrains.Annotations;

namespace Core.Arena
{
	public static class ArenaPlayerExtensions
	{
		[CanBeNull]
		public static ArenaPlayer TryGetLastVersion([NotNull] this ArenaPlayer[] playerVersions)
		{
			return playerVersions.Length == 0 ? null : playerVersions.GetLastVersion();
		}

		[NotNull]
		public static ArenaPlayer GetLastVersion([NotNull] this ArenaPlayer[] playerVersions)
		{
			return playerVersions.GetVersion(playerVersions.Length);
		}

		[NotNull]
		private static ArenaPlayer GetVersion([NotNull] this ArenaPlayer[] playerVersions, int version)
		{
			var player = playerVersions[version - 1];
			return new ArenaPlayer
			{
				Authors = playerVersions.Last(v => !string.IsNullOrWhiteSpace(v.Authors)).Authors,
				Name = player.Name,
				Version = version,
				Program = player.Program,
				Timestamp = player.Timestamp,
			};
		}
	}
}