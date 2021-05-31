using Matrixden.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matrixden.Test.SAKs
{
    /// <summary>
    /// Summary description for TestStringHelper
    /// </summary>
    [TestClass]
    public class TestStringHelper
    {
        public TestStringHelper()
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
        public void TestMethod_IsBase64Encoded()
        {
            var s1 = "msplWYyQiQ5PQQeCD0uBBn0mSMED7KhvwQZ GTLBAesdc8EFgiFsgQMJIkwBBncnV0EHdY9rAQcHonpBBoE9ZEECdZR1AQkINEEBDGdEU4ERZiQ8wQZuGkLBBXWcM8EDdZc1wQRwFFUBB36MLMEGeoo4AQd9kiBBCHIXb8EGhlRuQQtvNX4BBw85hoEEi7x/AQl/vYsBBRBOh4EJh09NQQhWkYVBBYCNdoEFgtxhgRNPooRBBQiwOcEM5QMRXV9jZ2xxdHV3AwQGCQoMAxFbXWBkaW9ydXcCBAYJDA0DEWBjZmlucnV3AgQFBwkKCgQRXF5iZmtvc3YCBAYJDA8CEWNlaGtucXN2dwMFBwgJCQoEEVlcX2NpbXN1AQMFCAwPAhFoamxvcXJ0dwIEBggJCQkJBRFZXGBmbHN1dwEDBwsPAhFsbnFyc3R3AQMFBwgJCgoJBRFWWF1iaXB0dwEDBgwQAhFwcXJzdXYBAgMGCAoKCggGBhFVWF5kbHJ2AQQIDhICEXNzdHV2AQIDBQgKCgkHBQQGEVJWW19mbXQBBAgOEQIRdHR1dgECAwUHCgoKBwUEBAcRVFdcYWlyAQUIDA8DEHcBAgMDBQcKCgkHBgUEBxFSVVhdZG11AwcLDgMPAgMEBQYICgwJCAYGBQgRU1RYX2hxAQcLDQQOBQYHCAsMDAgGBQYIEVBRVFhfanUGDA8FDQcICg0NDAcFAwkRTU9RVWBzBg0QChBLS01ZcQUN";
            var s2 = "msplWYyQiQ5PQQeCD0uBBn0mSMED7KhvwQZGTLBAesdc8EFgiFsgQMJIkwBBncnV0EHdY9rAQcHonpBBoE9ZEECdZR1AQkINEEBDGdEU4ERZiQ8wQZuGkLBBXWcM8EDdZc1wQRwFFUBB36MLMEGeoo4AQd9kiBBCHIXb8EGhlRuQQtvNX4BBw85hoEEi7x/AQl/vYsBBRBOh4EJh09NQQhWkYVBBYCNdoEFgtxhgRNPooRBBQiwOcEM5QMRXV9jZ2xxdHV3AwQGCQoMAxFbXWBkaW9ydXcCBAYJDA0DEWBjZmlucnV3AgQFBwkKCgQRXF5iZmtvc3YCBAYJDA8CEWNlaGtucXN2dwMFBwgJCQoEEVlcX2NpbXN1AQMFCAwPAhFoamxvcXJ0dwIEBggJCQkJBRFZXGBmbHN1dwEDBwsPAhFsbnFyc3R3AQMFBwgJCgoJBRFWWF1iaXB0dwEDBgwQAhFwcXJzdXYBAgMGCAoKCggGBhFVWF5kbHJ2AQQIDhICEXNzdHV2AQIDBQgKCgkHBQQGEVJWW19mbXQBBAgOEQIRdHR1dgECAwUHCgoKBwUEBAcRVFdcYWlyAQUIDA8DEHcBAgMDBQcKCgkHBgUEBxFSVVhdZG11AwcLDgMPAgMEBQYICgwJCAYGBQgRU1RYX2hxAQcLDQQOBQYHCAsMDAgGBQYIEVBRVFhfanUGDA8FDQcICg0NDAcFAwkRTU9RVWBzBg0QChBLS01ZcQUN";
            var s3 = "msplWYyQiQ5PQQeCD0uBBn0mSMED7KhvwQZ+GTLBAesdc8EFgiFsgQMJIkwBBncnV0EHdY9rAQcHonpBBoE9ZEECdZR1AQkINEEBDGdEU4ERZiQ8wQZuGkLBBXWcM8EDdZc1wQRwFFUBB36MLMEGeoo4AQd9kiBBCHIXb8EGhlRuQQtvNX4BBw85hoEEi7x/AQl/vYsBBRBOh4EJh09NQQhWkYVBBYCNdoEFgtxhgRNPooRBBQiwOcEM5QMRXV9jZ2xxdHV3AwQGCQoMAxFbXWBkaW9ydXcCBAYJDA0DEWBjZmlucnV3AgQFBwkKCgQRXF5iZmtvc3YCBAYJDA8CEWNlaGtucXN2dwMFBwgJCQoEEVlcX2NpbXN1AQMFCAwPAhFoamxvcXJ0dwIEBggJCQkJBRFZXGBmbHN1dwEDBwsPAhFsbnFyc3R3AQMFBwgJCgoJBRFWWF1iaXB0dwEDBgwQAhFwcXJzdXYBAgMGCAoKCggGBhFVWF5kbHJ2AQQIDhICEXNzdHV2AQIDBQgKCgkHBQQGEVJWW19mbXQBBAgOEQIRdHR1dgECAwUHCgoKBwUEBAcRVFdcYWlyAQUIDA8DEHcBAgMDBQcKCgkHBgUEBxFSVVhdZG11AwcLDgMPAgMEBQYICgwJCAYGBQgRU1RYX2hxAQcLDQQOBQYHCAsMDAgGBQYIEVBRVFhfanUGDA8FDQcICg0NDAcFAwkRTU9RVWBzBg0QChBLS01ZcQUN";

            Assert.IsFalse(s1.IsBase64Encoded(), "Giving string is not BASE64 value");
            Assert.IsFalse(s2.IsBase64Encoded(), "Giving string is not BASE64 value");
            Assert.IsTrue(s3.IsBase64Encoded(), "Giving string is BASE64 value");
        }
    }
}
