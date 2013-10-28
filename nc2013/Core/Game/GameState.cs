using JetBrains.Annotations;

namespace Core.Game
{
	public class GameState
	{
		[NotNull]
		public CellState[] MemoryState { get; set; }

		[NotNull]
		public ProgramState[] ProgramStates { get; set; }

		public int CurrentProgram { get; set; } 
	}
}