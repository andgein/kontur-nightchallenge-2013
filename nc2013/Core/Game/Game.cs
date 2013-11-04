using System;
using System.Collections.Generic;
using System.Linq;
using Core.Engine;
using Core.Parser;

namespace Core.Game
{
	public class Game : IGame
    {
        private readonly Engine.GameEngine engine;
        private readonly ProgramStartInfo[] programStartInfos = new ProgramStartInfo[0];

        public Game(ProgramStartInfo[] programStartInfos)
        {
            this.programStartInfos = programStartInfos;

            //TODO: RandomAllocator
            var r = new Random();

            var parser = new WarriorParser();
            var warriors = programStartInfos.Select(
                psi => new WarriorStartInfo(
                    parser.Parse(psi.Program),
                    psi.StartAddress.HasValue ? (int) psi.StartAddress : r.Next(Parameters.CORESIZE)
                    ));
            engine = new GameEngine(warriors);
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
                    GameOver = engine.GameOver,
                    Winner = engine.Winner,
                    MemoryState = engine.Memory.ToMemoryState(),
                    ProgramStates = engine.Warriors.Select(w => new ProgramState
                    {
                        LastPointer = (uint)engine.CurrentIp,
                        ProcessPointers = w.Queue.ToArray().Select(x => (uint) x).ToArray()
                    }).ToArray(),
                };
            }
        }

        public Diff Step(int stepCount)
        {
            if (stepCount == 0)
                return new Diff
                {
                    CurrentProgram = engine.CurrentWarrior,
                    CurrentStep = engine.CurrentStep,
                    GameOver = engine.GameOver,
                    Winner = engine.Winner,
                    ProgramStateDiffs = engine.Warriors.Select(w => new ProgramStateDiff
                    {
                        NextPointer = (uint) w.Queue.Peek(),
                        Program = w.Index,
                        ChangeType = ProcessStateChangeType.Executed
                    }).ToArray()
                };
            var memoryDiffs = new Dictionary<int, CellState>();
        	for (var i = 0; i < stepCount; ++i)
            {
            	var stepResult = engine.Step();
            	foreach (var md in stepResult.MemoryDiff)
					memoryDiffs[md.Key] = md.Value;
            }

        	return new Diff
            {
                CurrentProgram = engine.CurrentWarrior,
                CurrentStep = engine.CurrentStep,
                GameOver = engine.GameOver,
                Winner = engine.Winner,
                MemoryDiffs = memoryDiffs.Select(md => new MemoryDiff
                {
                    Address = (uint) md.Key,
                    CellState = md.Value
                }).ToArray(),
                ProgramStateDiffs = engine.Warriors.Select(w => new ProgramStateDiff
                {
                    NextPointer = w.Queue.Count > 0 ? (uint)w.Queue.Peek() : 0,
                    Program = w.Index,
                    ChangeType = ProcessStateChangeType.Executed
                }).ToArray()
            };
        }

        public void StepToEnd()
        {
			while (!engine.GameOver)
				Step(1);
        }
    }
}