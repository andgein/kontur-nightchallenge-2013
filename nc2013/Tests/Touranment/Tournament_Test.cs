using System.IO;
using System.Linq;
using Core.Arena;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Touranment
{
	[TestFixture]
	public class Tournament_Test
	{
		[Test]
		public void AllOkBots()
		{
			var players = TestWarriors.GetBotFiles("warriors-bad").Select(botFilename => new TournamentPlayer
			{
				Name = Path.GetFileNameWithoutExtension(botFilename),
				Program = File.ReadAllText(botFilename),
				Version = 1,
			}).ToArray();
			var battleRunner = new DobleCheckedBattleRunner();
			var tournament = new RoundRobinTournament(battleRunner, 1, "completeRanking", players, null, false);
			var result = tournament.Run();
			Assert.That(battleRunner.DifferentResults, Is.Empty);
			File.WriteAllText(@"complete-ranking.json", JsonConvert.SerializeObject(result, Formatting.Indented));
		}
	}
}
