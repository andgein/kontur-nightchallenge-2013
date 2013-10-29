using System;
using System.Collections.Generic;
using Core.Engine;

namespace Core.Parser
{
    abstract class Statement
    {
        public string Label;
        public AddressingMode ModeA;
        public Expression FieldA;
        public AddressingMode ModeB;
        public Expression FieldB;

        public bool HasLabel()
        {
            return Label != "";
        }

        public abstract void Execute(Engine.Engine engine);

        protected int CalcAddress(Engine.Engine engine, AddressingMode mode, Expression expression)
        {
            int address;
            switch (mode)
            {
                case AddressingMode.Immediate:
                    throw new InvalidOperationException();
                case AddressingMode.Direct:
                    return (engine.CurrentIp + expression.Calculate())%Parameters.CORESIZE;
                case AddressingMode.Indirect:
                    address = expression.Calculate();
                    return (address + engine.Memory[address].Statement.FieldB.Calculate())%Parameters.CORESIZE;
                case AddressingMode.PredecrementIndirect:
                    address = expression.Calculate();
                    engine.Memory[address].Statement.FieldB = engine.Memory[address].Statement.FieldB.Decremented();
                    return (address + engine.Memory[address].Statement.FieldB.Calculate()) % Parameters.CORESIZE;
            }
            throw new InvalidOperationException("Internal error. Unknown addressing mode");
        }

        public Statement Copy()
        {
            var constructor = GetType().GetConstructor(new Type[0]);
            if (constructor == null)
                throw new InvalidOperationException("Internal error. Can't create a copy of statement");
            var stmt = (Statement) constructor.Invoke(new object[0]);
            stmt.FieldA = FieldA;
            stmt.ModeA = ModeA;
            stmt.FieldB = FieldB;
            stmt.ModeB = ModeB;
            return stmt;
        }
    }

    class StatementFactory
    {
        public Dictionary<String, Type> Commands { get; private set; }

        public StatementFactory()
        {
            Commands = new Dictionary<string, Type>
                           {
                               {"MOV", typeof (MovStatement)},
                               {"ADD", typeof (AddStatement)},
                               {"SUB", typeof (SubStatement)},
                               {"CMP", typeof (CmpStatement)},
                               {"SLT", typeof (SltStatement)},
                               {"JMP", typeof (JmpStatement)},
                               {"JMZ", typeof (JmzStatement)},
                               {"JMN", typeof (JmnStatement)},
                               {"DJN", typeof (DjnStatement)},
                               {"SPL", typeof (SplStatement)},
                               {"DAT", typeof (DatStatement)}
                           };
        }

        public Statement Create(string command)
        {
            if (! Commands.ContainsKey(command.ToUpper()))
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

            var constructor = Commands[command.ToUpper()].GetConstructor(new Type[0]);
            if (constructor == null)
                throw new Exception(String.Format("Internal error: can't create statement from command {0}", command));

            return (Statement) constructor.Invoke(new object[0]);
        }
    }

    class MovStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            if (ModeA == AddressingMode.Immediate)
            {
                var address = CalcAddress(engine, ModeB, FieldB);
                var newStatement = engine.Memory[address].Statement.Copy();
                newStatement.FieldB = new NumberExpression(FieldA.Calculate());
                engine.WriteToMemory(address, newStatement);
            }
            else
            {
                var addressA = CalcAddress(engine, ModeA, FieldA);
                var addressB = CalcAddress(engine, ModeB, FieldB);
                engine.WriteToMemory(addressB, engine.Memory[addressA].Statement);
            }
        }
    }

    class AddStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class SubStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class CmpStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class SltStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class JmpStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class JmzStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class JmnStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class DjnStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class SplStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            throw new NotImplementedException();
        }
    }

    class DatStatement : Statement
    {
        public override void Execute(Engine.Engine engine)
        {
            engine.KillCurrentProcess();
        }
    }
}