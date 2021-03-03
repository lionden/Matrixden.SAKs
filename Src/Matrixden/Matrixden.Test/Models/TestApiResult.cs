using Matrixden.Utils.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Matrixden.Test.Models
{
    [TestClass]
    public class TestApiResult
    {
        [TestMethod]
        public void TestCtor()
        {
            var data = new
            {
                id = 1,
                name = "name"
            };

            var ar = new ApiResult(data);
            Assert.AreEqual(ApiResult.SUCCESS_SUCCESS, ar.isSuccess);
        }

        [TestMethod]
        public void TestCtor_OperationResult()
        {
            var or = new OperationResult
            {
                Result = true,
                Data = new
                {
                    id = 1,
                    name = "name"
                }
            };

            var ar = new ApiResult(or);
            Assert.AreEqual(ApiResult.SUCCESS_SUCCESS, ar.isSuccess);

            or = new OperationResult();
            ar = new ApiResult(or);
            Assert.AreEqual(ApiResult.SUCCESS_FAIL, ar.isSuccess);

            //Assert.AreEqual(or.Data, ar.data);
        }
    }
}
