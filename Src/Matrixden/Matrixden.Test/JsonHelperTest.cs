using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matrixden.Utils.Serialization;
using Matrixden.Utils.Web.Models;

namespace Matrixden.Test
{
    /// <summary>
    /// Summary description for JsonHelperTest
    /// </summary>
    [TestClass]
    public class JsonHelperTest
    {
        public JsonHelperTest()
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
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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
        [Obsolete]
        public void TestDeserialize()
        {
            var txt = "{\"code\":1,\"isSuccess\":1,\"message\":null,\"data\":[{\"no\":1,\"name\":\"测试1\",\"department_no\":1,\"department_name\":\"测试一部门\",\"post_no\":1,\"post_name\":\"岗位1\",\"worktype_no\":1,\"worktype_name\":0,\"district\":\"青岛\",\"mobile\":\"13791841183\",\"birthday\":\"2009-01-07T10:40:02.0000000Z\",\"entry_date\":\"0001-01-01T00:00:00.0000000Z\",\"login_id\":\"001\",\"login_pwd\":\"96E79218965EB72C92A549DD5A330112\",\"fingerprint1\":null,\"fingerprint2\":null,\"remark\":null,\"create_time\":\"0001-01-01T00:00:00.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"2019-03-20T22:19:22.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6},{\"no\":6,\"name\":\"118\",\"department_no\":1,\"department_name\":\"测试一部门\",\"post_no\":4,\"post_name\":\"岗位1\",\"worktype_no\":1,\"worktype_name\":0,\"district\":\"济南\",\"mobile\":\"\",\"birthday\":\"1990-01-01T00:00:00.0000000Z\",\"entry_date\":\"2019-03-25T00:00:00.0000000Z\",\"login_id\":\"118\",\"login_pwd\":\"1BBD886460827015E5D605ED44252251\",\"fingerprint1\":\"msplWZKdkUtowRACzmSBFAZRLUEF2VE1wQbX02bBQ1zVaAE8VMOXwQqExJDBCoiigwEGdiREAQjkIlJBCW/RgIEUKTJ3AQZytD5BBmuzV0EEabI4QQXnv2aBCOygZUEE5zhsgQfpUGIBGWSwWgEC4j10AQXsPUHBA9y/fcEKCrR6gQkHSUrBBt22fQEHd8JJQQPbyXoBCxNTd0EcMz5PAQZm2HFBFEvEPQEGYpyHQQcDqkFBC3DGjMEJF5FuwQXmFYFBBenZZ0ENWlfU9BoFFHJyb2xqbGttcHR2AgUGCAgEFG1vb2tqaWprbG9zdwMGCAkLBRR1dHJubGxrbnBzdQEEBgcIBBRqa2tpaWlqamxvc3cEBwoLDwUUdHRzcG5ubm9xc3V3AwUGBwQUaWppZmdnaWlscHR3BQkODxYFFG5ycnFwcXBvcHJ1dwIEBQcEFGlpaGVlZmhqbnN3AggMERIaBhRubm9xcnFvcHJ0dwIDBAYEFGVmZWNjZGdrcHUCBgsPExYhBhRpbG9ycnBub3F0dncCAwYEFGFjYmFhY2ZrcHYFCg8SFhoqBxRqbm9wcG9vcHJ0dQECBQQTXV9fX19hZWlwAQgPFBcaHwgTb29xcHBvcHFzdHcCBBNcXl5eXV9hZm8FFBkcHSEiCRJvcXFwb29vcnN3BBNdX19fX19fYWQIJiYnJycmAAD/BBJiZGNhYF5dXlxFNjQ0MjAAAP8EEmRkZGFhXl1ZVEtDPjs5NwAA/wURaWhjZGFi////SkNAPgYIaWVn\",\"fingerprint2\":\"msplWZWdkUhqgQ4Cy2eBFgbQaYFSWVBrgWpLVFQBBNXUWcEF01VfgQjSVlfBBV1WXsELW56BgQZ0H1CBCWyvdgEGcjBXgQNozoJBFSm9Z0EH6kGRAQyHVVHBA14dY0EE5RlLgQbpQ42BCRYtWkEC4blzwQXru0CBBNxHS4EF27F5QQgGMn3BB3c1bIEG51A6gQbXVHSBF0nASQEF2UV7wQsRUHlBGjLWaYERV01kARllPH3BCQcYhYEHAbCZwQcLO0+BBmUSfwEF6VkUtBoEFG9vb2tpaWpqbG90dwMFBggIBBRtbW1paWhpamxvc3cDBggKDQQUcnNyb2xqa2ttcHN2AgQGBwcEFGxraWdnZ2lpa25zdwQICw0SBRR0dHFua2trbXBzdQEDBQYHAxRqa2loZWVmaGltcXUBBgoOEBYFFHJzcnBubm5ucHJ1dwIEBQcDFGhoZ2VjY2Vnam9zAQUJDRATHAUUbXBvb3Bwb29wcnR3AgMEBgMTZGVkY2JiY2ZqcHUECA0QFBkFFGVtbW9xcXBub3J0dwECBAUDE2BgYWBfYGJmaXB3Bw0RFRgdBhRrbG9wcG9ub3FzdXcCAwUDE15fX15eXmBjZ28DDxUZGh0hBxNub29wb29wcXJ0dXcCAxNgYF9fX19fYGFmCiEiIyMlJggSb29wb3BwcXFydHcDEmRnZGJgX11dXl5CMjAwLy8AAP8EEmtpZmJhXl1bVkpBPTo4NgAA/wQRb2xoYmFfXlxXUEhCPz4AAP8EEG9ta2dlYmH//1NMSEQFCW5uaWlk\",\"remark\":null,\"create_time\":\"2019-03-25T16:44:47.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"0001-01-01T00:00:00.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6},{\"no\":13,\"name\":\"哈哈哈\",\"department_no\":1,\"department_name\":\"测试一部门\",\"post_no\":4,\"post_name\":\"岗位1\",\"worktype_no\":1,\"worktype_name\":0,\"district\":\"济南\",\"mobile\":\"\",\"birthday\":\"1980-01-01T00:00:00.0000000Z\",\"entry_date\":\"2019-03-21T00:00:00.0000000Z\",\"login_id\":\"1111\",\"login_pwd\":\"96E79218965EB72C92A549DD5A330112\",\"fingerprint1\":\"msplmYmMkRZvgRmLGXSBFpNAZMEEGkp7AQSSRTwBBAVHYEEIihllAQ4UoXtBBo7JakECGBwoQQICImJBBpIjb8EKFqs4QQIHsluBCIsqZQEFj1yBgQWYU3WBBx4pfMEIFUVogQSTJz9BCYIuRUEHgcRGQQR/20QBE3fJMoEPcqctgQh2HjmBB4CaToEMixRIQQ2IlF3BCJcxkIEJFhd5wQ4cLZPBB4uST8ETmplZQQYdGS3BB3oEE3R3BAcLDxQWGBkZGBcWFhgEE3R3BAcLDxIVFxkZGBcXGBgEE3UBBQkMEBUXGRoZFxYWFhkEE3N2AwYKDRATFxgZGBgYGRkEEnYCBQkNERUYGRkXFhYYGAQTc3YCBgoODxMXGRkYGBkaGwQSAQQHCg4RFRkaGRUVFxkZBBNwdQIGCg4QFRgaGhoaGRocBBEBBAcLDhEWGxoZFRgaHAQTcHUDBgkMEBUZGxsbGhoaHAQRAwYIDA8RFhsZGhcaGx0EE290AgYJCw8UGBobGxobGh8EEQMGCA0QFRkaFxkbIiAjBBNtcgEFCAoOFBkaGhsbGxofBRAHCQ4RFhoYFhUZHx8EEmlvdAQJCw8VGhobGxscGwYOCQ4RFhkVFBAUBRJtcgEIDBEYHBwdHR0dHAAA/wUSa292Bw4UGRwdICAgICAAAP8FEWhucwUNFBgcHR8gICEAAP8GEW5zAw0S//8ZGx0dHAcQcXcL/////xcZGQ==\",\"fingerprint2\":\"msplmZrooNVwQQyW1WqBDpy+dgEJID59wQcfzYCBCpZNe8EKlk51AQSYT4jBEJzUU4EoeFZWQR54uliBCYI6X4EIi8J1gQyXQ3KBCpdFe0EMoiRcwQoTqGEBCRVOgMEIH89vQQYfz3vBCx/DhoEImMWKARCttHUBDyE0b0EPJUmCAQieQjsBFOfBV8ENGMpdQQ4fwV9BDBfFXMEJjjVrARaT0GDBBpQ6h4EcJM43QRDX01nBGpOxX4EOhrYvQQjev1BBCHpFhYEIIkZLwQ92NoUBGR0gS8EOCy05wQ3rrzXBDmImPgEJ7DprwQ6Ou4NBEJ88fMEKlTODwQqUQF8BDoMhXQEHh6d3AQaMvHQBC5WqTcEKe59EAQdwJE5BCnvEfAEPKDtcwQsOBRJkaXB2AwgPFRkbHBwbGAUSY2lwdQMHDhccIB8cHBkFEWZpcXYECQ8UFhkbHR0EE1xgZm50AwgNFhwhIh8fGh0FEWZpcHYFCg8SFRgZHB8EE1lfZGxzAwgOFxsgIiAfGyEFEWRrcHYGCw8SFRgZHB8EE1ZdYmpzAwkOFRofIiIfGx0GEW1xdwYMEBMVGBocHwUTWV9pcwMLEhgcHyIjIh8gBhBwdQIHCw8TFRkbHwUTV15ncwMMFBofISYmJyIkBxABAgYKDhEUGBsgBRJTW2NvAg4ZHR8gJCUnIggPBAYJDhIVGRsGEldfanQNHSAhIiQlJyQLDg0QEhMGEVZeZ3IKHCAgISMlJQAA/wcRYGhxBBsjJSUlJSUAAP8IEWdvdhYkJiUkIiMAAP8IEWlwdRQkJiMhHR0JEGlw////IB8d\",\"remark\":null,\"create_time\":\"2019-03-27T17:26:36.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"2019-03-27T23:08:49.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6},{\"no\":14,\"name\":\"嘿嘿嘿\",\"department_no\":1,\"department_name\":\"测试一部门\",\"post_no\":4,\"post_name\":\"岗位1\",\"worktype_no\":1,\"worktype_name\":0,\"district\":\"济南\",\"mobile\":\"\",\"birthday\":\"1980-01-01T00:00:00.0000000Z\",\"entry_date\":\"2019-03-27T00:00:00.0000000Z\",\"login_id\":\"9999\",\"login_pwd\":\"96E79218965EB72C92A549DD5A330112\",\"fingerprint1\":\"\",\"fingerprint2\":\"\",\"remark\":null,\"create_time\":\"2019-03-27T17:26:36.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"0001-01-01T00:00:00.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6},{\"no\":4,\"name\":\"我是测试呀\",\"department_no\":3,\"department_name\":\"职教科\",\"post_no\":5,\"post_name\":\"岗位2\",\"worktype_no\":6,\"worktype_name\":0,\"district\":null,\"mobile\":null,\"birthday\":\"2019-03-01T00:00:00.0000000Z\",\"entry_date\":\"2019-03-01T00:00:00.0000000Z\",\"login_id\":\"1001\",\"login_pwd\":\"E10ADC3949BA59ABBE56E057F20F883E\",\"fingerprint1\":null,\"fingerprint2\":null,\"remark\":null,\"create_time\":\"2019-03-23T11:17:04.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"0001-01-01T00:00:00.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6},{\"no\":5,\"name\":\"张三三\",\"department_no\":11,\"department_name\":\"技术科\",\"post_no\":6,\"post_name\":\"岗位3\",\"worktype_no\":6,\"worktype_name\":0,\"district\":\"烟台\",\"mobile\":\"\",\"birthday\":\"1980-01-01T00:00:00.0000000Z\",\"entry_date\":\"2019-03-21T00:00:00.0000000Z\",\"login_id\":\"002\",\"login_pwd\":\"635092B43F6DAAB6E117B2429F5E6236\",\"fingerprint1\":null,\"fingerprint2\":null,\"remark\":null,\"create_time\":\"2019-03-23T11:47:50.0000000Z\",\"create_man\":0,\"create_man_name\":null,\"update_time\":\"0001-01-01T00:00:00.0000000Z\",\"update_man\":0,\"update_man_name\":null,\"detele_time\":\"0001-01-01T00:00:00.0000000Z\",\"detele_man\":0,\"state\":1,\"sumcount\":6}]}";
            var ar = JsonHelper.Deserialize<ApiResult>(txt);

            Assert.AreEqual(1, ar.isSuccess);
        }
    }
}
