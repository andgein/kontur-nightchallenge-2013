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
    public class ForRofContainerStatement : ContainerStatement
    {
        public ForRofContainerStatement(Location location)
            : base(location)
        {
        }

        public override void ExpandStatements(ExtendedWarrior warrior, IWarriorParser parser, ref int currentAddress,
                                              int coreSize, bool evaluate)
        {
            //set labels, except last which is FOR expression
            for (int l = 0; l < Labels.Count - 1; l++)
            {
                LabelName label = Labels[l];
                parser.Variables[label.GetFullName(parser, currentAddress)] = new Address(currentAddress);
            }

            string cnt = Labels[Labels.Count - 1].Name;
            int count = parser.Variables[LimitName].Evaluate(parser, currentAddress);

            for (int i = 1; i <= count; i++)
            {
                parser.Variables[cnt] = new Value(i);
                base.ExpandStatements(warrior, parser, ref currentAddress, coreSize, evaluate);
            }
        }

        public string LimitName=null;
    }
}