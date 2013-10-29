using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramState
	{
		[JsonProperty]
		public uint? LastPointer { get; set; }

		[JsonProperty]
		[NotNull]
		public uint[] ProcessPointers { get; set; }
	}
}