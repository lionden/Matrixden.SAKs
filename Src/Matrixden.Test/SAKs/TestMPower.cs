using Matrixden.SAK.Extensions;
using Matrixden.SwissArmyKnives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestMPower
    {
        [TestMethod]
        public void TestRange()
        {
            Trace.WriteLine("");
            MEnumerable.Range(1, 1).ForEach(x => Trace.Write(x + " "));
            Trace.WriteLine("");
            MEnumerable.Range(1, 3).ForEach(x => Trace.Write(x + " "));
            Trace.WriteLine("");

            MEnumerable.Range(3, 1).ForEach(x => Trace.Write(x + " "));
            Trace.WriteLine("");
            MEnumerable.Range(3, -2).ForEach(x => Trace.Write(x + " "));
        }

        [TestMethod]
        public void TestDirectoryMovement()
        {
            MDirectory.Move("D:\\tmp\\0\\simulator test tool", "D:\\tmp\\1");
        }

        [TestMethod]
        public void TestFileRename()
        {
            MFile file = new(@"D:\tmp\1.txt");
            file.Rename("1.txt.bk");
        }
    }
}
