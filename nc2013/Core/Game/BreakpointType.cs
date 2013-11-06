using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Game
{
	[JsonConverter(typeof (StringEnumConverter))]
	public enum BreakpointType
	{
		Execution,
		MemoryChange
	}
}