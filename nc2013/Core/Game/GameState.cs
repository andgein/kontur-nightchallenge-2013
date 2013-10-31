using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class GameState
	{
		[NotNull]
		[JsonProperty]
		public ProgramStartInfo[] ProgramStartInfos { get; set; }

		[JsonProperty]
		public int CurrentStep { get; set; }

		[JsonProperty]
		public int CurrentProgram { get; set; }

		[JsonProperty]
		public int? Winner { get; set; }

		[JsonProperty]
		public bool GameOver { get; set; }

		[JsonProperty]
		[NotNull]
		public CellState[] MemoryState { get; set; }

		[JsonProperty]
		[NotNull]
		public ProgramState[] ProgramStates { get; set; }
	}
}