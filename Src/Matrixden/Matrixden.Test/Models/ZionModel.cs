using System;
using System.Linq;
using System.Reflection;
using Matrixden.DBUtilities.Attributes;
using Matrixden.Utils.Extensions;

namespace Matrixden.Zion.Models
{
    public class ZionModel
    {
        [SqlSerializable(SerializationFlags.IgnoreOnUpdate)]
        public virtual Guid ID { get; set; }

        public virtual string Remark { get; set; }

        [SqlSerializable(SerializationFlags.IgnoreOnInsert | SerializationFlags.IgnoreOnUpdate)]
        public virtual DateTime CreateTime { get; set; }

        [SqlSerializable(SerializationFlags.IgnoreOnUpdate)]
        public virtual int CreateMan { get; set; }

        [SqlSerializable(SerializationFlags.IgnoreOnInsert | SerializationFlags.IgnoreOnUpdate)]
        public virtual DateTime UpdateTime { get; set; }

        [SqlSerializable(SerializationFlags.IgnoreOnInsert)]
        public virtual int UpdateMan { get; set; }

        public virtual short Status { get; set; }

        /// <summary>
        /// 实体对应数据库表名
        /// </summary>
        [SqlSerializable(SerializationFlags.Ignore)]
        public string TableName
        {
            get
            {
                string tbn;
                var cas = this.GetType().GetCustomAttributes<TableAttribute>(false);
                if (!cas.Any())
                {
                    tbn = this.GetType().Name;
                }
                else if (cas.Count() == 1)
                {
                    tbn = cas.Select(s => s.Name).FirstOrDefault();
                    if (tbn.IsNullOrEmptyOrWhiteSpace())
                        tbn = this.GetType().Name;
                }
                else
                {
                    throw new CustomAttributeFormatException("Multiple table attributes found.");
                }

                return tbn;
            }
        }
    }
}
