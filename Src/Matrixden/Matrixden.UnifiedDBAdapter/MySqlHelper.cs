namespace Matrixden.UnifiedDBAdapter
{
    using Matrixden.Utils.Extensions;
    using Matrixden.UnifiedDBAdapter.Logging;
    using Dapper;
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 数据库操作基类(for MySql)
    /// </summary>
    internal class MySqlHelper : IDBHelper
    {
        private string ConnectionString;
        private static readonly ILog _logger = LogProvider.GetCurrentClassLogger();

        public MySqlHelper(string ConnectionString)
        {
            // TODO: Complete member initialization
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 获取分页SQL
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="fldSort">排序字段（最后一个不需要填写正序还是倒序，例如：id asc, name）</param>
        /// <param name="tblName">表名</param>
        /// <param name="sort">最后一个排序字段的正序或倒序（true为倒序，false为正序）</param>
        /// <returns>返回用于分页的SQL语句</returns>
        private string GetPagerSQL(string condition, int pageSize, int pageIndex, string fldSort,
            string tblName, bool sort)
        {
            string strSort = sort ? " DESC" : " ASC";

            StringBuilder strSql = new StringBuilder("select * from " + tblName);
            if (!string.IsNullOrEmpty(condition))
            {
                strSql.AppendFormat(" where {0} order by {1}{2}", condition, fldSort, strSort);
            }
            else
            {
                strSql.AppendFormat(" order by {0}{1}", fldSort, strSort);
            }
            strSql.AppendFormat(" limit {0},{1}", pageSize * (pageIndex - 1), pageSize);

            return strSql.ToString();
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="tblName">表名</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="fldSort">排序字段</param>
        /// <param name="sort">升序{False}/降序(True)</param>
        /// <param name="condition">条件(不需要where)</param>
        public DbDataReader GetPageList(string connectionString, string tblName, int pageSize,
            int pageIndex, string fldSort, bool sort, string condition)
        {
            string sql = GetPagerSQL(condition, pageSize, pageIndex, fldSort, tblName, sort);
            return ExecuteReader(connectionString, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 得到数据条数
        /// </summary>
        public int GetCount(string connectionString, string tblName, string condition)
        {
            StringBuilder sql = new StringBuilder("select count(*) from " + tblName);
            if (!string.IsNullOrEmpty(condition))
                sql.Append(" where " + condition);

            object count = ExecuteScalar(connectionString, CommandType.Text, sql.ToString(), null);
            return int.Parse(count.ToString());
        }

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <exception cref="System.SystemException"></exception>
        /// <returns></returns>
        public DataSet ExecuteQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ds");
            cmd.Parameters.Clear();
            return ds;
        }


        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="sql">sql命令, 带参数</param>
        /// <param name="param">参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, string sql, object param)
        {
            var cnt = 0;
            if (connectionString.IsNullOrEmptyOrWhiteSpace() || sql.IsNullOrEmptyOrWhiteSpace())
                return -1;

            using (var conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cnt = conn.Execute(sql, param);
            }

            return cnt;
        }

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdText">sql命令, 带参数</param>
        /// <param name="parameters">参数集合.</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(string connectionString, string cmdText, List<DbParameter> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return -1;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(cmdText, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
                int val = 0;
                try
                {
                    val = cmd.ExecuteNonQuery();
                }
                catch (MySqlException sEx)
                {
                    _logger.ErrorException("Failed to execute non query, SQL command=[{0}]", sEx, cmdText);
                }
                catch (Exception ex)
                {
                    _logger.FatalException("", ex);
                }

                cmd.Parameters.Clear();
                return val;
            }
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
            int cnt = -1;
            if (string.IsNullOrEmpty(connectionString))
                return cnt;
            if (cmdTexts == null || cmdTexts.Count() <= 0)
                return 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    cnt = ExecuteNonQuery(trans, cmdType, string.Join(";", cmdTexts.Where(w => w.IsNotNullNorEmptyNorWhitespace()).Select(s => s.Trim(';', ' '))), cmdParms);

                    if (cnt > 0)
                        trans.Commit();
                    else
                        cnt = 0;
                }
            }

            return cnt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdTexts"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public int ExecuteNonQueryNew(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms)
        {
            int cnt = -1;
            if (string.IsNullOrEmpty(connectionString))
                return cnt;
            if (cmdTexts == null || cmdTexts.Count() <= 0)
                return 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    { //循环
                        foreach (string cmdText in cmdTexts)
                        {
                            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        cnt = 1;
                    }
                    catch (Exception ex)
                    {
                        //cnt = 0;
                        _logger.ErrorException("Failed to execute non query, SQL command=[{0}]", ex, cmdTexts);
                        trans.Rollback();
                    }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                }
            }

            return cnt;
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                    MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return rdr;
                }
                catch (Exception e)
                {
                    connection.Close();
                    throw e;
                }
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行Insert Sql语句，并返回最新插入记录的Id值(它是自增长的)。
        /// </summary>
        public int InsertGetLastId(string connectionString, string pSql, string tblName)
        {
            int nResult = 0;

            string strSql = pSql + ";select LAST_INSERT_ID()";
            object oVal = ExecuteScalar(connectionString, CommandType.Text, strSql, null);
            if (oVal != null)
                nResult = Convert.ToInt32(oVal.ToString());

            return nResult;
        }

        /// <summary>
        /// 生成要执行的命令
        /// </summary>
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType,
            string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            // 如果存在参数，则表示用户是用参数形式的SQL语句，可以替换
            if (cmdParms != null && cmdParms.Length > 0)
                cmdText = cmdText.Replace("@", "?").Replace(":", "?");

            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                {
                    parm.ParameterName = parm.ParameterName.Replace("@", "?").Replace(":", "?");
                    cmd.Parameters.Add(parm);
                }
            }
        }

        public int ExecuteNonQuery(string connectionString, int iTimeOut, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行sql command, 返回受影响函数.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int InsertGetAffectedRows(string connectionString, string sql, List<DbParameter> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return -1;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 根据SQL语句, 查询数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> Query(string connectionString, string sql, object param = null)
        {
            IEnumerable<dynamic> rslt;
            if (connectionString.IsNullOrEmptyOrWhiteSpace() || sql.IsNullOrEmptyOrWhiteSpace())
                return null;

            using (var conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                rslt = conn.Query(sql, param);
            }

            return rslt;
        }
    }
}