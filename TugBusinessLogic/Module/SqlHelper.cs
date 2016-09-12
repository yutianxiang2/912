using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataModel;

namespace BusinessLogic.Module
{
    /// <summary>
    /// 用于原生的sql语句的操作
    /// </summary>
    public class SqlHelper
    {
        public static DataTable GetDatatableBySP(string spName, SqlParameter[] param = null)
        {
            DataTable dt = null;
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                //TugDataEntities db = new TugDataEntities();
                using (TugDataEntities db = new TugDataEntities())
                {
                    DbConnection con = db.Database.Connection;
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = spName;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (param != null && param.Length > 0) cmd.Parameters.AddRange(param);
                    System.Data.Common.DbDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    ds.Tables.RemoveAt(0);
                    if (dt != null) dt.TableName = spName;
                }
                return dt;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 用于执行单条sql语句，并返回datatable...
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDatatableBySql(string sql)
        {
            if (sql.Trim() == string.Empty) return null;
            DataTable dt = null;
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                using (TugDataEntities db = new TugDataEntities())
                {
                    DbConnection con = db.Database.Connection;
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    System.Data.Common.DbDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    ds.Tables.RemoveAt(0);
                }
                return dt;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据查询条件返回一个有数据的记录集
        /// </summary>
        /// <param name="tbName"></param>
        /// <param name="strWhere">"ID=1 And name='x'"</param>
        /// <returns></returns>
        public static DataTable GetDataTableData(string tbName, string strWhere = "")
        {
            DataTable dt = null;
            string sql = string.Format("select * from {0} where (1 = 1)", tbName);
            if (strWhere != "") sql += string.Format(" And {0}", strWhere);
            dt = GetDatatableBySql(sql);
            if (dt != null) dt.TableName = tbName;
            return dt;
        }

        /// <summary>
        /// 返回一个有数据的datatable，还有有数据结构
        /// </summary>
        /// <param name="tbName"></param>
        /// <returns></returns>
        public static DataTable GetDataTableStructure(string tbName)
        {
            DataTable dt = null;
            //string sql = string.Format("select * from {0} where (1 <> 1)",tbName);
            string sql = string.Format("select top 30 * from {0} where (1 = 1)", tbName);
            dt = GetDatatableBySql(sql);
            if (dt != null) dt.TableName = tbName;
            return dt;
        }
    }
}