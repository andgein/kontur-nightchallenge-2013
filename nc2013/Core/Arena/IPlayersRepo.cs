using JetBrains.Annotations;

namespace Core.Arena
{
	public interface IPlayersRepo
	{
		void CreateOrUpdate([NotNull] ArenaPlayer request);

		[NotNull]
		ArenaPlayer[] LoadPlayerVersions([NotNull] string name);

		[NotNull]
		ArenaPlayer[] LoadLastVersions();
	}
}