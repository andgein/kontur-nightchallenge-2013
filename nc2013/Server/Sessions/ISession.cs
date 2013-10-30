﻿using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface ISession
	{
		[NotNull]
		ISessionItems Items { get; }

		void Save<T>([NotNull] string key, [CanBeNull] T value);

		[CanBeNull]
		T Load<T>([NotNull] string key);
	}
}