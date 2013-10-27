using System;
using System.Collections.Concurrent;
using System.Linq;
using Core;

namespace Server
{
	public class GameHttpServer
	{
		private readonly ConcurrentDictionary<Guid, Game> games = new ConcurrentDictionary<Guid, Game>();
		private readonly GameServer gameServer = new GameServer();

		public Guid StartNewGame(ProgramStartInfo[] programStartInfos)
		{
			var game = gameServer.StartNewGame(programStartInfos.Select(x => x.ToCore()).ToArray());
			var gameId = Guid.NewGuid();
			games[gameId] = game;
			return gameId;
		}

		public Game TryGetGame(Guid gameId)
		{
			Game game;
			return games.TryGetValue(gameId, out game) ? game : null;
		}

		public Game GetGame(Guid gameId)
		{
			var game = TryGetGame(gameId);
			if (game == null)
				throw new InvalidOperationException(string.Format("Game {0} not found", gameId));
			return game;
		}
	}
}