using System;
using Core.Parser;

namespace Core.Engine
{
    public static class ModularArith
    {
        public static int Mod(int a, int b = Parameters.CoreSize)
        {
			if (a < b && a >= 0)
				return a;
            var result = a % b;
            if (result < 0)
                result += b;
            return result;
        }

	    public static int Div(int a, int b)
	    {
			if (b == 0)
				throw new CompilationException("Division by zero");
		    var asgn = Math.Sign(a);
			var bsgn = Math.Sign(b);
		    a = Math.Abs(a);
		    b = Math.Abs(b);
		    return (asgn * bsgn) * (a / b);
	    }
    }
}
