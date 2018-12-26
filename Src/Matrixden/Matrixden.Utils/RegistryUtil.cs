using Matrixden.Utils.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils
{
    /// <summary>
    /// 注册表工具类
    /// </summary>
    public class RegistryUtil
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 根据注册表Key获取其值.
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetRegistryKeyValue(string subKey, string keyName)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null)
            {
                registryKey = Registry.CurrentUser.CreateSubKey(subKey);
            }
            string registryKeyValue = registryKey.GetValue(keyName).ToString();
            registryKey.Close();

            return registryKeyValue;
        }

        /// <summary>
        /// 根据注册表Key获取其值.
        /// </summary>
        /// <typeparam name="T">泛型.</typeparam>
        /// <param name="subKey"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static T GetRegistryKeyValue<T>(string subKey, string keyName)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null)
            {
                registryKey = Registry.CurrentUser.CreateSubKey(subKey);
            }
            T registryKeyValue = (T)registryKey.GetValue(keyName);
            registryKey.Close();

            return registryKeyValue;
        }

        /// <summary>
        /// 根据注册表Key获取其value collection.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static StringCollection GetRegistryKeyValues(string subKey, string keyName)
        {
            string registryKeyValue = GetRegistryKeyValue(subKey, keyName);
            StringCollection stringCollection = new StringCollection();
            if (registryKeyValue != null)
            {
                string[] array = registryKeyValue.Split(new char[]
                {
                    '|'
                });
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string value = array2[i];
                    stringCollection.Add(value);
                }
            }

            return stringCollection;
        }

        /// <summary>
        /// 保存Key-Value信息到注册表
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="keyValueModel">Key-Value Model</param>
        public static void SaveRegistryKeyValue(string subKey, KeyValuePair<string, object> keyValueModel)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(subKey, true);
                if (registryKey == null)
                {
                    registryKey = Registry.CurrentUser.CreateSubKey(subKey);
                }

                registryKey.SetValue(keyValueModel.Key, keyValueModel.Value);
                registryKey.Close();
            }
            catch (Exception ex)
            {
                log.ErrorException("添加信息到注册表失败!", ex);
            }
        }

        /// <summary>
        /// 保存Key-Value信息到注册表
        /// </summary>
        /// <param name="keyValueModel">Key-Value Model</param>
        public static void SaveRegistryKeyValue(string subKey, KeyValuePair<string, string> keyValueModel)
        {
            SaveRegistryKeyValue(subKey, new KeyValuePair<string, object>(keyValueModel.Key, keyValueModel.Value));
        }

        /// <summary>
        /// 保存信息到注册表, 多个值到一个key下
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="keyName"></param>
        /// <param name="values"></param>
        public static void SaveRegistryKeyValues(string subKey, string keyName, StringCollection values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string value = "";
            foreach (string current in values)
            {
                stringBuilder.Append(value);
                stringBuilder.Append(current);
                value = '|'.ToString();
            }

            SaveRegistryKeyValue(subKey, new KeyValuePair<string, string>(keyName, stringBuilder.ToString()));
        }
    }
}
