using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageReviewsForm : Form
    {
        private DataGridView dgv;

        public ManageReviewsForm()
        {
            Text = "Manage Reviews";
            Size = new Size(1000, 580);
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
                Text = "⭐  Manage Reviews",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(300, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(252, 228, 236)
            };

            var btnDel = new Button
            {
                Text = "🗑️ Delete Review",
                Location = new Point(10, 9),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(194, 24, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDel.FlatAppearance.BorderSize = 0;
            btnDel.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) return;
                int rid = Convert.ToInt32(
                    dgv.SelectedRows[0].Cells["ReviewID"].Value);
                if (MessageBox.Show("Delete?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "DELETE FROM Reviews WHERE ReviewID=@id",
                        new[] { new SqlParameter("@id", rid) });
                    LoadReviews();
                }
            };

            var btnRef = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(170, 9),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(194, 24, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRef.FlatAppearance.BorderSize = 0;
            btnRef.Click += (s, e) => LoadReviews();

            toolbar.Controls.AddRange(new Control[] { btnDel, btnRef });

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
                Color.FromArgb(194, 24, 91);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(252, 228, 236);

            Controls.Add(dgv);
            Controls.Add(toolbar);
            Controls.Add(header);

            LoadReviews();
        }

        private void LoadReviews()
        {
            string sql =
                @"SELECT r.ReviewID, p.Name AS Product,
                    c.FullName AS Customer,
                    r.Rating, r.Comment,
                    CONVERT(VARCHAR,r.ReviewDate,103) AS ReviewDate
                  FROM Reviews r
                  JOIN Products  p ON p.ProductID  = r.ProductID
                  JOIN Customers c ON c.CustomerID = r.CustomerID
                  ORDER BY r.ReviewDate DESC";
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }
    }
}