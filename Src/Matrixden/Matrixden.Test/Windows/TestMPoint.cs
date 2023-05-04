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
            var mp = new MPoint(955, 443);
            var tmir = new MPoint(955, 545);

            var mrp = mp.AxisymmetricPoint(new Rectangle(327, 283, 1008, 422), Directions.XParalle);
            Assert.AreEqual(tmir, mrp);

            var mrp2 = tmir.AxisymmetricPoint(new Rectangle(327, 283, 1008, 422), Directions.XParalle);
            Assert.AreEqual(mp, mrp2);
        }
    }
}
