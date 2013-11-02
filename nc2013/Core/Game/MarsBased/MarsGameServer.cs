using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Game.MarsBased
{
	public class MarsGameServer : IGameServer
	{
		private readonly Rules baseRules;

		public MarsGameServer([NotNull] Rules baseRules)
		{
			this.baseRules = baseRules;
		}

		[NotNull]
		public IGame StartNewGame([NotNull] ProgramStartInfo[] programStartInfos)
		{
			var rules = new Rules(baseRules) { WarriorsCount = programStartInfos.Length };
			return new MarsGame(rules, programStartInfos);
		}

		[NotNull]
		public IGame ResumeGame([NotNull] GameState gameState)
		{
			var rules = new Rules(baseRules) { WarriorsCount = gameState.ProgramStartInfos.Length };
			var game = new MarsGame(rules, gameState.ProgramStartInfos);
			game.Step(gameState.CurrentStep);
			return game;
		}
	}
}