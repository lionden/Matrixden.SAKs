using Matrixden.Utils.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matrixden.Utils.Redis
{
    /// <summary>
    /// Redis help methods.
    /// </summary>
    public partial class RedisHelper
    {
        #region -- KEY --
        #region 同步方法

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool IsKeyExist(string key, CommandFlags flags = CommandFlags.None)
        {
            return Do(db => db.KeyExists(key, flags));
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool KeyDelete(string key, CommandFlags flags = CommandFlags.None)
        {
            return Do(db => db.KeyDelete(key, flags));
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public long KeyDelete(CommandFlags flags = CommandFlags.None, params string[] keys)
        {
            return Do(db => db.KeyDelete(keys.Select(k => (RedisKey)k).ToArray(), flags));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> IsKeyExistAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return await Do(db => db.KeyExistsAsync(key, flags));
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return await Do(db => db.KeyDeleteAsync(key, flags));
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<long> KeyDeleteAsync(CommandFlags flags = CommandFlags.None, params string[] keys)
        {
            return await Do(db => db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray(), flags));
        }

        #endregion 异步方法
        #endregion

        #region -- Hash --
        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            return Do(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool HashSet(string key, string dataKey, object val)
        {
            return Do(db =>
            {
                return db.HashSet(key, dataKey, val.ToString());
            });
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            return Do(db =>
            {
                string json = JsonHelper.SerializeToJsonString(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            return Do(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> dataKeys)
        {
            return Do(db => db.HashDelete(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public string HashGet(string key, string dataKey)
        {
            return Do(db => db.HashGet(key, dataKey));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            return Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return JsonHelper.GetEntityFromString<T>(value);
            });
        }

        /// <summary>
        /// 从hash表中获取所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<string> HashGets(string key)
        {
            return Do(db =>
            {
                var rts = new ConcurrentBag<string>();
                var ks = db.HashKeys(key);
                foreach (var k in ks)
                    rts.Add(db.HashGet(key, k));

                return rts;
            });
        }

        /// <summary>
        /// 从hash表中获取所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<T> HashGets<T>(string key)
        {
            return Do(db =>
            {
                var rts = new ConcurrentBag<T>();
                var ks = db.HashKeys(key);
                foreach (var k in ks)
                    rts.Add(JsonHelper.DeserializeStringToObject<T>(db.HashGet(key, k)));

                return rts;
            });
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            return Do(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            return Do(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            return Do(db =>
            {
                RedisValue[] values = db.HashKeys(key);
                return ConvertList<T>(values);
            });
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            return await Do(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            return await Do(db =>
            {
                string json = JsonHelper.SerializeToJsonString(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            return await Do(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string key, List<RedisValue> dataKeys)
        {
            return await Do(db => db.HashDeleteAsync(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGeAsync<T>(string key, string dataKey)
        {
            string value = await Do(db => db.HashGetAsync(key, dataKey));
            return JsonHelper.GetEntityFromString<T>(value);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            return await Do(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            return await Do(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            RedisValue[] values = await Do(db => db.HashKeysAsync(key));
            return ConvertList<T>(values);
        }

        #endregion 异步方法
        #endregion Hash

        #region -- Set --
        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        public bool SetAdd(string key, string value, CommandFlags flags = CommandFlags.None)
        {
            return Do(redis => redis.SetAdd(key, value, flags));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public long SetAddRange(string key, params string[] values)
        {
            return Do(redis => redis.SetAdd(key, values.Select(s => (RedisValue)s).ToArray()));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public long SetAddRange(string key, IEnumerable<string> values)
        {
            return SetAddRange(key, values.ToArray());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SetRemove(string key, string value)
        {
            return Do(redis => redis.SetRemove(key, value));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public long SetRemoveRange(string key, params string[] values)
        {
            return Do(redis => redis.SetRemove(key, values.Select(s => (RedisValue)s).ToArray()));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public long SetRemoveRange(string key, IEnumerable<string> values)
        {
            return SetRemoveRange(key, values.ToArray());
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<string> SetMembers(string key)
        {
            return Do(redis => redis.SetMembers(key).Select(s => s.ToString()));
        }

        /// <summary>
        /// 随机获取一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string SetRandomMember(string key)
        {
            return Do(r => r.SetRandomMember(key));
        }

        /// <summary>
        /// 随机获取一些
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<string> SetRandomMembers(string key, int count)
        {
            return Do(r => r.SetRandomMembers(key, count).Select(s => s.ToString()));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string SetPop(string key)
        {
            return Do(r => r.SetPop(key));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<string> SetPop(string key, int count)
        {
            var ms = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                ms.Add(Do(r => r.SetPop(key)));
            }

            return ms;
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            return Do(redis => redis.SetLength(key));
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetContains(string key, string value)
        {
            return Do(redis => redis.SetContains(key, value));
        }

        #endregion 同步方法
        #endregion

        #region -- Sorted Set --
        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            return Do(redis => redis.SortedSetAdd(key, JsonHelper.SerializeToJsonString(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            return Do(redis => redis.SortedSetRemove(key, JsonHelper.SerializeToJsonString(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return ConvertList<T>(values);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            return Do(redis => redis.SortedSetLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            return await Do(redis => redis.SortedSetAddAsync(key, JsonHelper.SerializeToJsonString(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            return await Do(redis => redis.SortedSetRemoveAsync(key, JsonHelper.SerializeToJsonString(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            var values = await Do(redis => redis.SortedSetRangeByRankAsync(key));
            return ConvertList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            return await Do(redis => redis.SortedSetLengthAsync(key));
        }

        #endregion 异步方法
        #endregion  -- Sorted Set --

        private T Do<T>(Func<IDatabase, T> func)
        {
            return func(DataBase);
        }

        private List<T> ConvertList<T>(RedisValue[] values)
        {
            var lt = new System.Collections.Concurrent.ConcurrentBag<T>();
            Parallel.ForEach(values, val =>
            {
                var v = JsonHelper.GetEntityFromString<T>(val.ToString());
                if (v != null)
                    lt.Add(v);
            });

            return lt.ToList();
        }
    }
}
