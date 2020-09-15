namespace Matrixden.Utils.Extensions
{
    using Matrixden.Utils.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Extend methods.
    /// </summary>
    public static class Extension
    {
        private static readonly ILog _LOG = LogProvider.GetCurrentClassLogger();
        /// <summary>
        /// Generate a string from a byte array.
        /// </summary>
        /// <param name="sourceByteArr"></param>
        /// <returns></returns>
        public static string ToString2(this byte[] sourceByteArr)
        {
            if (sourceByteArr == null || sourceByteArr.Length <= 0)
                return null;

            return Encoding.UTF8.GetString(sourceByteArr);
        }

        #region -- DateTime --

        /// <summary>
        /// Using the specified format, Converts the value of the current System.DateTime object to a new System.DateTime object.
        /// </summary>
        /// <param name="ori">The date time to be formated.</param>
        /// <param name="format">A standard or custom date and time format string (see Remarks).</param>
        /// <returns></returns>
        public static DateTime Format(this DateTime ori, string format)
        {
            if (format.IsNullOrEmptyOrWhiteSpace())
                return ori;

            try
            {
                return ori.ToString(format).ToDateTime();
            }
            catch
            {
                return ori;
            }
        }

        #endregion

        #region -- Double --


        #endregion

        #region -- Int32 --

        /// <summary>
        /// 判断特定值是否在给定集合里
        /// </summary>
        /// <param name="this"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IN(this Int32 @this, params Int32[] values)
        {
            if (values == null || values.Length <= 0)
                return false;

            return values.Any(a => @this.Equals(a));
        }

        #endregion

        #region -- Int64 --

        /// <summary>
        /// Convert a unix timestamp to a local DateTime type.
        /// </summary>
        /// <param name="unixTimestamp">Unix timestamp value.</param>
        /// <returns>Local DateTime value.</returns>
        public static DateTime ToDateTime(this Int64 unixTimestamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(unixTimestamp).ToLocalTime();
        }

        /// <summary>
        /// Returns a value indicating whether the specified System.Int64 object occurs
        /// within this value.
        /// </summary>
        /// <param name="intA"></param>
        /// <param name="intB">The value to seek.</param>
        /// <returns></returns>
        public static bool Contains(this Int64 intA, Int64 intB)
        {
            return intA.ToString().Contains(intB.ToString());
        }

        /// <summary>
        /// 判断特定值是否在给定集合里
        /// </summary>
        /// <param name="this"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IN(this Int64 @this, params Int64[] values)
        {
            if (values == null || values.Length <= 0)
                return false;

            return values.Any(a => @this.Equals(a));
        }

        #endregion

        #region -- Enum --

        /// <summary>
        /// 判断特定枚举是否在指定集合里
        /// </summary>
        /// <param name="this">特定枚举</param>
        /// <param name="tar">指定合法集合</param>
        /// <returns></returns>
        public static bool IN(this Enum @this, params Enum[] tar)
        {
            if (tar == null || tar.Length <= 0)
                return false;

            return tar.Any(t => @this.Equals(t));
        }

        #endregion
    }
}
