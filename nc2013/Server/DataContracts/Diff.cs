using System.Linq;
using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class Diff
	{
		[JsonProperty]
		public MemoryDiff[] MemoryDiffs { get; set; }

		[JsonProperty]
		public ProgramStateDiff[] ProgramStateDiffs { get; set; }

		[JsonProperty]
		public GameState GameState { get; set; }

		public static Diff FromCore(Core.Diff diff)
		{
			return new Diff
			{
				MemoryDiffs = diff.MemoryDiffs.Select(MemoryDiff.FromCore).ToArray(),
				ProgramStateDiffs = diff.ProgramStateDiffs.Select(ProgramStateDiff.FromCore).ToArray()
			};
		}
	}
}