using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Server.Debugging
{
	[JsonObject]
	public class DebuggerStepResponse
	{
		[JsonProperty]
		[CanBeNull]
		public Diff Diff { get; set; }

		[JsonProperty]
		[CanBeNull]
		public GameState GameState { get; set; }
	}
}