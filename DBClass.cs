using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JGVocationalExamClient.DAL
{
    public sealed class DBClass
    {
        private static string varDBType;
        private static string varStrConnection;
        private static string varStrCommand;
        public static string DBType
        {
            get
            {
                return varDBType;
            }
        }
        public static string StrConnection
        {
            get
            {
                return varStrConnection;
            }
            set
            {
                if (varStrConnection != value)
                {
                    varStrConnection = value;
                    Regex reg = new Regex("OLEDB");
                    Match mat = reg.Match(varStrConnection);
                    if (mat.Success)
                    {
                        varDBType = "access";
                    }
                    else
                    {
                        varDBType = "sql";
                    }
                }
            }
        }
        public static string StrCommand
        {
            get
            {
                return varStrCommand;
            }
            set
            {
                if (varStrCommand != value)
                {
                    varStrCommand = value;
                }
            }
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            Regex reg = new Regex("OLEDB");
            Match mat = reg.Match(connectionString);
            if (mat.Success)
            {
                varDBType = "access";
                return OleDbHelper.ExecuteDataset(connectionString, commandType, commandText);
            }
            else
            {
                varDBType = "sql";
                return SqlHelper.ExecuteDataset(connectionString, commandType, commandText);
            }
        }
        public static DataSet ExecuteDataset(string commandText)
        {
            if (varDBType == "access")
            {
                return OleDbHelper.ExecuteDataset(varStrConnection, CommandType.Text, commandText);
            }
            else
            {
                return SqlHelper.ExecuteDataset(varStrConnection + ";Connect Timeout=8", CommandType.Text, commandText);
            }            
        }
        public static DataSet ExecuteDataset()
        {
            if (varDBType == "access")
            {
                return OleDbHelper.ExecuteDataset(varStrConnection, CommandType.Text, varStrCommand);
            }
            else 
            {
                return SqlHelper.ExecuteDataset(varStrConnection, CommandType.Text, varStrCommand);                
            }
        }
        public static Boolean TestConnect()
        {
            try
            {
                if (varDBType == "access")
                {
                    OleDbConnection conn = new OleDbConnection(varStrConnection + ";Connect Timeout=8");
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    conn.Open();
                    conn.Close();
                    cmd = null;
                    conn = null;
                }
                else
                {
                    SqlConnection conn = new SqlConnection(varStrConnection + ";Connect Timeout=8");
                    //SqlConnection conn = new SqlConnection(varStrConnection);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    conn.Open();
                    conn.Close();
                    cmd = null;
                    conn = null;
                }
                return true;
            }
            catch (SqlException ex)
            {
              //  MessageBox.Show ( "数据库连接失败。请检查网络或者配置信息" );
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show ( ex.Message );
                return false;
            }

        }
    
    }
}
