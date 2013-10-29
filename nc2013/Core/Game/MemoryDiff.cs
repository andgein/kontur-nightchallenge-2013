using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class MemoryDiff
	{
		[JsonProperty]
		public uint Address { get; set; }

		[JsonProperty]
		[NotNull]
		public CellState CellState { get; set; }
	}
}