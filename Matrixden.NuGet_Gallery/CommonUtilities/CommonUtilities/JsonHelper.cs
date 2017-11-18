/**
 * 
 */
namespace Clouds.XunmallPos.Utils
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public static class JsonHelper
    {
        private static readonly IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();

        public static readonly Encoding encoding = Encoding.UTF8;

        public static byte[] SerializeToJsonByte(object obj)
        {
            datetimeConverter.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
            string json = JsonConvert.SerializeObject(obj, datetimeConverter);
            byte[] buffer = encoding.GetBytes(json);

            return buffer;
        }

        public static string SerializeToJsonString(object obj, bool ignoreNullable = false)
        {
            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                                        {
                                            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                                            NullValueHandling = ignoreNullable ? NullValueHandling.Ignore : NullValueHandling.Include
                                        });
            return json;
        }

        public static byte[] ToByteArray(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return encoding.GetBytes(input);
        }

        public static T GetRequestEntity<T>(Stream stream)
        {

            MemoryStream ms = new MemoryStream();

            try
            {
                int d = -1;
                while ((d = stream.ReadByte()) != -1)
                {
                    ms.WriteByte((byte)d);
                }
            }
            finally
            {
                try
                {
                    stream.Close();
                }
                catch { }
            }

            ms.Close();

            String data = encoding.GetString(ms.ToArray());


            T entity = default(T);

            if (!String.IsNullOrEmpty(data))
            {
                //TraceHelper.Trace("Try to parse JSON string to type " + typeof(T).AssemblyQualifiedName + "\n" + data, System.Diagnostics.TraceLevel.Info);

                entity = JsonConvert.DeserializeObject<T>(data);
            }

            return entity;
        }

        public static T GetEntityFromString<T>(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                T entity = default(T);

                if (!String.IsNullOrEmpty(input))
                {
                    //TraceHelper.Trace("Try to parse JSON string to type " + typeof(T).AssemblyQualifiedName + "\n" + data, System.Diagnostics.TraceLevel.Info);
                    try
                    {
                        entity = JsonConvert.DeserializeObject<T>(input);
                    }
                    catch (Exception e)
                    {

                    }
                }

                return entity;
            }

            return default(T);
        }

        public static JObject DeserializeStringToJObject(string input)
        {
            return (JObject)DeserializeStringToObject(input);
        }

        public static object DeserializeStringToObject(string input)
        {
            if (input.IsNullOrEmptyOrWhiteSpace())
                return null;

            try
            {
                return JsonConvert.DeserializeObject(input);
            }
            catch
            {
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

            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch
            {
                return default(T);
            }
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

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return (T)serializer.ReadObject(mStream);
        }
    }
}
