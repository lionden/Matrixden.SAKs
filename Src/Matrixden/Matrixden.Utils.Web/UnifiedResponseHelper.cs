using Matrixden.Utils.Extensions;
using Matrixden.Utils.Serialization;
using Matrixden.Utils.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web
{
    /// <summary>
    /// 通用响应帮助类
    /// </summary>
    public class UnifiedResponseHelper
    {
        /// <summary>
        /// 将Object类型序列化为JSON格式输出.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseJsonMessage(Object obj)
        {
            string str = JsonHelper.SerializeToJsonString(obj);

            return Response(str, Encoding.GetEncoding("UTF-8"), Constants.MIME_TYPE_JSON);
        }

        /// <summary>
        /// 将Object类型序列化为JSON格式输出, 并带有特殊.
        /// </summary>
        /// <param name="parseResponse"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseJsonpMessage(string parseResponse, Object obj)
        {
            string str = string.Format(parseResponse.IsNullOrEmptyOrWhiteSpace() ? "{1}" : "{0}({1})", parseResponse, JsonHelper.SerializeToJsonString(obj));

            return Response(str, Encoding.GetEncoding("UTF-8"), Constants.MIME_TYPE_JSON);
        }

        /// <summary>
        /// 将Object类型序列化为JSON格式输出, 并带有特殊前缀..
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="responsePrefix">JSON串前缀, 默认: "var return_api_data = ".</param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseJsonMessage_WithPrefix(Object obj, string responsePrefix = "var return_api_data = ")
        {
            string str = responsePrefix + JsonHelper.SerializeToJsonString(obj);

            return Response(str, Encoding.GetEncoding("UTF-8"), Constants.MIME_TYPE_JSON);
        }

        private static HttpResponseMessage Response(string strContent, Encoding encoding, string mime_type)
        {
            if (strContent.IsNullOrEmptyOrWhiteSpace())
                return new HttpResponseMessage(HttpStatusCode.NoContent);

            return new HttpResponseMessage
            {
                Content = new StringContent(strContent, encoding, mime_type)
            };
        }
    }
}