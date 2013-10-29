using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class AddProgramToArenaRequest
	{
		[JsonProperty] public string Name;
		[JsonProperty] public string TeamName;
		[JsonProperty] public string Password;
		[JsonProperty] public string TeamAuthors;
		[JsonProperty] public string Program;
	}
}