using Core.Game;
using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public class BattleRunner : IBattleRunner
	{
		[NotNull]
		public GameState RunBattle([NotNull] Rules rules, [NotNull] Battle battle)
		{
			var finalGameState = GetFinalGameState(battle);
			PostProcessBattle(rules, battle, finalGameState);
			return finalGameState;
		}

		[NotNull]
		private static GameState GetFinalGameState([NotNull] Battle battle)
		{
			var programStartInfos = battle.GetProgramStartInfos();
			var game = new Game.Game(programStartInfos);
			game.StepToEnd();
			return game.GameState;
		}

		protected virtual void PostProcessBattle([NotNull] Rules rules, [NotNull] Battle battle, [NotNull] GameState finalGameState)
		{
		}
	}
}