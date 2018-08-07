using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Resources
{
    /// <summary>
    /// 数据行标识
    /// </summary>
    public struct DBColumn_StatusCode
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const Int16 DB_ROW_STATUS_ENABLED = 1;
        /// <summary>
        /// 已删除
        /// </summary>
        public const Int16 DB_ROW_STATUS_DELETED = -1;
    }
}
