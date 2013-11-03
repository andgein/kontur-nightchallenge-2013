using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	[DebuggerDisplay("{Instruction}")]
	public class CellState
	{
		[NotNull]
		[JsonProperty]
		public string Instruction { get; set; }

		[JsonProperty]
		public CellType CellType { get; set; }

		[JsonProperty]
		public int? LastModifiedByProgram { get; set; }

	}
}