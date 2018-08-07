﻿using Matrixden.Utils.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Extensions
{
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
        public static DateTime ToDateTime(this string val)
        {
            DateTime r = new DateTime();
            DateTime.TryParse(val, out r);

            return r;
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
        /// Safe Convert an object value to a DateTime value.
        /// </summary>
        /// <param name="val">The object to be converted.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object val)
        {
            if (val == null)
                return default(DateTime);

            return val.ToString().ToDateTime();
        }

        /// <summary>
        /// Safe Convert a object value to a String value.
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

        #endregion
    }
}
