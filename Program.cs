using System;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    static class Program
    {
        public static int LoggedInUserID = 0;
        public static string LoggedInUser = "";
        public static string UserRole = "";
        public static int VendorID = 0;
        public static int CustomerID = 0;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // This will show EXACT error message
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MultiVendorDB;Integrated Security=True;Encrypt=False;"))
                {
                    conn.Open();
                    MessageBox.Show("✅ Connected Successfully!\nDatabase is working.",
                        "Connection OK",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "❌ Connection Failed!\n\n" +
                    "Error: " + ex.Message + "\n\n" +
                    "Inner: " + ex.InnerException?.Message,
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}