using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class ProgramState
	{
		[JsonProperty]
		public uint[] ProcessPointers { get; set; }

		public static ProgramState FromCore(Core.ProgramState programState)
		{
			return new ProgramState
			{
				ProcessPointers = programState.ProcessPointers
			};
		}
	}
}