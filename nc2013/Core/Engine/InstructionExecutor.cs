using System;
using System.Collections.Generic;
using System.Linq;
using Core.Parser;

namespace Core.Engine
{
	public class InstructionExecutor
	{
		public enum OpType
		{
			Value,
			Address
		}

		private readonly Dictionary<StatementType, Action<GameEngine, EvaluatedOp, EvaluatedOp>> executers;
		private readonly Dictionary<StatementType, Action<GameEngine, EvaluatedVectorizedOp, EvaluatedVectorizedOp>> vectorizedExecuters;

		public InstructionExecutor()
		{
			executers = new Dictionary<StatementType, Action<GameEngine, EvaluatedOp, EvaluatedOp>>
			{
				{StatementType.Mov, Mov},
				{StatementType.Add, Add},
				{StatementType.Sub, Sub},
				{StatementType.Cmp, Cmp},
				{StatementType.Slt, Slt},
				{StatementType.Jmp, Jmp},
				{StatementType.Jmz, Jmz},
				{StatementType.Jmn, Jmn},
				{StatementType.Djn, Djn},
				{StatementType.Spl, Spl},
				{StatementType.Dat, Dat},
			};
			vectorizedExecuters = new Dictionary<StatementType, Action<GameEngine, EvaluatedVectorizedOp, EvaluatedVectorizedOp>>
			{
				{StatementType.Add4, Add4},
				{StatementType.Mov4, Mov4},
				{StatementType.Sub4, Sub4},
			};
		}

		public void Execute(GameEngine engine, Instruction instruction)
		{
			var statement = instruction.Statement;
			StatementType type = statement.Type;
			var method = FindExecuteMethod(type);
			if (method != null)
			{
				var a = EvalOp(engine, statement.FieldA, statement.ModeA);
				var b = EvalOp(engine, statement.FieldB, statement.ModeB);
				method(engine, a, b);
			}
			var vMethod = FindVectorizedExecuteMethod(type);
			if (vMethod != null)
			{
				var a = EvalVectorizedOp(engine, statement.FieldA, statement.ModeA);
				var b = EvalVectorizedOp(engine, statement.FieldB, statement.ModeB);
				vMethod(engine, a, b);
			}
		}

		private EvaluatedVectorizedOp EvalVectorizedOp(GameEngine engine, Expression field, AddressingMode mode)
		{
			if (mode == AddressingMode.Immediate) return new EvaluatedVectorizedOp(field.CalculateByMod());
			var addresses = new int[4];
			var statements = new Statement[4];
			var baseAddress = field.Calculate() + engine.CurrentIp;
			for (int i = 0; i < 4; i++)
			{
				var address = baseAddress + i;
				if (mode != AddressingMode.Direct) 
				{
					if (mode == AddressingMode.PredecrementIndirect)
						DecrementB(engine, address);
					address = address + engine.Memory[address].Statement.FieldB.Calculate();
				}
				addresses[i] = address;
				statements[i] = engine.Memory[address].Statement;
			}
			return new EvaluatedVectorizedOp(addresses, statements);
		}

		private EvaluatedOp EvalOp(GameEngine engine, Expression field, AddressingMode mode)
		{
			if (mode == AddressingMode.Immediate) return new EvaluatedOp(field.CalculateByMod());
			int address = field.Calculate() + engine.CurrentIp;
			if (mode == AddressingMode.Direct) return new EvaluatedOp(address, engine.Memory[address].Statement);
			if (mode == AddressingMode.PredecrementIndirect)
			{
				DecrementB(engine, address);
			}
			int inderectAddress = address + engine.Memory[address].Statement.FieldB.Calculate();
			return new EvaluatedOp(inderectAddress, engine.Memory[inderectAddress].Statement);
		}

		private Action<GameEngine, EvaluatedOp, EvaluatedOp> FindExecuteMethod(StatementType statementType)
		{
			if (!executers.ContainsKey(statementType))
				return null;
			return executers[statementType];
		}

		private Action<GameEngine, EvaluatedVectorizedOp, EvaluatedVectorizedOp> FindVectorizedExecuteMethod(StatementType statementType)
		{
			if (!vectorizedExecuters.ContainsKey(statementType))
				return null;
			return vectorizedExecuters[statementType];
		}

		private static void DecrementB(GameEngine engine, int address)
		{
			var oldStatement = engine.Memory[address].Statement;
			engine.WriteToMemory(address, new Statement(oldStatement)
			{
				FieldB = oldStatement.FieldB.Decremented()
			});
		}

		private void Mov(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var newStatement =
				a.IsImmediate ?
					b.Statement.SetB(a.Value)
					: a.Statement;
			engine.WriteToMemory(b.Addr, newStatement);
		}

