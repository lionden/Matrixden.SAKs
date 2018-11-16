using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Matrixden.DBUtilities.Attributes;
using Matrixden.DBUtilities.Resources;
using Matrixden.Utils.Extensions;

namespace Matrixden.DBUtilities.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string PrimaryKey(this Type type)
        {
            // ReSharper disable once PossibleNullReferenceException
            var pks = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi =>
                pi.GetCustomAttributes(typeof(ColumnAttribute), false).Any(a => ((a as ColumnAttribute).IsPK)));
            if (!pks.Any())
                return DBTableCommonColumns.ID;

            if (pks.Count() == 1)
            {
                var cln = pks.FirstOrDefault().GetCustomAttributes(typeof(ColumnAttribute), false)
                    .Select(s2 => (s2 as ColumnAttribute).Name).FirstOrDefault();
                return cln.IsNullOrEmptyOrWhiteSpace() ? pks.Select(s => s.Name).FirstOrDefault() : cln;
            }

            throw new Exception($"Multiple PK column found: {string.Join(", ", pks)}.");
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static string PrimaryKey(this object item) => item.GetType().PrimaryKey();
    }
}
