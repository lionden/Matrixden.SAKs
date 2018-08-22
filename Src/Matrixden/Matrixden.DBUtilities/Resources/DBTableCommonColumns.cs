using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Resources
{
    /// <summary>
    /// 数据库表通用字段
    /// </summary>
    public class DBTableCommonColumns
    {
        public const string ID = "ID";
        public const string Id = "Id";
        public const string id = "id";
        public const string CreateMan = "CreateMan";
        public const string CreateTime = "CreateTime";
        public const string UpdateMan = "UpdateMan";
        public const string UpdateTime = "UpdateTime";
        public const string DeleteMan = "DeleteMan";
        public const string DeleteTime = "DeleteTime";
        public const string Status = "Status";
        public const string Remark = "Remark";
    }

    /// <summary>
    /// 数据库表通用字段枚举
    /// </summary>
    public enum DBTableCommonColumnsEnum
    {
        ID,
        Id,
        id,
        CreateMan,
        CreateTime,
        UpdateMan,
        UpdateTime,
        DeleteMan,
        DeleteTime,
        Status,
        Remark
    }
}
