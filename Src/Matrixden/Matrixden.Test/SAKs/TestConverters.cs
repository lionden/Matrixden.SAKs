using Matrixden.SwissArmyKnives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestConverters
    {
        [TestMethod]
        public void TestConvertHexString2String()
        {
            var hs = "50 44 4b 39 36 35 30 39 7c 32 ";
            var tar = "PDK96509|2";

            Assert.AreEqual(tar, MConverter.HexString2String(hs,' '));
        }
    }
}
