namespace Matrixden.UnifiedDBAdapter
{
    using Matrixden.Utils.Extensions;
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// ���ݿ��������(for SQL Server)
    /// </summary>
    internal class SqlHelper : IDBHelper
    {
        private string ConnectionString;
        //private SqlConnection connection = null;

        public SqlHelper(string ConnectionString)
        {
            // TODO: Complete member initialization
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// ��ȡ��ҳSQL
        /// </summary>
        /// <param name="condition">����</param>
        /// <param name="pageSize">ÿҳ��ʾ����</param>
        /// <param name="pageIndex">�ڼ�ҳ</param>
        /// <param name="fldSort">�����ֶΣ����һ������Ҫ��д�����ǵ������磺id asc, name��</param>
        /// <param name="tblName">����</param>
        /// <param name="sort">���һ�������ֶε��������trueΪ����falseΪ����</param>
        /// <returns>�������ڷ�ҳ��SQL���</returns>
        private string GetPagerSQL(string condition, int pageSize, int pageIndex, string fldSort,
            string tblName, bool sort)
        {
            string strSort = sort ? " DESC" : " ASC";

            if (pageIndex == 1)
            {
                return "select top " + pageSize.ToString() + " * from " + tblName.ToString()
                    + ((string.IsNullOrEmpty(condition)) ? string.Empty : (" where " + condition))
                    + " order by " + fldSort.ToString() + strSort;
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("select top {0} * from {1} ", pageSize, tblName);
                strSql.AppendFormat(" where {1} not in (select top {0} {1} from {2} ", pageSize * (pageIndex - 1),
                    (fldSort.Substring(fldSort.LastIndexOf(',') + 1, fldSort.Length - fldSort.LastIndexOf(',') - 1)), tblName);
                if (!string.IsNullOrEmpty(condition))
                {
                    strSql.AppendFormat(" where {0} order by {1}{2}) and {0}", condition, fldSort, strSort);
                }
                else
                {
                    strSql.AppendFormat(" order by {0}{1}) ", fldSort, strSort);
                }
                strSql.AppendFormat(" order by {0}{1}", fldSort, strSort);
                return strSql.ToString();
            }
        }

        /// <summary>
        /// ��ҳ��ȡ����
        /// </summary>
        /// <param name="connectionString">�����ַ���</param>
        /// <param name="tblName">����</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <param name="pageIndex">�ڼ�ҳ</param>
        /// <param name="fldSort">�����ֶ�</param>
        /// <param name="sort">����{False}/����(True)</param>
        /// <param name="condition">����(����Ҫwhere)</param>
        public DbDataReader GetPageList(string connectionString, string tblName, int pageSize,
            int pageIndex, string fldSort, bool sort, string condition)
        {
            string sql = GetPagerSQL(condition, pageSize, pageIndex, fldSort, tblName, sort);
            return ExecuteReader(connectionString, CommandType.Text, sql, null);
        }

        /// <summary>
        /// �õ���������
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
        /// ִ�� Transact-SQL ���(������), ��������Ӱ���������
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="sql">sql����, ������</param>
        /// <param name="param">ʵ��</param>
        /// <returns>��Ӱ�������</returns>
        public int ExecuteNonQuery(string connectionString, string sql, object param)
        {
            var cnt = 0;
            if (connectionString.IsNullOrEmptyOrWhiteSpace() || sql.IsNullOrEmptyOrWhiteSpace())
                return -1;

            using (var conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cnt = conn.Execute(sql, param);
            }

            return cnt;
        }

        /// <summary>
        /// ִ�в�ѯ������DataSet
        /// </summary>
        public DataSet ExecuteQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
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
        /// ��������ִ�в�ѯ������DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ds");
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// ִ�� Transact-SQL ���(������), ��������Ӱ���������
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="cmdText">sql����, ������</param>
        /// <param name="parameters">��������.</param>
        /// <returns>��Ӱ���������</returns>
        public int ExecuteNonQuery(string connectionString, string cmdText, List<DbParameter> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary>
        /// ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(string connectionString, int iTimeOut, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = iTimeOut;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// ��������ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// ��������ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
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

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    cnt = ExecuteNonQuery(trans, cmdType, string.Join(";", cmdTexts.Where(w => w.IsNotNullNorEmptyNorWhitespace()).Select(s => s.Trim(';', ' ')), cmdParms));
                }
            }

            return cnt;
        }

        public int ExecuteNonQueryNew(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms)
        {
            int cnt = -1;
            if (string.IsNullOrEmpty(connectionString))
                return cnt;
            if (cmdTexts == null || cmdTexts.Count() <= 0)
                return 0;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    cnt = ExecuteNonQuery(trans, cmdType, string.Join(";", cmdTexts.ToArray()), cmdParms);
                }
            }

            return cnt;
        }

        /// <summary>
        /// ִ�в�ѯ������DataReader
        /// </summary>
        public DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                try
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
        /// ��������ִ�в�ѯ������DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        /// <summary>
        /// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����������л��С�
        /// </summary>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }

        }

        /// <summary>
        /// ��������ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����������л��С�
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        public IEnumerable<object> Query(Type type, string connectionString, string sql, object param = null)
        {
            IEnumerable<dynamic> rslt;
            if (connectionString.IsNullOrEmptyOrWhiteSpace() || sql.IsNullOrEmptyOrWhiteSpace())
                return null;

            using (var conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                rslt = conn.Query(sql, param);
            }

            return rslt;
        }

        /// <summary>
        /// ִ��Insert Sql��䣬���������²����¼��Idֵ(������������)��
        /// </summary>
        /// ��SqlServer���ݿ��У�����Microsoft�ĺ�����ȡ��ǰ�Ự����IDֵ
        public int InsertGetLastId(string connectionString, string pSql, string tblName)
        {
            int nResult = 0;

            string strSql = pSql + ";select @@ROWCOUNT";
            object oVal = ExecuteScalar(connectionString, CommandType.Text, strSql, null);
            if (oVal != null)
                nResult = Convert.ToInt32(oVal.ToString());

            return nResult;
        }

        /// <summary>
        /// ִ��sql command, ������Ӱ�캯��.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertGetAffectedRows(string connectionString, string sql, List<DbParameter> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// ����Ҫִ�е�����
        /// </summary>
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType,
            string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;

            // ������ڲ��������ʾ�û����ò�����ʽ��SQL��䣬�����滻
            if (cmdParms != null && cmdParms.Length > 0)
                cmdText = cmdText.Replace("?", "@").Replace(":", "@");

            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    parm.ParameterName = parm.ParameterName.Replace("?", "@").Replace(":", "@");
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}