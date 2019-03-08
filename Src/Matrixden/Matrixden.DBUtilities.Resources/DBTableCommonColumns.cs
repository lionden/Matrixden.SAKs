namespace Matrixden.DBUtilities.Resources
{
    /// <summary>
    /// 数据库表通用字段
    /// </summary>
    public class DBTableCommonColumns
    {
        public const string Id = "Id";
        public const string No = "No";
        public const string CreateMan = "CreateMan";
        public const string CreateTime = "CreateTime";
        public const string UpdateMan = "UpdateMan";
        public const string UpdateTime = "UpdateTime";
        public const string DeleteMan = "DeleteMan";
        public const string DeleteTime = "DeleteTime";
        public const string Status = "Status";
        public const string Remark = "Remark";
        public const string create_man = "create_man";
        public const string create_time = "create_time";
        public const string update_man = "update_man";
        public const string update_time = "update_time";
        public const string delete_man = "delete_man";
        public const string delete_time = "delete_time";
    }

    /// <summary>
    /// 数据库表通用字段枚举
    /// </summary>
    public enum DBTableCommonColumnsEnum
    {
        Id,
        No,
        CreateMan,
        CreateTime,
        UpdateMan,
        UpdateTime,
        DeleteMan,
        DeleteTime,
        create_man,
        create_time,
        update_man,
        update_time,
        delete_man,
        delete_time,
        Status,
        Remark
    }
}
