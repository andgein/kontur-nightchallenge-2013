using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramStateDiff
	{
		[JsonProperty]
		public int Program { get; set; }

		[JsonProperty]
		public ProcessStateChangeType ChangeType { get; set; }

		[JsonProperty]
		public uint? NextPointer { get; set; }
	}
}