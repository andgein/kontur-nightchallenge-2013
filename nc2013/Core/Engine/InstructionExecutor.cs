using System;
using System.Collections.Generic;
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
		}

		public void Execute(GameEngine engine, Instruction instruction)
		{
			var statement = instruction.Statement;
			var method = GetExecuteMethod(instruction);
			var a = EvalOp(engine, statement.FieldA, statement.ModeA);
			var b = EvalOp(engine, statement.FieldB, statement.ModeB);
			method(engine, a, b);
		}

		private EvaluatedOp EvalOp(GameEngine engine, Expression field, AddressingMode mode)
		{
			if (mode == AddressingMode.Immediate) return new EvaluatedOp(field.CalculateByMod());
			int address = field.Calculate() + engine.CurrentIp;
			if (mode == AddressingMode.Direct) return new EvaluatedOp(address, engine.Memory[address].Statement);
			if (mode == AddressingMode.PredecrementIndirect)
				DecrementB(engine, address);
			int inderectAddress = address + engine.Memory[address].Statement.FieldB.Calculate();
			return new EvaluatedOp(inderectAddress, engine.Memory[inderectAddress].Statement);
		}

		private Action<GameEngine, EvaluatedOp, EvaluatedOp> GetExecuteMethod(Instruction instruction)
		{
			var statement = instruction.Statement;
			var statementType = statement.Type;
			if (!executers.ContainsKey(statementType))
				throw new ArgumentException("Can't find executor for statement");
			return executers[statementType];
		}

		private static void DecrementB(GameEngine engine, int address)
		{
			Statement oldStatement = engine.Memory[address].Statement;
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

		private void Sub(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			if (a.IsImmediate)
			{
				var statementB = b.Statement;
				engine.WriteToMemory(b.Addr, statementB.SetB(statementB.FieldB.Calculate() - a.Value));
			}
			else
			{
				var statementA = a.Statement;
				var statementB = b.Statement;
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

		public class EvaluatedOp
		{
			public readonly OpType Type;
			private readonly int v;
			public readonly Statement Statement;

			public EvaluatedOp(int addr, Statement statement)
			{
				v = addr;
				Type = OpType.Address;
				Statement = statement;
			}

			public EvaluatedOp(int value)
			{
				v = value;
				Type = OpType.Value;
			}

			public int Value
			{
				get
				{
					if (Type != OpType.Value) throw new InvalidOperationException("should be immediate!");
					return v;
				}
			}

			public int Addr
			{
				get
				{
					if (Type != OpType.Address) throw new InvalidOperationException("should not be immediate!");
					return v;
				}
			}

			public bool IsImmediate
			{
				get { return Type == OpType.Value; }
			}
		}
	}
}