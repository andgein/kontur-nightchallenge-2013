using System;
using System.Collections.Generic;
using Core.Parser;

namespace Core.Engine
{
    public class InstructionExecutor
    {
        private readonly Instruction instruction;
        private readonly Dictionary<StatementType, Action<GameEngine>> executers;
        private Statement Statement
        {
            get { return instruction.Statement;  }
        }

        public InstructionExecutor(Instruction instruction)
        {
            this.instruction = instruction;
            executers = new Dictionary<StatementType, Action<GameEngine>>
            {
                {StatementType.Mov, Mov},
                {StatementType.Add, Add},
                {StatementType.Sub, Sub},
                {StatementType.Cmp, Cmp},
                {StatementType.Slt, Slt},
				{StatementType.Jmp, Jmp},
                {StatementType.Jmz, Jmz},
                {StatementType.Jmn, Jmn},
				{StatementType.Djn, Djn},
				{StatementType.Spl, Spl},

                {StatementType.Dat, Dat},
            };
        }
		
    	public void Execute(GameEngine engine)
        {
            var method = GetExecuteMethod();
            method.Invoke(engine);
        }

        private Action<GameEngine> GetExecuteMethod()
        {
            var statementType = Statement.Type;
            if (! executers.ContainsKey(statementType))
                throw new ArgumentException("Can't find executor for statement");
            return executers[statementType];
        }

        private static int CalcAddress(GameEngine engine, AddressingMode mode, Expression expression)
        {
            int address;
            switch (mode)
            {
                case AddressingMode.Immediate:
		            return 0;
                case AddressingMode.Direct:
                    return engine.CurrentIp + expression.Calculate();
                case AddressingMode.Indirect:
                    address = engine.CurrentIp + expression.Calculate();
                    return address + engine.Memory[address].Statement.FieldB.Calculate();
                case AddressingMode.PredecrementIndirect:
                    address = engine.CurrentIp + expression.Calculate();
                    DecrementB(engine, address);
		            return address + engine.Memory[address].Statement.FieldB.Calculate();
            }
            throw new InvalidOperationException("Internal error. Unknown addressing mode");
        }

	    private static void DecrementB(GameEngine engine, int address)
	    {
		    var oldStatement = engine.Memory[address].Statement;
		    engine.WriteToMemory(address, new Statement(oldStatement)
		    {
			    FieldB = oldStatement.FieldB.Decremented()
		    });
	    }

	    private void Mov(GameEngine engine)
        {
            if (Statement.ModeA == AddressingMode.Immediate)
            {
                var address = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var newStatement = new Statement(engine.Memory[address].Statement)
                {
                    FieldB = new NumberExpression(Statement.FieldA.Calculate())
                };
                engine.WriteToMemory(address, newStatement);
            }
            else
            {
                var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                engine.WriteToMemory(addressB, engine.Memory[addressA].Statement);
            }
        }

        private void Add(GameEngine engine)
        {
            if (Statement.ModeA == AddressingMode.Immediate)
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var oldStatement = engine.Memory[addressB].Statement;
                engine.WriteToMemory(addressB, new Statement(oldStatement)
                {
                    FieldB = new NumberExpression(Statement.FieldA.Calculate() + oldStatement.FieldB.Calculate())
                });
            }
            else
            {
                var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
                var statementA = engine.Memory[addressA].Statement;
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var statementB = engine.Memory[addressB].Statement;
                engine.WriteToMemory(addressB, new Statement(statementB)
                {
                    FieldA = new NumberExpression(statementA.FieldA.Calculate() + statementB.FieldA.Calculate()),
                    FieldB = new NumberExpression(statementA.FieldB.Calculate() + statementB.FieldB.Calculate()),
                });
            }
        }

        private void Sub(GameEngine engine)
        {
            if (Statement.ModeA == AddressingMode.Immediate)
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var oldStatement = engine.Memory[addressB].Statement;
                engine.WriteToMemory(addressB, new Statement(oldStatement)
                {
                    FieldB = new NumberExpression(oldStatement.FieldB.Calculate() - Statement.FieldA.Calculate())
                });
            }
            else
            {
                var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
                var statementA = engine.Memory[addressA].Statement;
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var statementB = engine.Memory[addressB].Statement;
                engine.WriteToMemory(addressB, new Statement(statementB)
                {
                    FieldA = new NumberExpression(statementB.FieldA.Calculate() - statementA.FieldA.Calculate()),
                    FieldB = new NumberExpression(statementB.FieldB.Calculate() - statementA.FieldB.Calculate()),
                });
            }
        }

        private void Jmp(GameEngine engine)
        {
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.JumpTo(addressA);
        }

        private void Jmz(GameEngine engine)
        {
            bool isJump;
            if (Statement.ModeB == AddressingMode.Immediate)
                isJump = Statement.FieldB.Calculate() == 0;
            else
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                isJump = engine.Memory[addressB].Statement.FieldB.Calculate() == 0;
            }
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            if (isJump)
                engine.JumpTo(addressA);
        }

        private void Jmn(GameEngine engine)
        {
            bool isJump;
            if (Statement.ModeB == AddressingMode.Immediate)
                isJump = Statement.FieldB.Calculate() != 0;
            else
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                isJump = engine.Memory[addressB].Statement.FieldB.Calculate() != 0;
            }
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            if (isJump)
                engine.JumpTo(addressA);
        }

        private void Cmp(GameEngine engine)
        {
            bool isJump;
            if (Statement.ModeA == AddressingMode.Immediate && Statement.ModeB == AddressingMode.Immediate)
                isJump = Statement.FieldA.Calculate() == Statement.FieldB.Calculate();
            else if (Statement.ModeA == AddressingMode.Immediate)
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                isJump = Statement.FieldA.Calculate() == engine.Memory[addressB].Statement.FieldB.Calculate();
            }
            else
            {
                var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var statementA = engine.Memory[addressA].Statement;
                var statementB = engine.Memory[addressB].Statement;
                isJump = statementA == statementB;
            }
            if (isJump)
                engine.JumpTo(engine.CurrentIp + 2);
        }

        private void Slt(GameEngine engine)
        {
            bool isJump;
            if (Statement.ModeA == AddressingMode.Immediate && Statement.ModeB == AddressingMode.Immediate)
                isJump = Statement.FieldA.Calculate() < Statement.FieldB.Calculate();
            else if (Statement.ModeA == AddressingMode.Immediate)
            {
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                isJump = Statement.FieldA.Calculate() < engine.Memory[addressB].Statement.FieldB.Calculate();
            }
            else
            {
                var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
                var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
                var statementA = engine.Memory[addressA].Statement;
                var statementB = engine.Memory[addressB].Statement;
                isJump = statementA.FieldB.Calculate() < statementB.FieldB.Calculate();
            }
            if (isJump)
                engine.JumpTo(engine.CurrentIp + 2);
        }

        private void Spl(GameEngine engine)
        {
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.SplitAt(addressA);
        }

		private void Djn(GameEngine engine)
		{
			var addressB = CalcAddress(engine, Statement.ModeB, Statement.FieldB);
			DecrementB(engine, addressB);
			var condition = engine.Memory[addressB].Statement.FieldB.Calculate();
			var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
			if (condition != 0)
				engine.JumpTo(addressA);
		}

        private void Dat(GameEngine engine)
        {
			if (Statement.ModeA != AddressingMode.Immediate)
				CalcAddress(engine, Statement.ModeA, Statement.FieldA);
			if (Statement.ModeB != AddressingMode.Immediate)
				CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.KillCurrentProcess();
        }
    }
}