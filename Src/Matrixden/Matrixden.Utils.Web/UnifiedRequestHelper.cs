using Matrixden.Utils.Extensions;
using Matrixden.Utils.Serialization;
using Matrixden.Utils.Web.Enums;
using Matrixden.Utils.Web.Logging;
using Matrixden.Utils.Web.Resources;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class UnifiedRequestHelper
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 发起Http请求, 获取JObject返回对象.
        /// </summary>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="keyValueList">请求参数列表</param>
        /// <param name="httpMethodEnum">请求方式, 默认: httpGet</param>
        /// <param name="cookies">请求需要携带的Cookie, 默认为空.</param>
        /// <returns></returns>
        public static JObject GetJobjectDataThrouthRequest(string requestUri, List<KeyValuePair<string, object>> keyValueList, HttpRequestMethod httpMethodEnum = HttpRequestMethod.Get, CookieContainer cookies = null)
        {
            return GetJobjectDataThrouthRequest(requestUri,
                    keyValueList == null || keyValueList.Count <= 0
                        ? null
                        : keyValueList.Select(k => new KeyValuePair<string, string>(k.Key, k.Value.ToString())).ToList(),
                    httpMethodEnum);
        }

        /// <summary>
        /// 发起Http请求, 获取JObject返回对象.
        /// </summary>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="keyValueList">请求参数列表</param>
        /// <param name="httpMethodEnum">请求方式, 默认: httpGet</param>
        /// <param name="cookies">请求需要携带的Cookie, 默认为空.</param>
        /// <returns></returns>
        public static JObject GetJobjectDataThrouthRequest(string requestUri, List<KeyValuePair<string, string>> keyValueList = null, HttpRequestMethod httpMethodEnum = HttpRequestMethod.Get, CookieContainer cookies = null)
        {
            var response = GetResponseMessage(requestUri, keyValueList, httpMethodEnum, cookies);
            if (HttpStatusCode.OK != response.StatusCode)
            {
                log.FatalFormat("Failed to request [{0}].", requestUri);
                return JsonHelper.DeserializeStringToJObject(Constants.RESPONSE_MESSAGE_FAIL_DEFAULT_JSON);
            }

            var result = GenerateStringFromResponse(response);
            var obj = JsonHelper.DeserializeStringToObject(result);
            if (obj == null)
            {
                log.FatalFormat("请求接口不成功.\r\nRequest Uri=[{0}],\r\nResponse Str=[{1}].", requestUri, result);
                obj = JsonConvert.DeserializeObject(Constants.RESPONSE_MESSAGE_FAIL_DEFAULT_JSON);
            }

            return obj as JObject;
        }

        /// <summary>
        /// 发起HttpPost请求, 获取JObject对象.
        /// </summary>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="keyValueList">请求参数列表.</param>
        /// <returns></returns>
        public static JObject GetJobjectDataThrouthPostRequest(string requestUri, List<KeyValuePair<string, object>> keyValueList = null)
        {
            return GetJobjectDataThrouthRequest(requestUri, keyValueList, HttpRequestMethod.Post);
        }

        /// <summary>
        /// 使用HttpPost请求, 获取JObject对象.
        /// </summary>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="keyValueList">请求参数列表.</param>
        /// <returns></returns>
        public static JObject GetJobjectDataThrouthPostRequest(string requestUri, List<KeyValuePair<string, string>> keyValueList = null)
        {
            return GetJobjectDataThrouthRequest(requestUri, keyValueList, HttpRequestMethod.Post);
        }

        /// <summary>
        /// 将HttpResponseMessage body内容转字符串输出
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GenerateStringFromResponse(HttpResponseMessage response)
        {
            if (HttpStatusCode.NoContent.Equals(response.StatusCode))
            {
                log.WarnFormat("There is no response content in request uri:\r\n{0}.", response.RequestMessage.RequestUri);
                return string.Empty;
            }

            string result = null;
            try
            {
                result = response.Content.ReadAsByteArrayAsync().Result.ToString2();
                log.DebugFormat("Request uri=[{0}],\r\nResponse content=[{1}].", response.RequestMessage.RequestUri, result);
            }
            catch (NullReferenceException nrEx)
            {
                log.ErrorException("内部错误, 接口返回数据为NULL.", nrEx);
            }
            catch (Exception ex)
            {
                log.FatalException("Unknow exception.", ex);
            }

            return result;
        }

        /// <summary>
        /// Send a request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="keyValueList">KeyValuePair List</param>
        /// <param name="httpMethodEnum">Which http method used.</param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponseMessage(string requestUri, List<KeyValuePair<string, object>> keyValueList, HttpRequestMethod httpMethodEnum, CookieContainer cookies)
        {
            return GetResponseMessage(requestUri, keyValueList.Select(k => new KeyValuePair<string, string>(k.Key, k.Value.ToString())).ToList(), httpMethodEnum, cookies);
        }

        /// <summary>
        /// Send a request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="keyValueList">KeyValuePair List</param>
        /// <param name="httpMethodEnum">Which http method used.</param>
        /// <param name="cookies"></param>
        /// <param name="timeout">Request times out, default is 100 seconds.</param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponseMessage(string requestUri, List<KeyValuePair<string, string>> keyValueList, HttpRequestMethod httpMethodEnum, CookieContainer cookies, TimeSpan timeout = default(TimeSpan))
        {
            var pms = CommonHelper.ConvertKeyValuePairList2QueryParamString(keyValueList);
            log.DebugFormat("Request Uri=[{0}],\r\nParams=[{1}].", requestUri, pms);
            var hrMsg = new HttpResponseMessage(HttpStatusCode.NoContent);

            try
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    UseCookies = true,
                    CookieContainer = cookies
                };
                using (var client = new HttpClient(httpClientHandler))
                {
                    if (timeout != default(TimeSpan))
                        client.Timeout = timeout;
                    switch (httpMethodEnum)
                    {
                        case HttpRequestMethod.Get:
                            hrMsg = client.GetAsync(requestUri.SetQueryParams(keyValueList)).Result;
                            break;
                        case HttpRequestMethod.Post:
                            hrMsg = client.PostAsync(requestUri, new FormUrlEncodedContent(keyValueList)).Result;
                            break;
                        case HttpRequestMethod.Put:
                            hrMsg = client.PutAsync(requestUri, new FormUrlEncodedContent(keyValueList)).Result;
                            break;
                        case HttpRequestMethod.Delete:
                            hrMsg = client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri)
                            {
                                Content = new FormUrlEncodedContent(keyValueList)
                            }).Result;
                            break;
                        case HttpRequestMethod.Options:
                        default:
                            break;
                    }
                }
            }
            catch (AggregateException aEx)
            {
                log.ErrorException("请求接口不成功. \r\nRequest API=[{0}],\r\nParams=[{1}].", aEx, requestUri, pms);
            }
            catch (TaskCanceledException)
            {
                log.ErrorFormat("请求超时. Timeout in seconds=[{2}].\r\nRequest API=[{0}],\r\nParams=[{1}].", requestUri, pms, timeout == default(TimeSpan) ? 100 : timeout.TotalSeconds);
            }
            catch (Exception ex)
            {
                log.FatalException("Unhandle exception occurred! \r\nRequest API=[{0}],\r\nParams=[{1}].", ex, requestUri, pms);
            }

            return hrMsg;
        }

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="keyValueList">KeyValuePair List</param>
        /// <returns></returns>
        public static HttpResponseMessage Response_Post(string requestUri, List<KeyValuePair<string, string>> keyValueList)
        {
            return GetResponseMessage(requestUri, keyValueList, HttpRequestMethod.Post, null);
        }

        /// <summary>
        /// 发起Http请求, 返回其实体.
        /// </summary>
        /// <typeparam name="T">泛型实体.</typeparam>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="httpMethod">请求方法, 默认为HttpGet.</param>
        /// <param name="timeout">请求超时时间</param>
        /// <param name="keyValues">参数列表.</param>
        /// <returns></returns>
        public static T MakeHttpRequest<T>(string requestUri, HttpRequestMethod httpMethod, TimeSpan timeout, params KeyValuePair<string, object>[] keyValues)
        {
            var response = GetResponseMessage(requestUri, keyValues.Select(a => new KeyValuePair<string, string>(a.Key, a.Value.ToString())).ToList(), httpMethod, null, timeout);
            var result = GenerateStringFromResponse(response);

            return JsonHelper.GetEntityFromString<T>(result);
        }

        /// <summary>
        /// 发起Http请求, 返回其实体.
        /// </summary>
        /// <typeparam name="T">泛型实体.</typeparam>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="httpMethod">请求方法, 默认为HttpGet.</param>
        /// <param name="keyValues">参数列表.</param>
        /// <returns></returns>
        public static T MakeHttpRequest<T>(string requestUri, HttpRequestMethod httpMethod = HttpRequestMethod.Get, params KeyValuePair<string, object>[] keyValues)
        {
            return MakeHttpRequest<T>(requestUri, httpMethod, default(TimeSpan), keyValues);
        }

        /// <summary>
        /// 发起Http请求, 返回其实体.
        /// </summary>
        /// <typeparam name="T">泛型实体.</typeparam>
        /// <param name="requestUri">请求地址.</param>
        /// <param name="keyValueList">请求参数列表.</param>
        /// <param name="httpMethodEnum">请求方法, 默认为HttpGet.</param>
        /// <returns></returns>
        public static T MakeHttpRequest<T>(string requestUri, List<KeyValuePair<string, string>> keyValueList = null, HttpRequestMethod httpMethodEnum = HttpRequestMethod.Get)
        {
            var response = GetResponseMessage(requestUri, keyValueList, httpMethodEnum, null);
            var result = GenerateStringFromResponse(response);

            return JsonHelper.GetEntityFromString<T>(result);
        }

        /// <summary>
        /// 发起HttpPost请求, 返回其实体.
        /// </summary>
        /// <typeparam name="T">泛型实体.</typeparam>
        /// <param name="url">请求地址.</param>
        /// <param name="data">向服务端提交的请求参数.</param>
        /// <param name="contentType">参数提交方式.</param>
        /// <param name="timeout">请求超时时间, 默认为60秒</param>
        /// <returns></returns>
        public static T MakeHttpPostRequest<T>(string url, byte[] data, string contentType, int timeout = 0)
        {
            log.DebugFormat("Request URL=[{0}].", url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout <= 0 ? Constants.GlobalTimeoutInMilliseconds : timeout;
            request.Referer = Constants.GlobalHttpHeader_Referer;
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = data.Length;

            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            T result = JsonHelper.GetRequestEntity<T>(response.GetResponseStream());
            return result;
        }
    }
}
