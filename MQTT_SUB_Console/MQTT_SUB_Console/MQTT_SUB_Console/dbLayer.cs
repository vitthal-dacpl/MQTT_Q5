﻿
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_SUB_Console
{
    public class dbLayer
    {
        public dbLayer()
        {
            //SqlCon = ConfigurationManager.ConnectionStrings["Connstring1"].ToString();
            SqlCon = ConfigurationManager.AppSettings["Sqlconnstring"].ToString();


        }

        string SqlCon;

        #region ExecSqlNonQuery
        public int ExecSqlNonQuery(string strSQL, CommandType cmdType)
        {
            return ExecSqlNonQuery(strSQL, cmdType, null);
        }
        public int ExecSqlNonQuery(string strSQL, CommandType cmdType, List<SqlParameter> ListSqlParams)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            try
            {
                getSqlPara(ListSqlParams, cmd);
                cmd.CommandType = cmdType;
                cmd.Connection = Conn;
                Conn.Open();
                cmd.CommandText = strSQL;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                cmd.Dispose();
            }
            return 0;
        }
        #endregion

        #region ExecSqlScalar
        public object ExecSqlScalar(string strSQL, CommandType cmdtype, List<SqlParameter> ListSqlParams)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            try
            {
                getSqlPara(ListSqlParams, cmd);
                cmd.CommandType = cmdtype;
                cmd.Connection = Conn;
                Conn.Open();
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 120;
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                cmd.Dispose();
            }
            return null;
        }

        public object ExecSqlScalar(string strSQL, CommandType cmdtype)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandType = cmdtype;
                cmd.Connection = Conn;
                Conn.Open();
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 120;
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                cmd.Dispose();
            }
            return null;
        }
        #endregion

        #region ExecSqlDataReader
        public SqlDataReader ExecSqlDataReader(string strSQL, CommandType cmdtype, List<SqlParameter> ListSqlParams)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            try
            {
                getSqlPara(ListSqlParams, cmd);
                cmd.CommandType = cmdtype;
                cmd.Connection = Conn;
                Conn.Open();
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 120;
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                cmd.Dispose();
            }
            return null;
        }
        public SqlDataReader ExecSqlDataReader(string strSQL, CommandType cmdtype)
        {
            return ExecSqlDataReader(strSQL, cmdtype, null);
        }
        #endregion

        #region ExecSqlDataSet
        public DataSet ExecSqlDataSet(string strSQL, CommandType cmdtype)
        {
            return ExecSqlDataSet(strSQL, cmdtype, null);
        }
        public DataSet ExecSqlDataSet(string strSQL, CommandType cmdtype, List<SqlParameter> ListSqlParams)
        {
            DataSet ds = new DataSet("DataSet");
            ExecSqlDataSet(ds, strSQL, cmdtype, ListSqlParams);
            return ds;
        }
        public void ExecSqlDataSet(DataSet ds, string strSQL, CommandType cmdtype)
        {
            ExecSqlDataSet(ds, strSQL, cmdtype, null);
        }
        public void ExecSqlDataSet(DataSet ds, string strSQL, CommandType cmdtype, List<SqlParameter> ListSqlParams)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                getSqlPara(ListSqlParams, cmd);
                cmd.CommandType = cmdtype;
                cmd.Connection = Conn;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 120;
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                cmd.Dispose();
            }
        }
        private void getSqlPara(List<SqlParameter> parameters, SqlCommand cmd)
        {
            if (parameters != null)
            {
                SqlParameter sqlPara;
                int iListCount = System.Convert.ToInt32(parameters.Count);
                if (iListCount > 0)
                {
                    for (var iCount = 0; iCount <= iListCount - 1; iCount++)
                    {
                        sqlPara = new SqlParameter();
                        sqlPara = parameters[iCount];
                        cmd.Parameters.Add(sqlPara);
                    }
                }
            }
        }
        #endregion

        #region ExecSqlRow
        public DataRow ExecuteSqlRow(string strSQL, CommandType cmdtype, List<SqlParameter> ListSqlParams)
        {
            SqlConnection Conn = new SqlConnection(SqlCon);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter oDataAdapter = new SqlDataAdapter();
            DataRow DataRow = null;
            DataTable oDataTable = new DataTable();
            try
            {
                getSqlPara(ListSqlParams, cmd);
                cmd.CommandType = cmdtype;
                cmd.Connection = Conn;
                Conn.Open();
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 120;
                oDataAdapter.Fill(oDataTable);
                cmd.Parameters.Clear();
                if (oDataTable.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    DataRow oRow = oDataTable.Rows[0];
                    return oRow;
                }
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                    Conn.Dispose();
                    oDataAdapter = null;
                }
            }
            return null;
        }
        #endregion

    }
}
