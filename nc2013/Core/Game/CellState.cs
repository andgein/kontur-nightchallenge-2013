using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class CellState
	{
		[JsonProperty]
		[NotNull]
		public string Command { get; set; }

		[JsonProperty]
		[NotNull]
		public string ArgA { get; set; }

		[JsonProperty]
		[NotNull]
		public string ArgB { get; set; }

		[JsonProperty]
		public int LastModifiedByProgram { get; set; }

		[JsonProperty]
		public int LastModifiedStep { get; set; }
	}
}