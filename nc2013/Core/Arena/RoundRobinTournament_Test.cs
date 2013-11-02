using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Core.Arena
{
	[TestFixture]
	public class RoundRobinTournament_Test
	{
		[Test]
		public void Test()
		{
			const string tournamentId = "id";
			var gamesLog = new FileInfo("log.txt");
			var rankingResultFile = new FileInfo("ranking.txt");
			var gamesRepo = new GamesRepo(new DirectoryInfo("test-games"));
			var tournament = new RoundRobinTournament(10, tournamentId, CreateDummyPlayers().Take(20).ToArray());
			var result = tournament.Run();
			gamesRepo.SaveTournamentResult(tournamentId, result);
			Console.WriteLine(File.ReadAllText(rankingResultFile.FullName));
			Console.WriteLine();
			Console.WriteLine(File.ReadAllText(gamesLog.FullName));
		}

		public IEnumerable<TournamentPlayer> CreateDummyPlayers()
		{
			var names = new[] {"xathis", "GreenTea", "protocolocon", "runevision", "teapotahedron", "lazarant", "ChrisH", "FlagCapper", "fourmidable", "Memetix", "a1k0n", "pguillory", "SDil_", "meduza", "Murashka", "Komaki", "delineate", "BenJackson", "itzkow", "cheeser"};
			for (int i = 0; i < 20; i++)
				yield return new TournamentPlayer {Name = names[i], Version = i};
		}
	}
}