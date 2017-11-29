/**
 * 工具类
 */

namespace Clouds.XunmallPos.Utils
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Text;
    using System.Windows.Input;

    public class Util
    {
        private static readonly Random RandomObject = new Random((int)DateTime.Now.ToBinary());
        public static bool AccessPublicInternet = true;

        /// <summary>
        /// Gets a value indicating whether a random result
        /// </summary>
        public static bool RandomBool()
        {
            return RandomInteger(0, 2) == 0;
        }

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
            catch (Exception)
            {
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
                LogHelper.WriteError(typeof(LogHelper).GetType().ToString(), ex.TargetSite.Name, ex.Message);
            }

            return publicIp;
        }

        /// <summary>
        /// 获取局域网本地IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            string localIp = "127.0.0.1";

            try
            {
                IPAddress[] ipAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList;   //获得当前IP地址
                localIp = ipAddr.LastOrDefault().ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(typeof(LogHelper).GetType().ToString(), ex.TargetSite.Name, ex.Message);
            }

            return localIp;
        }

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
                log.ErrorFormat("Error occurred during get MAC address. For more: {0}.", ex);
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
            catch
            {
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
            catch
            {
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
            catch
            {
                _cpuID = "unknow";
            }
            finally
            {
                moc = null;
            }

            return _cpuID;
        }

        /// <summary>
        /// Get current time range,
        /// </summary>
        /// <returns>早上, 上午, 下午或晚上.</returns>
        public static string GetCurrentTimeRange()
        {
            int curHour = DateTime.Now.Hour;
            if (curHour < 9)
            {
                return "";
            }
            else if (curHour < 12)
            {
                return "";
            }
            else if (curHour < 18)
            {
                return "";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get a timestamp from server.
        /// </summary>
        /// <returns>Total milliseconds from 1970年1月1日0点0时.</returns>
        public static string GetCurrentTimestampFromServer()
        {
            var u = Constants_RequestUri.ApiSiteAddr + "Open/ServerTimestamp";
            try
            {
                WebRequest wr = WebRequest.Create(u);
                using (Stream stream = wr.GetResponse().GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.Default))
                    {
                        return streamReader.ReadToEnd();  //网站所有数据
                    }
                }
            }
            catch (Exception ex)
            {
                log.FatalFormat("Failed to get timestamp from server. For detail: {0}", ex.Message);
                Mail.SendAPIErrorAsync(u, ex.Message);

                return GetCurrentTimestamp().ToString();
            }
        }

        public static long GetCurrentTimestamp()
        {
            return (long)Math.Round((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
        }

        /// <summary>
        /// 获取Unix TimeStamp.
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp()
        {
            return (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
        }

        public static long GetCurrentTicks()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Get random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min">The minimum number inclusive</param>
        /// <param name="max">The maximum number exclusive</param>
        /// <returns>A random integer between min and max</returns>
        public static int RandomInteger(int min, int max)
        {
            int val = RandomObject.Next(min, max);
            return val;
        }

        public static string RandomPhoneNumber()
        {
            return string.Format("{0}{1}{2}{3}", 1, RandomInteger(10, 99), RandomInteger(1000, 9999), RandomInteger(1000, 9999));
        }

        /// <summary>
        /// Get random integer between 0 and max
        /// </summary>
        /// <param name="max">The maximum integer to return.</param>
        /// <returns>A random integer between 0 and max.</returns>
        public static int RandomInteger(int max)
        {
            return RandomInteger(0, max);
        }

        /// <summary>
        /// Get random integer number.
        /// </summary>
        /// <returns></returns>
        public static int RandomInteger()
        {
            return RandomInteger(Int32.MaxValue);
        }

        /// <summary>
        /// Get a random string
        /// </summary>
        /// <returns>A random string</returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", string.Empty);
            return path;
        }

        /// <summary>
        /// 成指定数的汉字
        /// </summary>
        /// <param name="charLen"></param>
        /// <returns></returns>
        public static string RandomName(int charLen)
        {
            int area, code;//汉字由区位和码位组成(都为0-94,其中区位16-55为一级汉字区,56-87为二级汉字区,1-9为特殊字符区)
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < charLen; i++)
            {
                area = rand.Next(16, 88);
                if (area == 55)//第55区只有89个字符
                {
                    code = rand.Next(1, 90);
                }
                else
                {
                    code = rand.Next(1, 94);
                }

                sb.Append(Encoding.GetEncoding("GB2312").GetString(new byte[] { Convert.ToByte(area + 160), Convert.ToByte(code + 160) }));
            }

            return sb.ToString();
        }

        public static string GetUUID()
        {
            return GetUUID("N");
        }

        /// <summary>
        /// Initializes a new System.Guid value.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of this Guid.
        /// The format parameter can be "N", "D", "B", "P", or "X".</param>
        /// <returns>Return Upper case string when <c>format</c> is Upper case, else is lower.</returns>
        public static string GetUUID(string format)
        {
            if (format.IsNullOrEmptyOrWhiteSpace() || !format.ToUpper().IsEqualWithSpecificValue("N", "D", "B", "P", "X"))
                format = "N";

            var g = Guid.NewGuid().ToString(format);
            return char.IsUpper(format, 0) ? g.ToUpper() : g;
        }

        /// <summary>
        /// 校验身份号是否正确,
        /// </summary>
        /// <param name="idCardNoStr">身份证号码.</param>
        /// <returns></returns>
        public static bool IsValidateIdCardNo(string idCardNoStr)
        {
            if (idCardNoStr == null)
            {
                return false;
            }

            if (idCardNoStr.Length == 18)
            {
                return IsValidateIdCardNo_18(idCardNoStr);
            }
            else if (idCardNoStr.Length == 15)
            {
                return IsValidateIdCardNo_15(idCardNoStr);
            }

            return false;
        }

        /// <summary>
        /// 校验身份号是否正确, 仅适用于18位身份证号.
        /// </summary>
        /// <param name="idCardNoStr">身份证号码.</param>
        /// <returns></returns>
        public static bool IsValidateIdCardNo_18(string idCardNoStr)
        {
            if (idCardNoStr.Length != 18)
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(idCardNoStr.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idCardNoStr.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;   //数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idCardNoStr.Remove(2)) == -1)
            {
                return false;   //省份验证
            }

            string birth = idCardNoStr.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;   //生日验证
            }

            string[] parityBitArr = ("1,0,X,9,8,7,6,5,4,3,2").Split(',');
            int[] weightingFactorStr = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            char[] idCardNoArr = idCardNoStr.ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(idCardNoArr[i].ToString()) * weightingFactorStr[i];
            }

            int y;
            Math.DivRem(sum, 11, out y);
            if (parityBitArr[y] != idCardNoStr.Substring(17, 1).ToUpper())
            {
                return false;   //校验码验证
            }

            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 校验身份号是否正确, 仅适用于15位身份证号.
        /// </summary>
        /// <param name="idCardNoStr">身份证号码.</param>
        /// <returns></returns>
        public static bool IsValidateIdCardNo_15(string idCardNoStr)
        {
            if (idCardNoStr.Length != 15)
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(idCardNoStr, out n) == false || n < Math.Pow(10, 14))
            {
                return false;   //数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idCardNoStr.Remove(2)) == -1)
            {
                return false;   //省份验证
            }

            string birth = idCardNoStr.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;   //生日验证
            }

            return true;    //符合15位身份证标准
        }
    }
}
