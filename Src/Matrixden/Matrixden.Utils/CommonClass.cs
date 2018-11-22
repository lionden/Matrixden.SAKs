using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Matrixden.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonClass
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 使用反射动态定义object对象中的属性。若存在指定名称的属性则修改为指定的新值，并返回true；否则返回false。
        /// </summary>
        /// <param name="instance">要修改的目标对象</param>
        /// <param name="strProperty">属性名</param>
        /// <param name="value">新的属性值</param>
        public static bool SetPropertyValue(object instance, string strProperty, object value)
        {
            try
            {
                Type type = instance.GetType();
                var property = type.GetProperty(strProperty);
                if (property == null || !property.CanWrite)
                    return false;

                if (value == DBNull.Value)
                {
                    value = null;
                }
                else if (value != null && value.GetType() != property.PropertyType)
                {
                    switch (property.PropertyType.ToString())
                    {
                        case "System.String":
                            value = value.ToString2();
                            break;
                        case "System.Int16":
                            value = value.ToInt16();
                            break;
                        case "System.Int32":
                            value = value.ToInt32();
                            break;
                        case "System.Int64":
                            value = value.ToInt64();
                            break;
                        case "System.Decimal":
                            value = value.ToDecimal();
                            break;
                        case "System.Boolean":
                            if (value.GetType() == Type.GetType("System.Int32"))
                                value = value.ToInt32() == 1;
                            break;
                        case "System.Guid":
                            value = value.ToString2().ToGuid();
                            break;
                        case "System.DateTime":
                            value = value.ToDateTime();
                            break;
                    }
                }

                property.SetValue(instance, value, null);
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to set property value. Instance ={0}; Property ={1}; Value ={2}.", ex, instance.GetType(), strProperty, value);
                return false;
            }
        }

        /// <summary>
        /// 使用反射动态返回object对象中指定的属性（仅返回数据库字段, 所有非数据库字段放在该类的子类中）。若存在则返回指定属性值；否则返回null。
        /// </summary>
        /// <param name="type">要查询的对象类型</param>
        /// <returns>属性值</returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            try
            {
                System.Reflection.PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (properties.Length > 0)
                {
                    return properties;
                }

                return null;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get entity's properties.", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static TOut GetPropertyValue<TIn, TOut>(TIn instance, string property)
        {
            var type = instance.GetType();
            var pi = type.GetProperty(property);
            if (pi == null)
                return default(TOut);

            try
            {
                var func = (Func<TIn, TOut>)Delegate.CreateDelegate(typeof(Func<TIn, TOut>), pi.GetGetMethod());

                return func(instance);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get property({0})'s value.", ex, property);
                return default(TOut);
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instance">要查询的对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(object instance, string propertyName)
        {
            var type = instance.GetType();
            var pi = type.GetProperty(propertyName);
            if (pi == null)
                return null;

            try
            {
                var target = Expression.Parameter(typeof(object));
                var getter = Expression.Lambda(typeof(Func<object, object>),
                    Expression.Convert(Expression.Property(Expression.Convert(target, type), pi), typeof(object)),
                    target);
                var getValueDelegate = (Func<object, object>)getter.Compile();

                return getValueDelegate(instance);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to get property({0})'s value.", ex, propertyName);
                return null;
            }
        }

        /// <summary>
        /// 使用反射动态返回object对象中指定的属性。若存在则返回指定属性值；否则返回null。
        /// </summary>
        /// <param name="instance">要查询的对象</param>
        /// <param name="strProperty">属性名</param>
        /// <returns>属性值</returns>
        [Obsolete("Use GetPropertyValue instead.", true)]
        public static object GetFieldValue(object instance, string strProperty) =>
            GetPropertyValue(instance, strProperty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="strProperties"></param>
        /// <returns></returns>
        public static object GetFieldsValue(object instance, params string[] strProperties)
        {
            return strProperties?.Where(s => s.IsNotNullNorEmptyNorWhitespace()).Select(p => GetPropertyValue(instance, p));
        }

        /// <summary>
        /// 根据Model类, 获取Parameter List用于URL传参
        /// </summary>
        /// <typeparam name="T">泛型, Model类</typeparam>
        /// <param name="tModel">Model类</param>
        /// <returns>返回Parameter List</returns>
        public static List<KeyValuePair<string, string>> ToParamList<T>(T tModel)
        {
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

            PropertyInfo[] properties = CommonClass.GetProperties(typeof(T));
            foreach (PropertyInfo property in properties)
            {
                object value = CommonClass.GetPropertyValue(tModel, property.Name);
                string propertyType = property.PropertyType.ToString();
                switch (propertyType)
                {
                    case "System.Int32":
                        if (value.ToInt32() == 0)
                            value = null;
                        break;
                    case "System.Double":
                        if (value.ToDouble() == 0)
                            value = null;
                        break;
                    case "System.Boolean":
                        value = (bool)value ? "1" : "0";
                        break;
                    case "System.DateTime":
                        if (value.ToDateTime() == default(DateTime))
                            value = null;
                        break;
                    case "System.String":
                        if (value.ToString2().IsNullOrEmptyOrWhiteSpace())
                            value = null;
                        break;
                    default:
                        break;
                }
                if (value != null)
                {
                    paramList.Add(new KeyValuePair<string, string>(GetCamelCaseStringFromModelProperty(property.Name), value.ToString2()));
                }
            }

            return paramList;
        }

        /// <summary>
        /// 将Model属性名的首字母变为小写
        /// </summary>
        /// <param name="modelProperty">Model类的属性名</param>
        /// <returns>符合camelCase</returns>
        public static string GetCamelCaseStringFromModelProperty(string modelProperty)
        {
            return char.ToLower(modelProperty[0]) + modelProperty.Substring(1);
        }
    }
}
