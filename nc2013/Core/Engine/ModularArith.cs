using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Engine
{
    public class ModularArith
    {
        public static int Mod(int a, int b)
        {
            var result = a % b;
            if (result < 0)
                result += b;
            return result;
        }
    }
}
