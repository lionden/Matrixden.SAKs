using Matrixden.Utils.Extensions;
using Matrixden.Utils.PinYin.Logging;
using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.PinYin
{
    public static class Extension
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

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
                    log.ErrorException("Failed to get value[{0}]'s PinYin string.", ex, ori);
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
    }
}
