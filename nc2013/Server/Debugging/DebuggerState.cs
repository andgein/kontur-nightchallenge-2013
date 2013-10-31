using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Server.Debugging
{
	[JsonObject]
	public class DebuggerState
	{
		[CanBeNull]
		[JsonProperty]
		public ProgramStartInfo[] ProgramStartInfos { get; set; }

		[CanBeNull]
		[JsonProperty]
		public GameState GameState { get; set; }
	}
}