using Newtonsoft.Json;

namespace Server.Arena
{
	[JsonObject]
	public class ArenaSubmitFormResponse
	{
		[JsonProperty]
		public bool SubmitIsAllowed;

		[JsonProperty]
		public bool GodMode;
	}
}