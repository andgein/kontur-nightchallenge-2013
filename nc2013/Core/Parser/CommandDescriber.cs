using System;
using NUnit.Framework;

namespace Core.Parser
{
	public class CommandDescriber
	{
		private readonly WarriorParser warriorParser = new WarriorParser();

		public string Describe(string cmd)
		{
			Statement statement = warriorParser.ParseLine(cmd);
			string addrA = DescribeField(statement.FieldA, statement.ModeA);
			string addrB = DescribeField(statement.FieldB, statement.ModeB);
			if (statement is MovStatement) return addrB + " := " + addrA;
			else if (statement is AddStatement) return addrB + " += " + addrA;
			else if (statement is SubStatement) return addrB + " -= " + addrA;
			else if (statement is CmpStatement) return string.Format("if ({0} == {1}) goto 2", addrB, addrA);
			else if (statement is JmpStatement) return string.Format("goto {0}", addrA);
			else if (statement is JmzStatement) return string.Format("if ({0} == 0) goto {1}", addrB, addrA);
			else if (statement is JmnStatement) return string.Format("if ({0} != 0) goto {1}", addrB, addrA);
			else if (statement is SltStatement) return string.Format("if ({0} < {1}) goto 2", addrA, addrB);
			else if (statement is DjnStatement) return string.Format("if (--{0} != 0) goto {1}", addrB, addrA);
			else if (statement is SplStatement) return string.Format("Split {0}", addrA);
			else return cmd;
		}

		private string DescribeField(Expression field, AddressingMode mode)
		{
			var addr = field.Calculate();
			if (mode == AddressingMode.Immediate) return addr.ToString();
			else if (mode == AddressingMode.Direct) return string.Format("core[{0}]", addr);
			else if (mode == AddressingMode.Indirect) return string.Format("core[core[{0}]]", addr);
			else if (mode == AddressingMode.PredecrementIndirect) return string.Format("core[--core[{0}]]", addr);
			else
				throw new Exception(mode.ToString());
		}
	}

	[TestFixture]
	public class CommandDescriber_Test
	{
		[Test]
		public void Test()
		{
			Assert.AreEqual("core[--core[-1]] := 3", new CommandDescriber().Describe("MOV #3, <-1"));
		}
	}

}