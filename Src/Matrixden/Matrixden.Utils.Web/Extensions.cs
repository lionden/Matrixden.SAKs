using System.IO;
using System.Web.SessionState;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Serialization;

namespace Matrixden.Utils.Web
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;

    public static class Extensions
    {
        #region  -- HttpCookie --

        /// <summary>
        /// Validate is cookie expired.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool Expired(this HttpCookie cookie)
        {
            return cookie.Expires != new DateTime() && cookie.Expires <= DateTime.Now;
        }

        #endregion

        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="isToUpper">Whether change char's case.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(this string str, bool isToUpper)
        {
            if (!isToUpper)
                return HttpUtility.UrlEncode(str);

            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(this string str)
        {
            return str.UrlEncode(true);
        }

        /// <summary>
        /// 将给定key-value值, 存入session中.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Push(this HttpSessionState @this, string key, object obj)
        {
            if (key.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }

            HttpContext.Current.Session[key] = JsonHelper.Serialize2Bytes(obj);
        }

        /// <summary>
        /// 根据指定key, 从session中取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Pop<T>(this HttpSessionState @this, string key) where T : class, new()
        {
            return key.IsNullOrEmptyOrWhiteSpace() ? default(T) : JsonHelper.Deserialize<T>((byte[])HttpContext.Current.Session[key]);
        }
    }
}
