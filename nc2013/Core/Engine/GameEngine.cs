using System;
using System.Collections.Generic;
using Core.Game;
using Core.Parser;

namespace Core.Engine
{
	public class GameEngine
	{
		public List<RunningWarrior> Warriors { get; private set; }
		public readonly Memory Memory;
		public int? LastExecutedWarrior { get; private set; }
		public int CurrentWarrior { get; private set; }
		public int CurrentStep { get; private set; }
		public int CurrentIp { get; private set; }
		public bool GameOver { get; private set; }
		public int? Winner { get; private set; }

		private int countLivedWarriors;

		private StepResult stepResult;
		private readonly InstructionExecutor instructionExecutor = new InstructionExecutor();

		public GameEngine(params WarriorStartInfo[] warriorsStartInfos)
			: this((IEnumerable<WarriorStartInfo>)warriorsStartInfos)
		{

		}
		public GameEngine(IEnumerable<WarriorStartInfo> warriorsStartInfos)
		{
			Memory = new Memory(Parameters.CoreSize);
			Warriors = new List<RunningWarrior>();
			var idx = 0;
			foreach (var wsi in warriorsStartInfos)
			{
				var warrior = new RunningWarrior(wsi.Warrior, idx++, wsi.LoadAddress, Parameters.CoreSize);
				Warriors.Add(warrior);
				PlaceWarrior(warrior, wsi.LoadAddress);
			}
			CurrentWarrior = 0;
			CurrentStep = 0;
			countLivedWarriors = Warriors.Count;
		}

		private void PlaceWarrior(RunningWarrior warrior, int address)
		{
			var statements = warrior.Warrior.Statements;
			for (var i = 0; i < statements.Count; ++i)
				Memory[address + i] = new Instruction(statements[i], ModularArith.Mod(address + i), warrior.Index);
		}

		public StepResult Step()
		{
			if (GameOver)
				return new StepResult();
			if (CurrentStep >= Parameters.MaxStepsPerWarrior * Warriors.Count)
			{
				GameOver = true;
				return new StepResult();
			}

			CurrentIp = Warriors[CurrentWarrior].Queue.Dequeue();
			var instruction = Memory[CurrentIp];

			stepResult = new StepResult();

			ExecuteInstruction(Warriors[CurrentWarrior], instruction);

			if (stepResult.KilledInInstruction)
			{
				stepResult.ProgramStateDiffs.Add(new ProgramStateDiff { Program = CurrentWarrior, NextPointer = null, ChangeType = ProcessStateChangeType.Killed });
				if (Warriors[CurrentWarrior].Queue.Count == 0)
					countLivedWarriors--;
			}
			else
			{
				int nextIp;
				if (stepResult.SplittedInInstruction.HasValue)
				{
					if (Warriors[CurrentWarrior].Queue.Count < Parameters.MaxQueueSize - 1)
					{
						nextIp = ModularArith.Mod(stepResult.SplittedInInstruction.GetValueOrDefault());
						if (!Warriors[CurrentWarrior].Queue.Enqueue(nextIp))
							throw new InvalidOperationException("Execution should never overflow queue");
						stepResult.ProgramStateDiffs.Add(new ProgramStateDiff { Program = CurrentWarrior, NextPointer = (uint?)nextIp, ChangeType = ProcessStateChangeType.Splitted });
					}
				}
				nextIp = ModularArith.Mod(stepResult.SetNextIP.HasValue ? stepResult.SetNextIP.GetValueOrDefault() : CurrentIp + 1);
				if (!Warriors[CurrentWarrior].Queue.Enqueue(nextIp))
					throw new InvalidOperationException("Execution should never overflow queue");
				stepResult.ProgramStateDiffs.Add(new ProgramStateDiff { Program = CurrentWarrior, NextPointer = (uint?)nextIp, ChangeType = ProcessStateChangeType.Executed });
			}

			CurrentStep++;

			var nextWarrior = GetNextWarrior(CurrentWarrior);
			LastExecutedWarrior = CurrentWarrior;
			CurrentWarrior = nextWarrior.GetValueOrDefault();

			if (Warriors.Count > 1 && countLivedWarriors == 1 || Warriors.Count == 1 && countLivedWarriors == 0)
			{
				GameOver = true;
				Winner = nextWarrior;
				return stepResult;
			}
			return stepResult;
		}

		private int? GetNextWarrior(int currentWarrior)
		{
			var startWarrior = currentWarrior;
			currentWarrior = (currentWarrior + 1) % Warriors.Count;
			if (Warriors[currentWarrior].Queue.Count == 0)
			{
				while (currentWarrior != startWarrior && Warriors[currentWarrior].Queue.Count == 0)
					currentWarrior = (currentWarrior + 1) % Warriors.Count;
				if (currentWarrior == startWarrior)
					return null;
			}
			return currentWarrior;
		}

		private void ExecuteInstruction(RunningWarrior warrior, Instruction instruction)
		{
			instructionExecutor.Execute(this, instruction);
			warrior.LastPointer = (uint)instruction.Address;
		}

		public void WriteToMemory(int address, Statement statement)
		{
			if (Memory[address].Statement != statement || Memory[address].LastModifiedByProgram != CurrentWarrior)
			{
				Memory[address].Statement = statement;
				Memory[address].LastModifiedByProgram = CurrentWarrior;
				stepResult.ChangeMemory(address);
			}
		}

		public void KillCurrentProcess()
		{
			stepResult.KilledInInstruction = true;
		}

		public void JumpTo(int address)
		{
			stepResult.SetNextIP = address;
		}

		public void SplitAt(int address)
		{
			stepResult.SplittedInInstruction = address;
		}
	}
}
