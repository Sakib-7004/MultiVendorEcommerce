using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class LoginForm : Form
    {
        private Panel panelLeft, panelRight;
        private TextBox txtUsername, txtPassword;
        private Button btnLogin, btnRegister;
        private Label lblError;
        private CheckBox chkShow;

        public LoginForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "ShopHub - Login";
            Size = new Size(820, 520);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            // LEFT PANEL
            panelLeft = new Panel
            {
                Size = new Size(350, 520),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(26, 35, 126)
            };

            var lblIcon = new Label
            {
                Text = "🛒",
                Font = new Font("Segoe UI", 48),
                ForeColor = Color.White,
                Size = new Size(320, 80),
                Location = new Point(15, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblApp = new Label
            {
                Text = "ShopHub",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(320, 50),
                Location = new Point(15, 165),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTag = new Label
            {
                Text = "Multi-Vendor E-Commerce",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(197, 202, 233),
                Size = new Size(320, 30),
                Location = new Point(15, 220),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblFeat = new Label
            {
                Text = "✔  Multiple Vendors\n" +
                            "✔  Product Management\n" +
                            "✔  Order Tracking\n" +
                            "✔  Customer Reviews\n" +
                            "✔  Revenue Analytics",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(197, 202, 233),
                Size = new Size(280, 130),
                Location = new Point(35, 300)
            };

            panelLeft.Controls.AddRange(new Control[]
                { lblIcon, lblApp, lblTag, lblFeat });

            // RIGHT PANEL
            panelRight = new Panel
            {
                Size = new Size(470, 520),
                Location = new Point(350, 0),
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Size = new Size(420, 45),
                Location = new Point(25, 55)
            };

            var lblSub = new Label
            {
                Text = "Sign in to your account",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Size = new Size(420, 28),
                Location = new Point(25, 103)
            };

            var lblU = MkLbl("Username", 25, 150);
            txtUsername = MkTxt(25, 175, 410);
            txtUsername.Text = "admin";

            var lblP = MkLbl("Password", 25, 225);
            txtPassword = MkTxt(25, 250, 410);
            txtPassword.PasswordChar = '●';
            txtPassword.Text = "admin123";

            chkShow = new CheckBox
            {
                Text = "Show password",
                Location = new Point(25, 288),
                Size = new Size(150, 22),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            chkShow.CheckedChanged += (s, e) =>
                txtPassword.PasswordChar = chkShow.Checked ? '\0' : '●';

            lblError = new Label
            {
                Size = new Size(410, 24),
                Location = new Point(25, 316),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red
            };

            btnLogin = new Button
            {
                Text = "LOGIN",
                Size = new Size(195, 45),
                Location = new Point(25, 348),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 35, 126),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "REGISTER",
                Size = new Size(195, 45),
                Location = new Point(240, 348),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += (s, e) => new RegisterForm().ShowDialog();

            var lblHint = new Label
            {
                Text = "admin/admin123  |  vendor/vendor123  |  customer/cust123",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Size = new Size(420, 22),
                Location = new Point(25, 415),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelRight.Controls.AddRange(new Control[]
            {
                lblTitle, lblSub,
                lblU, txtUsername,
                lblP, txtPassword, chkShow,
                lblError, btnLogin, btnRegister, lblHint
            });

            Controls.AddRange(new Control[] { panelLeft, panelRight });

            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BtnLogin_Click(s, e);
            };
        }

        private Label MkLbl(string t, int x, int y)
        {
            return new Label
            {
                Text = t,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                Size = new Size(200, 22)
            };
        }

        private TextBox MkTxt(int x, int y, int w)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 32),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "⚠  Please enter username and password.";
                return;
            }

            string sql = "SELECT * FROM Users WHERE Username=@u AND Password=@p AND IsActive=1";
            SqlParameter[] prms =
            {
                new SqlParameter("@u", txtUsername.Text.Trim()),
                new SqlParameter("@p", txtPassword.Text.Trim())
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, prms);

            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                Program.LoggedInUserID = Convert.ToInt32(r["UserID"]);
                Program.LoggedInUser = r["Username"].ToString();
                Program.UserRole = r["Role"].ToString();

                if (Program.UserRole == "Vendor")
                {
                    var vdt = DatabaseHelper.ExecuteQuery(
                        "SELECT VendorID FROM Vendors WHERE UserID=@uid",
                        new[] { new SqlParameter("@uid", Program.LoggedInUserID) });
                    if (vdt.Rows.Count > 0)
                        Program.VendorID = Convert.ToInt32(vdt.Rows[0]["VendorID"]);
                }
                else if (Program.UserRole == "Customer")
                {
                    var cdt = DatabaseHelper.ExecuteQuery(
                        "SELECT CustomerID FROM Customers WHERE UserID=@uid",
                        new[] { new SqlParameter("@uid", Program.LoggedInUserID) });
                    if (cdt.Rows.Count > 0)
                        Program.CustomerID = Convert.ToInt32(cdt.Rows[0]["CustomerID"]);
                }

                Hide();

                Form dash;
                if (Program.UserRole == "Admin")
                    dash = new AdminDashboard();
                else if (Program.UserRole == "Vendor")
                    dash = new VendorDashboard();
                else
                    dash = new CustomerDashboard();

                dash.FormClosed += (s2, e2) => Close();
                dash.Show();
            }
            else
            {
                lblError.Text = "❌  Invalid username or password!";
                txtPassword.Clear();
            }
        }
    }
}