using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class ArenaSubmitRequest
	{
		[JsonProperty] public string Name;
		[JsonProperty] public string Password;
		[JsonProperty] public string Authors;
		[JsonProperty] public string Program;
	}
}