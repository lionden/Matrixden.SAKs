/*
 * 
 * 操作实例：
       DBRepository repository = new DBRepository();            //实例化类

       //创建数据表的模型对象，通过DBUtility.DBRepository.Models
       UserHistoryModel userHistory = new UserHistoryModel();   //UserHistory表的数据模型
   
       //设置数据表模型参数
       userHistory.id = 3;
       userHistory.acc_time = DateTime.Now;
       userHistory.accessType = 1;
       userHistory.user_link = 1;
       
       //返回全部数据记录
       UserHistoryModel[] items = repository.GetByCondition<UserHistoryModel>("userHistory", "");

       //添加一条新的数据记录
       bool bResult = repository.Add<UserHistoryModel>("userHistory", userHistory);
 
       //根据指定的条件，更新一条数据记录
       bool bResult = repository.Update<UserHistoryModel>("userHistory", userHistory, "id=" + userHistory.id);
 * 
 */

namespace Matrixden.DBUtilities
{
    using Matrixden.DBUtilities.Attributes;
    using Matrixden.DBUtilities.Interfaces;
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
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DBRepository : IDBRepository
    {
        internal static readonly ILog log = LogProvider.GetCurrentClassLogger();
        public static DataAccessHelper DataAccess = new DataAccessHelper();

        protected DBRepository() : this(
            ConfigurationManager.ConnectionStrings[DataAccessHelper.APP_CONFIG_DB_CONNCTION])
        {
        }

        protected DBRepository(ConnectionStringSettings config)
        {
            if (config == default(ConnectionStringSettings))
                throw new ArgumentException();

            switch (config.ProviderName)
            {
                case "MySql.Data.MySqlClient":
                    DataAccessHelper.DataBaseHelper.DatabaseType = DBHelper.DatabaseTypes.MySql;
                    break;
                case "System.Data.SqlClient":
                default:
                    DataAccessHelper.DataBaseHelper.DatabaseType = DBHelper.DatabaseTypes.MSSql;
                    break;
            }

            DataAccessHelper.DataBaseHelper.ConnectionString = config.ConnectionString;
        }

        private static object locker = new object();
        private static DBRepository instance;

        /// <summary>
        /// 单例
        /// </summary>
        public static DBRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            // 从config文件中读取数据库类型
                            try
                            {
                                var cnf = System.Configuration.ConfigurationManager.ConnectionStrings[
                                    DataAccessHelper.APP_CONFIG_DB_CONNCTION];
                                switch (cnf.ProviderName)
                                {
                                    case DataAccessHelper.PROVIDER_NAME_MSSQL:
                                        instance = new MSSqlRepository(cnf.ConnectionString);
                                        break;
                                    case DataAccessHelper.PROVIDER_NAME_MYSQL:
                                    default:
                                        instance = new MySqlRepository(cnf.ConnectionString);
                                        break;
                                }
                            }
                            catch (ConfigurationErrorsException cex)
                            {
                                log.ErrorException("Failed to get configuration info.", cex);
                            }
                            catch (Exception ex)
                            {
                                log.FatalException("Unexpected error occured.", ex);
                            }
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 根据指定表名和查询条件(也可无查询条件)，返回指定属性列的数据对象数组
        /// </summary>
        /// <param name="type">返回的指定数据对象类型</param>
        /// <param name="strTableName">要查询的表名</param>
        /// <param name="strColumns">所选列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]” (如果给空值或者null 等价于 *)</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns>返回的数据记录对象数组</returns>
        public abstract OperationResult GetByCondition(Type type, string strTableName, string strColumns, string strCondition,
            string strOrder);

        /// <summary>
        /// 根据指定表名和查询条件(也可无查询条件)，返回指定属性列的数据对象数组
        /// </summary>
        /// <param name="type">返回的指定数据对象类型</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns></returns>
        public OperationResult GetByCondition(Type type, string strCondition, string strOrder) => Do(type,
            tbn => GetByCondition(type, tbn, string.Empty, strCondition, strOrder));

        /// <summary>
        /// 根据指定表名和查询条件(也可无查询条件)，返回指定属性列的数据对象数组
        /// </summary>
        /// <typeparam name="T">泛型，返回的指定数据对象类型</typeparam>
        /// <param name="strTableName">要查询的表名</param>
        /// <param name="strColumns">所选列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]” (如果给空值或者null 等价于 *)</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns>返回的数据记录对象数组</returns>
        public OperationResult GetByCondition<T>(string strTableName, string strColumns, string strCondition,
            string strOrder) where T : class, new() => GetByCondition(typeof(T), strTableName, strColumns, strCondition, strOrder);

        /// <summary>
        /// 根据指定表名和查询条件(也可无查询条件)，返回指定属性列的数据对象数组
        /// </summary>
        /// <typeparam name="T">泛型，返回的指定数据对象类型</typeparam>
        /// <param name="strColumns">所选列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]” (如果给空值或者null 等价于 *)</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns>返回的数据记录对象数组</returns>
        public OperationResult GetByCondition<T>(string strColumns, string strCondition, string strOrder)
            where T : class, new() => Do<T>(tbn => GetByCondition<T>(tbn, strColumns, strCondition, strOrder));

        /// <summary>
        /// 根据条件保存实体, 如果存在则更新, 否则插入.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract bool Save(object item);

        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strSets">要更新的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”. 无需包含[UpdateTime]字段.</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public abstract bool Update(string strTableName, string strSets, string strCondition);

        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="strSets">要更新的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”. 无需包含[UpdateTime]字段.</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns></returns>
        public OperationResult Update<T>(string strSets, string strCondition) where T : class, new() =>
            Do<T>(tbn => new OperationResult(Update(tbn, strSets, strCondition)));

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <param name="strDataTable"></param>
        /// <param name="strCondition">数据库查询条件, 不含where关键字</param>
        /// <returns></returns>
        public abstract bool IsDataRowExist(string strDataTable, string strCondition);

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strCondition">数据库查询条件, 不含where关键字</param>
        /// <returns></returns>
        public bool IsDataRowExist(Type type, string strCondition) =>
            !strCondition.IsNullOrEmptyOrWhiteSpace() && Do(type, tbn => IsDataRowExist(tbn, strCondition));

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public bool IsDataRowExist<T>(string strCondition) => IsDataRowExist(typeof(T), strCondition);

        /// <summary>
        /// 此方法仅用于逻辑删除的数据库表, 即仅有增, 改, 查操作权限的表.
        /// 对带有物理删除的数据库表, 不适用.
        /// 如果在校验过程中, 发生UnExpected结果, 直接返回True.
        /// Microsoft SQL Server中, 使用"[Flags] [timestamp] NOT NULL"字段跟踪数据行的变化;
        /// MySQL中, 则使用"`Flags` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP"字段跟踪数据行的变化.
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="originalCount">已缓存的数据条</param>
        /// <param name="originalLatestUpdateFlag">缓存结束数据标记.</param>
        /// <param name="conditionStr"></param>
        /// <returns></returns>
        public abstract bool IsTableDataChanged(string table, int originalCount, object originalLatestUpdateFlag,
            string conditionStr);

        /// <summary>
        /// 根据实体生成Insert SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <param name="table">实体对应的表名</param>
        /// <param name="item">实体</param>
        public abstract string GenerateInsertSqlWithParameters(string table, object item);

        /// <summary>
        /// 根据实体生成Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <param name="condition"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract string GenerateUpdateSqlWithParameters<T>(string table, string condition, T t)
            where T : class, new();

        /// <summary>
        /// 根据实体生成Insert or Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract string GenerateInsertOrUpdateSqlWithParameters(Type type);

        /// <summary>
        /// 根据实体生成Insert or Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Use the overload method instead.")]
        public string GenerateInsertOrUpdateSqlWithParameters<T>() where T : class, new() =>
            GenerateInsertOrUpdateSqlWithParameters(typeof(T));

        /// <summary>
        /// 执行SQL语句, 返回数据集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public IEnumerable<T> GetBySqLCommand<T>(string sqlCommand) where T : class, new()
        {
            try
            {
                System.Data.DataSet dataSet = DataAccess.GetDataSet(sqlCommand);

                return GetModels<T>(dataSet);
            }
            catch (Exception ex)
            {
                log.ErrorException("SQL Command: {0}.", ex, sqlCommand);
            }

            return default(IEnumerable<T>);
        }

        /// <summary>
        /// 通过Sql语句，返回记录，返回格式string[]一维数据
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public string[] GetArrayBySqlCommand(string sqlCommand)
        {
            try
            {
                string[] results = DataAccess.GetArray(sqlCommand);
                return results;
            }
            catch (Exception ex)
            {
                log.ErrorException("SQL Command: {0}.", ex, sqlCommand);
            }

            return default(string[]);
        }

        /// <summary>
        /// 执行SQL, 返回第一行第一列数据结果
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public string ExecuteSqlReturnString(string sqlCommand)
        {
            try
            {
                var dataSet = DataAccess.GetDataSet(sqlCommand);
                if (dataSet != null)
                {
                    if (dataSet.Tables.Count > 0)
                    {
                        return dataSet.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("SQL Command: {0}.", ex, sqlCommand);
            }

            return string.Empty;
        }

        /// <summary>
        /// 执行存储过程, 返回数据集合.
        /// </summary>
        /// <typeparam name="T">返回结果的泛型类型.</typeparam>
        /// <param name="procName">存储过程名称.</param>
        /// <param name="paras">存储过程参数列表.</param>
        /// <returns></returns>
        public IEnumerable<T> GetByStoredProcedure<T>(string procName, params DbParameter[] paras)
            where T : class, new()
        {
            try
            {
                var dataSet = DataAccess.ExecuteStoredProcedure(procName, paras);

                return GetModels<T>(dataSet);
            }
            catch (Exception ex)
            {
                log.ErrorException("Procedure Name: {0}.", ex, procName);
            }

            return default(IEnumerable<T>);
        }

        /// <summary>
        /// 添加一条数据记录
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strColumns">要插入的属性列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]”</param>
        /// <param name="strValues">要插入的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”</param>
        /// <returns>是否执行成功</returns>
        public bool Add(string strTableName, string strColumns, string strValues)
        {
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strColumns.IsNullOrEmptyOrWhiteSpace() ||
                strValues.IsNullOrEmptyOrWhiteSpace())
                return false;

            //添加数据记录
            //执行sql语句，返回是否执行成功
            return DataAccess.ExecSql($"INSERT INTO {strTableName} ({strColumns}) VALUES({strValues})");
        }

        /// <summary>
        /// 添加一条数据记录
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public OperationResult Add(string strTableName, object item)
        {
            var result = new OperationResult(DBOperationMessage.Fail);
            if (strTableName.IsNullOrEmptyOrWhiteSpace())
                return result;

            var cnt = 0;
            var sql = instance.GenerateInsertSqlWithParameters(strTableName, item);
            if (sql.IsNullOrEmptyOrWhiteSpace())
                return result;

            try
            {
                if (CommonClass.GetPropertyValue(item, DBTableCommonColumns.ID) == null)
                    CommonClass.SetPropertyValue(item, DBTableCommonColumns.ID, Guid.NewGuid());
                cnt = DataAccess.ExecuteNonQuery(sql, item);
            }
            catch (SqlException sEx)
            {
                log.ErrorException("Error occured during SQL execute, Table: {0},\r\nSQL {1}.", sEx, strTableName, sql);
                cnt = -1;
                if (sEx.Message.Contains("违反了 PRIMARY KEY 约束") || sEx.Message.Contains("违反了 UNIQUE KEY 约束"))
                {
                    result.Message = "检查主键或唯一键是否重复.";
                }
                else if (sEx.Message.Contains("不能将值 NULL 插入列"))
                {
                    result.Message = "存在必填项为空.";
                }
            }
            catch (Exception ex)
            {
                cnt = -2;
                log.FatalException("Unexpected error occured. SQL: {0}.", ex, sql);
            }

            result.Result = cnt == 1;
            result.Data = cnt;

            return result;
        }

        /// <summary>
        /// 添加一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要插入的数据对象的类型</typeparam>
        /// <param name="item">要插入的数据对象</param>
        /// <returns>是否执行成功</returns>
        public OperationResult Add<T>(T item) where T : class, new() => Do(typeof(T), tbn => Add(tbn, item));

        /// <summary>
        /// 同时向数据库中添加多条数据.
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="items">实体值</param>
        /// <returns></returns>
        public OperationResult AddBulk<T>(string tableName, IEnumerable<T> items) where T : class, new()
        {
            var result = new OperationResult(DBOperationMessage.Fail);
            if (tableName.IsNullOrEmptyOrWhiteSpace() || items == null || !items.Any())
                return result;

            var cnt = 0;
            var sql = instance.GenerateInsertSqlWithParameters(tableName, items);
            if (sql.IsNullOrEmptyOrWhiteSpace())
                return result;

            try
            {
                cnt = DataAccess.ExecuteNonQuery(sql, items);
            }
            catch (Exception ex)
            {
                cnt = -1;
                log.ErrorException("Failed to execute non-query.", ex, sql);
                result.Message = ex.Message;
            }

            if (cnt == items.Count())
            {
                result = new OperationResult
                {
                    Result = true,
                    Data = cnt
                };
            }
            else if (cnt > 0)
            {
                result.Data = cnt;
                result.Message = "不是所有的数据都保存成功. Not all saved successfully.";
            }

            return result;
        }

        /// <summary>
        /// 同时向数据库中添加多条数据.
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="items">实体值</param>
        /// <returns></returns>
        public OperationResult AddBulk<T>(IEnumerable<T> items) where T : class, new() =>
            Do<T>(tbn => AddBulk<T>(tbn, items));

        /// <summary>
        /// 添加一条数据记录, 返回受影响的行数.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns>返回添加的行数.</returns>
        public int AddThenGetAffectedRows<T>(string strTableName, T item) where T : class, new()
        {
            var tp = GenerateInsertSqlWithParametersFromObject(strTableName, item);
            if (tp == null)
                return -1;

            try
            {
                return DataAccess.InsertGetAffectedRows(tp.Item1, tp.Item2);
            }
            catch (Exception ex)
            {
                log.ErrorException("表{0}主键重复, SQL: {1}.", ex, strTableName, tp.Item1);
            }

            return -1;
        }

        /// <summary>
        /// 根据条件从数据库中删除数据记录
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public bool Remove(string strTableName, string strCondition)
        {
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strCondition.IsNullOrEmptyOrWhiteSpace())
                return false;

            //根据指定查询条件，删除数据记录
            return DataAccess.ExecSql($"DELETE FROM {strTableName} WHERE {strCondition}");
        }

        /// <summary>
        /// 标记数据被删除, 非实际删除.
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public bool Remove_Logic(string strTableName, string strCondition) => Update(strTableName,
            $"{DBTableCommonColumns.Status}='{DBColumn_StatusCode.DB_ROW_STATUS_DELETED}'",
            strCondition);

        /// <summary>
        /// 更新一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要更新的数据对象的类型</typeparam>
        /// <param name="strTableName">表名</param>
        /// <param name="item">要更新的数据对象</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public OperationResult Update<T>(string strTableName, T item, string strCondition) where T : class, new()
        {
            var result = new OperationResult(DBOperationMessage.Fail);
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || item == default(T) ||
                strCondition.IsNullOrEmptyOrWhiteSpace())
                return result;

            var cnt = 0;
            var sql = instance.GenerateUpdateSqlWithParameters<T>(strTableName, strCondition, item);
            if (sql.IsNullOrEmptyOrWhiteSpace())
                return result;

            try
            {
                cnt = DataAccess.ExecuteNonQuery(sql, item);
            }
            catch (SqlException sEx)
            {
                cnt = -1;
                result.Message = sEx.Message;
                log.ErrorException("Failed to execute non-query. SQL=[{0}]", sEx, sql);
            }
            catch (Exception ex)
            {
                cnt = -2;
                result.Message = ex.Message;
                log.FatalException("Unexpected exception occured.", ex);
            }

            result.Result = cnt == 1;
            result.Data = cnt;

            return result;
        }

