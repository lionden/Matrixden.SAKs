/*
 * 
 */

namespace Matrixden.DBUtilities
{
    using Matrixden.DBUtilities.Attributes;
    using Matrixden.DBUtilities.Logging;
    using Matrixden.DBUtilities.Resources;
    using Matrixden.UnifiedDBAdapter;
    using Matrixden.Utils;
    using Matrixden.Utils.Extensions;
    using Matrixden.Utils.Models;
    using System;
    using System.Configuration;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class MSSqlRepository : DBRepository
    {
        private MSSqlRepository() : base()
        {
        }

        public MSSqlRepository(string connStr) : base(
            new ConnectionStringSettings(DataAccessHelper.APP_CONFIG_DB_CONNCTION, connStr,
                DataAccessHelper.PROVIDER_NAME_MSSQL))
        {
        }

        private static object locker = new object();
        private static MSSqlRepository instance;

        public new static MSSqlRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new MSSqlRepository();
                        }
                    }
                }

                return instance;
            }
        }

        /// <inheritdoc />
        public override OperationResult GetByCondition(Type type, string strTableName, string strColumns,
            string strCondition, string strOrder)
        {
            if (strTableName.IsNullOrEmptyOrWhiteSpace())
                return OperationResult.False;

            var sb = new StringBuilder(" ");
            if (strColumns.IsNullOrEmptyOrWhiteSpace() || "*".Equals(strColumns.CleanUp())) //属性列
            {
                foreach (var property in GenerateDataTableColumnsFromEntity(type, SerializationFlags.All))
                {
                    sb.AppendFormat("{0},", property.Name); // property.Name + ",";    //添加属性列名称
                }

                strColumns = sb.Remove(sb.Length - 1, 1).ToString();
            }
            else
            {
                //TODO: check all the columns exist or not
            }

            var sbSql = new StringBuilder($"SELECT {strColumns} FROM {strTableName} WHERE (CASE COL_LENGTH('{strTableName}', 'Status') WHEN 1 THEN Status ELSE '1' END)!='{DBColumn_StatusCode.DB_ROW_STATUS_DELETED}' ");
            if (strCondition.IsNotNullNorEmptyNorWhitespace())
                sbSql.AppendFormat(" AND {0}", strCondition); //添加查询条件

            if (strOrder.IsNotNullNorEmptyNorWhitespace())
                sbSql.AppendFormat(" ORDER BY {0}", strOrder);

            return new OperationResult(DataAccess.Query(sbSql.ToString()));
        }

        /// <inheritdoc />
        public override bool Save(object item)
        {
            if (item == default(object))
            {
                log.Warn("Instance is null.");
                return false;
            }

            var pk = DBUtil.GetPrimaryKeyName(item);
            if (CommonClass.GetPropertyValue(item, pk) == null)
            {
                log.WarnFormat("PK' value is null.");
                return false;
            }

            return Do(item.GetType(), tbn =>
            {
                var utp = GenerateUpdateSqlWithParametersFromObject(tbn, item);
                var itp = GenerateInsertSqlWithParametersFromObject(tbn, item);

                return DataAccess.ExecuteNonQuery(
                           $"IF EXISTS (SELECT {pk} FROM {tbn} WHERE {pk}=@{pk}) {utp.Item1} WHERE {pk}=@{pk} ELSE {itp.Item1};",
                           itp.Item2) == 1;
            });
        }

        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strSets">要更新的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”. 无需包含[UpdateTime]字段.</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public override bool Update(string strTableName, string strSets, string strCondition)
        {
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strSets.IsNullOrEmptyOrWhiteSpace() ||
                strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            // ToDo: Need to make sure there is no UpdateTime column in the strSets. If not, replace its value.
            if (strSets.Contains(DBTableCommonColumns.UpdateTime))
            {
            }

            string strSql =
                "UPDATE " + strTableName + " SET " + strSets + "," + DBTableCommonColumns.UpdateTime +
                "=GETDATE() WHERE " + strCondition; //根据指定查询条件，更新数据记录
            bool bResult = DataAccess.ExecSql(strSql);
            return bResult;
        }

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <param name="strDataTable"></param>
        /// <param name="strCondition">数据库查询条件, 不含where关键字</param>
        /// <returns></returns>
        public override bool IsDataRowExist(string strDataTable, string strCondition)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(strDataTable, strCondition))
                return false;

            string strSql = $"SELECT TOP 1 1 FROM {strDataTable} WHERE {strCondition};";
            var r = DataAccess.GetSingleRowValue(strSql);

            return r != null && r.Length == 1 && "1".Equals(r[0]);
        }

        /// <summary>
        /// 此方法仅用于逻辑删除的数据库表, 即仅有增, 改, 查操作权限的表.
        /// 对带有物理删除的数据库表, 不适用.
        /// 如果在校验过程中, 发生UnExpected结果, 直接返回True.
        /// Microsoft SQL Server中, 使用"[Flags] [timestamp] NOT NULL"字段跟踪数据行的变化;
        /// </summary>
        /// <param name="table"></param>
        /// <param name="originalCount"></param>
        /// <param name="originalLatestUpdateFlag"></param>
        /// <param name="conditionStr"></param>
        /// <returns></returns>
        public override bool IsTableDataChanged(string table, int originalCount, object originalLatestUpdateFlag,
            string conditionStr)
        {
            if (originalCount != DataAccess.GetCount(table, conditionStr))
                return true;

            string sql =
                $@"SELECT TOP 1 Flags FROM {table} ORDER BY Flags DESC {(conditionStr.IsNullOrEmptyOrWhiteSpace() ? "" : " WHERE " + conditionStr)};";
            var res = DataAccess.GetArray(sql);
            if (res == null || res.Length != 1)
            {
                return originalLatestUpdateFlag != null;
            }

            return originalLatestUpdateFlag == null || !originalLatestUpdateFlag.ToString().Equals(res[0]);
        }

        /// <inheritdoc />
        public override string GenerateInsertSqlWithParameters(string table, object item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GenerateUpdateSqlWithParameters<T>(string table, string condition, T t)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GenerateInsertOrUpdateSqlWithParameters(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
