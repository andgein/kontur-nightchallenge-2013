using System;

namespace Core.Parser
{
    public class CompilationException : Exception
    {
        public CompilationException(string message) : base(message)
        {
        }
    }
}