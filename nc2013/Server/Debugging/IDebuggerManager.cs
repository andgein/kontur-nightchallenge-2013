using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public interface IDebuggerManager
	{
		[NotNull]
		IDebugger GetDebugger([NotNull] ISession session);
	}
}