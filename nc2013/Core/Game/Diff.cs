using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class Diff
	{
		[JsonProperty]
		public int CurrentStep { get; set; }

		[JsonProperty]
		public int? Winner { get; set; }

		[JsonProperty]
		public bool GameOver { get; set; }

		[JsonProperty]
		public int CurrentProgram { get; set; }

		[JsonProperty]
		[CanBeNull]
		public MemoryDiff[] MemoryDiffs { get; set; }

		[JsonProperty]
		[CanBeNull]
		public ProgramStateDiff[] ProgramStateDiffs { get; set; }
	}
}