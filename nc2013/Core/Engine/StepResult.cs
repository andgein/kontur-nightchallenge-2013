namespace Core.Engine
{
    public class StepResult
    {
        public bool KilledInInstruction;
        public int? SetNextIP;
        public int? SplittedInInstruction;
        public bool GameFinished;

        public StepResult()
        {
            KilledInInstruction = false;
            SetNextIP = null;
            SplittedInInstruction = null;
            GameFinished = false;
        }
    }
}