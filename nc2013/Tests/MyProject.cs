using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using nMars.Parser.Warrior;
using nMars.RedCode;

namespace Tests
{
	public class MyProject : IProject
	{
		public MyProject([NotNull] Rules rules, [NotNull] params ExtendedWarrior[] warriors)
		{
			Rules = rules;
			Warriors = warriors;
			EngineOptions = EngineOptions.Default;
			ParserOptions = ParserOptions.Default;
			ParserOptions.Instructions = false;
		}

		[NotNull]
		public Rules Rules { get; private set; }

		[NotNull]
		public IList<IWarrior> Warriors { get; private set; }

		[NotNull]
		public IList<string> WarriorFiles
		{
			get { throw new NotImplementedException();}
		}

		[NotNull]
		public EngineOptions EngineOptions { get; private set; }

		[NotNull]
		public ParserOptions ParserOptions { get; private set; }

		[NotNull]
		public BreakPoints BreakPoints
		{
			get { throw new NotImplementedException(); }
		}
	}
}