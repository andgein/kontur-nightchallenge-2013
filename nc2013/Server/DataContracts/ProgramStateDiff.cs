using System;
using Newtonsoft.Json;

namespace Server.DataContracts
{
	[JsonObject]
	public class ProgramStateDiff
	{
		[JsonProperty]
		public int Program { get; set; }

		[JsonProperty]
		public ProcessStateChangeType ChangeType { get; set; }
		
		[JsonProperty]
		public uint NextPointer { get; set; }

		public static ProgramStateDiff FromCore(Core.ProgramStateDiff diff)
		{
			return new ProgramStateDiff
			{
				Program = diff.Program,
				ChangeType = Convert(diff.ChangeType),
				NextPointer = diff.NextPointer
			};
		}

		private static ProcessStateChangeType Convert(Core.ProcessStateChangeType changeType)
		{
			switch (changeType)
			{
				case Core.ProcessStateChangeType.Executed:
					return ProcessStateChangeType.Executed;
				case Core.ProcessStateChangeType.Killed:
					return ProcessStateChangeType.Killed;
				case Core.ProcessStateChangeType.Splitted:
					return ProcessStateChangeType.Splitted;
				default:
					throw new InvalidOperationException(string.Format("Invalid changeType {0}", changeType));
			}
		}
	}
}