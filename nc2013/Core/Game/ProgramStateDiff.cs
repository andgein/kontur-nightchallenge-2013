using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramStateDiff
	{
		[JsonProperty]
		public int Program { get; set; }

		[JsonProperty]
		public ProcessStateChangeType ChangeType { get; set; }

		[JsonProperty]
		public uint? NextPointer { get; set; }

		[NotNull]
		public override string ToString()
		{
			return string.Format("Program: {0}, ChangeType: {1}, NextPointer: {2}", Program, ChangeType, NextPointer);
		}
	}
}