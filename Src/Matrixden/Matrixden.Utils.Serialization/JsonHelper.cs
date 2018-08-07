namespace Matrixden.Utils.Serialization
{
    using Matrixden.Utils.Extensions;
    using Matrixden.Utils.Serialization.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public static class JsonHelper
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();
        private static readonly IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
        public static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToJsonByte(object obj)
        {
            string json = SerializeToJsonString(obj);
            byte[] buffer = encoding.GetBytes(json);

            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNullable"></param>
        /// <returns></returns>
        public static string SerializeToJsonString(object obj, bool ignoreNullable = false)
        {
            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                                        {
                                            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                                            NullValueHandling = ignoreNullable ? NullValueHandling.Ignore : NullValueHandling.Include
                                        });
            ////Workaround, for string, int type value.
            if (!json.IsJson())
                return obj.ToString();

            return json;
        }

        public static byte[] ToByteArray(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
            {
                return null;
            }

            return encoding.GetBytes(input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T GetRequestEntity<T>(Stream stream)
        {
            string data = string.Empty;
            using (var ms = new MemoryStream())
            {
                int d = -1;
                try
                {
                    while ((d = stream.ReadByte()) != -1)
                    {
                        ms.WriteByte((byte)d);
                    }

                    data = encoding.GetString(ms.ToArray());
                }
                catch (Exception ex)
                {
                    log.ErrorException(string.Empty, ex);
                }
            }

            return DeserializeStringToObject<T>(data);
        }

        /// <summary>
        /// 将JSON格式字符串转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T GetEntityFromString<T>(string input)
        {
            if (!input.IsJson())
                return input.ToT<T>();

            return DeserializeStringToObject<T>(input);
        }

        /// <summary>
        /// 反序列化JSON字符串到JObject
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static JObject DeserializeStringToJObject(string input)
        {
            return (JObject)DeserializeStringToObject(input);
        }

        /// <summary>
        /// 反序列化JSON字符串到Object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object DeserializeStringToObject(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return null;

            try
            {
                return JsonConvert.DeserializeObject(input);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to deserialize from string value[{0}].", ex, input);
                return null;
            }
        }

        /// <summary>
        /// Deserialize a string value to an entity, using Newtonsoft.Json.JsonConvert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T DeserializeStringToObject<T>(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return default(T);

            if (!input.IsJson())
                return input.ToT<T>();

            T entity = default(T);
            try
            {
                entity = JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception e)
            {
                log.ErrorException("Failed to parse JSON string to type [{0}], Origin String=[{1}].", e, typeof(T).AssemblyQualifiedName, input);
            }

            return entity;
        }

        /// <summary>
        /// Deserialize a string value to an entity, using System.Runtime.Serialization.Json.DataContractJsonSerializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The string value to be Deserialized.</param>
        /// <returns></returns>
        public static T DeserializeStringToObject2<T>(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return default(T);

            object obj = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                obj = serializer.ReadObject(mStream);
            }

            return (T)obj;
        }
    }
}
