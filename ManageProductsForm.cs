using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageProductsForm : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;

        public ManageProductsForm()
        {
            Text = "All Products";
            Size = new Size(1100, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(56, 142, 60)
            };
            header.Controls.Add(new Label
            {
                Text = "📦  All Products",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(350, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(232, 245, 233)
            };

            toolbar.Controls.Add(new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 15),
                Size = new Size(60, 22),
                ForeColor = Color.FromArgb(56, 142, 60)
            });

            txtSearch = new TextBox
            {
                Location = new Point(75, 12),
                Size = new Size(250, 28),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => LoadProducts();

            var btnToggle = new Button
            {
                Text = "🔄 Toggle Active",
                Location = new Point(345, 10),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += ToggleActive;

            toolbar.Controls.AddRange(new Control[]
                { txtSearch, btnToggle });

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
                Color.FromArgb(56, 142, 60);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(232, 245, 233);

            Controls.Add(dgv);
            Controls.Add(toolbar);
            Controls.Add(header);

            LoadProducts();
        }

        private void LoadProducts()
        {
            string sql =
                @"SELECT p.ProductID, p.Name, c.CategoryName AS Category,
                    v.StoreName AS Vendor, p.Price, p.Stock,
                    CASE p.IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status
                  FROM Products p
                  JOIN Categories c ON c.CategoryID = p.CategoryID
                  JOIN Vendors v    ON v.VendorID   = p.VendorID
                  WHERE p.Name LIKE @s
                  ORDER BY p.ProductID DESC";
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql,
                new[] { new SqlParameter("@s",
                    "%" + txtSearch.Text.Trim() + "%") });
        }

        private void ToggleActive(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            int pid = Convert.ToInt32(
                dgv.SelectedRows[0].Cells["ProductID"].Value);
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Products SET IsActive=CASE IsActive WHEN 1 THEN 0 ELSE 1 END WHERE ProductID=@id",
                new[] { new SqlParameter("@id", pid) });
            LoadProducts();
        }
    }
}