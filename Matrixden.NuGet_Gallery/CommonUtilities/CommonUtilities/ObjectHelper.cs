using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Matrixden.CommonUtilities
{
    public static class ObjectHelper
    {
        public static bool IsNull(this object @this)
        {
            return @this == null;
        }

        public static bool IsGUID(this string @this)
        {
            if (@this.IsNull())
                return false;

            return Guid.TryParse(@this.ToString(), out Guid result);
        }

        #region -- Safe Convert --

        /// <summary>
        /// Safe Convert an object value to an Int16 value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int16 ToInt16(this object @this, Int16 defaultVal = default(Int16))
        {
            if (@this.IsNull())
                return defaultVal;

            if (Int16.TryParse(@this.ToString(), out Int16 r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to an Int32 value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static int ToInt32(this object @this, int defaultVal = default(int))
        {
            if (@this.IsNull())
                return defaultVal;

            if (int.TryParse(@this.ToString(), out int r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to an Int64 value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int64 ToInt64(this object @this, Int64 defaultVal = default(Int64))
        {
            if (@this.IsNull())
                return defaultVal;

            if (Int64.TryParse(@this.ToString(), out Int64 r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to a Decimal value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object @this, decimal defaultVal = default(decimal))
        {
            if (@this.IsNull())
                return defaultVal;

            if (decimal.TryParse(@this.ToString(), out decimal r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to a DateTime value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object @this, DateTime defaultVal = default(DateTime))
        {
            if (@this.IsNull())
                return defaultVal;

            if (DateTime.TryParse(@this.ToString(), out DateTime r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to a Guid value.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Guid ToGuid(this object @this, Guid defaultVal = default(Guid))
        {
            if (@this.IsNull())
                return defaultVal;

            if (Guid.TryParse(@this.ToString(), out Guid result))
                return result;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert an object value to a string value.
        /// Return string.Empty if error occured.
        /// </summary>
        /// <param name="this">The object to be converted.</param>
        /// <returns></returns>
        public static string ToString2(this object @this)
        {
            if (@this.IsNull())
                return string.Empty;

            return @this.ToString();
        }

        #endregion
    }
}