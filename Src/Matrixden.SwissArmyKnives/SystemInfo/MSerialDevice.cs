using Matrixden.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives.SystemInfo
{
    public class MSerialDevice : SerialPort
    {
        /// <summary>
        /// This is NOT port name like 'COM1', this is the name show in Windows device manager. Maybe it should be named 'device name' more suitable. ;-)
        /// </summary>
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public int Port { get; private set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }

        private MSerialDevice()
        {

        }

        private const string portNamePattern = "\\(COM\\d+";
        public MSerialDevice(string name) : this()
        {
            if (name.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentNullException("name");

            var match = Regex.Match(name, portNamePattern);
            if (!match.Success)
                throw new ArgumentException("Name is illegal");

            PortName = match.Value.TrimStart(1);
            Port = PortName.TrimStart(3).ToInt32();
            Name = name;
        }

        public MSerialDevice(object nameObj) : this(nameObj.ToString2()) { }

        public MSerialDevice(int port) : this()
        {
            Port = port;
            PortName = $"COM{Port}";
        }
    }

    public enum MSerialDeviceFilter
    {
        DeviceName,
        Manufacturer,
    }
}