        /// <summary>
        /// 更新一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要更新的数据对象的类型</typeparam>
        /// <param name="item">要更新的数据对象</param>
        /// <returns>是否执行成功</returns>
        public bool Update<T>(T item) where T : class, new() => Update(item, "ID=@ID");

        /// <summary>
        /// 更新一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要更新的数据对象的类型</typeparam>
        /// <param name="item">要更新的数据对象</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public bool Update<T>(T item, string strCondition) where T : class, new() =>
            Do<T>(tbn => Update<T>(tbn, item, strCondition)).Result;

        /// <summary>
        /// 依据给定条件, 按格式拼接语句, 获取数据集行数. SQL拼接格式: SELECT COUNT(<c>countColumn</c>) FROM <c>table</c> WHERE <c>condition</c>;
        /// </summary>
        /// <param name="countColumn">计数列</param>
        /// <param name="table">给定表, 可以是多表关联</param>
        /// <param name="condition">给定条件</param>
        /// <returns></returns>
        public int Count(string countColumn, string table, string condition)
        {
            if (StringHelper.IsNullOrEmptyOrWhiteSpace(countColumn, table, condition))
            {
                log.DebugFormat("给定值存在空.\r\ncountColumn:{0};\r\ntable:{1};\r\ncondition:{2}.", countColumn, table,
                    condition);

                return 0;
            }

            return ExecuteSqlReturnString($"SELECT COUNT({countColumn}) FROM {table} WHERE {condition};").ToInt32();
        }

