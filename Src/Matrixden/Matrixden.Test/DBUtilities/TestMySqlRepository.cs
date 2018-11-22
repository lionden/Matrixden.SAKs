using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Matrixden.DBUtilities;
using Matrixden.Utils.Serialization;
using Matrixden.Zion.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matrixden.Test.DBUtilities
{
    /// <summary>
    /// Summary description for TestMySqlRepository
    /// </summary>
    [TestClass]
    public class TestMySqlRepository
    {
        public TestMySqlRepository()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestGetByCondition()
        {
            var r = DBRepository.Instance.GetByCondition(typeof(UserInfoModel), $"LoginId='Admin'", string.Empty);
            var ts = (IEnumerable<object>) r.Data;
            var t = ts.FirstOrDefault();
            var userInfo = (UserInfoModel) t;
            System.Diagnostics.Debug.WriteLine(JsonHelper.Serialize(userInfo));

            Assert.IsTrue(r.Result, "Error");
            Assert.IsNotNull(r.Data, "Error2");
            Assert.IsNotNull(userInfo, "Error3");
        }
    }
}
