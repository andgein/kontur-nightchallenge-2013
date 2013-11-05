using System;
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
		const string playersDir = @".\players";

		[SetUp]
		public void SetUp()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			if (Directory.Exists(playersDir))
				Directory.Delete(playersDir, true);
			Directory.CreateDirectory(playersDir);
		}

		[Test]
		public void ConvertToPlayers()
		{
			var botFiles = TestWarriors.GetBotFiles("warriors-best");
			foreach (var botFilename in botFiles)
			{
				var botName = Path.GetFileNameWithoutExtension(botFilename);
				var player = new ArenaPlayer
				{
					Name = botName,
					Password = Guid.NewGuid().ToString(),
					Authors = "Corewar community",
					Program = File.ReadAllText(botFilename),
					Timestamp = DateTime.UtcNow,
				};
				var playerFilename = Path.Combine(playersDir, botName + ".json");
				File.WriteAllText(playerFilename, JsonConvert.SerializeObject(new[] { player }, Formatting.Indented));
			}
		}

		[Test]
		public void UberTournament()
		{
			var players = TestWarriors.GetBotFiles("warriors-ok").Concat(TestWarriors.GetBotFiles("warriors-vec")).Select(botFilename => new TournamentPlayer
			{
				Name = Path.GetFileNameWithoutExtension(botFilename),
				Program = File.ReadAllText(botFilename),
				Version = 1,
			})
			.ToArray();
			Log.For(this).InfoFormat("Number of players: {0}", players.Length);
			var battleRunner = new BattleRunner();
			var tournament = new RoundRobinTournament(battleRunner, 1, "allBotsRanking", players, null, null, false);
			var result = tournament.Run();
			File.WriteAllText(@"all-bots-ranking.json", JsonConvert.SerializeObject(result.TournamentRanking, Formatting.Indented));
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
			File.WriteAllText(@"complete-ranking.json", JsonConvert.SerializeObject(result.TournamentRanking, Formatting.Indented));
			Assert.That(battlesWithDifferentResults.Count, Is.EqualTo(0));
		}
	}
}
