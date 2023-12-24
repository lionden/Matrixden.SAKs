using Matrixden.Utils.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Matrixden.SwissArmyKnives
{
    public partial class Util
    {
        /// <summary>
        /// 获取局域网IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPv4() => GetLocalIPv4(NetworkInterfaceType.Ethernet | NetworkInterfaceType.GigabitEthernet);

        /// <summary>
        /// 获取无线局域网IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPv4_WiFi() => GetLocalIPv4(NetworkInterfaceType.Wireless80211);

        /// <summary>
        /// 获取局域网IP
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetLocalIPv4(NetworkInterfaceType type) => GetLocalIPv4List(type).FirstOrDefault(f => !f.IsIPv6LinkLocal).ToString();

        /// <summary>
        /// 获取局域网IP列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIPv4List(NetworkInterfaceType type)
        {
            var ips = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => (n.NetworkInterfaceType & type) == n.NetworkInterfaceType)
                .SelectMany(s => s.GetIPProperties()?.UnicastAddresses)
                .Select(s2 => s2.Address)
                .Where(w => w.AddressFamily == AddressFamily.InterNetwork);

            if (ips == null)
            {
                log.Warn($"Failed to get local IPv4 list.");
            }

            return ips.ToList();
        }

        /// <summary>
        /// 获取局域网网关
        /// </summary>
        /// <returns></returns>
        public static string GetGateway() => GetGatewayList(NetworkInterfaceType.Ethernet | NetworkInterfaceType.GigabitEthernet).FirstOrDefault(f => !f.IsIPv6LinkLocal).ToString();

        /// <summary>
        /// 获取局域网网关
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<IPAddress> GetGatewayList(NetworkInterfaceType type)
        {
            var gwIPs = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => (n.NetworkInterfaceType & type) == n.NetworkInterfaceType)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null);

            if (gwIPs == null)
            {
                log.Warn($"Failed to get gateway.");
            }

            return gwIPs.ToList();
        }
    }
}
