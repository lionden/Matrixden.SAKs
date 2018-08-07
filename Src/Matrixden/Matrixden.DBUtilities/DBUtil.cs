using Matrixden.DBUtilities.Attributes;
using Matrixden.DBUtilities.Resources;
using Matrixden.Utils.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Matrixden.DBUtilities
{
    public class DBUtil
    {
        /// <summary>
        /// 根据实体, 获取主键字段名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPrimaryKeyName<T>()
        {
            var pks = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.GetCustomAttributes(typeof(ColumnAttribute), false).Any(a => ((a as ColumnAttribute).IsPK)));
            if (pks.Count() <= 0)
                return DBTableCommonColumns.ID;
            else if (pks.Count() == 1)
            {
                var cln = pks.FirstOrDefault().GetCustomAttributes(typeof(ColumnAttribute), false).Select(s2 => (s2 as ColumnAttribute).Name).FirstOrDefault();
                if (cln.IsNullOrEmptyOrWhiteSpace())
                    return pks.Select(s => s.Name).FirstOrDefault();

                return cln;
            }
            else
                throw new Exception("NO PK column.");
        }
    }
}
