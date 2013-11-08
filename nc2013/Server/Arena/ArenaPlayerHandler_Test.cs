using System;
using System.Collections.Generic;
using Core.Arena;
using NUnit.Framework;

namespace Server.Arena
{
	[TestFixture]
	public class ArenaPlayerHandler_Test
	{
		[Test]
		public void GetFinishedGamesWithEnemies()
		{
			var playerX = new TournamentPlayer { Name = "x", Version = 1 };
			var playerY = new TournamentPlayer { Name = "y", Version = 1 };
			var games = new List<BattleResult>
			{
				new BattleResult
				{
					Player1Result = new BattlePlayerResult { Player = playerX, ResultType = BattlePlayerResultType.Win, StartAddress = 1 },
					Player2Result = new BattlePlayerResult { Player = playerY, ResultType = BattlePlayerResultType.Loss, StartAddress = 2 },
				},
				new BattleResult
				{
					Player1Result = new BattlePlayerResult { Player = playerY, ResultType = BattlePlayerResultType.Loss, StartAddress = 2 },
					Player2Result = new BattlePlayerResult { Player = playerX, ResultType = BattlePlayerResultType.Win, StartAddress = 1 },
				},
			};
			var xStat = ArenaPlayerHandler.GetFinishedGamesWithEnemies(games, "x", 1, true)[0];
			Console.Out.WriteLine(xStat);
			Assert.That(xStat.Wins, Is.EqualTo(2));
			Assert.That(xStat.Loses, Is.EqualTo(0));
			Assert.That(xStat.Draws, Is.EqualTo(0));
			Assert.That(xStat.GameInfos[0].Label, Is.EqualTo("Win"));
			Assert.That(xStat.GameInfos[0].Player1Result.Player.Name, Is.EqualTo("x"));
			Assert.That(xStat.GameInfos[0].Player1Result.ResultType, Is.EqualTo(BattlePlayerResultType.Win));
			Assert.That(xStat.GameInfos[0].Player1Result.StartAddress, Is.EqualTo(1));

			Assert.That(xStat.GameInfos[1].Label, Is.EqualTo("Win"));
			Assert.That(xStat.GameInfos[1].Player1Result.Player.Name, Is.EqualTo("y"));
			Assert.That(xStat.GameInfos[1].Player1Result.ResultType, Is.EqualTo(BattlePlayerResultType.Loss));
			Assert.That(xStat.GameInfos[1].Player1Result.StartAddress, Is.EqualTo(2));
		}
	}
}