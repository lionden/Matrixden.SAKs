using Matrixden.SwissArmyKnives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Matrixden.Test
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        [Obsolete]
        public void TestGetLocalIp()
        {
            var ip = Matrixden.Utils.Util.GetLocalIp();
            Assert.AreEqual("192.168.1.192", ip);
        }

        [TestMethod]
        public void TestGetLocalIPs()
        {
            var ip = Util.GetLocalIPv4();

        }
    }
}
