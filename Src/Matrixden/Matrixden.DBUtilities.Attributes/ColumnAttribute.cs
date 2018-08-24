using Matrixden.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Attributes
{
    /// <summary>
    /// "实体-列"映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 映射的数据库列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPK { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="pk">是否是主键</param>
        public ColumnAttribute(string name, bool pk)
        {
            this.Name = name;
            this.IsPK = pk;
        }

        /// <summary>
        /// 仅使用列名初始化对象
        /// </summary>
        /// <param name="name"></param>
        public ColumnAttribute(string name) : this(name, false) { }
        /// <summary>
        /// 仅使用主键字段初始化对象
        /// </summary>
        /// <param name="pk"></param>
        public ColumnAttribute(bool pk) : this(string.Empty, pk) { }
    }

    /// <summary>
    /// 属性扩展方法
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// 解析属性值的<c>ColumnAttribute</c>特性, 获取其列字段名称.
        /// 如果已经特殊标识其映射字段, 则返回其标识值; 否则返回字段名.
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static string ColumnName(this PropertyInfo pi)
        {
            var cas = pi.GetCustomAttributes<ColumnAttribute>();
            if (cas.Count() != 1)
                return pi.Name;

            var cn = cas.FirstOrDefault().Name;
            if (cn.IsNullOrEmptyOrWhiteSpace())
                return pi.Name;

            return cn;
        }
    }
}
