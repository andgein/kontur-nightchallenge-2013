// This file is part of nMars - Core War MARS for .NET 
// Whole solution including it's license could be found at
// http://sourceforge.net/projects/nmars/
// 2006 Pavel Savara

using com.calitha.goldparser;
using nMars.RedCode;

namespace nMars.Parser.Expressions
{
    public class ComposedLabelName : LabelName
    {
        public ComposedLabelName(Location location, string labelName, string parameter)
            : base(location, labelName)
        {
            this.parameter = new LabelName(location, parameter);
        }

        protected LabelName parameter;

        public override string Name
        {
            get { return name + "&" + parameter; }
        }

        public override string GetFullName(IWarriorParser parser, int currentAddress)
        {
            return name + parameter.Evaluate(parser, currentAddress).ToString("00");
        }
    }
}