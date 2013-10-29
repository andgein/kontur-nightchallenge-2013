using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class StepResponse
	{
		[JsonProperty]
		[CanBeNull]
		public Diff Diff { get; set; }

		[JsonProperty]
		[CanBeNull]
		public GameState GameState { get; set; }
	}
}