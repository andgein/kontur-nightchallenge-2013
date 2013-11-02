using System;
using Core.Game;

namespace Core.Parser
{
    public class Statement
    {
        private AddressingMode modeA;
        private Expression fieldA;
        private AddressingMode modeB;
        private Expression fieldB;
        
        public string Label;
        public readonly StatementType Type;
        public AddressingMode ModeA { get { return modeA; } set { ExistsFieldA = true; modeA = value; } }
        public Expression FieldA { get { return fieldA; } set { ExistsFieldA = true; fieldA = value; } }
        public AddressingMode ModeB { get { return modeB; } set { ExistsFieldB = true; modeB = value; } }
        public Expression FieldB { get { return fieldB; } set { ExistsFieldB = true; fieldB = value; } }

        public bool ExistsFieldA { get; private set; }
        public bool ExistsFieldB { get; private set; }

    	private static StatementFactory statementFactory = new StatementFactory();

        public CellType CellType
        {
            get { return Type == StatementType.Dat ? CellType.Data : CellType.Command; }
        }

        public Statement(Statement another)
        {
            Type = another.Type;
            ModeA = another.ModeA;
            FieldA = another.FieldA;
            ModeB = another.ModeB;
            FieldB = another.FieldB;
            ExistsFieldA = another.ExistsFieldA;
            ExistsFieldB = another.ExistsFieldB;
        }

        public Statement(StatementType type = StatementType.Dat)
        {
        	Type = type;
            ModeA = AddressingMode.Direct;
            FieldA = new NumberExpression(0);
            ModeB = AddressingMode.Direct;
            FieldB = new NumberExpression(0);
            ExistsFieldA = ExistsFieldB = false;
        }

        public bool HasLabel()
        {
            return Label != "";
        }

        public static bool operator==(Statement left, Statement right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Type == right.Type &&
                   left.ModeA == right.ModeA &&
                   left.FieldA.Calculate() == right.FieldA.Calculate() &&
                   left.ModeB == right.ModeB &&
                   left.FieldB.Calculate() == right.FieldB.Calculate();
        }

        public static bool operator !=(Statement left, Statement right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this == (Statement) obj;
        }

        public override int GetHashCode()
        {
        	return Type.GetHashCode() ^ ModeA.GetHashCode() ^ FieldA.Calculate().GetHashCode() ^
        	       ModeB.GetHashCode() ^ FieldB.Calculate().GetHashCode();
        }

		public override string ToString()
		{
			return String.Format("{0} {1}{2} {3}{4}",
				statementFactory.GetStatementMnemonic(Type),
				(char) ModeA,
				FieldA.Calculate(),
				(char) ModeB,
				FieldB.Calculate());
		}
    }

    public enum StatementType
    {
        Mov,
        Add,
        Sub,
        Cmp,
        Slt,
        Jmp,
        Jmz,
        Jmn,
        Djn,
        Spl,
        Dat,
        End,
        Equ,
    }
}