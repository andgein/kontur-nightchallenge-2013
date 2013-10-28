namespace Core.Game
{
	public class ProgramStateDiff
	{
		public int Program { get; set; }
		public ProcessStateChangeType ChangeType { get; set; }
		public uint NextPointer { get; set; }
	}
}