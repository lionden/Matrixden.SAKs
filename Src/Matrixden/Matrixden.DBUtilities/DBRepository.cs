/*
 * 
 * 操作实例：
       DBRepository repository = new DBRepository();            //实例化类

       //创建数据表的模型对象，通过DBUtility.DBRepository.Models
       userhistoryModel userhistory = new userhistoryModel();   //userhistory表的数据模型
   
       //设置数据表模型参数
       userhistory.id = 3;
       userhistory.acc_time = DateTime.Now;
       userhistory.accesstype = 1;
       userhistory.user_link = 1;
       
       //返回全部数据记录
       userhistoryModel[] items = repository.GetByCondition<userhistoryModel>("userhistory", "");

       //添加一条新的数据记录
       bool bResult = repository.Add<userhistoryModel>("userhistory", userhistory);
 
       //根据指定的条件，更新一条数据记录
       bool bResult = repository.Update<userhistoryModel>("userhistory", userhistory, "id=" + userhistory.id);
 *
 * 
 */

namespace Matrixden.DBUtilities
{
    using Matrixden.DBUtilities;
    using Matrixden.DBUtilities.Attributes;
    using Matrixden.DBUtilities.Logging;
    using Matrixden.DBUtilities.Resources;
    using Matrixden.DBUtilities.Interfaces;
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

    public abstract class DBRepository : IDBRepository
    {
        internal static readonly ILog log = LogProvider.GetCurrentClassLogger();
        public static DataAccessHelper DataAccess = new DataAccessHelper();

        protected DBRepository() : this(ConfigurationManager.ConnectionStrings[DataAccessHelper.APP_CONFIG_DB_CONNCTION]) { }
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
                                var cnf = System.Configuration.ConfigurationManager.ConnectionStrings[DataAccessHelper.APP_CONFIG_DB_CONNCTION];
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
        /// <typeparam name="T">泛型，返回的指定数据对象类型</typeparam>
        /// <param name="strTableName">要查询的表名</param>
        /// <param name="strColumns">所选列(SQL语句)，如“[属性列1],[属性列2],……[属性列n]” (如果给空值或者null 等价于 *)</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <param name="strOrder">对查询返回的数据集进行排序，DESC为降序；ASC为升序；空为不添加排序条件。如“ID DESC”，即根据ID属性按降序排列</param>
        /// <returns>返回的数据记录对象数组</returns>
        public abstract IEnumerable<T> GetByCondition<T>(string strTableName, string strColumns = null, string strCondition = null, string strOrder = null) where T : class ,new();

        /// <summary>
        /// 根据条件保存实体, 如果存在则更新, 否则插入.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract bool Save<T>(T item) where T : class, new();

        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strSets">要更新的属性值(SQL语句)，如“[属性列1]=[值1], [属性列2]=[值2], ……[属性列n]=[值n]”. 无需包含[UpdateTime]字段.</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public abstract bool Update(string strTableName, string strSets, string strCondition);

        /// <summary>
        /// 根据特定条件查询表中是否含有该条数据.
        /// </summary>
        /// <param name="strDataTable"></param>
        /// <param name="strCondition">数据库查询条件, 不含where关键字</param>
        /// <returns></returns>
        public abstract bool IsDataRowExist(string strDataTable, string strCondition);

        /// <summary>
        /// 此方法仅用于逻辑删除的数据库表, 即仅有增, 改, 查操作权限的表.
        /// 对带有物理删除的数据库表, 不适用.
        /// 如果在校验过程中, 发生UnExpected结果, 直接返回True.
        /// Microsoft SQL Server中, 使用"[Flags] [timestamp] NOT NULL"字段跟踪数据行的变化;
        /// MySQL中, 则使用"`UpdateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP"字段跟踪数据行的变化.
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="originalCount">已缓存的数据条</param>
        /// <param name="originalLatestUpdateFlag">缓存结束数据标记.</param>
        /// <param name="conditionStr"></param>
        /// <returns></returns>
        public abstract bool IsTableDataChanged(string table, int originalCount, object originalLatestUpdateFlag, string conditionStr);

        /// <summary>
        /// 根据实体生成Insert SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <returns></returns>
        public abstract string GenerateInsertSQLWithParameters<T>(string table) where T : class,new();

        /// <summary>
        /// 根据实体生成Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">实体对应的表名</param>
        /// <returns></returns>
        public abstract string GenerateUpdateSQLWithParameters<T>(string table, string condition, T t) where T : class,new();

