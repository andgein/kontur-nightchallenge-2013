using JetBrains.Annotations;

namespace Core.Arena
{
	public interface IPlayersRepo
	{
		bool CreateOrUpdate([NotNull] ArenaPlayer request);

		[NotNull]
		ArenaPlayer[] LoadPlayerVersions([NotNull] string name);

		[NotNull]
		ArenaPlayer[] LoadLastVersions();

		void Remove([NotNull] string name);
	}
}