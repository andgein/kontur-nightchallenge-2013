namespace Core.Game
{
	public class GameState
	{
		public CellState[] MemoryState { get; set; }
		public ProgramState[] ProgramStates { get; set; }
		public int CurrentProgram { get; set; } 
	}
}