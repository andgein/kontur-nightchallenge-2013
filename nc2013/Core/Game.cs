using System;

namespace Core
{
	public class Game
	{
		protected readonly ProgramStartInfo[] programStartInfos;
		protected int currentStep = 0;

		public Game(ProgramStartInfo[] programStartInfos)
		{
			this.programStartInfos = programStartInfos;
		}

		public virtual Diff Step(int stepCount)
		{
			throw new NotImplementedException();
		}

		public GameState GameState { get; set; }
	}
}