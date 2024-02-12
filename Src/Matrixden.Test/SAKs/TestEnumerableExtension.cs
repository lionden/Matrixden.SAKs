using Matrixden.SAK.Extensions;
using Matrixden.Test.Models;
using Matrixden.Utils.Extensions;
using Matrixden.Zion.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestEnumerableExtension
    {
        [TestMethod]
        public void TestForEach()
        {
            IEnumerable<UserInfoModel> arr = default, arr2 = new List<UserInfoModel>(), arr3 = arr2.AppendM(new());
            Assert.AreEqual(0, arr.ForEach(a => { }).Count());
            Assert.AreEqual(0, arr2.ForEach(a => { }).Count());
            Assert.AreEqual(1, arr3.ForEach(a => { }).Count());
            Assert.AreEqual(0, arr3.ForEach(default(Action<UserInfoModel>)).Count());
        }

        [TestMethod]
        public void TestEnumerableIndexOf()
        {
            var ls = new List<UserInfoModel> { new() { Name = "Jack" }, new() { Name = "Bob" }, new() { Name = "Tom" } };
            UserInfoModel u = new() { Name = "Jerry" }, u2 = new() { Name = "Tom" };

            Assert.AreEqual(-1, new List<UserRoleStruct>().IndexOf(new(), string.Empty));
            Assert.AreEqual(-1, new List<int>().IndexOf(1, string.Empty));
            Assert.AreEqual(-1, ls.IndexOf(u2, "name"));
            Assert.AreEqual(2, ls.IndexOf(u2, "Name"));
            Assert.AreEqual(-1, ls.IndexOf(u));
        }

        [TestMethod]
        public void TestEnumerableAppend()
        {
            IEnumerable<UserInfoModel> arr = default;
            Assert.ThrowsException<ArgumentNullException>(() => arr.AppendM(new()));

            Assert.IsNotNull(arr.AppendM(new()));
            Assert.IsNull(arr);
        }

        [TestMethod]
        public void TestEnumerableSelect()
        {
            var ls = new List<int> { 1, 2, 3, 4 };

            var r = ls.SelectM((s, i) => $"{s}-{i}");
        }
    }
}
