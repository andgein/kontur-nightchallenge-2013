using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class AddProgramToArenaRequest
	{
		[JsonProperty]
		public string Program;
		[JsonProperty]
		public string Name;
		[JsonProperty]
		public string Author;
		[JsonProperty]
		public string Password;
	}
}