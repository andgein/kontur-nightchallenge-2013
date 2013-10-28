namespace Core.Game
{
	public class Diff
	{
		public int CurrentStep { get; set; }
		public int? Winner { get; set; } 
		public MemoryDiff[] MemoryDiffs { get; set; }
		public ProgramStateDiff[] ProgramStateDiffs { get; set; }
	}
}