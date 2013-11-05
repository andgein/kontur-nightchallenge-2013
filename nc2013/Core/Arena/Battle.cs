using Core.Engine;
using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class Battle
	{
		[JsonProperty]
		public int Number;

		[JsonProperty]
		public TournamentPlayer Player1;

		[JsonProperty]
		public int StartAddress1;

		[JsonProperty]
		public TournamentPlayer Player2;

		[JsonProperty]
		public int StartAddress2;

		[NotNull]
		public ProgramStartInfo[] GetProgramStartInfos()
		{
			return new[]
			{
				new ProgramStartInfo { Program = Player1.Program, StartAddress = StartAddress1 },
				new ProgramStartInfo { Program = Player2.Program, StartAddress = StartAddress2 }
			};
		}

		[NotNull]
		public WarriorStartInfo[] GetWarriorStartInfos()
		{
			return new[]
			{
				new WarriorStartInfo(Player1.Warrior, StartAddress1),
				new WarriorStartInfo(Player2.Warrior, StartAddress2)
			};
		}

		public override string ToString()
		{
			return string.Format("Number: {0}, StartAddress1: {1}, Player1: {2}, StartAddress2: {3}, Player2: {4}", Number, StartAddress1, Player1, StartAddress2, Player2);
		}
	}
}