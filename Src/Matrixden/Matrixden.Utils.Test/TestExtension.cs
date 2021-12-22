using Matrixden.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Matrixden.Utils.Test
{
    [TestClass]
    public class TestExtension
    {
        [TestMethod]
        public void TestObject2Boolean()
        {
            Assert.IsFalse(default(object).ToBoolean(), "Should be false.");
        }
    }
}
