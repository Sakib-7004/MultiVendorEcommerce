using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class CustomerProfileForm : Form
    {
        private TextBox txtFull, txtEmail, txtPhone,
                        txtAddr, txtOldPass, txtNewPass;

        public CustomerProfileForm()
        {
            Text = "My Profile";
            Size = new Size(600, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(194, 24, 91)
            };
            header.Controls.Add(new Label
            {
                Text = "👤  My Profile",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(300, 32)
            });

            var fp = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.White
            };

            var l1 = L("Full Name", 25, 25); txtFull = TB(25, 50, 520);
            var l2 = L("Email", 25, 95); txtEmail = TB(25, 120, 520);
            var l3 = L("Phone", 25, 165); txtPhone = TB(25, 190, 520);
            var l4 = L("Address", 25, 235); txtAddr = TB(25, 260, 520);

            var btnSave = new Button
            {
                Text = "💾 Save Profile",
                Location = new Point(25, 308),
                Size = new Size(160, 42),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(56, 142, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveProfile;

            var sep = new Label
            {
                Text = "──── Change Password ────",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(25, 365),
                Size = new Size(400, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var l5 = L("Old Password", 25, 392);
            txtOldPass = TB(25, 417, 240);
            txtOldPass.PasswordChar = '●';

            var l6 = L("New Password", 280, 392);
            txtNewPass = TB(280, 417, 240);
            txtNewPass.PasswordChar = '●';

            var btnChg = new Button
            {
                Text = "🔑 Change Password",
                Location = new Point(25, 460),
                Size = new Size(180, 42),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChg.FlatAppearance.BorderSize = 0;
            btnChg.Click += ChangePass;

            fp.Controls.AddRange(new Control[]
            {
                l1, txtFull, l2, txtEmail,
                l3, txtPhone, l4, txtAddr, btnSave,
                sep, l5, txtOldPass, l6, txtNewPass, btnChg
            });

            Controls.Add(fp);
            Controls.Add(header);

            LoadProfile();
        }

        private void LoadProfile()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Customers WHERE CustomerID=@id",
                new[] { new SqlParameter("@id", Program.CustomerID) });
            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                txtFull.Text = r["FullName"]?.ToString();
                txtEmail.Text = r["Email"]?.ToString();
                txtPhone.Text = r["Phone"]?.ToString();
                txtAddr.Text = r["Address"]?.ToString();
            }
        }

        private void SaveProfile(object s, EventArgs e)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Customers
                  SET FullName=@fn, Email=@em, Phone=@ph, Address=@addr
                  WHERE CustomerID=@id",
                new[]
                {
                    new SqlParameter("@fn",   txtFull.Text.Trim()),
                    new SqlParameter("@em",   txtEmail.Text.Trim()),
                    new SqlParameter("@ph",   txtPhone.Text.Trim()),
                    new SqlParameter("@addr", txtAddr.Text.Trim()),
                    new SqlParameter("@id",   Program.CustomerID)
                });
            MessageBox.Show("✅ Profile updated!",
                "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ChangePass(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOldPass.Text) ||
                string.IsNullOrWhiteSpace(txtNewPass.Text)) return;

            var chk = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE UserID=@id AND Password=@p",
                new[]
                {
                    new SqlParameter("@id", Program.LoggedInUserID),
                    new SqlParameter("@p",  txtOldPass.Text.Trim())
                });

            if (Convert.ToInt32(chk) == 0)
            {
                MessageBox.Show("Old password is incorrect!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Users SET Password=@p WHERE UserID=@id",
                new[]
                {
                    new SqlParameter("@p",  txtNewPass.Text.Trim()),
                    new SqlParameter("@id", Program.LoggedInUserID)
                });

            MessageBox.Show("✅ Password changed successfully!");
            txtOldPass.Clear();
            txtNewPass.Clear();
        }

        private Label L(string t, int x, int y) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(x, y),
            Size = new Size(220, 22)
        };
        private TextBox TB(int x, int y, int w) => new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(w, 28),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle
        };
    }
}