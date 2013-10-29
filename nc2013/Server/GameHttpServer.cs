using System;
using System.Collections.Concurrent;
using System.Net;
using Core.Game;
using JetBrains.Annotations;

namespace Server
{
	public class GameHttpServer
	{
		private readonly ConcurrentDictionary<Guid, IGame> games = new ConcurrentDictionary<Guid, IGame>();
		private readonly GameServer gameServer = new GameServer();

		public Guid StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			var gameId = Guid.NewGuid();
			games[gameId] = gameServer.StartNewGame(programStartInfos);
			return gameId;
		}

		[NotNull]
		public IGame GetGame(Guid gameId)
		{
			IGame game;
			if (!games.TryGetValue(gameId, out game))
				throw new HttpException(HttpStatusCode.NotFound, string.Format("Game {0} not found", gameId));
			return game;
		}
	}
}