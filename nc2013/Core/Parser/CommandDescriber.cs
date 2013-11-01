using System;
using NUnit.Framework;

namespace Core.Parser
{
	public class CommandDescriber
	{
		private readonly WarriorParser warriorParser = new WarriorParser();

		public string Describe(string cmd)
		{
			var statement = warriorParser.ParseLine(cmd);
			var addrA = DescribeField(statement.FieldA, statement.ModeA);
			var addrB = DescribeField(statement.FieldB, statement.ModeB);
			switch (statement.Type)
			{
			    case StatementType.Mov:
			        return addrB + " := " + addrA;
			    case StatementType.Add:
			        return addrB + " += " + addrA;
			    case StatementType.Sub:
			        return addrB + " -= " + addrA;
			    case StatementType.Cmp:
			        return string.Format("if ({0} == {1}) goto 2", addrB, addrA);
			    case StatementType.Jmp:
			        return string.Format("goto {0}", addrA);
			    case StatementType.Jmz:
			        return string.Format("if ({0} == 0) goto {1}", addrB, addrA);
			    case StatementType.Jmn:
			        return string.Format("if ({0} != 0) goto {1}", addrB, addrA);
			    case StatementType.Slt:
			        return string.Format("if ({0} < {1}) goto 2", addrA, addrB);
			    case StatementType.Djn:
			        return string.Format("if (--{0} != 0) goto {1}", addrB, addrA);
			    case StatementType.Spl:
			        return string.Format("Split {0}", addrA);
			    default:
			        return cmd;
			}
		}

		private static string DescribeField(Expression field, AddressingMode mode)
		{
		    var addr = field.Calculate();
		    switch (mode)
		    {
		        case AddressingMode.Immediate:
		            return addr.ToString();
		        case AddressingMode.Direct:
		            return string.Format("core[{0}]", addr);
		        case AddressingMode.Indirect:
		            return string.Format("core[core[{0}]]", addr);
		        case AddressingMode.PredecrementIndirect:
		            return string.Format("core[--core[{0}]]", addr);
		        default:
		            throw new Exception(mode.ToString());
		    }
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