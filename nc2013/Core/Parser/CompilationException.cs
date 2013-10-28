using System;

namespace Core.Parser
{
    class CompilationException : Exception
    {
        public CompilationException(string message) : base(message)
        {
        }
    }
}