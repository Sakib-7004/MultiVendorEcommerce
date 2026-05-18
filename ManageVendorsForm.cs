using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageVendorsForm : Form
    {
        private DataGridView dgv;

        public ManageVendorsForm()
        {
            Text = "Manage Vendors";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(0, 150, 136)
            };
            header.Controls.Add(new Label
            {
                Text = "🏪  Manage Vendors",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(350, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(224, 242, 241)
            };

            var btnApprove = Btn("✔ Approve", 10, 10,
                Color.FromArgb(56, 142, 60));
            btnApprove.Click += (s, e) => SetApproval(1);

            var btnReject = Btn("✖ Reject", 145, 10,
                Color.FromArgb(211, 47, 47));
            btnReject.Click += (s, e) => SetApproval(0);

            var btnRefresh = Btn("🔄 Refresh", 280, 10,
                Color.FromArgb(0, 150, 136));
            btnRefresh.Click += (s, e) => LoadVendors();

            toolbar.Controls.AddRange(new Control[]
                { btnApprove, btnReject, btnRefresh });

            dgv = MakeDGV(Color.FromArgb(0, 150, 136));

            Controls.Add(dgv);
            Controls.Add(toolbar);
            Controls.Add(header);

            LoadVendors();
        }

        private void LoadVendors()
        {
            string sql =
                @"SELECT v.VendorID, u.Username, v.StoreName,
                    v.Phone, v.Address,
                    CASE v.IsApproved WHEN 1 THEN 'Approved' ELSE 'Pending' END AS Status,
                    CONVERT(VARCHAR,u.CreatedDate,103) AS Joined
                  FROM Vendors v
                  JOIN Users u ON u.UserID = v.UserID
                  ORDER BY u.CreatedDate DESC";
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void SetApproval(int val)
        {
            if (dgv.SelectedRows.Count == 0) return;
            int vid = Convert.ToInt32(
                dgv.SelectedRows[0].Cells["VendorID"].Value);
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Vendors SET IsApproved=@v WHERE VendorID=@id",
                new[]
                {
                    new SqlParameter("@v",  val),
                    new SqlParameter("@id", vid)
                });
            LoadVendors();
        }

        private Button Btn(string t, int x, int y, Color c)
        {
            var b = new Button
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = c,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private DataGridView MakeDGV(Color hc)
        {
            var d = new DataGridView
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
            d.ColumnHeadersDefaultCellStyle.BackColor = hc;
            d.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            d.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            d.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(224, 242, 241);
            return d;
        }
    }
}