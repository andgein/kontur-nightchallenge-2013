using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Server.Debugging
{
	[JsonObject]
	public class DebuggerProgramStartInfo
	{
		[NotNull]
		[JsonProperty]
		public string Program { get; set; }

		[JsonProperty]
		public int? StartAddress { get; set; }

		[JsonProperty]
		public bool Disabled { get; set; }

		[NotNull]
		public override string ToString()
		{
			return string.Format("Program: {0}, StartAddress: {1}, Disabled: {2}", Program, StartAddress, Disabled);
		}
	}
}