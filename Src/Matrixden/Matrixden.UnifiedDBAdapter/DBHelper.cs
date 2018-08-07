namespace Matrixden.UnifiedDBAdapter
{
    using System.Data.Common;
    using System.Data;
    using System.Data.SqlClient;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;

    /// <summary>
    /// ���ݿ����
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// ö�٣����ݿ�����
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
        /// ���ݿ�����
        /// </summary>
        public DatabaseTypes DatabaseType
        {
            get { return databaseType; }
            set { databaseType = value; }
        }

        /// <summary>
        /// ���ݿ������ַ���
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
        /// �������ݿ�����
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

        #region === ����DbParameter��ʵ�� ===

        /// <summary>
        /// ��������DbParameter��ʵ��
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, int size, object value)
        {
            return CreateDbParameter(paraName, dbType, size, value, ParameterDirection.Input);
        }

        /// <summary>
        /// ��������DbParameter��ʵ��
        /// </summary>
        public DbParameter CreateInDbParameter(string paraName, DbType dbType, object value)
        {
            return CreateDbParameter(paraName, dbType, 0, value, ParameterDirection.Input);
        }

        /// <summary>
        /// �������DbParameter��ʵ��
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.Output);
        }

        /// <summary>
        /// �������DbParameter��ʵ��
        /// </summary>        
        public DbParameter CreateOutDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.Output);
        }

        /// <summary>
        /// ���췵��DbParameter��ʵ��
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType, int size)
        {
            return CreateDbParameter(paraName, dbType, size, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// ���췵��DbParameter��ʵ��
        /// </summary>        
        public DbParameter CreateReturnDbParameter(string paraName, DbType dbType)
        {
            return CreateDbParameter(paraName, dbType, 0, null, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// ����DbParameter��ʵ��
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

        #region === ���ݿ�ִ�з��� ===

        /// <summary>
        /// ִ�� Transact-SQL ���(������), ��������Ӱ���������
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="sql">sql����, ������</param>
        /// <param name="param">ʵ��</param>
        /// <returns>��Ӱ�������</returns>
        public int ExecuteNonQuery(string connectionString, string sql, object param = null)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, sql, param);
        }

        /// <summary>
        /// ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(int iTimeOut, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, iTimeOut, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ��������ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ�� Transact-SQL ���(������), ��������Ӱ���������
        /// </summary>
        /// <param name="cmdText">sql����, ������</param>
        /// <param name="parameters">��������.</param>
        /// <returns>��Ӱ���������</returns>
        public int ExecuteNonQuery(string cmdText, List<DbParameter> parameters)
        {
            return _DBHelper.ExecuteNonQuery(connectionString, cmdText, parameters);
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
            return _DBHelper.ExecuteNonQuery(connectionString, cmdType, cmdTexts, cmdParms);
        }
        /// <summary>
        /// ��������ִ�� Transact-SQL ��䲢������Ӱ���������
        /// </summary>
        /// <param name="connectionString">���ݿ����Ӵ�</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="cmdTexts">SQL list</param>
        /// <param name="cmdParms">SQL parms</param>
        ///    /// <returns></returns>
        public int ExecuteNonQueryNew(string connectionString, CommandType cmdType, IEnumerable<string> cmdTexts, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteNonQueryNew(connectionString, cmdType, cmdTexts, cmdParms);
        }
        /// <summary>
        /// ��������ִ�в�ѯ������DataSet
        /// </summary>
        public DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteQuery(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ�в�ѯ������DataSet
        /// </summary>
        public DataSet ExecuteQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteQuery(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ��������ִ�в�ѯ������DataReader
        /// </summary>
        public DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteReader(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ�в�ѯ������DataReader
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteReader(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ��������ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����������л��С�
        /// </summary>
        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteScalar(trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����������л��С�
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return _DBHelper.ExecuteScalar(connectionString, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// ִ��Insert Sql��䣬���������²����¼��Idֵ(������������)��
        /// �ýӿ���Ϊ�˻�ȡÿ���û���ִ��ĳ��Insert����ʱ�������ɵ��Զ�������IDֵ��
        /// ��ֹ�������������IDֵ��ȡ���������������ֵ����
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="pSql"></param>
        /// <param name="tblName">����</param>
        /// <returns>�������ɵ�IDֵ</returns>
        public int InsertGetLastId(string connectionString, string pSql, string tblName)
        {
            return _DBHelper.InsertGetLastId(connectionString, pSql, tblName);
        }

        /// <summary>
        /// ִ��sql command, ������Ӱ�������.
        /// ʹ��sql parameter����, ��ֹsqlע��.
        /// </summary>
        /// <param name="sql">�����ַ���</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertGetAffectedRows(string sql, List<DbParameter> parameters)
        {
            return _DBHelper.InsertGetAffectedRows(connectionString, sql, parameters);
        }

        /// <summary>
        /// ��ҳ��ȡ����
        /// </summary>
        /// <param name="tblName">����</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <param name="pageIndex">�ڼ�ҳ</param>
        /// <param name="fldSort">�����ֶ�</param>
        /// <param name="sort">����{False}/����(True)</param>
        /// <param name="condition">����(����Ҫwhere)</param>
        public DbDataReader GetPageList(string tblName, int pageSize, int pageIndex, string fldSort, bool sort, string condition)
        {
            return _DBHelper.GetPageList(connectionString, tblName, pageSize, pageIndex, fldSort, sort, condition);
        }

        /// <summary>
        /// �õ���������
        /// </summary>
        /// <param name="tblName">����</param>
        /// <param name="condition">����(����Ҫwhere)</param>
        /// <returns>��������</returns>
        public int GetCount(string tblName, string condition)
        {
            return _DBHelper.GetCount(connectionString, tblName, condition);
        }

        #endregion

        /// <summary>
        /// ����SQL���, ��ѯ���ݿ�
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
