using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramStartInfo
	{
		[NotNull]
		[JsonProperty]
		public string Program { get; set; }

		[JsonProperty]
		public int? StartAddress { get; set; }

		public override string ToString()
		{
			return string.Format("StartAddress: {0}, Program: {1}", StartAddress, Program);
		}
	}
}