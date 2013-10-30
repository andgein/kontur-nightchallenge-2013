using System;
using Core.Game;
using JetBrains.Annotations;

namespace Server.Debugging
{
	public interface IDebugger
	{
		void StartNewGame([NotNull] ProgramStartInfo[] programStartInfos);
		void Reset();
		T Play<T>([NotNull] Func<IGame, T> action);
		void Play([NotNull] Action<IGame> action);

		[CanBeNull]
		GameState GameState { get; }
	}
}