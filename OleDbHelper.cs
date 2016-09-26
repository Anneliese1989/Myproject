using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Collections;
using System.Data.Sql;
using System.Text;

namespace JGVocationalExamClient.DAL
{
    /// <summary>
    /// Summary description for OleDbHelper
    /// </summary>
    public sealed class OleDbHelper
    {

        //Database connection strings
        public static readonly string CONN_STRING = ConfigurationManager.AppSettings["OleDbConnectionString"];
        public static readonly string CONN_STRING1 = ConfigurationManager.AppSettings["OleDbConnectionString1"];
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        #region =ExecuteNonQuery=

        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(connString, cmdType, cmdText, null);
        }

        public static int ExecuteNonQuery(OleDbConnection conn, CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(conn, cmdType, cmdText, null);
        }

        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(trans, cmdType, cmdText, null);
        }

        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                //���cmd�Ĳ���
                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return val;
            }
        }

        public static int ExecuteNonQuery(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            return val;
        }

        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (cmd.Connection.State == ConnectionState.Open)
            {
                cmd.Connection.Close();
            }
            return val;
        }

        #endregion

        #region =ExecuteReader=

        public static OleDbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteReader(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        public static OleDbDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        #endregion

        #region =ExecuteDataset=

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            //using (OleDbConnection cn = new OleDbConnection(connectionString));

            OleDbConnection cn = new OleDbConnection(connectionString);
            {

                cn.Open();

                //�������ط���
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }



        public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (OleDbParameter[])null);
        }

        public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            //����һ��OleDbCommand���󣬲�������г�ʼ��
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters);

            //����OleDbDataAdapter�����Լ�DataSet
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();

            //���ds
            da.Fill(ds);

            // ���cmd�Ĳ�������   
            cmd.Parameters.Clear();
            if (cmd.Connection.State == ConnectionState.Open)
            {
                cmd.Connection.Close();
            }
            //����ds
            return ds;
        }

        #endregion

        #region =ExecuteDataTable=

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            using (OleDbConnection cn = new OleDbConnection(connectionString))
            {
                cn.Open();

                //�������ط���
                return ExecuteDataTable(cn, commandType, commandText, commandParameters);
            }
        }



        public static DataTable ExecuteDataTable(OleDbConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connection, commandType, commandText, (OleDbParameter[])null);
        }

        public static DataTable ExecuteDataTable(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            //����һ��OleDbCommand���󣬲�������г�ʼ��
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText,commandParameters);

            //����OleDbDataAdapter�����Լ�DataSet
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();

            //���ds
            da.Fill(ds);

            // ���cmd�Ĳ�������   
            cmd.Parameters.Clear();
            if (cmd.Connection.State == ConnectionState.Open)
            {
                cmd.Connection.Close();
            }
            //����ds
            return ds.Tables[0];
        }

        #endregion

        #region =ExecuteScalar=

        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText)
        {
            return ExecuteScalar(connString, cmdType, cmdText, null);
        }

        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return val;
            }
        }

        public static object ExecuteScalar(OleDbConnection conn, CommandType cmdType, string cmdText)
        {
            return ExecuteScalar(conn, cmdType, cmdText, null);
        }

        public static object ExecuteScalar(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            return val;
        }

        #endregion

        public static void CacheParameters(string cacheKey, params OleDbParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }
        public static OleDbParameter[] GetCachedParameters(string cacheKey)
        {
            OleDbParameter[] cachedParms = (OleDbParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OleDbParameter[] clonedParms = new OleDbParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OleDbParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        public static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            //�ж����ӵ�״̬������ǹر�״̬�����
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //cmd���Ը�ֵ
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //�Ƿ���Ҫ�õ�������
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            //���cmd��Ҫ�Ĵ洢���̲���
            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}

