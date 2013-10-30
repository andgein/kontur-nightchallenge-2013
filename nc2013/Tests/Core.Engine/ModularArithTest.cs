using Core.Engine;
using NUnit.Framework;

namespace Tests.Core.Engine
{
    [TestFixture]
    class ModularArithTest
    {
        [Test]
        public void TestPositiveNumbers()
        {
            Assert.AreEqual(5, ModularArith.Mod(5, 100));
            Assert.AreEqual(5, ModularArith.Mod(105, 100));
            Assert.AreEqual(0, ModularArith.Mod(45, 15));
        }
       
        [Test]
        public void TestNegativeNumber()
        {
            Assert.AreEqual(9, ModularArith.Mod(-1, 10));
            Assert.AreEqual(0, ModularArith.Mod(-100, 20));
            Assert.AreEqual(5, ModularArith.Mod(-15, 10));
            Assert.AreEqual(8, ModularArith.Mod(-12, 10));
        }
    }
}
