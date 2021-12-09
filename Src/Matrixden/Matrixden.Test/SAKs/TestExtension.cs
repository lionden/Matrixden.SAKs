using Matrixden.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestExtension
    {
        [TestMethod]
        public void TestGetFiles2()
        {
            var files = default(DirectoryInfo).GetFiles2(null);
            Assert.IsNotNull(files, "Files should not be null");

            var bd = new DirectoryInfo(@"D:\Lionden Lee\Music\阿杜1");
            files = bd.GetFiles2(null);
            Assert.IsNotNull(files, "Files should not be null");

            bd = new DirectoryInfo(@"D:\Lionden Lee\Music\周杰伦\2001 - 范特西\");
            files = bd.GetFiles2("|");
            Assert.IsNotNull(files, "Files should not be null");

            files = bd.GetFiles2("*.flac|*.MP3|*.WaV");
            Assert.IsNotNull(files, "Files should not be null");
        }
    }
}
