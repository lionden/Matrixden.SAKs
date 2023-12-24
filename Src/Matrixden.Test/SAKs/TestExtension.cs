using Matrixden.SAK.Extensions;
using Matrixden.Utils.Extensions;
using Matrixden.Zion.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [TestMethod]
        public void TestInt2Hex()
        {
            var a = 1235;
            var astr = "00 04 D3";

            Debug.WriteLine(default(char));

            Debug.WriteLine(a.HexString(6));
            Assert.AreEqual(astr, a.HexString(6, ' '));
        }

        [TestMethod]
        public void TestEnumerableIndexOf()
        {
            var ls = new List<UserInfoModel> { new() { Name = "Jack" }, new() { Name = "Bob" }, new() { Name = "Tom" } };
            UserInfoModel u = new() { Name = "Jerry" }, u2 = new() { Name = "Tom" };

            Assert.AreEqual(-1, ls.IndexOf(u2, "name"));
            Assert.AreEqual(2, ls.IndexOf(u2, "Name"));
            Assert.AreEqual(-1, ls.IndexOf(u));
        }
    }
}
