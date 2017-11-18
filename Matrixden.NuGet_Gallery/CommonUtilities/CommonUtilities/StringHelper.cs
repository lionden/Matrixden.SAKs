/*
 * StringHelper 是包含String处理方法的集合。
 */
namespace Clouds.XunmallPos.Utils
{
    using Clouds.XunmallPos.ErpBS.Resources;
    using log4net;
    using Microsoft.International.Converters.PinYinConverter;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class StringHelper
    {
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
            oriVal = string.Empty;
            if (encodedData.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }

            try
            {
                oriVal = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(encodedData));
                return true;
            }
            catch
            {
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
                //throw new ArgumentNullException("values");
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

            string t = Util.GetUUID().ToUpper();
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
            if (toEncode == null)
            {
                return toEncode;
            }

            return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode));
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

        public static string RSAValue(this string tobeEncrypt, string publicKey)
        {
            if (publicKey.IsNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentNullException("Public key MUST NOT be null or empty, please check it.");
            }

            RSACryptoService rsa = new RSACryptoService(null, publicKey);
            return rsa.Encrypt(tobeEncrypt);
        }

        /// <summary>
        /// Decodes the string from base64
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <returns>Decoded string</returns>
        public static string DecodeFromBase64(this string encodedData)
        {
            string oriVal;
            TryDecodeFromBase64(encodedData, out oriVal);

            return oriVal;
        }

        public static bool TryDecrypt_RSA(string cipherText, string privateKey, out string text)
        {
            text = string.Empty;

            if (privateKey.IsNullOrEmptyOrWhiteSpace())
                return false;
            RSACryptoService rsa = new RSACryptoService(privateKey);
            try
            {
                text = rsa.Decrypt(cipherText);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public static string Decrypt_RSA(this string cipherText, string privateKey)
        {
            string text;
            if (!TryDecrypt_RSA(cipherText, privateKey, out text))
            {
                log.WarnFormat("Decrypt fail. Cipher Text: '{0}'.", cipherText);
            }

            return text;
        }

        #endregion

        /// <summary>
        /// 清除字符串前后的空格
        /// </summary>
        /// <param name="val">需要清理的字符串</param>
        /// <returns>前后不带空格的字符串, 或者null值.</returns>
        public static string Cleanup(this string val)
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
        /// 移除部分sql关键字符, 防sql注入.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanUpSqlChars(this string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
                return null;

            return String.Join("", value.Split(Constants.DBInvalidChars.ToCharArray()));
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

            Regex regex = new Regex(Constants_RegularExpression.regularExpression_ChineseChar);

            return regex.Replace(source, string.Empty);
        }

        public static string GetChineseCharacter(this string source)
        {
            if (source == null)
            {
                return source;
            }

            // @"[\u4e00-\u9fa5]";
            Regex regex = new Regex(string.Format("{0}+", Constants_RegularExpression.regularExpression_ChineseChar));

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

            Regex regex = new Regex(Constants_RegularExpression.regularExpression_ChineseChar);

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

        public static bool IsGUID(this string source)
        {
            if (source == null)
            {
                return false;
            }

            Guid result = new Guid();

            return Guid.TryParse(source, out result);
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

        #region -- Safe Convert --

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
            catch { }

            return defaultVal;
        }

        public static DateTime ToDateTime(this string val)
        {
            DateTime r = new DateTime();
            DateTime.TryParse(val, out r);

            return r;
        }

        public static string CaseWhen(this string ori, string defaultVal, params Tuple<string, string>[] keyVal)
        {
            if (ori.IsNullOrEmptyOrWhiteSpace() || keyVal.Length <= 0)
                return defaultVal;

            return (keyVal.FirstOrDefault(f => f.Item1.Equals(ori)) ?? new Tuple<string, string>("", defaultVal)).Item2;
        }

        /// <summary>
        /// 获取字符串的拼音串.
        /// </summary>
        /// <param name="ori">需要解析的串</param>
        /// <param name="length">单个字符的拼音串长度. 0, 原始拼音串; -1, 去除声调标识; 其他, 对应长度串, 不足长度左边补空格. </param>
        /// <param name="useUpper">是否已大写形式返回.</param>
        /// <returns>这个字符的拼音。</returns>
        public static string GetPinyin(this string ori, int length, bool useUpper)
        {
            if (ori.IsNullOrEmptyOrWhiteSpace())
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var c in ori)
            {
                try
                {
                    var py = ChineseChar.IsValidChar(c) ? new ChineseChar(c).Pinyins.FirstOrDefault() : c.ToString();
                    switch (length)
                    {
                        case -1:
                            sb.Append(py.Length > 1 ? py.TrimEnd('1', '2', '3', '4') : py);
                            break;
                        case 0:
                            sb.Append(py);
                            break;
                        default:
                            sb.Append(py.PadLeft(length).Substring(0, length));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    continue;
                }
            }

            return useUpper ? sb.ToString().ToUpper() : sb.ToString();
        }

        /// <summary>
        /// 获取字符串的拼音串.
        /// </summary>
        /// <param name="ori">需要解析的串</param>
        /// <returns></returns>
        public static string GetPinyin(this string ori)
        {
            return ori.GetPinyin(0, false);
        }

        /// <summary>
        /// 获取字符串首字母.
        /// </summary>
        /// <param name="ori">需要解析的串</param>
        /// <returns></returns>
        public static string GetFirstLetter(this string ori)
        {
            return ori.GetPinyin(1, true);
        }

        #endregion

        #region -- Extension for bussiness process.

        /// <summary>
        /// 返回接口的完全限定地址. 如: car-gateway/user/login
        /// </summary>            
        /// <param name="uriAddr"></param>
        /// <returns></returns>
        public static string ToFullUri(this string uriAddr, string baseAddr = null)
        {
            if (baseAddr.IsNullOrEmptyOrWhiteSpace())
                baseAddr = Constants_RequestUri.ApiSiteAddr;

            return string.Concat(baseAddr, uriAddr);
        }

        /// <summary>
        /// 获取商品助记码。
        /// 如：营养快线 椰味
        /// 规格名称为椰味，
        /// 商品名称为营养快线，
        /// 则助记码为YYKXYW。
        /// </summary>
        /// <param name="goodsName">商品规格名称。</param>
        /// <param name="prefix">助记码前缀， 商品名称。</param>
        /// <returns></returns>
        public static string GenerateMnemonicCode(this string goodsName, string prefix)
        {
            return string.Concat(prefix.GetFirstLetter() + goodsName.GetFirstLetter());
        }

        #endregion
    }
}