		private void Mov4(GameEngine engine, EvaluatedVectorizedOp a, EvaluatedVectorizedOp b)
		{
			var newStatements =
				a.IsImmediate ?
					b.Statements.Select((stm, i) => stm.SetB(a.Value[i])).ToArray()
					: a.Statements;
			for (int i = 0; i < 4; i++)
				engine.WriteToMemory(b.Addr[i], newStatements[i]);
		}

		private void Add(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var statementB = b.Statement;
			if (a.IsImmediate)
				engine.WriteToMemory(b.Addr, statementB.SetB(a.Value + statementB.FieldB.Calculate()));
			else
			{
				var statementA = a.Statement;
				engine.WriteToMemory(b.Addr,
					statementB
						.SetA(statementA.FieldA.Calculate() + statementB.FieldA.Calculate())
						.SetB(statementA.FieldB.Calculate() + statementB.FieldB.Calculate()));
			}
		}

		private void Add4(GameEngine engine, EvaluatedVectorizedOp a, EvaluatedVectorizedOp b)
		{
			var statementsB = b.Statements;
			if (a.IsImmediate)
				for (int i = 0; i < 4; i++)
					engine.WriteToMemory(b.Addr[i], statementsB[i].SetB(a.Value[i] + statementsB[i].FieldB.Calculate()));
			else
			{
				var statementsA = a.Statements;
				for (int i = 0; i < 4; i++)
					engine.WriteToMemory(b.Addr[i],
					statementsB[i]
						.SetA(statementsA[i].FieldA.Calculate() + statementsB[i].FieldA.Calculate())
						.SetB(statementsA[i].FieldB.Calculate() + statementsB[i].FieldB.Calculate()));
			}
		}

		private void Sub4(GameEngine engine, EvaluatedVectorizedOp a, EvaluatedVectorizedOp b)
		{
			var statementsB = b.Statements;
			if (a.IsImmediate)
				for (int i = 0; i < 4; i++)
					engine.WriteToMemory(b.Addr[i], statementsB[i].SetB(a.Value[i] - statementsB[i].FieldB.Calculate()));
			else
			{
				var statementsA = a.Statements;
				for (int i = 0; i < 4; i++)
					engine.WriteToMemory(b.Addr[i],
					statementsB[i]
						.SetA(statementsA[i].FieldA.Calculate() - statementsB[i].FieldA.Calculate())
						.SetB(statementsA[i].FieldB.Calculate() - statementsB[i].FieldB.Calculate()));
			}
		}
		private void Sub(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var statementB = b.Statement;
			if (a.IsImmediate)
			{
				engine.WriteToMemory(b.Addr, statementB.SetB(statementB.FieldB.Calculate() - a.Value));
			}
			else
			{
				var statementA = a.Statement;
				engine.WriteToMemory(b.Addr,
					statementB
						.SetA(statementB.FieldA.Calculate() - statementA.FieldA.Calculate())
						.SetB(statementB.FieldB.Calculate() - statementA.FieldB.Calculate()));
			}
		}

		private void Jmp(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			engine.JumpTo(a.Addr);
		}

		private void Jmz(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			int cond = b.IsImmediate ? b.Value : b.Statement.FieldB.CalculateByMod();
			if (cond == 0)
				engine.JumpTo(a.Addr);
		}

		private void Jmn(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var cond = b.IsImmediate ? b.Value : b.Statement.FieldB.CalculateByMod();
			if (cond != 0)
				engine.JumpTo(a.Addr);
		}

		private void Cmp(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			bool condition;
			if (a.IsImmediate)
			{
				var left = a.Value;
				var right = b.Statement.FieldB.CalculateByMod();
				condition = left == right;
			}
			else
				condition = a.Statement == b.Statement;
			if (condition)
				engine.JumpTo(engine.CurrentIp + 2);
		}

		private void Slt(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var left = a.IsImmediate ? a.Value : a.Statement.FieldB.CalculateByMod();
			var right = b.Statement.FieldB.CalculateByMod();
			if (left < right)
				engine.JumpTo(engine.CurrentIp + 2);
		}

		private void Spl(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			engine.SplitAt(a.Addr);
		}

		private void Djn(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			var bAddr = b.IsImmediate ? engine.CurrentIp : b.Addr;
			DecrementB(engine, bAddr);
			var condition = engine.Memory[bAddr].Statement.FieldB.CalculateByMod();
			if (condition != 0)
				engine.JumpTo(a.Addr);
		}

		private void Dat(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			engine.KillCurrentProcess();
		}
	}
}