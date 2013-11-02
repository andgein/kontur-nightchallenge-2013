using System;

namespace Core.Arena
{
	public class BadBotExcpetion : Exception
	{
		public BadBotExcpetion(string message)
			: base(message)
		{
		}
	}
}