namespace Core.Game
{
    public class GameServer : IGameServer {
        public IGame StartNewGame(ProgramStartInfo[] programStartInfos)
        {
            return new Game(programStartInfos);
        }

        public IGame ResumeGame(GameState gameState)
        {
            var game = new Game(gameState.ProgramStartInfos);
            game.Step(gameState.CurrentStep);
            return game;
        }
    }
}