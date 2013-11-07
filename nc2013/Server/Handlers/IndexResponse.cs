using Newtonsoft.Json;

namespace Server.Handlers
{
	[JsonObject]
	public class IndexResponse
	{
		[JsonProperty]
		public bool NavigationIsDisabled;
	}
}