        /// <summary>
        /// 根据实体生成Insert or Update SQL语句, 使用参数方式赋值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract string GenerateInsertOrUpdateSQLWithParameters<T>() where T : class,new();

        public IEnumerable<T> GetBySqLCommand<T>(string sqlCommand) where T : class,new()
        {
            try
            {
                System.Data.DataSet dataSet = DataAccess.GetDataSet(sqlCommand);

                return GetModels<T>(dataSet);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SQL Command: {0}; Exception Msg: {1}.", sqlCommand, ex);
            }

            return default(IEnumerable<T>);
        }

        public string[] GetArrayBySqlCommand(string sqlCommand)
        {
            try
            {
                string[] results = DataAccess.GetArray(sqlCommand);
                return results;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SQL Command: {0}; Exception Msg: {1}.", sqlCommand, ex);
            }

            return default(string[]);
        }

        public string ExecuteSqlReturnString(string sqlCommand)
        {
            try
            {
                System.Data.DataSet dataSet = DataAccess.GetDataSet(sqlCommand);
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
                log.ErrorFormat("SQL Command: {0}; Exception Msg: {1}.", sqlCommand, ex);
            }

            return default(string);
        }

        /// <summary>
        /// 执行存储过程, 返回数据集合.
        /// </summary>
        /// <typeparam name="T">返回结果的泛型类型.</typeparam>
        /// <param name="procName">存储过程名称.</param>
        /// <param name="paras">存储过程参数列表.</param>
        /// <returns></returns>
        public IEnumerable<T> GetByStoredProcedure<T>(string procName, params DbParameter[] paras) where T : class, new()
        {
            try
            {
                var dataSet = DataAccess.ExecuteStoredProcedure(procName, paras);

                return GetModels<T>(dataSet);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Procedure Name: {0}; Exception Msg: {1}.", procName, ex);
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
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || strColumns.IsNullOrEmptyOrWhiteSpace() || strValues.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }

            string strSql =
                "INSERT INTO " + strTableName + " (" + strColumns + ") " +
                "VALUES(" + strValues + ")";  //添加数据记录

            return DataAccess.ExecSql(strSql);              //执行sql语句，返回是否执行成功
        }

        /// <summary>
        /// 添加一条数据记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public OperationResult Add<T>(string strTableName, T item) where T : class, new()
        {
            var result = new OperationResult(DBOperationMessage.Fail);
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || item == default(T))
                return result;

            var cnt = 0;
            var sql = instance.GenerateInsertSQLWithParameters<T>(strTableName);
            if (sql.IsNullOrEmptyOrWhiteSpace())
                return result;

