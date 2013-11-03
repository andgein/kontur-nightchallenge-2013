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
		private readonly Instruction instruction;

		public InstructionExecutor(Instruction instruction)
		{
			this.instruction = instruction;
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

		private Statement Statement
		{
			get { return instruction.Statement; }
		}

		public void Execute(GameEngine engine)
		{
			Action<GameEngine, EvaluatedOp, EvaluatedOp> method = GetExecuteMethod();
			EvaluatedOp a = EvalOp(engine, Statement.FieldA, Statement.ModeA);
			EvaluatedOp b = EvalOp(engine, Statement.FieldB, Statement.ModeB);
			method(engine, a, b);
		}

		private EvaluatedOp EvalOp(GameEngine engine, Expression field, AddressingMode mode)
		{
			if (mode == AddressingMode.Immediate) return new EvaluatedOp(field.Calculate(), OpType.Value);
			int address = field.Calculate() + engine.CurrentIp;
			if (mode == AddressingMode.Direct) return new EvaluatedOp(address, OpType.Address);
			if (mode == AddressingMode.PredecrementIndirect)
				DecrementB(engine, address);
			int inderectAddress = address + engine.Memory[address].Statement.FieldB.Calculate();
			return new EvaluatedOp(inderectAddress, OpType.Address);
		}

		private Action<GameEngine, EvaluatedOp, EvaluatedOp> GetExecuteMethod()
		{
			StatementType statementType = Statement.Type;
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
			Statement newStatement =
				a.IsImmediate ?
					engine.Memory[b.Addr].Statement.SetB(a.Value)
					: engine.Memory[a.Addr].Statement;
			engine.WriteToMemory(b.Addr, newStatement);
		}

		private void Add(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			Statement statementB = engine.Memory[b.Addr].Statement;
			if (a.IsImmediate)
				engine.WriteToMemory(b.Addr, statementB.SetB(a.Value + statementB.FieldB.Calculate()));
			else
			{
				Statement statementA = engine.Memory[a.Addr].Statement;
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
				Statement statementB = engine.Memory[b.Addr].Statement;
				engine.WriteToMemory(b.Addr, statementB.SetB(statementB.FieldB.Calculate() - a.Value));
			}
			else
			{
				Statement statementA = engine.Memory[a.Addr].Statement;
				Statement statementB = engine.Memory[b.Addr].Statement;
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
			int cond = b.IsImmediate ? b.Value : engine.Memory[b.Addr].Statement.FieldB.Calculate();
			if (cond == 0)
				engine.JumpTo(a.Addr);
		}

		private void Jmn(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			int cond = b.IsImmediate ? b.Value : engine.Memory[b.Addr].Statement.FieldB.Calculate();
			if (cond != 0)
				engine.JumpTo(a.Addr);
		}

		private void Cmp(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			int left = a.IsImmediate ? a.Value : engine.Memory[a.Addr].Statement.FieldB.Calculate();
			int right = engine.Memory[b.Addr].Statement.FieldB.Calculate();
			if (left == right)
				engine.JumpTo(engine.CurrentIp + 2);
		}

		private void Slt(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			int left = a.IsImmediate ? a.Value : engine.Memory[a.Addr].Statement.FieldB.Calculate();
			int right = engine.Memory[b.Addr].Statement.FieldB.Calculate();
			if (left < right)
				engine.JumpTo(engine.CurrentIp + 2);
		}

		private void Spl(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			engine.SplitAt(a.Addr);
		}

		private void Djn(GameEngine engine, EvaluatedOp a, EvaluatedOp b)
		{
			int bAddr = b.IsImmediate ? engine.CurrentIp : b.Addr;
			DecrementB(engine, bAddr);
			int condition = engine.Memory[bAddr].Statement.FieldB.Calculate();
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

			public EvaluatedOp(int value, OpType type)
			{
				v = value;
				Type = type;
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