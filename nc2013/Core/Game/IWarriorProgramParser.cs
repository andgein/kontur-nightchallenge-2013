using JetBrains.Annotations;

namespace Core.Game
{
	public interface IWarriorProgramParser
	{
		[CanBeNull]
		string ValidateProgram([NotNull] string program);
	}
}