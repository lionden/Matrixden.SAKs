using Matrixden.SwissArmyKnives.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Matrixden.Test.Windows
{
    [TestClass]
    public class TestMPoint
    {
        [TestMethod]
        public void TestAxisymmetricPoint()
        {
            var mp = new MPoint(1317, 365);
            var tmir = new MPoint(1317, 763);
            var rect = new Rectangle(417, 245, 1055, 638);

            var mrp = mp.AxisymmetricPoint(rect, Directions.XParalle);
            Assert.AreEqual(tmir, mrp);

            var mrp2 = tmir.AxisymmetricPoint(rect, Directions.XParalle);
            Assert.AreEqual(mp, mrp2);
        }
    }
}
