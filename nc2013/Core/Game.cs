using System;

namespace Core
{
	public class Game
	{
		public Game(ProgramStartInfo[] programStartInfos)
		{
		}

		public Diff Step(int stepCount)
		{
			throw new InvalidOperationException();
		}

		public GameState GameState
		{
			get
			{
				return new GameState();
			}
		}
	}
}