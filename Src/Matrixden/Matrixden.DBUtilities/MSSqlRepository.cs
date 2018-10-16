/*
 * 
 */

namespace Matrixden.DBUtilities
{
    using Matrixden.DBUtilities.Logging;
    using Matrixden.DBUtilities.Resources;
    using Matrixden.UnifiedDBAdapter;
    using Matrixden.Utils;
    using Matrixden.Utils.Extensions;
    using Matrixden.Utils.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;

    public class MSSqlRepository : DBRepository
    {
        private MSSqlRepository() : base() { }
        public MSSqlRepository(string connStr) : base(new ConnectionStringSettings(DataAccessHelper.APP_CONFIG_DB_CONNCTION, connStr, DataAccessHelper.PROVIDER_NAME_MSSQL)) { }

        private static object locker = new object();
        private static MSSqlRepository instance;
        public static new MSSqlRepository Instance
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
        public override OperationResult GetByCondition<T>(string strTableName, string strColumns, string strCondition, string strOrder)
        {
            var sbSql = new StringBuilder();
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace())
                    return new OperationResult(false);

                if (strColumns.IsNullOrEmptyOrWhiteSpace() || "*".Equals(strColumns.CleanUp())) //属性列
                {
                    StringBuilder sb = new StringBuilder(" ");
                    foreach (System.Reflection.PropertyInfo property in GenerateDatatableColumnsFromEntity<T>())
                    {
                        sb.AppendFormat("{0},", property.Name);// property.Name + ",";    //添加属性列名称
                    }

                    strColumns = sb.Remove(sb.Length - 1, 1).ToString();
                }
                else
                {
                    //TODO: check all the columns exist or not
                }

                sbSql.AppendFormat("SELECT {0} FROM {1} WHERE (CASE COL_LENGTH('{1}', 'Status') WHEN 1 THEN Status ELSE '1' END)!='{2}' ", strColumns, strTableName, DBColumn_StatusCode.DB_ROW_STATUS_DELETED);
                if (strCondition.IsNotNullNorEmptyNorWhitespace())
                    sbSql.AppendFormat(" AND {0}", strCondition); //添加查询条件

                if (strOrder.IsNotNullNorEmptyNorWhitespace())
                    sbSql.AppendFormat(" ORDER BY {0}", strOrder);

                return new OperationResult(GetBySqLCommand<T>(sbSql.ToString()));
            }
            catch (Exception ex)
            {
                log.ErrorException("SQL Command: {0}.", ex, sbSql);
            }

            return new OperationResult(false);
        }

        /// <inheritdoc />
        public override OperationResult GetByCondition<T>(string strColumns, string strCondition, string strOrder)
        {
            return Do<T>(tbn => GetByCondition<T>(tbn, strColumns, strCondition, strOrder));
        }

        /// <summary>
        /// 根据条件保存实体, 如果存在则更新, 否则插入.
        /// 仅适用于有主键索引的表，且PK参数为主键字段。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="pk">主键字段</param>
        /// <returns></returns>
        public override bool Save<T>(T item)
        {
            if (item == default(T))
            {
                log.WarnFormat("Instance[{0}] is null.", typeof(T));
                return false;
            }

            var pk = DBUtil.GetPrimaryKeyName<T>();
            if (CommonClass.GetFieldValue(item, pk) == null)
            {
                log.WarnFormat("PK' value is null.");
                return false;
            }

            return Do<T>(tbn =>
            {
                var utp = GenerateUpdateSQLWithParametersFromObject(tbn, item);
                var itp = GenerateInsertSQLWithParametersFromObject(tbn, item);

                return DataAccess.ExecuteNonQuery(
                        string.Format("IF EXISTS (SELECT {0} FROM {1} WHERE {0}=@{0}) {2} ELSE {3};",
                            pk,
                            tbn,
                            utp.Item1 + string.Format(" WHERE {0}=@{0}", pk),
                            itp.Item1),
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
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strSets.IsNullOrEmptyOrWhiteSpace() || strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            // ToDo: Need to make sure there is no UpdateTime column in the strSets. If not, replace its value.
            if (strSets.Contains(DBTableCommonColumns.UpdateTime))
            {
            }

            string strSql =
                "UPDATE " + strTableName + " SET " + strSets + "," + DBTableCommonColumns.UpdateTime + "=GETDATE() WHERE " + strCondition;  //根据指定查询条件，更新数据记录
            bool bResult = DataAccess.ExecSql(strSql);
            return bResult;
        }

        /// <summary>
        /// 更新一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要更新的数据对象的类型</typeparam>
        /// <param name="strTableName">表名</param>
        /// <param name="item">要更新的数据对象</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public override OperationResult Update<T>(string strSets, string strCondition)
        {
            throw new NotImplementedException();
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

            string strSql = string.Format("SELECT TOP 1 1 FROM {0} WHERE {1};", strDataTable, strCondition);
            var r = DataAccess.GetSingleRowValue(strSql);

            return r != null && r.Length == 1 && "1".Equals(r[0]);
        }

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <param name="strCondition">数据库查询条件, 不含where关键字</param>
        /// <returns></returns>
        public override bool IsDataRowExist<T>(string strCondition)
        {
            throw new NotImplementedException();
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
        public override bool IsTableDataChanged(string table, int originalCount, object originalLatestUpdateFlag, string conditionStr)
        {
            if (originalCount != DataAccess.GetCount(table, conditionStr))
                return true;

            string sql = string.Format(@"SELECT TOP 1 Flags FROM {0} ORDER BY Flags DESC {1};",
                            table,
                            conditionStr.IsNullOrEmptyOrWhiteSpace() ? "" : " WHERE " + conditionStr);
            var res = DataAccess.GetArray(sql);
            if (res == null || res.Length != 1)
            {
                return originalLatestUpdateFlag != null;
            }

            return originalLatestUpdateFlag == null || !originalLatestUpdateFlag.ToString().Equals(res[0]);
        }

        /// <summary>
        /// 根据实体生成Insert SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <returns></returns>
        public override string GenerateInsertSQLWithParameters<T>(string table)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据实体生成Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <param name="condition"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string GenerateUpdateSQLWithParameters<T>(string table, string condition, T t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据实体生成Insert or Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override string GenerateInsertOrUpdateSQLWithParameters<T>()
        {
            throw new NotImplementedException();
        }
    }
}
