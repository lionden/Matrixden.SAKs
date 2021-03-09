using Matrixden.SwissArmyKnives.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestProcessUtil
    {
        [TestMethod]
        public void TestGets()
        {
            var ps = ProcessUtil.Gets("Authentication.Tray");
            Assert.IsTrue(ps.Length >= 1);
        }

        [TestMethod]
        public void TestEnd()
        {
            ProcessUtil.EndTask("Authentication.Tray");

            Thread.Sleep(200);
            var ps = ProcessUtil.Gets("Authentication.Tray");
            Assert.IsTrue(ps.Length <= 0);
        }

        [TestMethod]
        public void TestStart()
        {
            var b = ProcessUtil.Start(@"C:\Users\Lionden Lee\source\repos\BioidentificationCenter\_Build\release\JBSS Client\Authentication.Tray.exe");
            Assert.IsTrue(b);

            TestGets();
        }

        [TestMethod]
        public void TestRestart()
        {
            var b = ProcessUtil.ReStart(@"C:\Users\Lionden Lee\source\repos\BioidentificationCenter\_Build\release\JBSS Client\Authentication.Tray.exe");
            Assert.IsTrue(b);

            TestGets();
        }

        [TestMethod]
        public void TestWorkingDirectory()
        {
            var b = ProcessUtil.TryStart(@"B:\Program Files\Jieyun Info\JBSS Client\Authentication.Tray.exe");
            Assert.IsTrue(b.Result);

            Assert.IsNotNull(b.Data);
        }
    }
}
