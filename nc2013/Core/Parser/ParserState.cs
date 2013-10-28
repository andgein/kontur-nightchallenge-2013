using System;

namespace Core.Parser
{
    internal class ParserState
    {
        public String Str { get; private set; }
        public int Pos;

        public ParserState(string str, int pos = 0)
        {
            Str = str;
            Pos = pos;
        }
        
        public char Current
        {
            get { return Str[Pos]; }
        }

        public string Tail
        {
            get { return Str.Substring(Pos); }
        }

        public bool Finished()
        {
            return Pos >= Str.Length;
        }

        public void Next()
        {
            Pos++;
        }
    }
}