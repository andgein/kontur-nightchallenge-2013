using System;
using System.Collections.Generic;
using System.Linq;
using Core.Engine;
using Core.Parser;

namespace Core.Game
{
	public class Game : IGame
    {
        private readonly GameEngine engine;
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
                        LastPointer = engine.LastPointer,
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
                };

        	var programStateDiffs = new List<ProgramStateDiff>();

        	var memoryDiffs = new HashSet<int>();
        	for (var i = 0; i < stepCount; ++i)
            {
            	var stepResult = engine.Step();
				memoryDiffs.UnionWith(stepResult.MemoryDiffs);
				programStateDiffs.Add(stepResult.ProgramStateDiff);
            }

        	return new Diff
            {
                CurrentProgram = engine.CurrentWarrior,
                CurrentStep = engine.CurrentStep,
                GameOver = engine.GameOver,
                Winner = engine.Winner,
                MemoryDiffs = memoryDiffs.Select(address => new MemoryDiff
                {
                    Address = (uint) address,
                    CellState = new CellState
                    {
                    	Instruction = engine.Memory[address].Statement.ToString(),
						CellType = engine.Memory[address].Statement.CellType,
						LastModifiedByProgram = engine.Memory[address].LastModifiedByProgram
                    }
                }).ToArray(),
                ProgramStateDiffs = programStateDiffs.ToArray()
            };
        }

        public void StepToEnd()
        {
			while (!engine.GameOver)
				Step(1);
        }
    }
}