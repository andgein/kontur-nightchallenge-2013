using System;
using JetBrains.Annotations;
using Server.Debugging;

namespace Server.Sessions
{
	public interface ISession
	{
		Guid SessionId { get; }

		[NotNull]
		IDebugger Debugger { get; }

		void Save<T>([NotNull] string key, [CanBeNull] T value);

		[CanBeNull]
		T Load<T>([NotNull] string key);
	}
}