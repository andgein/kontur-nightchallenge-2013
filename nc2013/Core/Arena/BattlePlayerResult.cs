using System;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class BattlePlayerResult
	{
		[JsonProperty]
		public TournamentPlayer Player;

		[JsonProperty]
		public int StartAddress;

		[JsonProperty]
		public BattlePlayerResultType ResultType;

		public int Score()
		{
			switch (ResultType)
			{
				case BattlePlayerResultType.Win:
					return 3;
				case BattlePlayerResultType.Draw:
					return 1;
				case BattlePlayerResultType.Loss:
					return 0;
				default:
					throw new InvalidOperationException(string.Format("Invalid result type: {0}", ResultType));
			}
		}
	}
}