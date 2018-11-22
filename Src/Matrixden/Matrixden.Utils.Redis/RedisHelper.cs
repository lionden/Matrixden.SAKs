using Matrixden.Utils;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Redis.Logging;
using Matrixden.Utils.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Matrixden.Utils.Redis
{
    public partial class RedisHelper
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        string _defaultConnectionString;
        string DefaultConnectionString
        {
            get
            {
                if (_defaultConnectionString.IsNullOrEmptyOrWhiteSpace())
                {
                    try
                    {
                        _defaultConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
                    }
                    catch (ConfigurationErrorsException ceEx)
                    {
                        log.ErrorException("Failed to get Redis connection string from configuration file.", ceEx);
                    }
                    finally
                    {
                        if (_defaultConnectionString.IsNullOrEmptyOrWhiteSpace())
                            _defaultConnectionString = "127.0.0.1:6379,allowadmin=true";
                    }
                }

                return _defaultConnectionString;
            }
        }

        #region -- Public Properties --

        private object _locker = new object();
        private ConnectionMultiplexer _client;
        public ConnectionMultiplexer Client
        {
            get
            {
                if (_client == null)
                    lock (_locker)
                        if (_client == null)
                            _client = GetClient();

                return _client;
            }
        }

        private object _databaseLocker = new object();
        private IDatabase _database;
        public IDatabase DataBase
        {
            get
            {
                if (_database == null)
                    lock (_databaseLocker)
                        if (_database == null)
                            _database = Client.GetDatabase();

                return _database;
            }
        }

        #endregion

        private static object _instanceLocker = new object();
        private static RedisHelper _instance;
        public static RedisHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLocker)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisHelper();
                        }
                    }
                }

                return _instance;
            }
        }

        private RedisHelper()
        {

        }

        private ConnectionMultiplexer GetClient(string connStr = null)
        {
            if (connStr.IsNullOrEmptyOrWhiteSpace())
                connStr = DefaultConnectionString;

            try
            {
                log.DebugFormat("Redis connection string: {0}.", connStr);
                return ConnectionMultiplexer.Connect(connStr);
            }
            catch (RedisConnectionException rcEx)
            {
                log.ErrorException("Failed connected to Redis Server: {0}.", rcEx, connStr);
                return null;
            }
        }

        #region -- String --

        public string StringGet(string key)
        {
            if (!DataBase.KeyExists(key))
                return string.Empty;

            return DataBase.StringGet(key);
        }

        /// <summary>
        /// 获取经二进制序列化的对象值.
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            var bts = DataBase.StringGet(key);
            if (bts.Equals(RedisValue.Null))
                return default(T);

            object obj;
            using (var ms = new MemoryStream(bts))
            {
                ms.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(ms);
            }

            return (T)obj;
        }

        /// <summary>
        /// Without overwritten.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet(string key, string value)
        {
            if (DataBase.KeyExists(key))
                return false;

            return DataBase.StringSet(key, value);
        }

        public bool StringSet(string key, string value, TimeSpan expiry)
        {
            if (DataBase.KeyExists(key))
                return false;

            return DataBase.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 将对象经二进制序列化后存储
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet(string key, object value)
        {
            byte[] bts;
            using (var ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, value);
                bts = ms.GetBuffer();
            }

            return DataBase.StringSet(key, bts);
        }

        /// <summary>
        /// Without new create.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringUpdate(string key, string value)
        {
            if (DataBase.KeyExists(key))
                return false;

            return DataBase.StringSet(key, value, null, When.Exists);
        }

        public bool StringSave(string key, string value)
        {
            return DataBase.StringSet(key, value);
        }

        public bool StringSave(string key, string value, TimeSpan expiry)
        {
            return DataBase.StringSet(key, value, expiry);
        }

        public bool StringDelete(string key)
        {
            return DataBase.KeyDelete(key);
        }

        #endregion

        #region -- Hash table --

        public bool ModelSet<T>(string key, string dataKey, T t)
        {
            return DataBase.HashSet(key, dataKey, JsonHelper.SerializeToJsonString(t));
        }

        public void ModelSet<T>(string key, IEnumerable<T> t, string fieldKey, string fieldPrefix, string valueKey = null)
        {
            var hes = new ConcurrentBag<HashEntry>();
            if (valueKey.IsNullOrEmptyOrWhiteSpace())
            {
                Parallel.ForEach(t, v =>
                {
                    hes.Add(new HashEntry($"{fieldPrefix}:{CommonClass.GetPropertyValue(v, fieldKey)}", JsonHelper.SerializeToJsonString(v)));
                });
            }
            else
            {
                var hesR = new ConcurrentBag<HashEntry>();
                Parallel.ForEach(t, v =>
                {
                    var fk = $"{fieldPrefix}:{CommonClass.GetPropertyValue(v, fieldKey)}";
                    hes.Add(new HashEntry(fk, JsonHelper.SerializeToJsonString(v)));
                    hesR.Add(new HashEntry(fk, CommonClass.GetPropertyValue(v, valueKey).ToString()));
                });

                DataBase.HashSet(string.Format("{0}_{1}_{2}", key, fieldKey, valueKey), hesR.ToArray());
            }

            DataBase.HashSet(key, hes.ToArray());
        }

        public async Task ModelSetAsync<T>(string key, IEnumerable<T> t, string fieldKey, string fieldPrefix)
        {
            var hes = t.Select(v => new HashEntry($"{fieldPrefix}:{CommonClass.GetPropertyValue(v, fieldKey)}", JsonHelper.SerializeToJsonString(v)));

            await DataBase.HashSetAsync(key, hes.ToArray());
        }

        public bool ModelDelete(string key, string dataKey)
        {
            return DataBase.HashDelete(key, dataKey);
        }

        public long ModelDelete(string key, List<RedisValue> dataKeys)
        {
            return DataBase.HashDelete(key, dataKeys.ToArray());
        }

        public T ModelGet<T>(string key, string dataKey)
        {
            return JsonHelper.GetEntityFromString<T>(DataBase.HashGet(key, dataKey));
        }

        public IEnumerable<T> ModelGet<T>(string key, IEnumerable<RedisValue> fKeys) where T : new()
        {
            if (fKeys == null)
                return Enumerable.Empty<T>();

            var lt = new System.Collections.Concurrent.ConcurrentBag<T>();
            if (typeof(T) == typeof(object))
                Parallel.ForEach(fKeys, fk =>
                {
                    var v = DataBase.HashGet(key, fk);
                    if (!v.IsNullOrEmpty)
                        lt.Add((T)(v as object));
                });
            else
                Parallel.ForEach(fKeys, fk =>
                {
                    var v = JsonHelper.GetEntityFromString<T>(DataBase.HashGet(key, fk));
                    if (v != null)
                        lt.Add(v);
                });

            return lt;
        }

        public IEnumerable<T> ModelGet_KMP_And<T>(string key, params string[] keysPattern) where T : new()
        {
            if (keysPattern == null)
                return Enumerable.Empty<T>();

            var fks = DataBase.HashKeys(key).Where(f => f.ToString().MatchAll(keysPattern));

            return ModelGet<T>(key, fks);
        }

        public IEnumerable<T> ModelGet_KMP_Or<T>(string key, params string[] keysPattern) where T : new()
        {
            if (keysPattern == null)
                return Enumerable.Empty<T>();

            var fks = DataBase.HashKeys(key).Where(f => f.ToString().MatchAny(keysPattern));

            return ModelGet<T>(key, fks);
        }

        public List<T> ModelGet<T>(string key) where T : class, new()
        {
            HashEntry[] hes = DataBase.HashGetAllAsync(key).Result;
            return hes.Select(e => JsonHelper.GetEntityFromString<T>(e.Value)).ToList();
        }

        #endregion
    }
}
