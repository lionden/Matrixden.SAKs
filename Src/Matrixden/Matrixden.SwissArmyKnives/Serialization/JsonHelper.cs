namespace Matrixden.Utils.Serialization
{
    using Matrixden.Utils.Extensions;
    using Matrixden.Utils.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class JsonHelper
    {
        private static readonly ILog log = Utils.Logging.LogProvider.GetCurrentClassLogger();
        private static readonly IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
        /// <summary>
        /// Encode method
        /// </summary>
        public static Encoding Encoding { get; set; } = Encoding.UTF8;

        #region -- Serialize --

        /// <summary>
        /// Serialize object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNullable"></param>
        /// <returns></returns>
        public static string Serialize(object obj, bool ignoreNullable)
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                NullValueHandling = ignoreNullable ? NullValueHandling.Ignore : NullValueHandling.Include
            });

            ////Workaround, for string, int type value.
            return !json.IsJson() ? obj.ToString() : json;
        }

        /// <summary>
        /// Serialize object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj) => Serialize(obj, false);

        /// <summary>
        /// Serialize object to bytes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize2Bytes(object obj)
        {
            var json = Serialize(obj);
            var buffer = json.Bytes(Encoding);

            return buffer;
        }

        #endregion

        #region -- Deserialize --

        /// <summary>
        /// 反序列化JSON字符串到Object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object Deserialize(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return default(object);

            var obj = default(object);
            try
            {
                obj = JsonConvert.DeserializeObject(input);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to deserialize from string value[{0}].", ex, input);
            }

            return obj;
        }

        /// <summary>
        /// Deserialize a string value to an entity, using Newtonsoft.Json.JsonConvert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return default(T);

            if (!input.IsJson())
                return input.ToT<T>();

            var entity = default(T);
            try
            {
                entity = JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception e)
            {
                log.ErrorException("Failed to parse JSON string to type [{0}], Origin String=[{1}].", e,
                    typeof(T).AssemblyQualifiedName, input);
            }

            return entity;
        }

        /// <summary>
        /// Deserialize a byte array to an entity, using Newtonsoft.Json.JsonConvert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] bytes) => Deserialize<T>(Encoding.GetString(bytes));

        /// <summary>
        /// 反序列化JSON字符串到JObject
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static JObject Deserialize2JObject(string input) => (JObject)Deserialize(input);

        /// <summary>
        /// Deserialize a string value to an entity, using System.Runtime.Serialization.Json.DataContractJsonSerializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T Deserialize2<T>(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return default(T);

            if (!input.IsJson())
                return input.ToT<T>();

            var obj = default(object);
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var mStream = new MemoryStream(Encoding.GetBytes(input)))
            {
                obj = serializer.ReadObject(mStream);
            }

            return (T)obj;
        }

        /// <summary>
        /// Deserialize stream to T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream)
        {
            var data = string.Empty;
            using (var ms = new MemoryStream())
            {
                try
                {
                    var d = -1;
                    while ((d = stream.ReadByte()) != -1)
                    {
                        ms.WriteByte((byte)d);
                    }

                    data = Encoding.GetString(ms.ToArray());
                }
                catch (Exception ex)
                {
                    log.ErrorException(string.Empty, ex);
                }
            }

            return Deserialize<T>(data);
        }

        #endregion

        #region -- Legacy methods --

        /// <summary>
        /// Serialize object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNullable"></param>
        /// <returns></returns>
        public static string SerializeToJsonString(object obj, bool ignoreNullable = false) =>
            Serialize(obj, ignoreNullable);

        /// <summary>
        /// Deserialize a string value to an entity, using Newtonsoft.Json.JsonConvert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T DeserializeStringToObject<T>(string input) => Deserialize<T>(input);

        /// <summary>
        /// 将JSON格式字符串转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T GetEntityFromString<T>(string input) => Deserialize<T>(input);

        #endregion
    }

    /// <summary>
    /// 扩展方法.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 判断字符串是否是JSON格式
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsJson(this string @this)
        {
            if (@this.IsNullOrEmptyOrWhiteSpace())
                return false;

            try
            {
                JToken.Parse(@this);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
