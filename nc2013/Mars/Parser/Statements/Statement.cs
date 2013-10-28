// This file is part of nMars - Core War MARS for .NET 
// Whole solution including it's license could be found at
// http://sourceforge.net/projects/nmars/
// 2006 Pavel Savara

using System.Collections.Generic;
using com.calitha.goldparser;
using nMars.Parser.Expressions;
using nMars.Parser.Warrior;
using nMars.RedCode;

namespace nMars.Parser.Statements
{
    public abstract class Statement
    {
        public Statement(Location location)
        {
            Location = location;
        }

        public abstract void ExpandStatements(ExtendedWarrior warrior, IWarriorParser parser, ref int currentAddress,
                                              int coreSize, bool evaluate);

        public List<LabelName> Labels = null;
        public List<string> Comments = null;
        public Location Location=null;
    }
}