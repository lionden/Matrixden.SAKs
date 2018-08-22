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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MySqlRepository : DBRepository
    {
        private MySqlRepository() : base() { }
        public MySqlRepository(string connStr) : base(new ConnectionStringSettings(DataAccessHelper.APP_CONFIG_DB_CONNCTION, connStr, DataAccessHelper.PROVIDER_NAME_MYSQL)) { }

        private static object locker = new object();
        private static MySqlRepository instance;
        public static new MySqlRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new MySqlRepository();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 根据指定表名和查询条件(也可无查询条件)，返回指定属性列的数据对象数组
        /// </summary>
        /// <typeparam name="T">泛型，返回的指定数据对象类型</typeparam>
        /// <param name="strColumns">所选列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]” (如果给空值或者null 等价于 *)</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns>返回的数据记录对象数组</returns>
        public override OperationResult GetByCondition<T>(string strColumns = null, string strCondition = null, string strOrder = null)
        {
            return Do<T>(tbn =>
            {
                var sbSql = new StringBuilder();
                try
                {
                    if (tbn.IsNullOrEmptyOrWhiteSpace())
                        return new OperationResult(false);

                    if (strColumns.IsNullOrEmptyOrWhiteSpace() || "*".Equals(strColumns.CleanUp())) //属性列
                    {
                        var pis = GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags.Ignore | SerializationFlags.IgnoewOnQuery);
                        if (pis == null || pis.Count() <= 0)
                            return new OperationResult(false);

                        var cls = pis.Select(p => p.Name);
                        if (cls == null || cls.Count() <= 0)
                            return new OperationResult(false);

                        strColumns = string.Format("`{0}`", string.Join("`,`", cls));
                    }

                    sbSql.AppendFormat("SELECT {0} FROM {1} WHERE Status !={2} ", strColumns, tbn, DBColumn_StatusCode.DB_ROW_STATUS_DELETED);
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
            });
        }

        /// <summary>
        /// 根据条件保存实体, 如果存在则更新, 否则插入.
        /// 仅适用于有主键索引的表，且PK参数为主键字段。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Save<T>(T item)
        {
            if (item == default(T))
            {
                log.WarnFormat("Instance[{0}] is null.", typeof(T));
                return false;
            }

            if (CommonClass.GetFieldValue(item, DBUtil.GetPrimaryKeyName<T>()) == null)
            {
                log.WarnFormat("PK' value is null.");
                return false;
            }

            var iu = GenerateInsertOrUpdateSQLWithParameters<T>();

            return DataAccess.ExecuteNonQuery(iu, item) >= 1;
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

            // ToDo: Need to make sure there is no UpdateTime column in the strSets. If not, remove it.
            if (strSets.Contains(DBTableCommonColumns.UpdateTime))
            {
            }

            string strSql = "UPDATE " + strTableName + " SET " + strSets + " WHERE " + strCondition;  //根据指定查询条件，更新数据记录
            bool bResult = DataAccess.ExecSql(strSql);

            return bResult;
        }

        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="strSets">要更新的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”. 无需包含[UpdateTime]字段.</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns></returns>
        public override OperationResult Update<T>(string strSets, string strCondition)
        {
            return Do<T>(tbn =>
            {
                return new OperationResult(Update(tbn, strSets, strCondition));
            });
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

            string strSql = string.Format("SELECT 1 FROM {0} WHERE {1} LIMIT 1;", strDataTable, strCondition);
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
            if (strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            return Do<T>(tbn =>
            {
                return IsDataRowExist(tbn, strCondition);
            });
        }

        /// <summary>
        /// 此方法仅用于逻辑删除的数据库表, 即仅有增, 改, 查操作权限的表.
        /// 对带有物理删除的数据库表, 不适用.
        /// 如果在校验过程中, 发生UnExpected结果, 直接返回True.
        /// MySQL中, 则使用"`UpdateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP"字段跟踪数据行的变化.
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

            string sql = string.Format(@"SELECT UpdateTime FROM {0} ORDER BY UpdateTime DESC {1} LIMIT 1;",
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
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace())
                return strSql;

            try
            {
                var strColumns = GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags.IgnoreOnInsert | SerializationFlags.Ignore).Select(p => p.Name);
                if (strColumns == null || strColumns.Count() <= 0)
                    return strSql;

                strSql = string.Format(";INSERT INTO `{0}` (`{1}`) VALUES(@{2});", table, string.Join("`,`", strColumns.ToArray()), string.Join(",@", strColumns.ToArray()));
#if DEBUG
                log.Debug(strSql);
#endif
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to generate insert sql script from object. For more: {0}.", ex);
            }

            return strSql;
        }

        /// <summary>
        /// 根据实体生成Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <returns></returns>
        public override string GenerateUpdateSQLWithParameters<T>(string table, string condition, T t)
        {
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace() || condition.IsNullOrEmptyOrWhiteSpace() || t == null)
                return strSql;

            try
            {
                var prps = GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore);
                if (prps == null || prps.Count() <= 0)
                    return strSql;

                ConcurrentBag<string> strColumns = new ConcurrentBag<string>();
                Parallel.ForEach(prps, p =>
                {
                    var val = CommonClass.GetFieldValue(t, p.Name);
                    if (val == null)
                        return;

                    switch (p.PropertyType.ToString())
                    {
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Double":
                        case "System.Decimal":
                            if (0 != val.ToDecimal())
                                strColumns.Add(string.Format("`{0}`=@{0}", p.Name));
                            break;
                        case "System.DateTime":
                            if (default(DateTime) != val.ToDateTime())
                                strColumns.Add(string.Format("`{0}`=@{0}", p.Name));
                            break;
                        default:
                            strColumns.Add(string.Format("`{0}`=@{0}", p.Name));
                            break;
                    }
                });

                if (strColumns == null || strColumns.Count() <= 0)
                    return strSql;

                strSql = string.Format(";UPDATE `{0}` SET {1} WHERE {2};", table, string.Join(",", strColumns.ToArray()), condition);
#if DEBUG
                log.Debug(strSql);
#endif
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to generate update sql script from object. For more: {0}.", ex);
            }

            return strSql;
        }

        /// <summary>
        /// 根据实体生成Insert or Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override string GenerateInsertOrUpdateSQLWithParameters<T>()
        {
            return Do<T>(tbn =>
            {
                string strSql = string.Empty;
                try
                {
                    var strIC = GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags.IgnoreOnInsert | SerializationFlags.Ignore).Select(p => p.Name);
                    var strUC = GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore).Select(p => string.Format("`{0}`=@{0}", p.Name));
                    if (strIC == null || strIC.Count() <= 0 || strUC == null || strUC.Count() <= 0)
                        return strSql;

                    strSql = string.Format(@"
INSERT INTO `{0}` (`{1}`) VALUES(@{2})
ON DUPLICATE KEY
UPDATE {3}",
               tbn,
               string.Join("`,`", strIC),
               string.Join(",@", strIC),
               string.Join(",", strUC));
                    log.Debug(strSql);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Failed to generate insert or update sql script from object. For more: {0}.", ex);
                }

                return strSql;
            });
        }
    }
}