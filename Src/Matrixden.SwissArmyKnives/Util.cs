/**
 * 工具类
 */

namespace Matrixden.Utils
{
    using Matrixden.Utils.Logging;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Windows.Input;

    public partial class Util
    {
        public static bool AccessPublicInternet = true;
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 价格输入过滤器, 仅接受数字, 小数点(.), 删除键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void KeyPressFilterForCurrencyTextBoxes(object sender, KeyEventArgs args)
        {
            bool isDigit = char.IsDigit((char)KeyInterop.VirtualKeyFromKey(args.Key));
            bool isDecimal = args.Key == Key.OemPeriod || args.Key == Key.Decimal;
            bool isDelete = args.Key == Key.Back || args.Key == Key.Delete;

            if (!isDigit && !isDecimal && !isDelete)
            {
                args.Handled = true;
            }
        }

        /// <summary>
        /// 数字输入过滤器, 仅接受数字, 删除键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void KeyPressFilterForNumericTextBoxes(object sender, KeyEventArgs args)
        {
            if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(args.Key)) && args.Key != Key.Back)
            {
                args.Handled = true;
            }
        }

        /// <summary>
        /// 密码输入过滤器, 仅接受字母, 数字, 下划线(_)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void KeyPressFilterForPasswordTextBoxes(object sender, KeyEventArgs args)
        {
            if (!char.IsLetterOrDigit((char)args.Key) && (char)args.Key != '_')
            {
                args.Handled = true;
            }
        }

        /// <summary>
        /// 是否连接到外网
        /// </summary>
        /// <returns></returns>
        public static bool IsPublicNetworkUsable()
        {
            try
            {
                Ping p = new Ping();
                PingReply pr = p.Send("www.baidu.com", 5000);

                AccessPublicInternet = true;
            }
            catch (Exception ex)
            {
                log.ErrorException("Error occured during ping public server.", ex);
                AccessPublicInternet = false;
            }

            return AccessPublicInternet;
        }

        /// <summary>
        /// 获取公网IP
        /// </summary>
        /// <returns></returns>
        public static string GetPublicIp()
        {
            string publicIp = "127.0.0.1";

            try
            {
                WebRequest wr = WebRequest.Create(Constants.PublicIpVendor_Url);
                using (Stream stream = wr.GetResponse().GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.Default))
                    {
                        string all = streamReader.ReadToEnd();  //网站所有数据

                        int start = all.IndexOf(Constants.PublicIpVendor_StartIdentity) + Constants.PublicIpVendor_StartIdentity_Length;
                        int end = all.IndexOf(Constants.PublicIpVendor_EndIdentity, start);

                        publicIp = all.Substring(start, end - start);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get public ip.", ex);
            }

            return publicIp;
        }

        /// <summary>
        /// 获取局域网本地IP
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use \"Matrixden.SwissArmyKnives.Util.GetLocalIPv4\" instead.")]
        public static string GetLocalIp() => SwissArmyKnives.Util.GetLocalIPv4();

        /// <summary>
        /// 获取机器MAC地址.
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }

                moc = null;
                mc = null;
            }
            catch (Exception ex)
            {
                log.ErrorException("Error occurred during get MAC address.", ex);
            }

            return macs.FirstOrDefault();
        }

        /// <summary>
        /// 获取机器的硬盘序列号.
        /// </summary>
        /// <returns></returns>
        public static string GetDiskSN()
        {
            string _diskSN = string.Empty;
            ManagementObjectCollection moc;
            try
            {
                moc = new ManagementClass("Win32_DiskDrive").GetInstances();
                foreach (var mo in moc)
                {
                    if (mo["SerialNumber"] == null)
                        continue;

                    _diskSN += mo["SerialNumber"].ToString();
                }

                _diskSN += '-';
                moc = new ManagementClass("Win32_PhysicalMedia").GetInstances();
                foreach (var mo in moc)
                {
                    if (mo["SerialNumber"] == null)
                        continue;

                    _diskSN += mo["SerialNumber"].ToString();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get disk SN.", ex);
                _diskSN = "unknow";
            }
            finally
            {
                moc = null;
            }

            return _diskSN;
        }

        /// <summary>
        /// 获取主板序列号
        /// </summary>
        /// <returns></returns>
        public static string GetMotherboardSN()
        {
            string _mbSN = string.Empty;
            ManagementObjectSearcher mos;
            try
            {
                mos = new ManagementObjectSearcher("select * from Win32_baseboard");
                foreach (var mo in mos.Get())
                {
                    if (mo["SerialNumber"] == null)
                        continue;

                    _mbSN = mo["SerialNumber"].ToString();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get motherboard SN.", ex);
                _mbSN = "unknow";
            }
            finally
            {
                mos = null;
            }

            return _mbSN;
        }

        /// <summary>
        /// 获取CPU序列号.
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            string _cpuID = string.Empty;
            ManagementObjectCollection moc;
            try
            {
                moc = new ManagementClass("Win32_Processor").GetInstances();
                foreach (var mo in moc)
                {
                    if (mo["ProcessorId"] == null)
                        continue;

                    _cpuID = mo["ProcessorId"].ToString();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get CPU ID.", ex);
                _cpuID = "unknow";
            }
            finally
            {
                moc = null;
            }

            return _cpuID;
        }
    }
}
