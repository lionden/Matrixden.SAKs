using Matrixden.Utils.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Matrixden.Utils
{
    /// <summary>
    /// 系统信息类
    /// </summary>
    public class Util_SystemInfo
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        private static Util_SystemInfo _instance;
        private static object _obj = new object();
        public static Util_SystemInfo GetInstance()
        {
            if (_instance == null)
            {
                lock (_obj)
                {
                    if (_instance == null)
                        _instance = new Util_SystemInfo();
                }
            }

            return _instance;
        }

        private Util_SystemInfo()
        {
            try
            {
                PublicIP = Util.GetPublicIp();
                LocalIp = Util.GetLocalIp();
                Mac = Util.GetMacAddress();
                DiskSN = Util.GetDiskSN();
                MotherboardSN = Util.GetMotherboardSN();
                CpuID = Util.GetCpuID();
                HostName = Environment.MachineName;
                CurrentUser = Environment.UserName;
                var OSVer = Environment.OSVersion;
                OSVersion = OSVer.VersionString;
                ServicePack = OSVer.ServicePack;
                OSBuild = OSVer.Version.ToString();
                SystemType = OSVer.Platform.ToString();
                WorkingTimeSpan = new TimeSpan(Environment.TickCount);
                ProductVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                ScreenResolution_Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
                ScreenResolution_Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            }
            catch (InvalidOperationException ioEx)
            {
                log.ErrorException("Invalid Operation Exception occurred.", ioEx);
            }
            catch (Exception ex)
            {
                log.ErrorException("Error occurred during get MAC address.", ex);
            }
        }

        /// <summary>
        /// ERP版本号.
        /// </summary>
        public string ProductVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据库版本号.
        /// </summary>
        public string DatabaseVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// 局域网IP.
        /// </summary>
        public string LocalIp
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户端机器名称.
        /// </summary>
        public string HostName
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前登录用户.
        /// </summary>
        public string CurrentUser
        {
            get;
            private set;
        }

        /// <summary>
        /// 系统名称.
        /// </summary>
        public string OSName
        {
            get;
            private set;
        }

        /// <summary>
        /// 系统版本.
        /// </summary>
        public string OSVersion
        {
            get;
            private set;
        }

        public string ServicePack
        {
            get;
            private set;
        }

        public string OSBuild
        {
            get;
            private set;
        }

        /// <summary>
        /// 平台类型.
        /// </summary>
        public string SystemType
        {
            get;
            private set;
        }

        public string FrameworkVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// 公网IP.
        /// </summary>
        public string PublicIP
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户机MAC地址.
        /// </summary>
        public string Mac
        {
            get;
            private set;
        }

        /// <summary>
        /// Get disk's serial number.
        /// </summary>
        public string DiskSN
        {
            get;
            private set;
        }

        public string MotherboardSN
        {
            get;
            private set;
        }

        public string CpuID
        {
            get;
            private set;
        }

        private DateTime _systemBootTime;
        /// <summary>
        /// 系统开机时间.
        /// </summary>
        public DateTime SystemBootTime
        {
            get
            {
                return _systemBootTime;
            }
            private set
            {
                _systemBootTime = value;
            }
        }

        /// <summary>
        /// 已开机时间.
        /// </summary>
        public TimeSpan WorkingTimeSpan
        {
            get;
            private set;
        }

        /// <summary>
        /// 屏幕分辨率, 宽.
        /// </summary>
        public int ScreenResolution_Width
        {
            get;
            private set;
        }

        /// <summary>
        /// 屏幕分辨率, 高.
        /// </summary>
        public int ScreenResolution_Height
        {
            get;
            private set;
        }

        public string ProductWindow_Width
        {
            get;
            private set;
        }

        public string ProductWindow_Height
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户端网络类型.
        /// </summary>
        public string Network
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserCity
        {
            get;
            private set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            var list = CommonClass.ToParamList<Util_SystemInfo>(this);
            foreach (var l in list)
            {
                sb.Append(string.Format("{0}:\t\t{1}.\r\n", l.Key.PadRight(23), l.Value));
            }

            return sb.ToString();
        }

        public string ToHtmlTable()
        {
            StringBuilder sb = new StringBuilder("<br /><table><thead><tr><th colspan=\"2\">系统信息 System Info</th></tr></thead><tbody>");
            var list = CommonClass.ToParamList<Util_SystemInfo>(this);
            foreach (var l in list)
            {
                sb.Append(string.Format("<tr><th>{0}</th><td>{1}</td></tr>", l.Key, l.Value));
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }
    }
}
