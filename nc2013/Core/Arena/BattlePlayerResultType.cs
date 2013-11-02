using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Arena
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum BattlePlayerResultType
	{
		Win,
		Draw,
		Loss,
	}
}