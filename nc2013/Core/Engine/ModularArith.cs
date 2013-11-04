using System;
using System.Diagnostics;

namespace Core.Engine
{
    public static class ModularArith
    {
        public static int Mod(int a, int b = Parameters.CORESIZE)
        {
            var result = a % b;
            if (result < 0)
                result += b;
			Debug.Assert(result >= 0);
			Debug.Assert(result < b);
            return result;
        }

	    public static int Div(int a, int b)
	    {
		    int asgn = Math.Sign(a);
			int bsgn = Math.Sign(b);
		    a = Math.Abs(a);
		    b = Math.Abs(b);
		    return (asgn*bsgn)*(a/b);
	    }
    }
}
