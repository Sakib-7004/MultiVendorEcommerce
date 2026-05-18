using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageUsersForm : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbRole;

        public ManageUsersForm()
        {
            Text = "Manage Users";
            Size = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(26, 35, 126)
            };
            header.Controls.Add(new Label
            {
                Text = "👥  Manage Users",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 14),
                Size = new Size(300, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(232, 234, 246)
            };

            toolbar.Controls.Add(new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 16),
                Size = new Size(60, 22),
                ForeColor = Color.FromArgb(26, 35, 126)
            });

            txtSearch = new TextBox
            {
                Location = new Point(75, 13),
                Size = new Size(250, 28),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => LoadUsers();

            toolbar.Controls.Add(new Label
            {
                Text = "Role:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(340, 16),
                Size = new Size(40, 22),
                ForeColor = Color.FromArgb(26, 35, 126)
            });

            cmbRole = new ComboBox
            {
                Location = new Point(385, 13),
                Size = new Size(130, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[]
                { "All", "Admin", "Vendor", "Customer" });
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += (s, e) => LoadUsers();

            var btnToggle = new Button
            {
                Text = "🔄 Toggle Active",
                Location = new Point(540, 12),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += BtnToggle_Click;

            var btnDel = new Button
            {
                Text = "🗑️ Delete",
                Location = new Point(695, 12),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(211, 47, 47),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDel.FlatAppearance.BorderSize = 0;
            btnDel.Click += BtnDel_Click;

            toolbar.Controls.AddRange(new Control[]
                { txtSearch, cmbRole, btnToggle, btnDel });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                EnableHeadersVisualStyles = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(26, 35, 126);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 242, 255);

            Controls.Add(dgv);
            Controls.Add(toolbar);
            Controls.Add(header);

            LoadUsers();
        }

        private void LoadUsers()
        {
            string where = "WHERE 1=1";
            var prms = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                where += " AND (Username LIKE @s OR Email LIKE @s OR FullName LIKE @s)";
                prms.Add(new SqlParameter("@s",
                    "%" + txtSearch.Text.Trim() + "%"));
            }
            if (cmbRole.SelectedItem?.ToString() != "All")
            {
                where += " AND Role=@r";
                prms.Add(new SqlParameter("@r",
                    cmbRole.SelectedItem.ToString()));
            }

            string sql =
                $@"SELECT UserID, Username, FullName, Email, Phone, Role,
                     CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                     CONVERT(VARCHAR,CreatedDate,103) AS Joined
                   FROM Users {where}
                   ORDER BY CreatedDate DESC";

            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql, prms.ToArray());
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            int uid = Convert.ToInt32(
                dgv.SelectedRows[0].Cells["UserID"].Value);
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Users SET IsActive=CASE IsActive WHEN 1 THEN 0 ELSE 1 END WHERE UserID=@id",
                new[] { new SqlParameter("@id", uid) });
            LoadUsers();
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            int uid = Convert.ToInt32(
                dgv.SelectedRows[0].Cells["UserID"].Value);
            if (MessageBox.Show("Delete this user?", "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM Users WHERE UserID=@id",
                    new[] { new SqlParameter("@id", uid) });
                LoadUsers();
            }
        }
    }
}