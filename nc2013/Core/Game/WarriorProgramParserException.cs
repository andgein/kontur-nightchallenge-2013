using System;
using JetBrains.Annotations;

namespace Core.Game
{
	public class WarriorProgramParserException : Exception
	{
		public WarriorProgramParserException([NotNull] string message)
			: base(message)
		{
		}
	}
}