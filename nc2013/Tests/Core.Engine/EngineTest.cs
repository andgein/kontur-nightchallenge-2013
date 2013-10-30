using System.Collections.Generic;
using Core.Parser;
using NUnit.Framework;

namespace Tests.Core.Engine
{
    [TestFixture]
    class EngineTest
    {
        [Test]
        public void TestEngineWithOneWarrior()
        {
            /* Just imp */ 
            var warrior = new Warrior(new List<Statement>{ new MovStatement
            {
                ModeA = AddressingMode.Direct,
                FieldA = new NumberExpression(0),
                ModeB = AddressingMode.Direct,
                FieldB = new NumberExpression(1),
            }});

            var engine = new global::Core.Engine.Engine(new[]{warrior});
            engine.Step();
            engine.Step();
            engine.Step();
            engine.Step();
            engine.Step();
        }
    }
}
