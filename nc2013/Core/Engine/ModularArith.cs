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
    }
}
