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

		[JsonProperty]
		public int CurrentStep { get; set; }

		[JsonProperty]
		public int? Winner { get; set; }

		public static Diff FromCore(Core.Game.Diff diff)
		{
			return new Diff
			{
				CurrentStep = diff.CurrentStep,
				MemoryDiffs = diff.MemoryDiffs.Select(MemoryDiff.FromCore).ToArray(),
				ProgramStateDiffs = diff.ProgramStateDiffs.Select(ProgramStateDiff.FromCore).ToArray(),
				Winner = diff.Winner
			};
		}
	}
}