using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramStartInfo
	{
		[JsonProperty]
		[CanBeNull]
		public string Program { get; set; }

		[JsonProperty]
		public uint? StartAddress { get; set; }
	}
}