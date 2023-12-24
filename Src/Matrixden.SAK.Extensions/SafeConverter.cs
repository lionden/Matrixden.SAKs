using System;
using System.ComponentModel;
using System.Globalization;

namespace Matrixden.Utils.Extensions
{
    /// <summary>
    /// Safe converter without <c>System.ArgumentNullException</c>.
    /// </summary>
    public static class SafeConverter
    {
        private static readonly ILog _logger = LogProvider.GetCurrentClassLogger();

        #region -- String --

        /// <summary>
        /// Safe Convert a string value to an Int16 value.
        /// </summary>
        /// <param name="val">The string to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int16 ToInt16(this string val, Int16 defaultVal = default(Int16))
        {
            Int16 r = 0;
            if (Int16.TryParse(val, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert a string value to an Int32 value.
        /// </summary>
        /// <param name="val">The string to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static int ToInt32(this string val, int defaultVal = default(int))
        {
            int r = 0;
            if (int.TryParse(val, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe converts the string representation of a number in a specified base to an equivalent 32-bit signed integer.
        /// </summary>
        /// <param name="val">A string that contains the number to convert.</param>
        /// <param name="defaultVal">Default value when exception occured.</param>
        /// <param name="fromBase">The base of the number in value, which must be 2, 8, 10, or 16.</param>
        /// <returns>A 32-bit signed integer that is equivalent to the number in value, or 0 (zero) if value is null, or default value when exception occured..</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ToInt32(this string val, int defaultVal, int fromBase)
        {
            if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
            {
                throw new ArgumentException($"{fromBase} is invalid base.");
            }

            try
            {
                defaultVal = Convert.ToInt32(val, fromBase);
            }
            catch (FormatException fEx)
            {
                _logger.ErrorException("Failed to convert [{0}] from base [{1}].", fEx, val, fromBase);
            }
            catch (OverflowException ofEx)
            {
                _logger.ErrorException($"The value [{val}] is over flow for convert from base [{fromBase}].", ofEx);
            }

            return defaultVal;
        }

        /// <summary>
        /// Safe Convert a string value to an Int64 value.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="style"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static Int64 ToInt64(this string @this, NumberStyles style, Int64 defaultVal = default(Int64))
        {
            Int64 r = 0;
            if (Int64.TryParse(@this, style, null, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert a string value to an Int64 value.
        /// </summary>
        /// <param name="val">The string to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int64 ToInt64(this string val, Int64 defaultVal = default(Int64))
        {
            return val.ToInt64(NumberStyles.Integer, defaultVal);
        }

        /// <summary>
        /// Safe Convert a string value to a Double value.
        /// </summary>
        /// <param name="val">The string to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static double ToDouble(this string val, double defaultVal = default(double))
        {
            double r = 0;
            if (double.TryParse(val, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe Convert a string value to a Decimal value.
        /// </summary>
        /// <param name="val">The string to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string val, decimal defaultVal = default(decimal))
        {
            decimal r = 0;
            if (decimal.TryParse(val, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T ToT<T>(this string val, T defaultVal = default(T))
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                    return (T)converter.ConvertFrom(val);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Failed to convert [{0}] to type [{1}].", ex, val, typeof(T).AssemblyQualifiedName);
            }

            return defaultVal;
        }

        /// <summary>
        /// Safe convert a string value to datetime.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string val) => val.ToDateTime(default(DateTime));

        /// <summary>
        /// Safe convert a string value to datetime.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string val, DateTime defaultVal)
        {
            DateTime r = new DateTime();
            if (DateTime.TryParse(val, out r))
                return r;
            else
                return defaultVal;
        }

        /// <summary>
        /// Safe convert a string value to boolean. True if any in "true", "t", "1", otherwise is false, ignore case.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string @this)
        {
            if (@this.IsNullOrEmptyOrWhiteSpace())
                return false;

            return @this.ToUpper().IsEqualWithSpecificValue("TRUE", "T", "1");
        }

        #endregion

        #region -- Object --

        /// <summary>
        /// Safe Convert an object value to an Int16 value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int16 ToInt16(this object val, Int16 defaultVal = default(Int16))
        {
            if (val == null)
                return defaultVal;

            return val.ToString().ToInt16(defaultVal);
        }

        /// <summary>
        /// Safe Convert an object value to an Int32 value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static int ToInt32(this object val, int defaultVal = default(int))
        {
            if (val == null)
                return defaultVal;

            return val.ToString().ToInt32(defaultVal);
        }

        /// <summary>
        /// Safe Convert an object value to an Int64 value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static Int64 ToInt64(this object val, Int64 defaultVal = default(Int64))
        {
            if (val == null)
                return defaultVal;

            return val.ToString().ToInt64(defaultVal);
        }

        /// <summary>
        /// Safe Convert a object value to a Double value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static double ToDouble(this object val, double defaultVal = default(double))
        {
            if (val == null)
                return defaultVal;

            return val.ToString().ToDouble(defaultVal);
        }

        /// <summary>
        /// Safe Convert a object value to a Decimal value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object val, decimal defaultVal = default(decimal))
        {
            if (val == null)
                return defaultVal;

            return val.ToString().ToDecimal(defaultVal);
        }

        /// <summary>
        /// Safe convert an object value to a DateTime value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object val) => val.ToDateTime(default(DateTime));

        /// <summary>
        /// Safe convert a string value to datetime.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object val, DateTime defaultVal) => val.ToString2().ToDateTime(defaultVal);

        /// <summary>
        /// Safe convert an object value to a String value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <param name="defaultVal">The default value should be reutrned if error occured.</param>
        /// <returns></returns>
        public static string ToString2(this object val, string defaultVal = null)
        {
            if (val == null)
                return defaultVal;

            return val.ToString();
        }

        /// <summary>
        /// Safe convert an object value to a bool value.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object @this) => @this.ToString2().ToBoolean();

        #endregion
    }
}
