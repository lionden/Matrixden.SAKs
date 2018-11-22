using System;
using Matrixden.Utils;
using Matrixden.Zion.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matrixden.Test.Utils
{
    [TestClass]
    public class TestCommonClass
    {
        [TestMethod]
        public void TestMethod1()
        {
            var name = Util.RandomName(3);
            var model = new UserInfoModel
            {
                ID = Guid.NewGuid(),
                Name = name
            };

            Assert.AreEqual(CommonClass.GetPropertyValue(model, "ID"), CommonClass.GetPropertyValue<UserInfoModel,Guid>(model, "ID"), "Error");
            Assert.AreEqual(name, CommonClass.GetPropertyValue(model, "Name"), "Error2");
        }
    }
}
