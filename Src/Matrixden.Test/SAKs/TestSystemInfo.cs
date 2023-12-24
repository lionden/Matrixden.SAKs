using Matrixden.SwissArmyKnives.SystemInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace Matrixden.Test.SAKs
{
    [TestClass]
    public class TestSystemInfo
    {
        [TestMethod]
        public void TestSerialPorts()
        {
            var r = DeviceSpecifications.SerialDevices();
        }

        [TestMethod]
        public void TestCheckAndInstallDotNetVersion()
        {
            WindowsSpecifications.CheckAndInstallDotNetVersion("6.0.0", r =>
            {
                if (!r.Result)
                {
                    return MessageBoxResult.Yes;
                }

                return default;
            });
        }
    }
}
