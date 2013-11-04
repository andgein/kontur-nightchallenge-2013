using System;
using Core.Engine;

namespace Core.Parser
{
	public abstract class Expression
	{
		public abstract int Calculate(Warrior warrior = null, int address = 0);
		
		public abstract Expression ExpandConstants(Warrior warrior);

		public Expression Decremented()
		{
			return new NumberExpression(ModularArith.Mod(Calculate() - 1));
		}
	}

	public class BinaryExpression : Expression
	{
		public BinaryOperation Op { get; private set; }
		public Expression Left { get; private set; }
		public Expression Right { get; private set; }

		public BinaryExpression(BinaryOperation op, Expression left, Expression right)
		{
			Op = op;
			Left = left;
			Right = right;
		}

		public override int Calculate(Warrior warrior = null, int address = 0)
		{
			int answer;
			switch (Op)
			{
				case BinaryOperation.Sum:
					answer = Left.Calculate(warrior, address) + Right.Calculate(warrior, address);
					break;
				case BinaryOperation.Sub:
					answer = Left.Calculate(warrior, address) - Right.Calculate(warrior, address);
					break;
				case BinaryOperation.Mul:
					answer = Left.Calculate(warrior, address) * Right.Calculate(warrior, address);
					break;
				case BinaryOperation.Div:
					answer = Left.Calculate(warrior, address) / Right.Calculate(warrior, address);
					break;
				default:
					throw new InvalidOperationException("Invalid operation to calculate: " + Op);
			}
			return ModularArith.Mod(answer);
		}

		public override Expression ExpandConstants(Warrior warrior)
		{
			return new BinaryExpression(Op, Left.ExpandConstants(warrior), Right.ExpandConstants(warrior));
		}
	}

	public class UnaryExpression : Expression
	{
		public UnaryOperation Op { get; private set; }
		public Expression Sub { get; private set; }

		public UnaryExpression(UnaryOperation op, Expression sub)
		{
			Op = op;
			Sub = sub;
		}

		public override int Calculate(Warrior warrior = null, int address = 0)
		{
			switch (Op)
			{
				case UnaryOperation.Negate:
					return -Sub.Calculate(warrior, address);
				case UnaryOperation.Positive:
					return Sub.Calculate(warrior, address);
			}
			throw new InvalidOperationException("Internal error. Unknown unary operation.");
		}

		public override Expression ExpandConstants(Warrior warrior)
		{
			return new UnaryExpression(Op, Sub.ExpandConstants(warrior));
		}
	}

	public class NumberExpression : Expression
	{
		public int Value { get; private set; }

		public NumberExpression(int value)
		{
			Value = ModularArith.Mod(value);
		}

		public override int Calculate(Warrior warrior = null, int address = 0)
		{
			return Value;
		}

		public override Expression ExpandConstants(Warrior warrior)
		{
			return this;
		}
	}

	public class VariableExpression : Expression
	{
		public string Name { get; private set; }

		public VariableExpression(string name)
		{
			Name = name;
		}

		public override int Calculate(Warrior warrior = null, int address = 0)
		{
			if (warrior == null)
				throw new InvalidOperationException("Internal error: can't calculate expression without labels and constants lists");
			if (! warrior.Labels.ContainsKey(Name))
				throw new CompilationException("Unknown label or constant '" + Name + "'", null, -1);
			return warrior.Labels[Name] - address;
		}

		public override Expression ExpandConstants(Warrior warrior)
		{
			if (warrior.Labels.ContainsKey(Name))
                return this;
			if (!warrior.Constants.ContainsKey(Name))
				throw new InvalidOperationException("Unknown constant " + Name);
			return warrior.ExpandConstant(Name);
		}
	}
}