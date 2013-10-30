using System;
using JetBrains.Annotations;
using nMars.Engine;
using nMars.RedCode;

namespace Core.Game.MarsBased
{
	public class MarsEngine : EngineInstructions
	{
		private readonly MarsProject project;
		private StepResult lastStepResult;
		private bool initBefore;
		private int rounds;

		public MarsEngine([NotNull] MarsProject project)
		{
			this.project = project;
		}

		public bool Run(int turnsToMake)
		{
			if (turnsToMake < 0)
				throw new InvalidOperationException("turnsToMake must be non negative");
			var turns = 0;
			var finished = false;
			BeginMatch();
			if (turnsToMake > 0)
			{
				StepResult stepResult;
				do
				{
					stepResult = NextStep();
				} while (++turns < turnsToMake && stepResult != StepResult.Finished);
				finished = stepResult == StepResult.Finished;
			}
			return finished;
		}

		[NotNull]
		public MatchResult Run()
		{
			BeginMatch();
			StepResult stepResult;
			do
			{
				stepResult = NextStep();
			} while (stepResult != StepResult.Finished);
			return EndMatch();
		}

		private void BeginMatch()
		{
			InitializeMatch(project);
			InitializeRound();
			results = new MatchResult(project);
			lastStepResult = StepResult.Start;
			initBefore = Project.EngineOptions.InitRoundBefore;
			rounds = rules.Rounds;
		}

		private StepResult NextStep()
		{
			if (initBefore && lastStepResult == StepResult.NextRound)
			{
				InitRound();
			}
			if (round >= rounds)
			{
				return StepResult.Finished;
			}

			PerformInstruction();

			lastStepResult = StepResult.Continue;
			if (LiveWarriorsCount == 1 && WarriorsCount > 1)
			{
				liveWarriors.Peek().Result = RoundResult.Win;
				lastStepResult = StepResult.NextRound;
				NextRound();
			}
			else if (LiveWarriorsCount == 0 || cyclesLeft == 0)
			{
				lastStepResult = StepResult.NextRound;
				NextRound();
			}
			return lastStepResult;
		}

		private void InitRound()
		{
			round++;
			if (round >= rounds)
			{
				lastStepResult = StepResult.Finished;
			}
			else
			{
				InitializeRound();
			}
		}

		private void NextRound()
		{
			for (var w = 0; w < rules.WarriorsCount; w++)
			{
				var warrior = warriors[w];
				results.results[w, round] = warrior.Result;
				if (warrior.Result != RoundResult.Loss)
				{
					warrior.PSpace[0] = LiveWarriorsCount;
				}
				else
				{
					warrior.PSpace[0] = 0;
				}
			}
			FinalizeRound();
			if (!Project.EngineOptions.InitRoundBefore)
			{
				InitRound();
			}
		}

		private MatchResult EndMatch()
		{
			FinalizeMatch();
			results.ComputePoints();
			return results;
		}

		private void PerformInstruction()
		{
			var warrior = liveWarriors.Dequeue();
			var insructionPointer = warrior.Tasks.Dequeue();

			activeWarrior = warrior;
			InitializeCycle(insructionPointer);

			PerformInstruction(insructionPointer);

			if (warrior.LiveTasks > 0)
			{
				liveWarriors.Enqueue(warrior);
			}
			else
			{
				warrior.Result = RoundResult.Loss;
				cyclesLeft = cyclesLeft - 1 - (cyclesLeft - 1) / (LiveWarriorsCount + 1);
			}
			FinalizeCycle();
			cyclesLeft--;
			cycle++;
		}
	}
}