using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSan.Common
{
    /// <summary>
    /// Lớp truy cập CSDL dùng chung cho toàn bộ tầng DataAccess.
    /// Mọi phương thức đều gọi Stored Procedure (không nối chuỗi SQL) theo đúng yêu cầu đề bài.
    /// </summary>
    public static class DbHelper
    {
        private static string _connectionString;

        private static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    _connectionString = ConfigurationManager
                        .ConnectionStrings["QLKhachSanDb"].ConnectionString;
                }
                return _connectionString;
            }
        }

        public static void TestConnection()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
            }
        }

        public static int ExecuteNonQuery(string spName, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable ExecuteDataTable(string spName, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public static DataRow ExecuteDataRow(string spName, params SqlParameter[] parameters)
        {
            var table = ExecuteDataTable(spName, parameters);
            return table.Rows.Count > 0 ? table.Rows[0] : null;
        }

        public static object ExecuteScalar(string spName, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static SqlParameter OutputParam(string name, SqlDbType type, int size = 0)
        {
            var p = size > 0 ? new SqlParameter(name, type, size) : new SqlParameter(name, type);
            p.Direction = ParameterDirection.Output;
            return p;
        }
    }
}
