using System.Collections.Generic;
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
		public DebuggerProgramStartInfo[] ProgramStartInfos { get; set; }

		[CanBeNull]
		[JsonProperty]
		public GameState GameState { get; set; }

		[JsonProperty]
		[CanBeNull]
		public IEnumerable<Breakpoint> Breakpoints { get; set; }
	}
}