            try
            {
                if (CommonClass.GetFieldValue(item, DBTableCommonColumns.ID) == null)
                    CommonClass.SetPropertyValue(item, DBTableCommonColumns.ID, Guid.NewGuid());
                cnt = DataAccess.ExecuteNonQuery(sql, item);
            }
            catch (SqlException sEx)
            {
                log.ErrorException("Error occured during SQL excute, Table: {0},\r\nSQL {1}.", sEx, strTableName, sql);
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
        public bool Add<T>(T item) where T : class, new()
        {
            return Do<T>(tbn =>
            {
                return Add<T>(tbn, item);
            }).Result;
        }

        /// <summary>
        /// 同时向数据库中添加多条数据.
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="items">实体值</param>
        /// <returns></returns>
        public OperationResult AddBulk<T>(string tableName, IEnumerable<T> items) where T : class,new()
        {
            var result = new OperationResult(DBOperationMessage.Fail);
            if (tableName.IsNullOrEmptyOrWhiteSpace() || items == null || items.Count() <= 0)
                return result;

            var cnt = 0;
            var sql = instance.GenerateInsertSQLWithParameters<T>(tableName);
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
        /// 添加一条数据记录, 返回受影响的行数.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns>返回添加的行数.</returns>
        public int AddThenGetAffectedRows<T>(string strTableName, T item) where T : class, new()
        {
            var tp = GenerateInsertSQLWithParametersFromObject(strTableName, item);
            if (tp == null)
                return -1;

            try
            {
                return DataAccess.InsertGetAffectedRows(tp.Item1, tp.Item2);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("表{0}主键重复, SQL: {1}.", ex, strTableName, tp.Item1);
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

            string strSql = "DELETE FROM " + strTableName + " WHERE " + strCondition;             //根据指定查询条件，删除数据记录

            return DataAccess.ExecSql(strSql);
        }

        /// <summary>
        /// 标记数据被删除, 非实际删除.
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public bool Remove_Logic(string strTableName, string strCondition)
        {
            return Update(strTableName, string.Format("{0}='{1}'", DBTableCommonColumns.Status, DBColumn_StatusCode.DB_ROW_STATUS_DELETED), strCondition);
        }

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
            if (strTableName.IsNullOrEmptyOrWhiteSpace() || item == default(T) || strCondition.IsNullOrEmptyOrWhiteSpace())
                return result;

            var cnt = 0;
            var sql = instance.GenerateUpdateSQLWithParameters<T>(strTableName, strCondition, item);
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
        public bool Update<T>(T item) where T : class, new()
        {
            return Update<T>(item, "ID=@ID");
        }

        /// <summary>
        /// 更新一条数据记录
        /// </summary>
        /// <typeparam name="T">泛型，要更新的数据对象的类型</typeparam>
        /// <param name="strTableName">表名</param>
        /// <param name="item">要更新的数据对象</param>
        /// <param name="strCondition">自定义WHERE查询条件(不加WHERE)，如“[属性列1] = [值1] AND [属性列2] = [值2] ……”</param>
        /// <returns>是否执行成功</returns>
        public bool Update<T>(T item, string strCondition) where T : class, new()
        {
            return Do<T>(tbn =>
            {
                return Update<T>(tbn, item, strCondition);
            }).Result;
        }

        public void TryTestDBConnection()
        {
            DataAccess.GetDataSet("SHOW TABLES");
        }

        /// <summary>
        /// GenerateInsertSQLFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GenerateInsertSQLFromObject<T>(string strTableName, T t) where T : class, new()
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
                string strValues = "";  //属性值
                string strProp = "";
                foreach (System.Reflection.PropertyInfo property in GenerateDatatableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetFieldValue(item, property.Name);
                    if (value == null)
                    {
                        continue;
                    }

                    strProp = property.PropertyType.ToString();
                    switch (strProp)
                    {
                        case "System.String":
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Int32":
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += value + ",";
                            break;
                        case "System.Double":
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += value + ",";
                            break;
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime, StringComparison.CurrentCultureIgnoreCase))
                            {
                                strValues += "GETDATE(),";
                            }
                            else
                            {
                                if ((DateTime)value == default(DateTime))
                                    break;

                                strValues += "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                            }
                            strColumns += property.Name + ",";    //添加属性列名称
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Boolean":
                            value = (bool)value ? 1 : 0;
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += "'" + value + "',";
                            break;
                        case "System.Decimal":
                            strColumns += property.Name + ",";    //添加属性列名称
                            strValues += value + ",";
                            break;
                    }//end switch
                }//end foreach
                strColumns = strColumns.Remove(strColumns.Length - 1);
                strValues = strValues.Remove(strValues.Length - 1);
                string strSql =
                    "INSERT INTO " + strTableName + " (" + strColumns + ") " +
                    "VALUES(" + strValues + ");";  //添加数据记录

                return strSql;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate insert sql script from object.", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// GenerateUpdateSQLFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public string GenerateUpdateSQLFromObject<T>(string strTableName, T t, string strCondition) where T : class, new()
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
                foreach (System.Reflection.PropertyInfo property in GenerateDatatableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetFieldValue(item, property.Name);
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
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime, StringComparison.CurrentCultureIgnoreCase)
                                                || property.Name.Equals(DBTableCommonColumns.DeleteTime, StringComparison.CurrentCultureIgnoreCase))
                                break;

                            strSets.AppendFormat("{0}={1},",
                                                property.Name, property.Name.Equals(DBTableCommonColumns.UpdateTime, StringComparison.CurrentCultureIgnoreCase)
                                                    ? "GETDATE()" : "'" + value + "'");
                            break;
                        case "System.Boolean":
                            strSets.AppendFormat("{0} = {1},", property.Name, (bool)value ? 1 : 2);     // 2 for false, 1 for true
                            break;
                    }//end switch
                }//end foreach
                strSets.Remove(strSets.Length - 1, 1);
                string strSql = string.Format("UPDATE {0} SET {1} WHERE {2};", strTableName, strSets.ToString(), strCondition);//根据指定查询条件，更新数据记录

