using System.Net.Http;

namespace Matrixden.Utils.Web
{
    internal class JsonHelper2 : Matrixden.Utils.Serialization.JsonHelper
    {
        /// <summary>
        /// 反序列化JSON字符串到Object
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static object Deserialize(HttpResponseMessage response) => Deserialize(response.Text());

        /// <summary>
        /// Deserialize a string value to an entity, using Newtonsoft.Json.JsonConvert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T Deserialize<T>(HttpResponseMessage response) => Deserialize<T>(response.Text());
    }
}
