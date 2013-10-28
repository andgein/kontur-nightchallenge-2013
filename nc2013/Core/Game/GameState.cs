using JetBrains.Annotations;

namespace Core.Game
{
	public class GameState
	{
		public int CurrentProgram { get; set; }
		public int? Winner { get; set; }
		public int CurrentStep { get; set; } 

		[NotNull]
		public CellState[] MemoryState { get; set; }

		[NotNull]
		public ProgramState[] ProgramStates { get; set; }

	}
}