﻿using Core;
using Newtonsoft.Json;

namespace Server
{
	[JsonObject]
	public class ProgramStartInfo
	{
		[JsonProperty]
		public string Program { get; set; }

		[JsonProperty]
		public uint StartAddress { get; set; }

		public Core.ProgramStartInfo ToCore()
		{
			return new Core.ProgramStartInfo
			{
				Program = Program,
				StartAddress = StartAddress
			};
		}
	}
}