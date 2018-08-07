using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Resources
{
    /// <summary>
    /// 数据库操作结果信息
    /// </summary>
    public class DBOperationMessage
    {
        public static string Fail = "操作失败!";
        public static string Create_Fail = "添加失败!";
        public static string Query_Fail = "查询失败!";
        public static string Update_Fail = "更新失败!";
        public static string Delete_Fail = "删除失败!";
        public static string Save_Fail = "保存失败!";
        public static string Query_WithoutData = "查无数据";
    }
}
