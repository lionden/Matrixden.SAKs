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
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class MySqlRepository : DBRepository
    {
        private MySqlRepository() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        public MySqlRepository(string connStr) : base(
            new ConnectionStringSettings(DataAccessHelper.APP_CONFIG_DB_CONNCTION, connStr,
                DataAccessHelper.PROVIDER_NAME_MYSQL))
        {
        }

        private static readonly object Locker = new object();
        private static MySqlRepository _instance;

        /// <summary>
        /// 
        /// </summary>
        public new static MySqlRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new MySqlRepository();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <inheritdoc />
        public override OperationResult GetByCondition(Type type, string strTableName, string strColumns,
            string strCondition, string strOrder)
        {
            if (strTableName.IsNullOrEmptyOrWhiteSpace())
                return OperationResult.False;

            if (strColumns.IsNullOrEmptyOrWhiteSpace() || "*".Equals(strColumns.CleanUp())) //属性列
            {
                var pis = GenerateDataTableColumnsFromEntityWithFilter(type,
                    SerializationFlags.Ignore | SerializationFlags.IgnoewOnQuery);
                if (pis == null || !pis.Any())
                    return OperationResult.False;

                var cls = pis.Select(p => p.Name);
                strColumns = $"`{string.Join("`,`", cls)}`";
            }

            var sbSql = new StringBuilder(
                $"SELECT {strColumns} FROM {strTableName} WHERE Status !={DBColumn_StatusCode.DB_ROW_STATUS_DELETED} ");
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
            if (CommonClass.GetFieldValue(item, pk) == null)
            {
                log.WarnFormat("PK[{0}]'s value is null.", pk);
                return false;
            }

            var iu = GenerateInsertOrUpdateSqlWithParameters(item.GetType());

            return DataAccess.ExecuteNonQuery(iu, item) >= 1;
        }

        /// <inheritdoc />
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
        public override bool IsDataRowExist(string strDataTable, string strCondition)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(strDataTable, strCondition))
                return false;

            var strSql = $"SELECT 1 FROM {strDataTable} WHERE {strCondition} LIMIT 1;";
            var r = DataAccess.GetSingleRowValue(strSql);

            return r != null && r.Length == 1 && "1".Equals(r[0]);
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
        public override string GenerateInsertSqlWithParameters(string table, object item)
        {
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace())
                return strSql;

            try
            {
                var strColumns =
                    GenerateDataTableColumnsFromEntityWithFilter(item,
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
        public override string GenerateUpdateSqlWithParameters<T>(string table, string condition, T t)
        {
            string strSql = string.Empty;
            if (table.IsNullOrEmptyOrWhiteSpace() || condition.IsNullOrEmptyOrWhiteSpace() || t == null)
                return strSql;

            try
            {
                var prps = GenerateDataTableColumnsFromEntityWithFilter(t,
                    SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore);
                if (prps == null || !prps.Any())
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

                if (!strColumns.Any())
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
        public override string GenerateInsertOrUpdateSqlWithParameters(Type type)
        {
            return Do(type, tbn =>
            {
                var strSql = string.Empty;
                try
                {
                    var strIC = GenerateDataTableColumnsFromEntityWithFilter(type,
                        SerializationFlags.IgnoreOnInsert | SerializationFlags.Ignore).Select(p => p.Name);
                    var strUC = GenerateDataTableColumnsFromEntityWithFilter(type,
                            SerializationFlags.IgnoreOnUpdate | SerializationFlags.Ignore)
                        .Select(p => $"`{p.Name}`=@{p.Name}");
                    if (!strIC.Any() || !strUC.Any())
                        return strSql;

                    strSql = $@"
INSERT INTO `{tbn}` (`{string.Join("`,`", strIC)}`) VALUES(@{string.Join(",@", strIC)})
ON DUPLICATE KEY
UPDATE {string.Join(",", strUC)}";
                    log.Debug(strSql);
                }
                catch (Exception ex)
                {
                    log.ErrorException("Failed to generate insert or update sql script from object.", ex);
                }

                return strSql;
            });
        }
    }
}