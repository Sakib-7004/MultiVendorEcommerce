using System;
using System.Configuration; // Added to read App.config
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public static class DatabaseHelper
    {
        // Dynamically pulls your string from the App.config configuration settings
        public static string ConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MultiVendorDB;Integrated Security=True;Encrypt=False;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch { return false; }
        }

        public static DataTable ExecuteQuery(string sql, SqlParameter[] prms = null)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (prms != null) cmd.Parameters.AddRange(prms);
                        new SqlDataAdapter(cmd).Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Query Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql, SqlParameter[] prms = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (prms != null) cmd.Parameters.AddRange(prms);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Execution Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static object ExecuteScalar(string sql, SqlParameter[] prms = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (prms != null) cmd.Parameters.AddRange(prms);
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Scalar Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}