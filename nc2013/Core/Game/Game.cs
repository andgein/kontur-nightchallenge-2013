using System;
using System.Collections.Generic;
using System.Linq;
using Core.Engine;
using Core.Parser;

namespace Core.Game
{
    class Game : IGame
    {
        private readonly Engine.Engine engine;
        private readonly ProgramStartInfo[] programStartInfos = new ProgramStartInfo[0];

        public Game(IEnumerable<ProgramStartInfo> programStartInfos)
        {
            if (programStartInfos != null)
                this.programStartInfos = programStartInfos.ToArray();

            //TODO:
            var r = new Random();

            var parser = new WarriorParser();
            var warriors = programStartInfos.Select(
                psi => new WarriorStartInfo(
                    parser.Parse(psi.Program),
                    psi.StartAddress.HasValue ? (int) psi.StartAddress : r.Next(Parameters.CORESIZE)
                    ));
            engine = new Engine.Engine(warriors);
        }

        public GameState GameState
        {
            get
            {
                return new GameState
                {
                    CurrentProgram = engine.CurrentWarrior,
                    CurrentStep = engine.CurrentStep,
                    ProgramStartInfos = programStartInfos,
                    MemoryState = engine.Memory.ToMemoryState()
                };
            }
        }

        public Diff Step(int stepCount)
        {
            if (stepCount == 0)
                return new Diff();
            var memoryDiffs = new Dictionary<int, CellState>();
            StepResult stepResult = null;
            for (var i = 0; i < stepCount; ++i)
            {
                stepResult = engine.Step();
                memoryDiffs = memoryDiffs.Concat(stepResult.MemoryDiff).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            
            return new Diff
            {
                CurrentProgram = engine.CurrentWarrior,
                CurrentStep = engine.CurrentStep,
                GameOver = stepResult.GameFinished,
                MemoryDiffs = stepResult.MemoryDiff.Select(md => new MemoryDiff
                {
                    Address = (uint) md.Key,
                    CellState = md.Value
                }).ToArray()
            };
        }

        public void StepToEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}