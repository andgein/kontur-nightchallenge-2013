using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Parser;
using NUnit.Framework;

namespace Core.Engine
{
    [TestFixture]
    class EngineTest
    {
        [Test]
        public void TestEngineWithOneWarrior()
        {
            /* Just imp */ 
            var warrior = new Warrior(new List<Statement>{ new MovStatement()
            {
                FieldA = new NumberExpression(0),
                ModeA = AddressingMode.Direct,
                FieldB = new NumberExpression(1),
                ModeB = AddressingMode.Direct,
            }});

            var engine = new Engine(new[]{warrior});
            engine.Step();
            engine.Step();
            engine.Step();
            engine.Step();
            engine.Step();
        }
    }
}
