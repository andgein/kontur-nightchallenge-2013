using JetBrains.Annotations;
using nMars.RedCode;

namespace Core.Game.MarsBased
{
	public class MarsWarriorProgramParser : IWarriorProgramParser
	{
		private readonly Rules rules;

		public MarsWarriorProgramParser([NotNull] Rules baseRules)
		{
			rules = new Rules(baseRules) { WarriorsCount = 1 };
		}

		[CanBeNull]
		public string ValidateProgram([NotNull] string program)
		{
			var warriorParser = new MarsWarriorParser(rules);
			var warrior = warriorParser.TryParse(program, string.Empty);
			return warrior == null ? warriorParser.GetErrorMessages() : null;
		}
	}
}