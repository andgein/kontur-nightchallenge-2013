using Core.Game;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class Battle
	{
		[JsonProperty]
		public TournamentPlayer Player1;

		[JsonProperty]
		public TournamentPlayer Player2;

		public ProgramStartInfo[] GetProgramStartInfos()
		{
			return new[]
			{
				new ProgramStartInfo { Program = Player1.Program },
				new ProgramStartInfo { Program = Player2.Program }
			};
		}

		public override string ToString()
		{
			return string.Format("Player1: {0}, Player2: {1}", Player1, Player2);
		}
	}
}