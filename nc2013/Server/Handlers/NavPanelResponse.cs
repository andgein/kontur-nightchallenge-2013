using Newtonsoft.Json;

namespace Server.Handlers
{
	[JsonObject]
	public class NavPanelResponse
	{
		[JsonProperty]
		public bool NavigationIsDisabled;

		[JsonProperty]
		public bool ContestIsRunning;

		[JsonProperty]
		public string TimeToContestStart;

		[JsonProperty]
		public string ContestTimeLeft;
	}
}