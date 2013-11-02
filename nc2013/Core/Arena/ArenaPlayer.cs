using System;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class ArenaPlayer
	{
		[JsonProperty]
		public string Name;

		[JsonProperty]
		public string Password;

		[JsonProperty]
		public string Authors;

		[JsonProperty]
		public string Program;

		[JsonProperty]
		public DateTime Timestamp;

		[JsonIgnore]
		public int Version;

		public override string ToString()
		{
			return string.Format("Name: {0}, Version: {1}, Timestamp: {2}, Program: {3}", Name, Version, Timestamp, Program);
		}
	}
}