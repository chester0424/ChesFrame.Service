using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    /// <summary>
    /// DbCommand的扩展
    /// </summary>
    public static class DbCommandExtend
    {
        /// <summary>
        /// 执行并返回第一行第一列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this DbCommand dbCommand)
        {
            using (var conn = dbCommand.Connection)
            {
                conn.Open();

                object scalar = dbCommand.ExecuteScalar();

                T result = (T)Convert.ChangeType(scalar, typeof(T));
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// 执行并返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static T ExecuteEntity<T>(this DbCommand dbCommand) where T : new()
        {
            using (var conn = dbCommand.Connection)
            {
                conn.Open();
                DbDataReader reader = dbCommand.ExecuteReader();
                var result = DataReaderModelConverter.ReaderToModel<T>(reader);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// 执行并返回实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static List<T> ExecuteEntityList<T>(this DbCommand dbCommand) where T : new()
        {
            using (var conn = dbCommand.Connection)
            {
                conn.Open();
                DbDataReader reader = dbCommand.ExecuteReader();
                var result = DataReaderModelConverter.ReaderToList<T>(reader);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// 执行并返回数据集
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this DbCommand dbCommand)
        {
            using (var conn = dbCommand.Connection)
            {
                conn.Open();

                DbDataReader reader = dbCommand.ExecuteReader();
                DataSet ds = new DataSet(); ;
                do
                {
                    if (reader.HasRows)
                    {
                        var dtReturn = CreateTableBySchemaTable(reader.GetSchemaTable());
                        var value = new object[reader.FieldCount];
                        while (reader.Read())
                        {
                            reader.GetValues(value);
                            dtReturn.LoadDataRow(value, true);
                        }
                        value = null;
                        ds.Tables.Add(dtReturn);
                    }
                }
                while (reader.NextResult());//下一个结果集

                conn.Close();

                return ds;
            }
        }

        /// <summary>
        /// 执行并返回数据表
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this DbCommand dbCommand)
        {
            using (var conn = dbCommand.Connection)
            {
                conn.Open();

                DbDataReader reader = dbCommand.ExecuteReader();
                DataTable dtReturn = null;//new DataTable();
                if (reader.HasRows)
                {
                    dtReturn = CreateTableBySchemaTable(reader.GetSchemaTable());
                    var value = new object[reader.FieldCount];
                    while (reader.Read())
                    {
                        reader.GetValues(value);
                        dtReturn.LoadDataRow(value, true);
                    }
                }
                conn.Close();
                return dtReturn;
            }
        }

        /// <summary>
        /// 获取送的行数（只针对分页查询后获取总行数）
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public static int? GetTotalCount(this DbCommand dbCommand)
        {
            if (dbCommand.Parameters != null
                && dbCommand.Parameters.Count > 0)
            {
                var outPname = SqlConst.TotalCount;//"@TotalCount";
                DbParameter pOut = dbCommand.Parameters[outPname];
                return int.Parse(pOut.Value.ToString());
            }
            return null;
        }

        #region helper

        private static DataTable CreateTableBySchemaTable(DataTable pSchemaTable)
        {
            DataTable dtReturn = new DataTable();
            DataColumn dc = null;
            DataRow dr = null;
            for (int i = 0; i < pSchemaTable.Rows.Count; i++)
            {
                dr = pSchemaTable.Rows[i];
                dc = new DataColumn(dr["ColumnName"].ToString(), dr["DataType"] as Type);
                dtReturn.Columns.Add(dc);
            }
            dr = null;
            dc = null;
            return dtReturn;
        }

        #endregion
    }
}
