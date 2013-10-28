using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class ProgramStartInfo
	{
		[JsonProperty]
		public string Program { get; set; }

		[JsonProperty]
		public uint? StartAddress { get; set; }

		public Core.Game.ProgramStartInfo ToCore()
		{
			return new Core.Game.ProgramStartInfo
			{
				Program = Program,
				StartAddress = StartAddress
			};
		}
	}
}