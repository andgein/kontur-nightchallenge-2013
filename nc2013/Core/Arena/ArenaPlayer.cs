using System;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class ArenaPlayer
	{
		[JsonProperty] public string Name;
		[JsonProperty] public string Password;
		[JsonProperty] public string Authors;
		[JsonProperty] public string Program;
		[JsonProperty] public DateTime Timestamp;
		[JsonIgnore] public int Version;
	}
}