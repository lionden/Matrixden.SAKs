using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SqlSerializableAttribute : Attribute
    {
        public SerializationFlags Serializable { get; set; }

        public SqlSerializableAttribute()
        {
            Serializable = SerializationFlags.Serialize;
        }

        public SqlSerializableAttribute(SerializationFlags able)
        {
            Serializable = able;
        }
    }

    [Flags]
    public enum SerializationFlags : int
    {
        None = 0,
        Serialize = 0x1,
        /// <summary>
        /// 生成sql脚本时，忽略该字段
        /// </summary>
        Ignore = 0x1 << 11,
        /// <summary>
        /// 生成查询语句时，忽略该字段
        /// </summary>
        IgnoewOnQuery = 0x1 << 12,
        /// <summary>
        /// 生成插入语句时，忽略该字段
        /// </summary>
        IgnoreOnInsert = 0x1 << 13,
        /// <summary>
        /// 生成更新语句时，忽略该字段
        /// </summary>
        IgnoreOnUpdate = 0x1 << 14,
        All = Serialize | Ignore | IgnoewOnQuery | IgnoreOnInsert | IgnoreOnUpdate
    }
}
