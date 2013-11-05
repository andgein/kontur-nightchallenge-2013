using Core.Game;
using Core.Game.MarsBased;
using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Arena
{
	public class BattleRunner : IBattleRunner
	{
		public int? RunBattle([NotNull] Rules rules, [NotNull] Battle battle)
		{
			var finalGameState = GetFinalGameState(battle);
			PostProcessBattle(rules, battle, finalGameState);
			return finalGameState.Winner;
		}

		[NotNull]
		private static GameState GetFinalGameState([NotNull] Battle battle)
		{
			var programStartInfos = battle.GetProgramStartInfos();
			var warriorStartInfos = battle.GetWarriorStartInfos();
			var game = new Game.Game(programStartInfos, warriorStartInfos);
			game.StepToEnd();
			return game.GameStateFast;
		}

		[NotNull]
		protected static GameState GetFinalGameStateByMars([NotNull] Rules rules, [NotNull] Battle battle)
		{
			var programStartInfos = battle.GetProgramStartInfos();
			var game = new MarsGame(rules, programStartInfos);
			game.StepToEnd();
			return game.GameState;
		}

		protected virtual void PostProcessBattle([NotNull] Rules rules, [NotNull] Battle battle, [NotNull] GameState finalGameState)
		{
		}
	}
}