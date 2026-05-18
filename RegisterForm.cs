using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class RegisterForm : Form
    {
        private TextBox txtUser, txtPass, txtConfirm,
                          txtEmail, txtFullName, txtPhone;
        private ComboBox cmbRole;
        private Button btnReg, btnCancel;

        public RegisterForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Register New Account";
            Size = new Size(520, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(26, 35, 126)
            };
            header.Controls.Add(new Label
            {
                Text = "🛒  Create New Account",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });

            int y = 88, gap = 58;

            var l1 = ML("Full Name *", 30, y);
            txtFullName = TB(30, y + 24, 440);
            y += gap;

            var l2 = ML("Username *", 30, y);
            txtUser = TB(30, y + 24, 440);
            y += gap;

            var l3 = ML("Email *", 30, y);
            txtEmail = TB(30, y + 24, 440);
            y += gap;

            var l4 = ML("Phone", 30, y);
            txtPhone = TB(30, y + 24, 440);
            y += gap;

            var l5 = ML("Password *", 30, y);
            txtPass = TB(30, y + 24, 200);
            txtPass.PasswordChar = '●';

            var l6 = ML("Confirm Password *", 245, y);
            txtConfirm = TB(245, y + 24, 225);
            txtConfirm.PasswordChar = '●';
            y += gap;

            var l7 = ML("Register As *", 30, y);
            cmbRole = new ComboBox
            {
                Location = new Point(30, y + 24),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[] { "Customer", "Vendor" });
            cmbRole.SelectedIndex = 0;
            y += gap;

            btnReg = new Button
            {
                Text = "✔  Register",
                Location = new Point(30, y),
                Size = new Size(200, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 35, 126),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReg.FlatAppearance.BorderSize = 0;
            btnReg.Click += BtnReg_Click;

            btnCancel = new Button
            {
                Text = "✖  Cancel",
                Location = new Point(260, y),
                Size = new Size(200, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(211, 47, 47),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,
                l1, txtFullName, l2, txtUser,
                l3, txtEmail,    l4, txtPhone,
                l5, txtPass,     l6, txtConfirm,
                l7, cmbRole,
                btnReg, btnCancel
            });
        }

        private Label ML(string t, int x, int y)
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

        private TextBox TB(int x, int y, int w)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnReg_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please fill all required fields!",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPass.Text != txtConfirm.Text)
            {
                MessageBox.Show("Passwords do not match!",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chk = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Username=@u",
                new[] { new SqlParameter("@u", txtUser.Text.Trim()) });

            if (Convert.ToInt32(chk) > 0)
            {
                MessageBox.Show("Username already exists!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string role = cmbRole.SelectedItem.ToString();
            string sqlUser =
                @"INSERT INTO Users (Username,Password,Email,FullName,Phone,Role,IsActive)
                  VALUES (@u,@p,@em,@fn,@ph,@role,1);
                  SELECT SCOPE_IDENTITY();";

            SqlParameter[] prms =
            {
                new SqlParameter("@u",    txtUser.Text.Trim()),
                new SqlParameter("@p",    txtPass.Text.Trim()),
                new SqlParameter("@em",   txtEmail.Text.Trim()),
                new SqlParameter("@fn",   txtFullName.Text.Trim()),
                new SqlParameter("@ph",   txtPhone.Text.Trim()),
                new SqlParameter("@role", role)
            };

            object uid = DatabaseHelper.ExecuteScalar(sqlUser, prms);

            if (uid != null)
            {
                int userID = Convert.ToInt32(uid);

                if (role == "Vendor")
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO Vendors (UserID,StoreName,IsApproved) VALUES (@uid,@sn,0)",
                        new[]
                        {
                            new SqlParameter("@uid", userID),
                            new SqlParameter("@sn",
                                txtFullName.Text.Trim() + "'s Store")
                        });
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO Customers (UserID,FullName,Email,Phone)
                          VALUES (@uid,@fn,@em,@ph)",
                        new[]
                        {
                            new SqlParameter("@uid", userID),
                            new SqlParameter("@fn",  txtFullName.Text.Trim()),
                            new SqlParameter("@em",  txtEmail.Text.Trim()),
                            new SqlParameter("@ph",  txtPhone.Text.Trim())
                        });
                }

                MessageBox.Show("✅ Registration successful! You can now login.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }
    }
}