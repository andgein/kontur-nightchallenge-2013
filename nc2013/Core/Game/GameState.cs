using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	[DebuggerDisplay("CurStep={CurrentStep} CurProg={CurrentProgram} Instr={CurrentInstruction}")]
	public class GameState
	{
		[JsonProperty]
		[NotNull]
		public CellState[] MemoryState { get; set; }

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
		public ProgramState[] ProgramStates { get; set; }

		[NotNull]
		[JsonIgnore]
		public string CurrentInstruction
		{
			get
			{
				var pointers = ProgramStates[CurrentProgram].ProcessPointers;
				if (pointers.Length > 0)
					return pointers[0] + ": " + MemoryState[pointers[0]].Instruction;
				return "";
			}
		}
	}
}