using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public class TimeUtil
    {
        /// <summary>
        /// 获取Unix TimeStamp.
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
