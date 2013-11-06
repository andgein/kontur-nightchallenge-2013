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

		public string CurrentInstruction
		{
			get
			{
				var pointer = ProgramStates[CurrentProgram].LastPointer;
				if (pointer.HasValue)
					return pointer.Value + ": " + MemoryState[pointer.Value].Instruction;
				else
					return "";
			}
		}
	}
}