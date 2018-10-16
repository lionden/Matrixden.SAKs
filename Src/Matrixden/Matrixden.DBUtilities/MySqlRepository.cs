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
        private MySqlRepository() : base()
        {
        }

        public MySqlRepository(string connStr) : base(
            new ConnectionStringSettings(DataAccessHelper.APP_CONFIG_DB_CONNCTION, connStr,
                DataAccessHelper.PROVIDER_NAME_MYSQL))
        {
        }

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
                    var pis = GenerateDatatableColumnsFromEntityWithFilter<T>(
                        SerializationFlags.Ignore | SerializationFlags.IgnoewOnQuery);
                    if (pis == null || pis.Count() <= 0)
                        return new OperationResult(false);

                    var cls = pis.Select(p => p.Name);
                    if (cls == null || cls.Count() <= 0)
                        return new OperationResult(false);

                    strColumns = $"`{string.Join("`,`", cls)}`";
                }

                sbSql.AppendFormat("SELECT {0} FROM {1} WHERE Status !={2} ", strColumns, strTableName,
                    DBColumn_StatusCode.DB_ROW_STATUS_DELETED);
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
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strSets.IsNullOrEmptyOrWhiteSpace() ||
                strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            // ToDo: Need to make sure there is no UpdateTime column in the strSets. If not, remove it.
            if (strSets.Contains(DBTableCommonColumns.UpdateTime))
            {
            }

            string strSql = "UPDATE " + strTableName + " SET " + strSets + " WHERE " + strCondition; //根据指定查询条件，更新数据记录
            bool bResult = DataAccess.ExecSql(strSql);

            return bResult;
        }

        /// <inheritdoc />
        public override OperationResult Update<T>(string strSets, string strCondition)
        {
            return Do<T>(tbn => new OperationResult(Update(tbn, strSets, strCondition)));
        }

        /// <inheritdoc />
        public override bool IsDataRowExist(string strDataTable, string strCondition)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(strDataTable, strCondition))
                return false;

            var strSql = $"SELECT 1 FROM {strDataTable} WHERE {strCondition} LIMIT 1;";
            var r = DataAccess.GetSingleRowValue(strSql);

            return r != null && r.Length == 1 && "1".Equals(r[0]);
        }

        /// <inheritdoc />
        public override bool IsDataRowExist<T>(string strCondition)
        {
            if (strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            return Do<T>(tbn => { return IsDataRowExist(tbn, strCondition); });
        }

        /// <inheritdoc />
        public override bool IsTableDataChanged(string table, int originalCount, object originalLatestUpdateFlag,
            string conditionStr)
        {
            if (originalCount != DataAccess.GetCount(table, conditionStr))
                return true;

            var sql =
                $@"SELECT UpdateTime FROM {table} ORDER BY UpdateTime DESC {(conditionStr.IsNullOrEmptyOrWhiteSpace() ? string.Empty : $" WHERE {conditionStr}")} LIMIT 1;";
            var res = DataAccess.GetArray(sql);
            if (res == null || res.Length != 1)
            {
                return originalLatestUpdateFlag != null;
            }

            return originalLatestUpdateFlag == null || !originalLatestUpdateFlag.ToString().Equals(res[0]);
        }

        /// <inheritdoc />
        public override string GenerateInsertSQLWithParameters<T>(string table)
        {
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace())
                return strSql;

            try
            {
                var strColumns =
                    GenerateDatatableColumnsFromEntityWithFilter<T>(
                        SerializationFlags.IgnoreOnInsert | SerializationFlags.Ignore).Select(p => p.Name);
                var ca = strColumns.ToArray();
                if (!ca.Any())
                    return strSql;

                strSql =
                    $";INSERT INTO `{table}` (`{string.Join("`,`", ca)}`) VALUES(@{string.Join(",@", ca)});";
#if DEBUG
                log.Debug(strSql);
#endif
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate insert sql script from object.", ex);
            }

            return strSql;
        }

        /// <inheritdoc />
        public override string GenerateUpdateSQLWithParameters<T>(string table, string condition, T t)
        {
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace() || condition.IsNullOrEmptyOrWhiteSpace() || t == null)
                return strSql;

            try
            {
                var prps = GenerateDatatableColumnsFromEntityWithFilter<T>(
                    SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore);
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

                strSql = $";UPDATE `{table}` SET {string.Join(",", strColumns.ToArray())} WHERE {condition};";
#if DEBUG
                log.Debug(strSql);
#endif
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate update sql script from object.", ex);
            }

            return strSql;
        }

        /// <inheritdoc />
        public override string GenerateInsertOrUpdateSQLWithParameters<T>()
        {
            return Do<T>(tbn =>
            {
                string strSql = string.Empty;
                try
                {
                    var strIC = GenerateDatatableColumnsFromEntityWithFilter<T>(
                        SerializationFlags.IgnoreOnInsert | SerializationFlags.Ignore).Select(p => p.Name);
                    var strUC =
                        GenerateDatatableColumnsFromEntityWithFilter<T>(
                                SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore)
                            .Select(p => string.Format("`{0}`=@{0}", p.Name));
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