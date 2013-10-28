// This file is part of nMars - Core War MARS for .NET 
// Whole solution including it's license could be found at
// http://sourceforge.net/projects/nmars/
// 2006 Pavel Savara

using com.calitha.goldparser;
using nMars.RedCode;

namespace nMars.Parser.Expressions
{
    public class LabelName : Expression
    {
        public LabelName(Location location, string name)
            : base(location)
        {
            this.name = name;
        }

        private bool inEval = false;
        protected string name;

        public virtual string Name
        {
            get { return name; }
        }

		public override int Evaluate(IWarriorParser parser, int currentAddress)
        {
            if (inEval)
            {
                parser.WriteError("Cyclic definition of function : " + name + " at " + Location, Location);
                return 0;
            }
            try
            {
                return EvaluateInternal(parser, currentAddress);
            }
            finally
            {
                inEval = false;
            }
        }

		protected virtual int EvaluateInternal(IWarriorParser parser, int currentAddress)
        {
            string fullName = GetFullName(parser, currentAddress);
            if (parser.Variables.ContainsKey(fullName))
            {
                Expression ex = parser.Variables[fullName];
                if (ex == this)
                {
                    parser.WriteError("Label not yet resolved: " + fullName + " at " + Location, Location);
                    return 0;
                }
                return ex.Evaluate(parser, currentAddress);
            }
            else
            {
                parser.WriteError("Label not defined: " + fullName + " at " + Location, Location);
                return 0;
            }
        }

		public override Mode GetMode(IWarriorParser parser, int currentAddress)
        {
            string fullName = GetFullName(parser, currentAddress);
            if (parser.Variables.ContainsKey(fullName))
            {
                Expression ex = parser.Variables[fullName];
                if (ex == this)
                {
                    parser.WriteError("Label not yet resolved: " + fullName + " at " + Location, Location);
                    return 0;
                }
                return ex.GetMode(parser, currentAddress);
            }
            else
            {
                parser.WriteError("Label not defined: " + fullName + " at " + Location, Location);
                return 0;
            }
        }

        public virtual string GetFullName(IWarriorParser parser, int currentAddress)
        {
            return name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}