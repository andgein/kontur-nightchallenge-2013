using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class MemoryDiff
	{
		[JsonProperty]
		public uint Address { get; set; }

		[JsonProperty]
		public CellState CellState { get; set; }

		public static MemoryDiff FromCore(Core.MemoryDiff diff)
		{
			return new MemoryDiff
			{
				Address = diff.Address,
				CellState = CellState.FromCore(diff.CellState)
			};
		}
	}
}