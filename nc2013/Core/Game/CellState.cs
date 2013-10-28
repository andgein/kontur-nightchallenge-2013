namespace Core.Game
{
	public class CellState
	{
		public string Command { get; set; }
		public string ArgA { get; set; }
		public string ArgB { get; set; }
		public int LastModifiedByProgram { get; set; }
		public int LastModifiedStep { get; set; }
	}
}