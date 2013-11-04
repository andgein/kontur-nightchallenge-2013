using System;
using Core.Parser;

namespace Core.Engine
{
	public class EvaluatedOp
	{
		public readonly InstructionExecutor.OpType Type;
		private readonly int v;
		public readonly Statement Statement;

		public EvaluatedOp(int addr, Statement statement)
		{
			v = addr;
			Type = InstructionExecutor.OpType.Address;
			Statement = statement;
		}

		public EvaluatedOp(int value)
		{
			v = value;
			Type = InstructionExecutor.OpType.Value;
		}

		public int Value
		{
			get
			{
				if (Type != InstructionExecutor.OpType.Value) throw new InvalidOperationException("should be immediate!");
				return v;
			}
		}

		public int Addr
		{
			get
			{
				if (Type != InstructionExecutor.OpType.Address) throw new InvalidOperationException("should not be immediate!");
				return v;
			}
		}

		public bool IsImmediate
		{
			get { return Type == InstructionExecutor.OpType.Value; }
		}
	}

	public class EvaluatedVectorizedOp
	{
		public readonly InstructionExecutor.OpType Type;
		private readonly int[] v;
		public readonly Statement[] Statements;

		public EvaluatedVectorizedOp(int[] addr, Statement[] statements)
		{
			v = addr;
			Type = InstructionExecutor.OpType.Address;
			Statements = statements;
		}

		public EvaluatedVectorizedOp(int value)
		{
			v = new[]{value, value, value,value};
			Type = InstructionExecutor.OpType.Value;
		}

		public int[] Value
		{
			get
			{
				if (Type != InstructionExecutor.OpType.Value) throw new InvalidOperationException("should be immediate!");
				return v;
			}
		}

		public int[] Addr
		{
			get
			{
				if (Type != InstructionExecutor.OpType.Address) throw new InvalidOperationException("should not be immediate!");
				return v;
			}
		}

		public bool IsImmediate
		{
			get { return Type == InstructionExecutor.OpType.Value; }
		}
	}

}