using System;
using System.Collections.Generic;
using Core.Parser;

namespace Core.Engine
{
    public class InstructionExecutor
    {
        private readonly Instruction instruction;
        private readonly Dictionary<StatementType, Action<Engine>> executers;
        private Statement Statement
        {
            get { return instruction.Statement;  }
        }

        public InstructionExecutor(Instruction instruction)
        {
            this.instruction = instruction;
            executers = new Dictionary<StatementType, Action<Engine>>
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
		
    	public void Execute(Engine engine)
        {
            var method = GetExecuteMethod();
            method.Invoke(engine);
        }

        private Action<Engine> GetExecuteMethod()
        {
            var statementType = Statement.Type;
            if (! executers.ContainsKey(statementType))
                throw new ArgumentException("Can't find executor for statement");
            return executers[statementType];
        }

        private static int CalcAddress(Engine engine, AddressingMode mode, Expression expression)
        {
            int address;
            switch (mode)
            {
                case AddressingMode.Immediate:
                    throw new InvalidOperationException();
                case AddressingMode.Direct:
                    return engine.CurrentIp + expression.Calculate();
                case AddressingMode.Indirect:
                    address = engine.CurrentIp + expression.Calculate();
                    return address + engine.Memory[address].Statement.FieldB.Calculate();
                case AddressingMode.PredecrementIndirect:
                    address = engine.CurrentIp + expression.Calculate();
                    var oldStatement = engine.Memory[address].Statement;
                    engine.WriteToMemory(address, new Statement(oldStatement)
                    {
                        FieldB = oldStatement.FieldB.Decremented()
                    });
                    return address + engine.Memory[address].Statement.FieldB.Calculate();
            }
            throw new InvalidOperationException("Internal error. Unknown addressing mode");
        }

        private void Mov(Engine engine)
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

        private void Add(Engine engine)
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

        private void Sub(Engine engine)
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

        private void Jmp(Engine engine)
        {
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.JumpTo(addressA);
        }

        private void Jmz(Engine engine)
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

        private void Jmn(Engine engine)
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

        private void Cmp(Engine engine)
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

        private void Slt(Engine engine)
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

        private void Spl(Engine engine)
        {
            var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
            CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.SplitAt(addressA);
        }

		private void Djn(Engine engine)
		{
			CalcAddress(engine, Statement.ModeB, Statement.FieldB);
			Statement.FieldB = Statement.FieldB.Decremented();
			var addressA = CalcAddress(engine, Statement.ModeA, Statement.FieldA);
			if (Statement.FieldB.Calculate() != 0)
				engine.JumpTo(addressA);
		}

        private void Dat(Engine engine)
        {
			if (Statement.ModeA != AddressingMode.Immediate)
				CalcAddress(engine, Statement.ModeA, Statement.FieldA);
			if (Statement.ModeB != AddressingMode.Immediate)
				CalcAddress(engine, Statement.ModeB, Statement.FieldB);
            engine.KillCurrentProcess();
        }
    }
}