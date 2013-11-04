using System.Collections.Generic;
using Core.Engine;
using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Engine
{
	[TestFixture]
    class EngineTest
    {
        //[Test]
        //public void TestEngineWithOneWarrior()
        //{
        //    /* Just imp */ 
        //    var warrior = new Warrior(new List<Statement>{ new Statement
        //    {
        //        Type = StatementType.Mov,
        //        ModeA = AddressingMode.Direct,
        //        FieldA = new NumberExpression(0),
        //        ModeB = AddressingMode.Direct,
        //        FieldB = new NumberExpression(1),
        //    }});

        //    var engine = new global::Core.Engine.Engine(new[]{warrior});
        //    engine.Step();
        //    engine.Step();
        //    engine.Step();
        //    engine.Step();
        //    engine.Step();
        //}

        [Test]
        public void TestDwarf()
        {
            /* 
                ADD #4, 3        ; execution begins here
                MOV 2, @2
                JMP -2
                DAT #0, #0
            */
            var warrior = new Warrior(new List<Statement>
            {
                new Statement(null, StatementType.Add)
                {
                    ModeA = AddressingMode.Immediate,
                    FieldA = new NumberExpression(4),
                    ModeB = AddressingMode.Direct,
                    FieldB = new NumberExpression(3),
                },
                new Statement(null, StatementType.Mov)
                {
                    ModeA = AddressingMode.Direct,
                    FieldA = new NumberExpression(2),
                    ModeB = AddressingMode.Indirect,
                    FieldB = new NumberExpression(2),
                },
                new Statement(null, StatementType.Jmp)
                {
                    ModeA = AddressingMode.Direct,
                    FieldA = new NumberExpression(-2),
                    ModeB = AddressingMode.Direct,
                    FieldB = new NumberExpression(0),
                },
                new Statement(null, StatementType.Dat)
                {
                    ModeA = AddressingMode.Immediate,
                    FieldA = new NumberExpression(0),
                    ModeB = AddressingMode.Immediate,
                    FieldB = new NumberExpression(0),
                },
            });
        	var wsi = new WarriorStartInfo(warrior, 0);
            var engine = new GameEngine(new[]{wsi});
            for (var i = 0; i < 80000; ++i)
                engine.Step();
        }
    }
}
