using Matrixden.SwissArmyKnives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestMTimer
    {
        [TestMethod]
        public void TestMTimerTick()
        {
            var timer = new MTimer(3);
            timer.Tick += (s, e) =>
            {
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss fffffff}");
            };
            timer.Start();

            Thread.Sleep(1000);
            timer.Stop();
        }
    }
}
