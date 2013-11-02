using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class BotVersionInfo
	{
		[JsonProperty]
		public string Name;

		[JsonProperty]
		public int Version;
	}
}