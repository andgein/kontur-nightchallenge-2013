using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using Core;
using ProgramStartInfo = Server.DataContracts.ProgramStartInfo;

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

		public Game GetGame(Guid gameId)
		{
			Game game;
			if (!games.TryGetValue(gameId, out game))
				throw new HttpException(HttpStatusCode.NotFound, string.Format("Game {0} not found", gameId));
			return game;
		}
	}
}