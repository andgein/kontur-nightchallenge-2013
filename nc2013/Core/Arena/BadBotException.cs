using System;

namespace Core.Arena
{
	public class BadBotException : Exception
	{
		public BadBotException(string message)
			: base(message)
		{
		}
	}
}