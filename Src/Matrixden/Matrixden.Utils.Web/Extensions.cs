using Matrixden.Utils.Extensions;
using Matrixden.Utils.Serialization;
using Matrixden.Utils.Web.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Matrixden.Utils.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

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

        #region -- HttpResponseMessage 

        /// <summary>
        /// 将HttpResponseMessage body内容转字符串输出
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string Text(this HttpResponseMessage response)
        {
            if (HttpStatusCode.NoContent.Equals(response.StatusCode))
            {
                log.WarnFormat("There is no response content in request uri:\r\n{0}.",
                    response.RequestMessage.RequestUri);

                return string.Empty;
            }

            string result = null;
            try
            {
                result = response.Content.ReadAsByteArrayAsync().Result.ToString2();
                log.DebugFormat("Request uri=[{0}],\r\nResponse content=[{1}].", response.RequestMessage.RequestUri,
                    result);
            }
            catch (NullReferenceException nrEx)
            {
                log.ErrorException("内部错误, 接口返回数据为NULL.", nrEx);
            }
            catch (Exception ex)
            {
                log.FatalException("Unknown exception.", ex);
            }

            return result;
        }

        #endregion

        #region -- HttpSessionState 

        /// <summary>
        /// 将给定key-value值, 存入session中.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Push(this HttpSessionStateBase @this, string key, object obj)
        {
            if (@this == default(HttpSessionStateBase) || key.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }

            @this[key] = JsonHelper.Serialize2Bytes(obj);
        }

        /// <summary>
        /// 根据指定key, 从session中取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Pop<T>(this HttpSessionStateBase @this, string key) where T : class, new()
        {
            return @this == default(HttpSessionStateBase) || key.IsNullOrEmptyOrWhiteSpace()
                ? default(T)
                : JsonHelper.Deserialize<T>((byte[]) @this[key]);
        }

        #endregion
    }
}
