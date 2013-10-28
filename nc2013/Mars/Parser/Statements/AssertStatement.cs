// This file is part of nMars - Core War MARS for .NET 
// Whole solution including it's license could be found at
// http://sourceforge.net/projects/nmars/
// 2006 Pavel Savara

using com.calitha.goldparser;
using nMars.Parser.Expressions;
using nMars.Parser.Warrior;
using nMars.RedCode;

namespace nMars.Parser.Statements
{
    public class AssertStatement : Statement
    {
        public AssertStatement(Expression expression, Location location)
            : base(location)
        {
            this.expression = expression;
        }

        public override void ExpandStatements(ExtendedWarrior warrior, IWarriorParser parser, ref int currentAddress,
                                              int coreSize, bool evaluate)
        {
            if (!evaluate)
                return;
            if (expression.Evaluate(parser, currentAddress) == 0)
            {
                parser.WriteError("Assert failed : " + this + " at : " + Location, Location);
            }
        }

        private Expression expression;

        public override string ToString()
        {
            return expression.ToString();
        }
    }
}
