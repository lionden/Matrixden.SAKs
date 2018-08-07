using Matrixden.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web.Resources
{
    class Constants
    {
        /// <summary>
        /// MIME Type: JSON
        /// </summary>
        public const string MIME_TYPE_JSON = "application/json";
        /// <summary>
        /// MIME Type: XML
        /// </summary>
        public const string MIME_TYPE_XML = "application/xml";
        /// <summary>
        /// MIME Type: HTML
        /// </summary>
        public const string MIME_TYPE_HTML = "text/html";

        /// <summary>
        /// 默认失败返回值, 用于服务器宕机等使接口返回非2xx请求值情况.
        /// </summary>
        internal static string RESPONSE_MESSAGE_FAIL_DEFAULT
        {
            get
            {
                return Get("RESPONSE_MESSAGE_FAIL_DEFAULT", v => v, "服务器暂时开小差了, 请稍后再试. Seems the server cannot response you right now, please try again later.");
            }
        }

        /// <summary>
        /// 默认失败返回值, 用于服务器宕机等使接口返回非2xx请求值情况.
        /// </summary>
        internal static string RESPONSE_MESSAGE_FAIL_DEFAULT_JSON
        {
            get
            {
                return Get("RESPONSE_MESSAGE_FAIL_DEFAULT_JSON", v => v, "{'message':'服务器暂时开小差了, 请稍后再试. Seems the server cannot response you right now, please try again later.'}");
            }
        }

        /// <summary>
        /// 全局超时时间
        /// </summary>
        internal static int GlobalTimeoutInMilliseconds
        {
            get
            {
                return Get("GlobalTimeoutInMilliseconds", v => v.ToInt32(60 * 1000));
            }
        }

        /// <summary>
        /// 全局Referer请求头
        /// </summary>
        internal static string GlobalHttpHeader_Referer
        {
            get
            {
                return Get("GlobalHttpHeader_Referer", v => v);
            }
        }

        /// <summary>
        /// 根据指定key, 从配置文件中读取其值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">给定键值</param>
        /// <param name="func"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        private static T Get<T>(string key, Func<string, T> func, string defaultVal)
        {
            if (key.IsNullOrEmptyOrWhiteSpace()
                || !(ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings.AllKeys.Contains(key)))
                return func(defaultVal);

            return func(ConfigurationManager.AppSettings[key]);
        }

        /// <summary>
        /// 根据指定key, 从配置文件中读取其值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">给定键值</param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static T Get<T>(string key, Func<string, T> func)
        {
            return Get(key, func, string.Empty);
        }
    }
}
