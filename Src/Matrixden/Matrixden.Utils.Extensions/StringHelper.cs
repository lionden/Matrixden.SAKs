﻿using Matrixden.Utils.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Matrixden.Utils.Extensions
{
    public static class StringHelper
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 匹配汉字
        /// </summary>
        const string REGULAR_EXPRESSION_CHINESE_CHARS = @"[\u4e00-\u9fa5]";

        /// <summary>
        /// 检查字符串中是否包含非法字符。
        /// </summary>
        /// <param name="input"></param>
        /// <param name="InvaildString"></param>
        /// <returns></returns>
        public static bool CheckInvaildCharacter(string input, string[] InvaildString)
        {
            throw new NotImplementedException();
            //TODO:
        }

        /// <summary>
        /// 检查字符串在字符类型和长度上是否满足要求
        /// </summary>
        /// <param name="strText">输入的字符串</param>
        /// <param name="strExpression">正则表达式字符串</param>
        /// <returns><c>true</c>If pass. Else<c>false.</c></returns>
        public static bool CheckVaildInput(string strText, string strExpression)
        {
            if (string.IsNullOrWhiteSpace(strText))
            {
                return false;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(strExpression))
                {
                    Regex regex = new Regex(strExpression, RegexOptions.IgnoreCase);
                    MatchCollection matchSet = regex.Matches(strText);
                    if (!(matchSet == null || matchSet.Count <= 0))
                    {
                        foreach (Match match in matchSet)
                        {
                            if (match.Value != strText)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测一组字符串中是否有Null或White Space.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(params string[] args)
        {
            if (args == null)
            {
                return false;
            }

            bool flag = false;
            foreach (string arg in args)
            {
                if (IsNullOrEmptyOrWhiteSpace(arg))
                {
                    flag = true;

                    break;
                }
            }

            return flag;
        }

        public static bool TryDecodeFromBase64(string encodedData, out string oriVal)
        {
            return TryDecodeFromBase64(encodedData, out oriVal, Encoding.ASCII);
        }

        public static bool TryDecodeFromBase64(string encodedData, out string oriVal, Encoding encod)
        {
            oriVal = string.Empty;
            if (encodedData.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }

            try
            {
                oriVal = encod.GetString(Convert.FromBase64String(encodedData));
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to decode from Base64 value[{0}].", ex, encodedData);
                return false;
            }
        }

        /// <summary>
        /// Get several string's lexicographic order.
        /// </summary>
        /// <param name="values">An array of string instances.</param>
        /// <returns>Lexicographic order string.</returns>
        public static string GetParamsLexicographicOrderString(params string[] values)
        {
            if (values == null)
            {
                return null;
            }
            Array.Sort(values);

            return string.Join("", values);
        }

        /// <summary>
        /// 将数字字符串高位补0, 以达到预期长度
        /// </summary>
        /// <param name="value">数字串</param>
        /// <param name="stringLength">数字串长度</param>
        /// <returns>固定长度的数字串</returns>
        public static string RepairNumericStringLength(string value, int stringLength = 2)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(value))
            {
                return value;
            }

            string result = value.PadLeft(stringLength, '0');

            return result;
        }

        /// <summary>
        /// 检测多个字符串是否彼此相等,将null默认为不相等
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsEqualWithEachOther(params string[] args)
        {
            if (args == null)
            {
                return false;
            }

            if (args.Length < 2)
            {
                throw new ArgumentException("参数个数至少为2.");
            }

            string specialValue = args[0];

            return IsEqualWithSpecificValue(specialValue, args);
        }

        /// <summary>
        /// 检测一个字符串是否是Null或空或White Space.
        /// </summary>
        /// <param name="value">待检测字符串</param>
        /// <returns>如果是Null或空或White Space, 则返回<c>true</c>, 否则返回<c>fals</c>.</returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string source)
        {
            return string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source);
        }

        public static bool IsNotNullNorEmptyNorWhitespace(this string source)
        {
            return !source.IsNullOrEmptyOrWhiteSpace();
        }

        /// <summary>
        /// Splite a string use upper case character.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <returns>Splited string array.</returns>
        public static string[] SpliteWithUpperCase(this string source)
        {
            if (source == null)
            {
                return null;
            }

            var t = Guid.NewGuid().ToString();
            string t2 = Regex.Replace(source, "(?!^)([A-Z])", t + "$1");

            return t2.Split(t);
        }

        public static string[] Split(this string source, string separator, StringSplitOptions options = StringSplitOptions.None)
        {
            if (source == null)
            {
                return null;
            }

            return source.Split(new string[] { separator }, options);
        }

        /// <summary>
        /// Get a string's byte array.
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static byte[] Bytes(this string sourceStr)
        {
            if (sourceStr.IsNullOrEmptyOrWhiteSpace())
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(sourceStr);
        }

        #region -- Encode / Decode --

        /// <summary>
        /// Encodes string to base64.
        /// </summary>
        /// <param name="toEncode">String to encode.</param>
        /// <returns>Encoded string</returns>
        public static string Base64Value(this string toEncode)
        {
            return toEncode.Base64Value(Encoding.ASCII);
        }

        /// <summary>
        /// Encodes string to base64.
        /// </summary>
        /// <param name="toEncode">String to encode.</param>
        /// <param name="encod">Characters' encoding.</param>
        /// <returns>Encoded string</returns>
        public static string Base64Value(this string toEncode, Encoding encod)
        {
            if (toEncode == null)
            {
                return toEncode;
            }

            return Convert.ToBase64String(encod.GetBytes(toEncode));
        }

        public static string MD5Value(this string toBeEncode)
        {
            if (toBeEncode == null)
            {
                return toBeEncode;
            }

            StringBuilder dest = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(toBeEncode));
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < 16)
                {
                    dest.AppendFormat("0{0}", s[i].ToString("X"));
                }
                else
                {
                    dest.Append(s[i].ToString("X"));
                }
            }

            return dest.ToString();
        }

        /// <summary>
        /// Get string's SHA1 value.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string SHA1Value(this string plainText)
        {
            if (plainText == null)
            {
                return plainText;
            }

            SHA1 hash = SHA1.Create();
            hash.ComputeHash(new ASCIIEncoding().GetBytes(plainText));
            return BitConverter.ToString(hash.Hash).Replace("-", string.Empty).ToUpper();
        }

        /// <summary>
        /// Decodes the string from base64
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <returns>Decoded string</returns>
        public static string DecodeFromBase64(this string encodedData)
        {
            return encodedData.DecodeFromBase64(Encoding.ASCII);
        }

        /// <summary>
        /// Decodes the string from base64
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="encod">Characters' encoding.</param>
        /// <returns>Decoded string</returns>
        public static string DecodeFromBase64(this string encodedData, Encoding encod)
        {
            string oriVal;
            TryDecodeFromBase64(encodedData, out oriVal, encod);

            return oriVal;
        }

        #endregion

        /// <summary>
        /// Removes leading character(s) in an array from the current System.String object.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len">Count of character(s)</param>
        /// <returns></returns>
        public static string TrimStart(this string val, int len)
        {
            if (val.IsNullOrEmptyOrWhiteSpace() || len <= 0 || val.Length <= len)
                return val;

            return val.Substring(1);
        }

        /// <summary>
        /// Removes trailing character(s) in an array from the current System.String object.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len">Count of character(s)</param>
        /// <returns></returns>
        public static string TrimEnd(this string val, int len)
        {
            if (val.IsNullOrEmptyOrWhiteSpace() || len <= 0 || val.Length <= len)
                return val;

            return val.Substring(0, val.Length - len);
        }

        /// <summary>
        /// 清除字符串前后的空格
        /// </summary>
        /// <param name="val">需要清理的字符串</param>
        /// <returns>前后不带空格的字符串, 或者null值.</returns>
        public static string CleanUp(this string val)
        {
            if (val != null)
            {
                val = val.Trim();
                if (val.Length == 0)
                {
                    val = null;
                }
            }

            return val;
        }

        /// <summary>
        /// Remove invalid chars from string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string CleanUpInvalidChars(this string source, params char[] chars)
        {
            if (source.IsNullOrEmptyOrWhiteSpace() || chars == null || chars.Count() <= 0)
                return source;

            return String.Join(string.Empty, source.Split(chars));
        }

        /// <summary>
        /// Remove invalid substring from string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="substr"></param>
        /// <returns></returns>
        public static string CleanUpInvalidSubstring(this string source, params string[] substr)
        {
            if (source.IsNullOrEmptyOrWhiteSpace() || substr == null || substr.Count() <= 0)
                return source;

            return String.Join(string.Empty, source.Split(substr, StringSplitOptions.None));
        }

        /// <summary>
        /// 检测多个字符串是否与特定的字符串相等
        /// </summary>
        /// <param name="specificValue"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsEqualWithSpecificValue(this string specificValue, params string[] args)
        {
            if (specificValue == null || args == null)
            {
                return false;
            }

            return args.Any(a => specificValue.Equals(a));
        }

        /// <summary>
        /// 检测多个字符串是否与特定的字符串相等
        /// </summary>
        /// <param name="specificValue"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsFuzzyMatchSpecificValue(this string specificValue, params string[] args)
        {
            if (specificValue == null || args == null)
            {
                return false;
            }

            return args.Any(a => a != null && a.Contains(specificValue));
        }

        /// <summary>
        /// 移除字符串中的汉字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CleanChineseCharacter(this string source)
        {
            if (source == null)
            {
                return source;
            }

            Regex regex = new Regex(REGULAR_EXPRESSION_CHINESE_CHARS);

            return regex.Replace(source, string.Empty);
        }

        public static string GetChineseCharacter(this string source)
        {
            if (source == null)
            {
                return source;
            }

            // @"[\u4e00-\u9fa5]";
            Regex regex = new Regex(string.Format("{0}+", REGULAR_EXPRESSION_CHINESE_CHARS));

            if (regex.IsMatch(source))
            {
                return regex.Match(source).Value;
            }

            return "";
        }

        public static bool ValidateWhetherContainsChineseChar(this string source)
        {
            if (source == null)
            {
                return false;
            }

            Regex regex = new Regex(REGULAR_EXPRESSION_CHINESE_CHARS);

            return regex.IsMatch(source);
        }

        public static bool IsStartWithLetter(this string source)
        {
            if (source == null)
            {
                return false;
            }

            Regex rg = new Regex("^[a-zA-Z]");

            return rg.IsMatch(source);
        }

        public static bool IsContainLetter(this string source)
        {
            if (source == null)
            {
                return false;
            }

            Regex rg = new Regex("[a-zA-Z]");

            return rg.IsMatch(source);
        }

        /// <summary>
        /// 判断字符串是否是GUID格式.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGUID(this string source)
        {
            if (source == null)
            {
                return false;
            }

            Guid result = new Guid();

            return Guid.TryParse(source, out result);
        }

        /// <summary>
        /// 判断字符串是否是JSON格式
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsJson(this string @this)
        {
            if (@this.IsNullOrEmptyOrWhiteSpace())
                return false;

            @this = @this.CleanUp();
            if ((@this.StartsWith("{") && @this.EndsWith("}")) || //For object
                (@this.StartsWith("[") && @this.EndsWith("]"))) //For array
                return true;

            return false;
        }

        public static Guid ToGuid(this string source)
        {
            Guid result;
            if (!Guid.TryParse(source, out result))
                result = default(Guid);
            return result;
        }

        public static bool MatchAll(this string source, params string[] pattern)
        {
            if (source == null)
                return false;

            if (pattern == null)
                return true;

            return !pattern.Any(p => !Regex.IsMatch(source, p));
        }

        public static bool MatchAny(this string source, params string[] pattern)
        {
            if (source == null)
                return false;

            if (pattern == null)
                return true;

            return pattern.Any(p => Regex.IsMatch(source, p));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ori"></param>
        /// <param name="defaultVal"></param>
        /// <param name="keyVal"></param>
        /// <returns></returns>
        public static string CaseWhen(this string ori, string defaultVal, params Tuple<string, string>[] keyVal)
        {
            if (ori.IsNullOrEmptyOrWhiteSpace() || keyVal.Length <= 0)
                return defaultVal;

            return (keyVal.FirstOrDefault(f => f.Item1.Equals(ori)) ?? new Tuple<string, string>("", defaultVal)).Item2;
        }

        /// <summary>
        /// 安全截取字符串左边N个字符子串
        /// </summary>
        /// <param name="this"></param>
        /// <param name="len">截取长度, 正数为左对齐(右边补指定字符); 负数为右对齐(左边补指定字符).</param>
        /// <param name="paddingChar">长度不足时的补位字符</param>
        /// <returns></returns>
        public static string Left(this string @this, int len, char paddingChar)
        {
            if (len == 0)
                return string.Empty;

            if (@this.IsNullOrEmptyOrWhiteSpace())
                return string.Empty.PadLeft(len, paddingChar);

            return (len > 0
                    ? @this.PadRight(len, paddingChar)
                        : @this.PadLeft(-len, paddingChar))
                    .Substring(0, Math.Abs(len));
        }

        /// <summary>
        /// 安全截取字符串左边N个字符子串
        /// </summary>
        /// <param name="this"></param>
        /// <param name="len">截取长度</param>
        /// <returns></returns>
        public static string Left(this string @this, int len)
        {
            if (len <= 0 || @this.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;

            return @this.Substring(0, len);
        }

        /// <summary>
        /// 安全截取字符串右边N个字符子串
        /// </summary>
        /// <param name="this"></param>
        /// <param name="len">截取长度, 正数为左对齐(右边补指定字符); 负数为右对齐(左边补指定字符).</param>
        /// <param name="paddingChar">长度不足时的补位字符</param>
        /// <returns></returns>
        public static string Right(this string @this, int len, char paddingChar)
        {
            if (len == 0)
                return string.Empty;

            if (@this.IsNullOrEmptyOrWhiteSpace())
                return string.Empty.PadLeft(len, paddingChar);

            var s = len > 0
                    ? @this.PadRight(len, paddingChar)
                        : @this.PadLeft(-len, paddingChar);

            return s.Substring(s.Length - Math.Abs(len));
        }

        /// <summary>
        /// 安全截取字符串右边N个字符子串
        /// </summary>
        /// <param name="this"></param>
        /// <param name="len">截取长度</param>
        /// <returns></returns>
        public static string Right(this string @this, int len)
        {
            if (len <= 0 || @this.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;

            return @this.Substring(@this.Length - len);
        }
    }
}
