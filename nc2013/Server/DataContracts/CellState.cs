using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class CellState
	{
		[JsonProperty]
		public string Command { get; set; }

		[JsonProperty]
		public string ArgA { get; set; }

		[JsonProperty]
		public string ArgB { get; set; }

		[JsonProperty]
		public int LastModifiedByProgram { get; set; }

		[JsonProperty]
		public int LastModifiedStep { get; set; }

		public static CellState FromCore(Core.CellState cellState)
		{
			return new CellState
			{
				ArgA = cellState.ArgA,
				ArgB = cellState.ArgB,
				Command = cellState.Command,
				LastModifiedByProgram = cellState.LastModifiedByProgram,
				LastModifiedStep = cellState.LastModifiedStep
			};
		}
	}
}