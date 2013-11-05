using System.IO;
using System.Linq;
using Core;
using Core.Arena;
using log4net.Config;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Touranment
{
	[TestFixture]
	public class Tournament_Test
	{
		[SetUp]
		public void SetUp()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
		}

		[Test]
		public void AllOkBots()
		{
			var players = TestWarriors.GetBotFiles("warriors-ok").Select(botFilename => new TournamentPlayer
			{
				Name = Path.GetFileNameWithoutExtension(botFilename),
				Program = File.ReadAllText(botFilename),
				Version = 1,
			}).ToArray();
			Log.For(this).InfoFormat("Number of players: {0}", players.Length);
			var battleRunner = new BattleRunner();
			var tournament = new RoundRobinTournament(battleRunner, 1, "allBotsRanking", players, null, null, false);
			var result = tournament.Run();
			File.WriteAllText(@"all-bots-ranking.json", JsonConvert.SerializeObject(result, Formatting.Indented));
		}

		[Test]
		public void DoubleChecked()
		{
			var players = TestWarriors.GetBotFiles("warriors-bad").Select(botFilename => new TournamentPlayer
			{
				Name = Path.GetFileNameWithoutExtension(botFilename),
				Program = File.ReadAllText(botFilename),
				Version = 1,
			}).ToArray();
			var battleRunner = new DobleCheckedBattleRunner();
			var tournament = new RoundRobinTournament(battleRunner, 1, "completeRanking", players, null, null, false);
			var result = tournament.Run();
			var battlesWithDifferentResults = battleRunner.BattlesWithDifferentResults;
			File.WriteAllText(@"failed-battles.json", JsonConvert.SerializeObject(battlesWithDifferentResults, Formatting.Indented));
			File.WriteAllText(@"complete-ranking.json", JsonConvert.SerializeObject(result, Formatting.Indented));
			Assert.That(battlesWithDifferentResults.Count, Is.EqualTo(0));
		}
	}
}
