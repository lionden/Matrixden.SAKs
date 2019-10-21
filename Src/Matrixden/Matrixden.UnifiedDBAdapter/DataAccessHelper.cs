namespace Matrixden.UnifiedDBAdapter
{
    using Matrixden.Utils.Extensions;
    using Matrixden.UnifiedDBAdapter.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    /// <summary>
    /// 
    /// </summary>
    public class DataAccessHelper
    {
        /// <summary>
        /// 数据库连接串, 键名.
        /// </summary>
        public static readonly string APP_CONFIG_DB_CONNCTION = "LocalDB_Connection";
        /// <summary>
        /// MySQL provider identify
        /// </summary>
        public const string PROVIDER_NAME_MYSQL = "MySql.Data.MySqlClient";
        /// <summary>
        /// MS-SQL provider identify
        /// </summary>
        public const string PROVIDER_NAME_MSSQL = "System.Data.SqlClient";
        /// <summary>
        /// LibLog object.
        /// </summary>
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        public DataAccessHelper(bool updateConnString)
        {
            if (updateConnString)
            {
                DataBaseHelper = GetHelper(APP_CONFIG_DB_CONNCTION);
            }
        }

        public DataAccessHelper() : this(false) { }

        public DataAccessHelper(string dataAccessHelperstr)
        {
            DataBaseHelper = GetHelper(dataAccessHelperstr);
        }

        public static DBHelper DataBaseHelper = GetHelper(APP_CONFIG_DB_CONNCTION);

        /// <summary>
        /// 从Web.config中读取数据库的连接以及数据库类型
        /// </summary>
        private static DBHelper GetHelper(string connectionStringName)
        {
            DBHelper dbHelper = new DBHelper(connectionStringName);

            // 从config文件中读取数据库类型
            string providerName = "";
            try
            {
                providerName = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            }
            catch (Exception e)
            {
                log.ErrorException($"Failed to get connection string, connection name=[{connectionStringName}]", e);

                return null;
            }

            switch (providerName)
            {
                case "MySql.Data.MySqlClient":
                    dbHelper.DatabaseType = DBHelper.DatabaseTypes.MySql;
                    break;
                case "System.Data.SqlClient":
                default:
                    dbHelper.DatabaseType = DBHelper.DatabaseTypes.MSSql;
                    break;
            }

            // 从config文件中读取数据库连接
            try
            {
                switch (dbHelper.DatabaseType)
                {
                    default:
                        ConfigurationManager.RefreshSection("connectionStrings");
                        dbHelper.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                        if (StringHelper.IsNullOrEmptyOrWhiteSpace(dbHelper.ConnectionString))
                        {
                            dbHelper.ConnectionString = null;
                            throw new Exception("The database connection string is empty.");
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                log.ErrorException("The program can not find the database connection string in config file.", e);
            }

            return dbHelper;
        }

        /// <summary>
        /// 通过执行Sql语句返回DataSet数据集
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public DataSet GetDataSet(string pSql)
        {
            DataSet dataSet = null;
            if (DataBaseHelper.ConnectionString != null)
            {
                try
                {
                    dataSet = DataBaseHelper.ExecuteQuery(CommandType.Text, pSql, null);
                }
                catch (Exception e)
                {
                    log.ErrorException("Failed to execute query sql: \r\n[{0}]", e, pSql);
                    dataSet = null;
                }
            }

            return dataSet;
        }

        /// <summary>
        /// 通过执行Sql语句返回DataSet数据集
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, DbTransaction trans)
        {
            DataSet dataSet = null;
            if (DataBaseHelper.ConnectionString != null)
                dataSet = DataBaseHelper.ExecuteQuery(trans, CommandType.Text, sql, null);

            return dataSet;
        }

        /// <summary>
        /// 通过执行Sql语句返回DataTable数据表
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public DataTable GetFirstDataTable(string pSql)
        {
            DataSet dataSet = GetDataSet(pSql);

            if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count <= 0)
            {
                return null;
            }

            return dataSet.Tables[0];
        }

        /// <summary>
        /// 根据指定条件, 获取数据行数.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int GetCount(string tableName, string condition)
        {
            return DataBaseHelper.GetCount(tableName, condition);
        }

        /// <summary>
        /// get multidimensional array: sql->dataset->string[,]
        /// 获取数据数组，执行sql，获取Dataset数据集，转换成string[,]数组，返回多条记录
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public string[,] GetMultidimensionalArray(string pSql)
        {
            DataSet dataSet = GetDataSet(pSql);
            return this.GetRowsValues(dataSet);
        }

        /// <summary>
        /// 通过Sql语句，返回记录，返回格式string[]一维数据
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public string[] GetArray(string pSql)
        {
            log.Trace(pSql);
            string[] result = null;
            DataSet ds = GetDataSet(pSql);
            result = this.GetRowValues(ds);

            if (result != null && result.Length == 0) result = null;
            return result;
        }

        /// <summary>
        /// 执行Sql语句，返回单行记录。
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public string[] GetSingleRowValue(string pSql)
        {
            log.Trace(pSql);
            string[,] res1 = GetBigArray(pSql, 1);
            if (res1 == null || res1.GetLength(0) < 1) return null;
            string[] res = new string[res1.GetLength(1)];
            for (int i = 0; i < res1.GetLength(1); i++)
                res[i] = res1[0, i];

            return res;
        }

        /// <summary>
        /// get big array
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string[,] GetBigArray(string pSql, int size)
        {
            string[,] result = null;
            DataSet ds = GetDataSet(pSql);
            result = this.GetRowsValues(ds, size);

            return result;
        }

        /// <summary>
        /// 执行Sql语句，操作数据库。
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>返回受影响的行数. 如果异常发生, 返回-1.</returns>
        public int ExecSql(string sql, params DbParameter[] cmdParms)
        {
            log.Trace(sql);
            int nResult = 0;
            if (DataBaseHelper.ConnectionString != null)
            {
                try
                {
                    nResult = DataBaseHelper.ExecuteNonQuery(CommandType.Text, sql, cmdParms);
                }
                catch (Exception ex)
                {
                    nResult = -1;
                    log.ErrorException("Failed to execute non-query sql:\r\n[{0}].", ex, sql);
                }
            }

            return nResult;
        }

        /// <summary>
        /// 执行Sql语句，操作数据库。
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <returns>如果受影响行数大于0, 则返回<c>true</c>, 否则返回<c>false</c>.</returns>
        public bool ExecSql(string pSql)
        {
            return ExecSql(pSql, null) > 0 ? true : false;
        }

        /// <summary>
        /// 执行Sql语句，操作数据库。
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="iTimeOut">命令超时</param>
        /// <param name="cmdParms"></param>
        /// <returns>返回受影响的行数. 如果异常发生, 返回-1.</returns>
        public int ExecSql(string cmdText, int iTimeOut, params DbParameter[] cmdParms)
        {
            int nResult = 0;
            if (DataBaseHelper.ConnectionString != null)
            {
                try
                {
                    nResult = DataBaseHelper.ExecuteNonQuery(iTimeOut, CommandType.Text, cmdText, cmdParms);
                }
                catch (Exception ex)
                {
                    nResult = -1;
                    log.ErrorException("Failed to execute non-query sql:\r\n[{0}].", ex, cmdText);
                }
            }

            return nResult;
        }

        /// <summary>
        /// 执行Sql语句，操作数据库。
        /// </summary>
        /// <param name="pSql">SQL语句</param>
        /// <param name="iTimeOut">命令超时</param>
        /// <returns></returns>
        public bool ExecSql(string pSql, int iTimeOut)
        {
            return ExecSql(pSql, iTimeOut, null) > 0 ? true : false;
        }

        /// <summary>
        /// 执行Insert Sql语句，并返回最新插入记录的Id值(它是自增长的)。
        /// 该接口是为了获取每个用户在执行某段Insert代码时自身生成的自动增长的ID值，
        /// 防止因大量并发用户导致ID值获取错误，引起数据外键值跳号
        /// </summary>
        /// <param name="pSql">Insert SQL语句</param>
        /// <param name="tblName">表名</param>
        /// <returns>
        /// 返回刚刚插入的ID值. -1, SQL语句未执行成功.
        /// </returns>
        ///
        /// 调用示例：
        /// DataAccessHelper dataAccess = new DataAccessHelper();
        /// string strSql = "insert into rights(type,user_role_link,ctrdevice_link) values(0,5,4)";
        /// int nLastID = dataAccess.InsertGetLastId(strSql, "rights");
        public int InsertGetLastId(string pSql, string tblName)
        {
            int nResult = -1;

            if (DataBaseHelper.ConnectionString != null)
                nResult = DataBaseHelper.InsertGetLastId(DataBaseHelper.ConnectionString, pSql, tblName);

            return nResult;
        }

        /// <summary>
        /// 执行sql command, 返回受影响的行数.
        /// 使用sql parameter传参, 防止sql注入.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertGetAffectedRows(string sql, List<DbParameter> parameters)
        {
            if (DataBaseHelper.ConnectionString != null)
                return DataBaseHelper.InsertGetAffectedRows(sql, parameters);
            else
                return -1;
        }

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="sql">sql命令, 带参数</param>
        /// <param name="param">实体</param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the <paramref>sql</paramref> is null or empty or whitespaces.
        /// Or when the <paramref>t</paramref> is null.
        /// </exception>
        public int ExecuteNonQuery(string sql, object param = null)
        {
            if (sql.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentNullException();

            return DataBaseHelper.ExecuteNonQuery(DataBaseHelper.ConnectionString, sql, param);
        }

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText">sql命令, 带参数</param>
        /// <param name="parameters">参数集合.</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(string cmdText, List<DbParameter> parameters)
        {
            try
            {
                return DataBaseHelper.ExecuteNonQuery(cmdText, parameters);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to execute non-query sql:\r\n[{0}].", ex, cmdText);
                return -1;
            }
        }

        /// <summary>
        /// 在事务中执行Sql语句。
        /// </summary>
        /// <param name="trans">外部创建的事务</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public int ExecSqlWithTransaction(DbTransaction trans, string cmdText, params DbParameter[] cmdParms)
        {
            int nResult = 0;
            if (DataBaseHelper.ConnectionString != null)
            {
                try
                {
                    nResult = DataBaseHelper.ExecuteNonQuery(trans, CommandType.Text, cmdText, cmdParms);
                }
                catch (Exception ex)
                {
                    nResult = -1;
                    log.ErrorException($"Failed to execute non-query sql:\r\n[{cmdText}].", ex);
                }
            }

            return nResult;
        }

        /// <summary>
        /// 在事务中执行Sql语句。
        /// </summary>
        /// <param name="trans">外部创建的事务</param>
        /// <param name="pSql">SQL语句</param>
        /// <returns></returns>
        public bool ExecSqlWithTransaction(DbTransaction trans, string pSql)
        {
            return ExecSqlWithTransaction(trans, pSql, null) > 0 ? true : false;
        }

        /// <summary>
        /// 将DataSet的第一个表格的第一列转换成一维string数组
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public string[] GetRowValues(DataSet ds)
        {
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1) return null;
            string[] S = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < S.Length; i++)
                S[i] = ds.Tables[0].Rows[i][0].ToString();

            return S;
        }

        /// <summary>
        /// 将DataSet的第一个表格转换成二维string数组
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public string[,] GetRowsValues(DataSet ds)
        {
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1) return null;

            int i, j;
            string[,] S = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
            for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                for (j = 0; j < ds.Tables[0].Columns.Count; j++)
                    S[i, j] = ds.Tables[0].Rows[i][j].ToString();
            if (S.GetLength(0) == 0) S = null;

            return S;
        }

        /// <summary>
        /// 将DataSet的第一个表格的前nSize行转换成二维string数组
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="nSize">行数</param>
        /// <returns></returns>
        public String[,] GetRowsValues(DataSet ds, int nSize)
        {
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1) return null;

            int nColumns = ds.Tables[0].Columns.Count;
            String[,] res = new String[nSize, nColumns];
            for (int i = 0; i < nSize; i++)
                for (int j = 0; j < nColumns; j++) res[i, j] = ds.Tables[0].Rows[i][j].ToString();
            if (res.GetLength(0) == 0) res = null;

            return res;
        }

        /// <summary>
        /// 通过DataSet获取key-value 键值对list表,每个键值对为 列名-某行该列值
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetKeyValuePair(DataSet ds)
        {
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1)
            {
                return null;
            }

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Dictionary<string, string> dicTemp = new Dictionary<string, string>();
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    dicTemp.Add(ds.Tables[0].Columns[j].ColumnName, ds.Tables[0].Rows[i][j].ToString());
                }

                result.Add(dicTemp);
            }

            return result;
        }

        /// <summary>
        /// 执行批量sql
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public bool ExecuteSqlTran(IEnumerable<string> SQLStringList, params DbParameter[] parms)
        {
            if (SQLStringList == null)
                return true;

            return DataBaseHelper.ExecuteNonQuery(DataBaseHelper.ConnectionString, CommandType.Text, SQLStringList, parms) >= 0;
        }

        /// <summary>
        /// 执行批量sql 新的
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public bool ExecuteSqlTranNew(IEnumerable<string> SQLStringList, params DbParameter[] parms)
        {
            if (SQLStringList == null)
                return true;

            return DataBaseHelper.ExecuteNonQueryNew(DataBaseHelper.ConnectionString, CommandType.Text, SQLStringList, parms) >= 0;
        }
        /// <summary>
        /// 执行存储过程.
        /// </summary>
        /// <param name="procName">存储过程名字.</param>
        /// <param name="paras">存储过程所需参数.</param>
        /// <returns>dataset形式的返回结果.</returns>
        public DataSet ExecuteStoredProcedure(string procName, params DbParameter[] paras)
        {
            return DataBaseHelper.ExecuteQuery(CommandType.StoredProcedure, procName, paras);
        }

        /// <summary>
        /// 根据SQL语句, 查询数据库
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<object> Query(Type type, string sql, object param = null)
        {
            return DataBaseHelper.Query(type, sql, param);
        }

        /// <summary>
        /// 根据SQL语句, 查询数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<object> Query(string sql, object param = null) => Query(typeof(object), sql, param);

        /// <summary>
        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数
        /// </summary>
        /// <param name="command">要处理的SqlCommand</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">一个有效的事务或者是null值</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名或都SQL命令文本</param>
        /// <param name="commandParameters">和命令相关联的SqlParameter参数数组,如果没有参数为'null'</param>
        /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param>
        private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            try
            {
                if (command == null) throw new ArgumentNullException("command");
                if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

                // 如果连接没有打开则打开连接
                if (connection.State != ConnectionState.Open)
                {
                    mustCloseConnection = true;
                    connection.Open();
                }
                else
                {
                    mustCloseConnection = false;
                }

                // 给命令分配一个数据库连接.
                command.Connection = connection;
                // 设置命令文本(存储过程名或SQL语句)
                command.CommandText = commandText;
                // 分配事务
                if (transaction != null)
                {
                    if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                    command.Transaction = transaction;
                }

                // 设置命令类型.
                command.CommandType = commandType;
                // 分配命令参数
                if (commandParameters != null)
                {
                    command.Parameters.Clear();
                    foreach (SqlParameter parm in commandParameters)
                        command.Parameters.Add(parm);
                }

                return;
            }
            catch (Exception ex)
            {
                log.FatalException("Failed to prepare command.", ex);
                mustCloseConnection = false;
            }
        }
    }
}