                return strSql;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate update sql script from object.", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// GenerateInsertSQLWithParametersFromObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Tuple<string, List<DbParameter>> GenerateInsertSQLWithParametersFromObject<T>(string strTableName, T t) where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return default(Tuple<string, List<DbParameter>>);
                }

                //var item = new T();
                //CommonHelper.SyncModelToModel<T, T>(t, item);
                List<string> columns = new List<string>();
                List<DbParameter> parameters = new List<DbParameter>();

                string strProp = "";
                foreach (System.Reflection.PropertyInfo property in GenerateDatatableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetFieldValue(t, property.Name);
                    if (value == null)
                    {
                        continue;
                    }

                    strProp = property.PropertyType.ToString();
                    columns.Add(property.Name);
                    switch (strProp)
                    {
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (value.Equals(default(DateTime)))
                                {
                                    columns.Remove(property.Name);
                                    break;
                                }
                                else
                                    parameters.Add(new SqlParameter { ParameterName = property.Name, SqlDbType = SqlDbType.DateTime, Value = DateTime.Now });
                            }
                            else if (property.Name.Equals(DBTableCommonColumns.UpdateTime, StringComparison.CurrentCultureIgnoreCase)
                                        || property.Name.Equals(DBTableCommonColumns.DeleteTime, StringComparison.CurrentCultureIgnoreCase))
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

                                parameters.Add(new SqlParameter { ParameterName = property.Name, SqlDbType = SqlDbType.DateTime, Value = value });
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
                    }//end switch
                }//end foreach

                var strColumns = string.Join(",", columns.ToArray());
                var values = string.Join(",@", columns.ToArray());
                return new Tuple<string, List<DbParameter>>(string.Format("INSERT INTO {0}({1}) VALUES(@{2});", strTableName, strColumns, values), parameters);  //添加数据记录  strColumns;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate insert sql script from object.", ex);
            }

            return default(Tuple<string, List<DbParameter>>);
        }

        /// <summary>
        /// GenerateUpdateSQLWithParametersFromObject, 需要在外部添加条件.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName"></param>
        /// <param name="item"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public Tuple<string, List<DbParameter>> GenerateUpdateSQLWithParametersFromObject<T>(string strTableName, T t) where T : class, new()
        {
            try
            {
                if (strTableName.IsNullOrEmptyOrWhiteSpace() || t == null)
                {
                    return default(Tuple<string, List<DbParameter>>);
                }

                //var item = new T();
                //CommonHelper.SyncModelToModel<T, T>(t, item);
                List<DbParameter> parameters = new List<DbParameter>();
                StringBuilder strSets = new StringBuilder(); //属性设置
                foreach (System.Reflection.PropertyInfo property in GenerateDatatableColumnsFromEntity<T>())
                {
                    object value = CommonClass.GetFieldValue(t, property.Name);
                    if (value == null || property.Name.Equals(DBUtil.GetPrimaryKeyName<T>()))
                        continue;

                    var strProp = property.PropertyType.ToString();
                    switch (strProp)
                    {
                        case "System.DateTime":
                            if (property.Name.Equals(DBTableCommonColumns.CreateTime, StringComparison.CurrentCultureIgnoreCase)
                                                || property.Name.Equals(DBTableCommonColumns.DeleteTime, StringComparison.CurrentCultureIgnoreCase))
                                break;

                            strSets.AppendFormat("{0}={1},",
                                                property.Name, property.Name.Equals(DBTableCommonColumns.UpdateTime, StringComparison.CurrentCultureIgnoreCase)
                                                    ? "GETDATE()" : "'" + value + "'");
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
                            strSets.AppendFormat("{0} = {1},", property.Name, (bool)value ? 1 : 2);     // 2 for false, 1 for true
                            break;
                        default:
                            strSets.AppendFormat("{0}=@{0},", property.Name, value);
                            parameters.Add(new SqlParameter { ParameterName = property.Name, Value = value });
                            break;
                    }//end switch
                }//end foreach
                strSets.Remove(strSets.Length - 1, 1);

                return new Tuple<string, List<DbParameter>>(string.Format("UPDATE {0} SET {1} ", strTableName, strSets.ToString()), parameters);//根据指定查询条件，更新数据记录
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to generate update sql script from object.", ex);
            }

            return default(Tuple<string, List<DbParameter>>);
        }

        /// <summary>
        /// 根据实体属性, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDatatableColumnsFromEntity<T>()
        {
            return GenerateDatatableColumnsFromEntity<T>(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 根据实体属性, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingAttr">
        ///     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        ///     how the search is conducted.-or- Zero, to return null.
        /// </param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDatatableColumnsFromEntity<T>(BindingFlags bindingAttr)
        {
            var pis = typeof(T).GetProperties(bindingAttr);
            return pis;
        }

        /// <summary>
        /// 根据实体属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">待匹配的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDatatableColumnsFromEntity<T>(SerializationFlags flags)
        {
            var pis = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.GetCustomAttributes(typeof(SqlSerializableAttribute), false).Any(a => ((a as SqlSerializableAttribute).Serializable & flags) != SerializationFlags.None));

            return pis;
        }

        /// <summary>
        /// 根据实体属性的特性标签, 获取数据库表字段.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterFlags">待排除的标签</param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GenerateDatatableColumnsFromEntityWithFilter<T>(SerializationFlags filterFlags)
        {
            var pis = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => !pi.GetCustomAttributes(typeof(SqlSerializableAttribute), false).Any(a => ((a as SqlSerializableAttribute).Serializable & filterFlags) != SerializationFlags.None));

            return pis;
        }

        /// <summary>
        /// 将数据库返回的DataSet转化为数据模型对象数组
        /// </summary>
        /// <typeparam name="T">泛型，指定的数据模型</typeparam>
        /// <param name="dataSet">数据库返回的DataSet</param>
        /// <returns>转化后的数据模型对象数组</returns>
        protected IEnumerable<T> GetModels<T>(System.Data.DataSet dataSet) where T : class, new()
        {
            try
            {
                IEnumerable<T> models = Enumerable.Empty<T>();
                if (dataSet != null)
                {
                    if (dataSet.Tables.Count > 0)
                    {
                        int iusersCount = 0;
                        int iColumCount = 0;
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            iusersCount = dataSet.Tables[0].Rows.Count;
                        }
                        if (dataSet.Tables[0].Columns.Count > 0)
                        {
                            iColumCount = dataSet.Tables[0].Columns.Count;
                        }
                        if (iusersCount > 0 && iColumCount > 0)
                        {
                            // Use parallel only when the entity count more than 50.
                            if (iusersCount > 50)
                            {
                                var ts = new ConcurrentBag<T>();
                                Parallel.For(0, iusersCount, i =>
                                {
                                    var t = new T();
                                    Parallel.For(0, iColumCount, j =>
                                    {
                                        CommonClass.SetPropertyValue(
                                            t,
                                            dataSet.Tables[0].Columns[j].ColumnName,
                                            dataSet.Tables[0].Rows[i][j]);
                                    });

                                    ts.Add(t);
                                });

                                models = ts.OrderBy(t => CommonClass.GetFieldValue(t, "UpdateTime"));
                            }
                            else
                            {
                                var users = new T[iusersCount];
                                for (int i = 0; i < iusersCount; i++)
                                {
                                    users[i] = new T();
                                    for (int j = 0; j < iColumCount; j++)
                                    {
                                        CommonClass.SetPropertyValue(
                                            users[i],
                                            dataSet.Tables[0].Columns[j].ColumnName,
                                            dataSet.Tables[0].Rows[i][j]);
                                    }//end for j
                                }//end for i

                                models = users;
                            }
                        }//end if iusersCount
                    }//end if Tables
                }//end if strArray

                return models;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return default(IEnumerable<T>);
        }

        /// <summary>
        /// 根据实体, 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="succ"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        protected OperationResult Do<T>(Func<string, OperationResult> succ, Func<Exception, OperationResult> err)
        {
            var tbn = string.Empty;
            var cas = typeof(T).GetCustomAttributes<TableAttribute>(false);
            if (cas.Count() <= 0)
                tbn = typeof(T).Name;
            else if (cas.Count() == 1)
            {
                tbn = cas.Select(s => s.Name).FirstOrDefault();
                if (tbn.IsNullOrEmptyOrWhiteSpace())
                    tbn = typeof(T).Name;
            }
            else
                return err(new Exception("Multiple table attributes found."));

            return succ(tbn);
        }

        /// <summary>
        /// 根据实体, 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected OperationResult Do<T>(Func<string, OperationResult> func)
        {
            return Do<T>(func, ex =>
            {
                log.ErrorException("Error during get table name.", ex);
                return new OperationResult(false);
            });
        }

        protected bool Do<T>(Func<string, bool> func)
        {
            return Do<T>(tbn =>
            {
                return new OperationResult(func(tbn));
            }).Result;
        }

        protected string Do<T>(Func<string, string> func)
        {
            return Do<T>(tbn =>
            {
                return new OperationResult(func(tbn));
            }).Message;
        }
    }
}
