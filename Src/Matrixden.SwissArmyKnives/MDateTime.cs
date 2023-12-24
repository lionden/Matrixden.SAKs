using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public class MDateTime
    {
        /// <summary>
        /// 获取Unix TimeStamp.
        /// </summary>
        /// <returns></returns>
        public static long UnixTimestamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
