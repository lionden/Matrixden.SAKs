namespace Matrixden.UnifiedDBAdapter
{
    using System;
    using System.Data.Common;
    using System.Data;
    using System.Collections.Generic;

    interface IDBHelper
    {
        /// <summary>
        /// 根据SQL语句, 查询数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<dynamic> Query(string connectionString, string sql, object param = null);

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="sql">sql命令, 带参数</param>
        /// <param name="param">实体</param>
        /// <returns>受影响的行数</returns>
        int ExecuteNonQuery(string connectionString, string sql, object param);

        /// <summary>
        /// 执行 Transact-SQL 语句(带参数), 并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdText">sql命令, 带参数</param>
        /// <param name="parameters">参数集合.</param>
        /// <returns>受影响的行数。</returns>
        int ExecuteNonQuery(string connectionString, string cmdText, List<DbParameter> parameters);

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        int ExecuteNonQuery(string connectionString, int iTimeOut, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        DataSet ExecuteQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="cmdTexts">SQL list</param>
        /// <param name="cmdParms">SQL parms</param>
        /// <returns></returns>
        int ExecuteNonQuery(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms);

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="cmdTexts">SQL list</param>
        /// <param name="cmdParms">SQL parms</param>
        /// <returns></returns>
        int ExecuteNonQueryNew(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms);
        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

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
        DbDataReader GetPageList(string connectionString, string tblName, int pageSize, int pageIndex, string fldSort, bool sort, string condition);

        /// <summary>
        /// 得到数据条数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tblName">表名</param>
        /// <param name="condition">条件(不需要where)</param>
        /// <returns>数据条数</returns>
        int GetCount(string connectionString, string tblName, string condition);

        /// <summary>
        /// 执行一条插入语句并且
        /// 获取单一会话内最后一条Insert语句生成的自动增长ID值
        /// 该接口是为了获取每个用户在执行某段Insert代码时自身生成的自动增长的ID值，
        /// 防止因大量并发导致ID值获取错误，引起数据外键值错误
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="pSql"></param>
        /// <param name="tblName">表名</param>
        /// <returns>最新生成的ID值</returns>
        int InsertGetLastId(string connectionString, string pSql, string tblName);

        /// <summary>
        /// 执行sql command, 返回受影响的行数.
        /// 使用sql parameter传参, 防止sql注入.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int InsertGetAffectedRows(string connectionString, string sql, List<DbParameter> parameters);
    }
}
