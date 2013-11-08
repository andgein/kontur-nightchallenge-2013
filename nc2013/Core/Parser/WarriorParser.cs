using System;
using System.Collections.Generic;
using System.Linq;
using Core.Engine;
using Core.Game;

namespace Core.Parser
{
	public class WarriorParser : Parser, IWarriorProgramParser
	{
		private readonly StatementFactory statementFactory = new StatementFactory();
		private readonly ExpressionParser expressionParser = new ExpressionParser();
		private readonly static Tuple<StatementType, AddressingMode>[] restrictedModesA = new []
		{
			Tuple.Create(StatementType.Jmp, AddressingMode.Immediate),
			Tuple.Create(StatementType.Jmz, AddressingMode.Immediate),
			Tuple.Create(StatementType.Jmn, AddressingMode.Immediate),
			Tuple.Create(StatementType.Djn, AddressingMode.Immediate),
			Tuple.Create(StatementType.Spl, AddressingMode.Immediate),
			Tuple.Create(StatementType.Dat, AddressingMode.Direct   ),
			Tuple.Create(StatementType.Dat, AddressingMode.Indirect ),
		};
		private readonly static Tuple<StatementType, AddressingMode>[] restrictedModesB = new []
		{
			Tuple.Create(StatementType.Mov, AddressingMode.Immediate),
			Tuple.Create(StatementType.Add, AddressingMode.Immediate),
			Tuple.Create(StatementType.Sub, AddressingMode.Immediate),
			Tuple.Create(StatementType.Mov4, AddressingMode.Immediate),
			Tuple.Create(StatementType.Add4, AddressingMode.Immediate),
			Tuple.Create(StatementType.Sub4, AddressingMode.Immediate),
			Tuple.Create(StatementType.Cmp, AddressingMode.Immediate),
			Tuple.Create(StatementType.Slt, AddressingMode.Immediate),
			Tuple.Create(StatementType.Dat, AddressingMode.Direct),
			Tuple.Create(StatementType.Dat, AddressingMode.Indirect),
		};

		private static readonly Dictionary<char, AddressingMode> allowedAddressingModes = new Dictionary<char, AddressingMode>
		{
			{'$', AddressingMode.Direct},
			{'#', AddressingMode.Immediate},
			{'@', AddressingMode.Indirect},
			{'<', AddressingMode.PredecrementIndirect},
		};

		public Warrior Parse(String text)
		{
			var warrior = new Warrior();
			var lines = text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				var stLine = TrimComment(line);
				if (stLine.All(Char.IsWhiteSpace))
					continue;
				var statement = TryParseLine(warrior, stLine);
				if (statement.Type == StatementType.End)
				{
					if (statement.HasLabel)
						warrior.Labels[statement.Label] = warrior.Statements.Count;
					if (statement.ExistsFieldA)
						warrior.StartAddressExpression = statement.FieldA;
					break;
				}
				if (statement.Type == StatementType.Equ)
				{
					if (!statement.HasLabel)
						throw new CompilationException("EQU statement should have label", State);
					if (warrior.Constants.ContainsKey(statement.Label))
						throw new CompilationException("Constant with same label '" + statement.Label + "' already exists", State);
					if (warrior.Labels.ContainsKey(statement.Label))
						throw new CompilationException("Label with same label '" + statement.Label + "' already exists", State);
					warrior.Constants[statement.Label] = statement.FieldA;
					continue;
				}
				warrior.AddStatement(statement, State);
			}

			if (warrior.Statements.Count > Parameters.MaxWarriorLength)
				throw new CompilationException(string.Format("Too long program. Max allowed length is {0}", Parameters.MaxWarriorLength), State);

			warrior.EvaluateAllExpressions();
			return warrior;
		}

		private static string TrimComment(string line)
		{
			var commentPos = line.IndexOf(';');
			return commentPos != -1 ? line.Substring(0, commentPos) : line;
		}

		private Statement TryParseLine(Warrior warrior, string line)
		{
			try
			{
				return ParseLine(warrior, line);
			}
			catch (CompilationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new CompilationException(e.Message, State);
			}
		}

		public Statement ParseLine(Warrior warrior, String line)
		{
			var label = "";
			string command;

			State = new ParserState(line);
			var token = ParseToken(IsIdentificatorChar);
			if (!IsCommandToken(token))
			{
				label = token;
				command = ParseToken(IsIdentificatorChar);
			}
			else
			{
				command = token;
			}

			if (!IsCommandToken(command))
				throw new CompilationException("Not a command", State);

			var statement = statementFactory.Create(warrior, command);
			statement.Label = label;

			Tuple<AddressingMode, Expression> a = null, b = null;
			if (!RestOnlyWhitespaces())
				a = ReadModeAndField();
			else if (statement.Type != StatementType.End)
				throw new CompilationException("A-Field expected in " + command.ToUpper(), State);

			if (!RestOnlyWhitespaces())
			{
				if (statement.Type == StatementType.Equ || statement.Type == StatementType.End)
					throw new CompilationException("Expected end of command", State);
				ParseComma();
				b = ReadModeAndField();
			}
			else if (statement.Type != StatementType.Equ 
				&& statement.Type != StatementType.End 
				&& statement.Type != StatementType.Spl
				&& statement.Type != StatementType.Jmp
				)
				throw new CompilationException("B-Field Expected in " + command.ToUpper(), State);

			if (! RestOnlyWhitespaces())
				throw new CompilationException("Expected end of command", State);

			statement.SetFields(a, b);
			CheckStatementIsCorrect(statement);
			return statement;
		}

		private void CheckStatementIsCorrect(Statement statement)
		{
			if (restrictedModesA.Contains(Tuple.Create(statement.Type, statement.ModeA)) 
				||restrictedModesB.Contains(Tuple.Create(statement.Type, statement.ModeB)))
				throw new CompilationException("Invalid addressing mode", State);
		}

		private Tuple<AddressingMode, Expression> ReadModeAndField()
		{
			var mode = ParseAddressingMode();
			var field = expressionParser.Parse(State);
			return Tuple.Create(mode, field);
		}

		private void ParseComma()
		{
			SkipWhitespaces();
			if (State.Finished() || State.Current != ',')
				throw new CompilationException("Expected comma", State);
			State.Next();
		}

		private AddressingMode ParseAddressingMode()
		{
			SkipWhitespaces();
			AddressingMode mode;
			if (!allowedAddressingModes.TryGetValue(State.Current, out mode))
				return AddressingMode.Direct;
			State.Next();
			return mode;
		}

		private bool RestOnlyWhitespaces()
		{
			return State.Tail.All(Char.IsWhiteSpace);
		}

		private bool IsCommandToken(string token)
		{
			return statementFactory.Commands.ContainsKey(token.ToUpper());
		}

		public string ValidateProgram(string program)
		{
			try
			{
				Parse(program);
				return null;
			}
			catch (CompilationException e)
			{
				return e.Message;
			}
		}
	}
}
