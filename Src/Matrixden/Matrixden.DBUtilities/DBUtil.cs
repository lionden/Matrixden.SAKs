using Matrixden.DBUtilities.Attributes;
using Matrixden.DBUtilities.Resources;
using Matrixden.Utils.Extensions;
using System;
using System.Linq;
using System.Reflection;
using Matrixden.DBUtilities.Utils;

namespace Matrixden.DBUtilities
{
    /// <summary>
    /// 
    /// </summary>
    public class DBUtil
    {
        /// <summary>
        /// 根据实体, 获取主键字段名称
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetPrimaryKeyName(object item) => item.PrimaryKey();

        /// <summary>
        /// 根据实体, 获取主键字段名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPrimaryKeyName<T>() => typeof(T).PrimaryKey();
    }
}