        /// <summary>
        /// 根据指定条件, 获取数据集行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Count<T>(string condition) => Do<T, int>(tbn => Count("*", tbn, condition));

        /// <summary>
        /// 检测数据库连接是否正常
        /// </summary>
        public void TryTestDBConnection() => DataAccess.GetDataSet("SHOW TABLES");

        /// <summary>
        /// GenerateInsertSqlFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GenerateInsertSqlFromObject<T>(string strTableName, T t) where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return string.Empty;
                }

                var item = new T();
                CommonHelper.SyncModel<T>(t, item);
                CommonClass.SetPropertyValue(item, DBTableCommonColumns.UpdateMan, null);
                CommonClass.SetPropertyValue(item, DBTableCommonColumns.DeleteMan, null);

                string strColumns = ""; //属性列
                string strValues = ""; //属性值
                foreach (var property in GenerateDataTableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetPropertyValue(item, property.Name);
                    if (value == null)
                    {
                        continue;
                    }

                    var strProp = property.PropertyType.ToString();
                    switch (strProp)
                    {
                        case "System.String":
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Int32":
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += value + ",";
                            break;
                        case "System.Double":
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += value + ",";
                            break;
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                strValues += "GETDATE(),";
                            }
                            else
                            {
                                if ((DateTime)value == default(DateTime))
                                    break;

                                strValues += "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                            }

                            strColumns += property.Name + ","; //添加属性列名称
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Boolean":
                            value = (bool)value ? 1 : 0;
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Decimal":
                            strColumns += property.Name + ","; //添加属性列名称
                            strValues += value + ",";
                            break;
                    } //end switch
                } //end foreach

                strColumns = strColumns.Remove(strColumns.Length - 1);
                strValues = strValues.Remove(strValues.Length - 1);
                string strSql =
                    "INSERT INTO " + strTableName + " (" + strColumns + ") " +
                    "VALUES(" + strValues + ");"; //添加数据记录

                return strSql;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate insert sql script from object.", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// GenerateUpdateSqlFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="t"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public string GenerateUpdateSqlFromObject<T>(string strTableName, T t, string strCondition)
            where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || strCondition.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return string.Empty;
                }

                var item = new T();
                CommonHelper.SyncModelToModel<T, T>(t, item);
                CommonClass.SetPropertyValue(item, DBTableCommonColumns.CreateMan, null);
                CommonClass.SetPropertyValue(item, DBTableCommonColumns.DeleteMan, null);

                StringBuilder strSets = new StringBuilder(); //属性设置
                foreach (System.Reflection.PropertyInfo property in GenerateDataTableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetPropertyValue(item, property.Name);
                    if (value == null)
                        continue;

                    var strProp = property.PropertyType.ToString();
                    switch (strProp)
                    {
                        case "System.String":
                            strSets.AppendFormat("{0}='{1}',", property.Name, value);
                            break;
                        case "System.Int32":
                        case "System.Int16":
                            strSets.AppendFormat("{0}={1},", property.Name, value);
                            break;
                        case "System.Double":
                            strSets.AppendFormat("{0}={1},", property.Name, value);
                            break;
                        case "System.Decimal":
                            strSets.AppendFormat("{0}={1},", property.Name, value);
                            break;
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime,
                                    StringComparison.CurrentCultureIgnoreCase)
                                || property.Name.Equals(DBTableCommonColumns.DeleteTime,
                                    StringComparison.CurrentCultureIgnoreCase))
                                break;

                            strSets.AppendFormat("{0}={1},",
                                property.Name, property.Name.Equals(DBTableCommonColumns.UpdateTime,
                                    StringComparison.CurrentCultureIgnoreCase)
                                    ? "GETDATE()"
                                    : "'" + value + "'");
                            break;
                        case "System.Boolean":
                            strSets.AppendFormat("{0} = {1},", property.Name,
                                (bool)value ? 1 : 2); // 2 for false, 1 for true
                            break;
                    } //end switch
                } //end foreach

                strSets.Remove(strSets.Length - 1, 1);
                var strSql = $"UPDATE {strTableName} SET {strSets.ToString()} WHERE {strCondition};"; //根据指定查询条件，更新数据记录

                return strSql;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate update sql script from object.", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// GenerateInsertSqlWithParametersFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tuple<string, List<DbParameter>> GenerateInsertSqlWithParametersFromObject<T>(string strTableName, T t)
            where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return default(Tuple<string, List<DbParameter>>);
                }

                var columns = new List<string>();
                var parameters = new List<DbParameter>();

                foreach (System.Reflection.PropertyInfo property in GenerateDataTableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetPropertyValue(t, property.Name);
                    if (value == null)
                    {
                        continue;
                    }

                    var strProp = property.PropertyType.ToString();
                    columns.Add(property.Name);
                    switch (strProp)
                    {
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (value.Equals(default(DateTime)))
                                {
                                    columns.Remove(property.Name);
                                    break;
                                }
                                else
                                    parameters.Add(new SqlParameter
                                    {
                                        ParameterName = property.Name,
                                        SqlDbType = SqlDbType.DateTime,
                                        Value = DateTime.Now
                                    });
                            }
                            else if (property.Name.Equals(DBTableCommonColumns.UpdateTime,
                                         StringComparison.CurrentCultureIgnoreCase)
                                     || property.Name.Equals(DBTableCommonColumns.DeleteTime,
                                         StringComparison.CurrentCultureIgnoreCase))
                            {
                                columns.Remove(property.Name);
                                break;
                            }
                            else
                            {
                                if ((DateTime)value == default(DateTime))
                                {
                                    columns.Remove(property.Name);
                                    break;
                                }

                                parameters.Add(new SqlParameter
                                { ParameterName = property.Name, SqlDbType = SqlDbType.DateTime, Value = value });
                            }

                            break;
                        case "System.Guid":
                            if ((Guid)value == default(Guid))
                            {
                                columns.Remove(property.Name);
                                break;
                            }
                            else
                                parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });

                            break;
                        case "System.Boolean":
                            value = (bool)value ? 1 : 0;
                            parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });
                            break;
                        default:
                            parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });
                            break;
                    } //end switch
                } //end foreach

                var strColumns = string.Join(",", columns.ToArray());
                var values = string.Join(",@", columns.ToArray());
                return new Tuple<string, List<DbParameter>>(
                    $"INSERT INTO {strTableName}({strColumns}) VALUES(@{values});",
                    parameters); //添加数据记录  strColumns;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate insert sql script from object.", ex);
            }

            return default(Tuple<string, List<DbParameter>>);
        }

        /// <summary>
        /// GenerateUpdateSqlWithParametersFromObject, 需要在外部添加条件.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tuple<string, List<DbParameter>> GenerateUpdateSqlWithParametersFromObject<T>(string strTableName, T t)
            where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return default(Tuple<string, List<DbParameter>>);
                }

                var parameters = new List<DbParameter>();
                var strSets = new StringBuilder(); //属性设置
                foreach (var property in GenerateDataTableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetPropertyValue(t, property.Name);
                    if (value == null || property.Name.Equals(DBUtil.GetPrimaryKeyName<T>()))
                        continue;

                    var strProp = property.PropertyType.ToString();
                    switch (strProp)
                    {
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime,
                                    StringComparison.CurrentCultureIgnoreCase)
                                || property.Name.Equals(DBTableCommonColumns.DeleteTime,
                                    StringComparison.CurrentCultureIgnoreCase))
                                break;

                            strSets.AppendFormat("{0}={1},",
                                property.Name, property.Name.Equals(DBTableCommonColumns.UpdateTime,
                                    StringComparison.CurrentCultureIgnoreCase)
                                    ? "GETDATE()"
                                    : "'" + value + "'");
                            break;
                        case "System.Guid":
                            if ((Guid)value == default(Guid))
                                break;
                            else
                            {
                                strSets.AppendFormat("{0}=@{0},", property.Name);
                                parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });
                            }

                            break;
                        case "System.Boolean":
                            strSets.AppendFormat("{0} = {1},", property.Name,
                                (bool)value ? 1 : 2); // 2 for false, 1 for true
                            break;
                        default:
                            strSets.AppendFormat("{0}=@{0},", property.Name, value);
                            parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });
                            break;
                    } //end switch
                } //end foreach

                strSets.Remove(strSets.Length - 1, 1);

                return new Tuple<string, List<DbParameter>>(
                    $"UPDATE {strTableName} SET {strSets} ",
                    parameters); //根据指定查询条件，更新数据记录
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate update sql script from object.", ex);
            }

            return default(Tuple<string, List<DbParameter>>);
        }

        /// <summary>
        /// 给定实体类型, 和待匹配标签, 由属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntity(Type type, SerializationFlags flags) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi =>
                pi.GetCustomAttributes(typeof(SqlSerializableAttribute), false).Any(a =>
                    // ReSharper disable once PossibleNullReferenceException
                    ((a as SqlSerializableAttribute).Serializable & flags) != SerializationFlags.None));

        /// <summary>
        /// 给定实体, 和待匹配标签, 由属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags">待匹配的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntity(object obj, SerializationFlags flags) => GenerateDataTableColumnsFromEntity(obj.GetType(), flags);

        /// <summary>
        /// 根据实体属性, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingAttr">
        ///     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        ///     how the search is conducted.-or- Zero, to return null.
        /// </param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntity<T>(BindingFlags bindingAttr) =>
            typeof(T).GetProperties(bindingAttr);

        /// <summary>
        /// 根据实体属性, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntity<T>() =>
            GenerateDataTableColumnsFromEntity<T>(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// 给定实体类型, 和待排除标签, 由属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filterFlags">待排除的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntityWithFilter(Type type,
            SerializationFlags filterFlags) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
            pi =>
                !pi.GetCustomAttributes(typeof(SqlSerializableAttribute), false).Any(a =>
                    ((a as SqlSerializableAttribute).Serializable & filterFlags) != SerializationFlags.None));

        /// <summary>
        /// 给定实体, 和待排除标签, 由属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filterFlags">待排除的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDataTableColumnsFromEntityWithFilter(object obj,
            SerializationFlags filterFlags) => GenerateDataTableColumnsFromEntityWithFilter(obj.GetType(), filterFlags);

        /// <summary>
        /// 给定泛型, 和待排除标签, 由属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterFlags">待排除的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo>
            GenerateDataTableColumnsFromEntityWithFilter<T>(SerializationFlags filterFlags) =>
            GenerateDataTableColumnsFromEntityWithFilter(typeof(T), filterFlags);

        /// <summary>
        /// 将数据库返回的DataSet转化为数据模型对象数组
        /// </summary>
        /// <typeparam name="T">泛型，指定的数据模型</typeparam>
        /// <param name="dataSet">数据库返回的DataSet</param>
        /// <returns>转化后的数据模型对象数组</returns>
        protected IEnumerable<T> GetModels<T>(DataSet dataSet) where T : class, new()
        {
            try
            {
                var models = Enumerable.Empty<T>();
                if (dataSet == null)
                    return models;

                if (dataSet.Tables.Count <= 0)
                    return models;

                var iUsersCount = 0;
                var iColumnCount = 0;
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    iUsersCount = dataSet.Tables[0].Rows.Count;
                }

                if (dataSet.Tables[0].Columns.Count > 0)
                {
                    iColumnCount = dataSet.Tables[0].Columns.Count;
                }

                if (iUsersCount <= 0 || iColumnCount <= 0)
                    return models;

                // Use parallel only when the entity count more than 50.
                if (iUsersCount > 50)
                {
                    var ts = new ConcurrentBag<T>();
                    Parallel.For(0, iUsersCount, i =>
                    {
                        var t = new T();
                        Parallel.For(0, iColumnCount, j =>
                        {
                            CommonClass.SetPropertyValue(
                                t,
                                dataSet.Tables[0].Columns[j].ColumnName,
                                dataSet.Tables[0].Rows[i][j]);
                        });

                        ts.Add(t);
                    });

                    models = ts.OrderBy(t => CommonClass.GetPropertyValue(t, DBTableCommonColumns.UpdateTime));
                }
                else
                {
                    var users = new T[iUsersCount];
                    for (var i = 0; i < iUsersCount; i++)
                    {
                        users[i] = new T();
                        for (var j = 0; j < iColumnCount; j++)
                        {
                            CommonClass.SetPropertyValue(
                                users[i],
                                dataSet.Tables[0].Columns[j].ColumnName,
                                dataSet.Tables[0].Rows[i][j]);
                        } //end for j
                    } //end for i

                    models = users;
                }

                return models;
            }
            catch (Exception ex)
            {
                log.ErrorException(string.Empty, ex);
            }

            return default(IEnumerable<T>);
        }

        /// <summary>
        /// 根据实体获取表名, 执行后续操作.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="suc">解析成功时的函数</param>
        /// <param name="err">解析表名失败时执行函数</param>
        /// <returns></returns>
        protected OperationResult DecodeTableName(Type type, Func<string, OperationResult> suc,
            Func<Exception, OperationResult> err)
        {
            string tbn;
            var cas = type.GetCustomAttributes<TableAttribute>(false);
            if (!cas.Any())
                tbn = type.Name;
            else if (cas.Count() == 1)
            {
                tbn = cas.Select(s => s.Name).FirstOrDefault();
                if (tbn.IsNullOrEmptyOrWhiteSpace())
                    tbn = type.Name;
            }
            else
                return err(new Exception("Multiple table attributes found."));

            return suc(tbn);
        }

        /// <summary>
        /// 根据实体获取表名, 执行后续操作.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="func">解析成功时的函数</param>
        /// <returns></returns>
        protected OperationResult Do(Type type, Func<string, OperationResult> func)
        {
            return DecodeTableName(type, func, ex =>
            {
                log.ErrorException("Error during get table name.", ex);
                return new OperationResult(false);
            });
        }

        /// <summary>
        /// 根据实体获取表名, 执行后续操作.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">解析成功时的函数</param>
        /// <returns></returns>
        protected OperationResult Do<T>(Func<string, OperationResult> func) => Do(typeof(T), func);

        /// <summary>
        /// 根据实体获取表名, 执行后续操作. 返回<c>bool</c>类型结果.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected bool Do(Type type, Func<string, bool> func) => Do(type, tbn => new OperationResult(func(tbn))).Result;

        /// <summary>
        /// 根据实体获取表名, 执行后续操作. 返回<c>string</c>类型操作信息.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected string Do(Type type, Func<string, string> func) =>
            Do(type, tbn => new OperationResult(func(tbn))).Message;

        /// <summary>
        /// 根据实体获取表名, 执行后续操作.
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <typeparam name="TK"></typeparam>
        /// <param name="func">解析成功时的函数</param>
        /// <returns></returns>
        protected TK Do<TU, TK>(Func<string, TK> func)
        {
            return (TK)Do(typeof(TU), tbn => new OperationResult(func(tbn))).Data;
        }
    }
}
