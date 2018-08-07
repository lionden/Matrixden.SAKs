namespace Matrixden.UnifiedDBAdapter
{
    using System.Data.Common;
    using System.Data;
    using System.Data.SqlClient;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;

    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// 枚举：数据库类型
        /// </summary>
        public enum DatabaseTypes
        {
            /// <summary>
            /// Microsoft SQL Server
            /// </summary>
            MSSql,
            /// <summary>
            /// MySQL
            /// </summary>
            MySql
        }

        private DatabaseTypes databaseType;
        /// <summary>
        /// must give a value to connectionString
        /// </summary>
        private string connectionString;

        public DBHelper(string connectionString) : this(DatabaseTypes.MSSql, connectionString) { }

        public DBHelper(DatabaseTypes type, string connectionString)
        {
            this.databaseType = type;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseTypes DatabaseType
        {
            get { return databaseType; }
            set { databaseType = value; }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        private IDBHelper _DBHelper
        {
            get
            {
                switch (databaseType)
                {
                    case DatabaseTypes.MySql:
                        return new MySqlHelper(ConnectionString);
                    case DatabaseTypes.MSSql:
                    default:
                        return new SqlHelper(ConnectionString);
                }
            }
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        public DbConnection CreateConnection()
        {
            switch (databaseType)
            {
                case DatabaseTypes.MySql:
                    return new MySqlConnection(connectionString);
                case DatabaseTypes.MSSql:
                default:
                    return new SqlConnection(connectionString);
            }
        }

        #region === 创造DbParameter的实例 ===

        /// <summary>
        /// 创造输入DbParameter的实例
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, int size, object value)
        {
            return CreateDbParameter(paraName, dbType, size, value, ParameterDirection.Input);
        }

        /// <summary>
        /// 创造输入DbParameter的实例
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, object value)
        {
            return CreateDbParameter(paraName, dbType, 0, value, ParameterDirection.Input);
        }

        /// <summary>
        /// 创造输出DbParameter的实例
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.Output);
        }

        /// <summary>
        /// 创造输出DbParameter的实例
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.Output);
        }

        /// <summary>
        /// 创造返回DbParameter的实例
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// 创造返回DbParameter的实例
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// 创造DbParameter的实例
        /// </summary>
        public DbParameter CreateDbParameter(string paraName, DbType dbType, int size, object value, ParameterDirection direction)
        {
            DbParameter para;
            switch (databaseType)
            {
                case DatabaseTypes.MySql:
                    para = new MySqlParameter();
                    break;
                case DatabaseTypes.MSSql:
                default:
                    para = new SqlParameter();
                    break;
            }
            para.ParameterName = paraName;

            if (size != 0)
                para.Size = size;

            para.DbType = dbType;

            if (value != null)
                para.Value = value;

            para.Direction = direction;

            return para;
        }

        #endregion

        #region === 数据库执行方法 ===

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="sql">sql命令, 带参数</param>
        /// <param name="param">实体</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, string sql, object param = null)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, sql, param);
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(int iTimeOut, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, iTimeOut, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="cmdText">sql命令, 带参数</param>
        /// <param name="parameters">参数集合.</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(string cmdText, List<DbParameter> parameters)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, cmdText, parameters);
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="cmdTexts">SQL list</param>
        /// <param name="cmdParms">SQL parms</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, cmdType, cmdTexts, cmdParms);
        }
        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="cmdTexts">SQL list</param>
        /// <param name="cmdParms">SQL parms</param>
        ///    /// <returns></returns>
        public int ExecuteNonQueryNew(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQueryNew(connectionString, cmdType, cmdTexts, cmdParms);
        }
        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteQuery(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteReader(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteReader(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteScalar(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteScalar(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 执行Insert Sql语句，并返回最新插入记录的Id值(它是自增长的)。
        /// 该接口是为了获取每个用户在执行某段Insert代码时自身生成的自动增长的ID值，
        /// 防止因大量并发导致ID值获取错误，引起数据外键值错误
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="pSql"></param>
        /// <param name="tblName">表名</param>
        /// <returns>最新生成的ID值</returns>
        public int InsertGetLastId(string connectionString, string pSql, string tblName)
        {
            return _DBHelper.InsertGetLastId(connectionString, pSql, tblName);
        }

        /// <summary>
        /// 执行sql command, 返回受影响的行数.
        /// 使用sql parameter传参, 防止sql注入.
        /// </summary>
        /// <param name="sql">连接字符串</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertGetAffectedRows(string sql, List<DbParameter> parameters)
        {
            return _DBHelper.InsertGetAffectedRows(connectionString, sql, parameters);
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="fldSort">排序字段</param>
        /// <param name="sort">升序{False}/降序(True)</param>
        /// <param name="condition">条件(不需要where)</param>
        public DbDataReader GetPageList(string tblName, int pageSize, int pageIndex, string fldSort, bool sort, string condition)
        {
            return _DBHelper.GetPageList(connectionString, tblName, pageSize, pageIndex, fldSort, sort, condition);
        }

        /// <summary>
        /// 得到数据条数
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="condition">条件(不需要where)</param>
        /// <returns>数据条数</returns>
        public int GetCount(string tblName, string condition)
        {
            return _DBHelper.GetCount(connectionString, tblName, condition);
        }

        #endregion

        /// <summary>
        /// 根据SQL语句, 查询数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> Query(string sql, object param = null)
        {
            return _DBHelper.Query(connectionString, sql, param);
        }
    }
}
