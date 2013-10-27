using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Server.DataContracts
{
	[JsonConverter(typeof (StringEnumConverter))]
	public enum ProcessStateChangeType
	{
		Executed,
		Killed,
		Splitted
	}
}