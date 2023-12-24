using Matrixden.SAK.Extensions;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives.SystemInfo
{
    public class DeviceSpecifications
    {
        private static ManagementObjectSearcher _serialPortSearcher = new("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%)'");
        public static MSerialDevice[] SerialDevices()
        {
            var ps = _serialPortSearcher.Get();
            if (ps == null || ps.Count <= 0)
            {
                return default;
            }

            var sds = new MSerialDevice[ps.Count];
            ps.For<ManagementObject>((p, i) =>
            {
                sds[i] = new(p["Caption"])
                {
                    DeviceID = p["DeviceID"].ToString2(),
                    Description = p["Description"].ToString2(),
                    Manufacturer = p["Manufacturer"].ToString2()
                };
            });

            return sds;
        }

        public static MSerialDevice[] SerialPorts(string value, MSerialDeviceFilter filter, bool partialMatch = true)
        {
            var sds = SerialDevices();
            if (sds == null || sds.Length <= 0)
                return sds;

            IEnumerable<MSerialDevice> rs;
            switch (filter)
            {
                case MSerialDeviceFilter.Manufacturer:
                    rs = sds.Where(s => partialMatch ? s.Manufacturer.Contains(value) : s.Manufacturer == value);
                    break;
                case MSerialDeviceFilter.DeviceName:
                default:
                    rs = sds.Where(s => partialMatch ? s.Name.Contains(value) : s.Name == value);
                    break;
            }

            return rs.ToArray();
        }
    }
}
