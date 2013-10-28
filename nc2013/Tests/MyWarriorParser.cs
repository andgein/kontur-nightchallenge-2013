using System;
using JetBrains.Annotations;
using nMars.Parser;
using nMars.Parser.Expressions;
using nMars.Parser.Statements;
using nMars.Parser.Warrior;
using nMars.RedCode;
using ParserException = nMars.RedCode.ParserException;

namespace Tests
{
	public class MyWarriorParser : ParserTokens, IWarriorParser
	{
		private readonly Rules rules;

		public MyWarriorParser([NotNull] Rules rules)
		{
			this.rules = rules;
		}

		protected override IWarrior Parse(string aFileName)
		{
			throw new NotImplementedException();
		}

		[CanBeNull]
		public ExtendedWarrior Parse([NotNull] string sourceText, [NotNull] string implicitName)
		{
			errCount = 0;
			Prepare();
			try
			{
				Statement statement = ParseInternal(sourceText);
				if (statement == null)
					return null;
				var warrior = new ExtendedWarrior(rules);

				//first pass to expand for-rof cycles
				var currentAddress = 0;
				variables["CURLINE"] = new Value(0);
				statement.ExpandStatements(warrior, this, ref currentAddress, rules.CoreSize, false);
				SetRegisters();

				//second pass to evaluate variables/labels in context of for cycles
				currentAddress = 0;
				variables["CURLINE"] = new Value(0);
				statement.ExpandStatements(warrior, this, ref currentAddress, rules.CoreSize, true);

				SetOrg(warrior);
				SetPin(warrior);
				SetName(warrior, implicitName);
				SetAuthor(warrior);
				Asserts(warrior);
				warrior.Variables = variables;
				if (errCount > 0)
					return null;
				return warrior;
			}
			catch (ParserException)
			{
				return null;
			}
		}

		private void Asserts(ExtendedWarrior warrior)
		{
			if (warrior.Length > warrior.Rules.MaxLength)
			{
				WriteError("Too many instructions");
			}
		}
		private void SetAuthor(ExtendedWarrior warrior)
		{
			if (authorName != null)
			{
				warrior.Author = authorName;
			}
		}

		private void SetName(ExtendedWarrior warrior, string implicitName)
		{
			if (warriorName != null)
			{
				warrior.Name = warriorName;
			}
			else
			{
				warrior.Name = implicitName;
			}
		}

		private void SetPin(ExtendedWarrior warrior)
		{
			if (pin != null)
			{
				warrior.Pin = pin.Evaluate(this, 0);
			}
			else
			{
				warrior.Pin = PSpace.UNSHARED;
			}
		}

		private void SetOrg(ExtendedWarrior warrior)
		{
			if (org != null)
			{
				warrior.StartOffset = org.Evaluate(this, 0);
			}
		}

		private void Prepare()
		{
			variables = new Variables();
			org = null;
			rofforCounter = 0;
			warriorName = null;
			authorName = null;
			variables["CORESIZE"] = new Value(rules.CoreSize);
			variables["MAXPROCESSES"] = new Value(rules.MaxProcesses);
			variables["MAXCYCLES"] = new Value(rules.MaxCycles);
			variables["MAXLENGTH"] = new Value(rules.MaxLength);
			variables["MINDISTANCE"] = new Value(rules.MinDistance);
			variables["ROUNDS"] = new Value(rules.Rounds);
			variables["PSPACESIZE"] = new Value(rules.PSpaceSize);
			variables["VERSION"] = new Value(rules.Version);
			variables["WARRIORS"] = new Value(rules.WarriorsCount);
		}

		private void SetRegisters()
		{
			for (char c = 'a'; c <= 'z'; c++)
			{
				string reg = c.ToString();
				if (!variables.ContainsKey(reg))
				{
					variables[reg] = new Value(0);
				}
			}
		}
	}
}