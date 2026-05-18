using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class VendorOrdersForm : Form
    {
        private DataGridView dgvOrders, dgvItems;
        private ComboBox cmbStatus;

        public VendorOrdersForm()
        {
            Text = "My Orders";
            Size = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(245, 124, 0)
            };
            header.Controls.Add(new Label
            {
                Text = "🛒  My Orders",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(400, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(255, 243, 224)
            };
            toolbar.Controls.Add(new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 15),
                Size = new Size(55, 22)
            });
            cmbStatus = new ComboBox
            {
                Location = new Point(70, 12),
                Size = new Size(140, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[]
                { "Pending","Processing","Shipped","Delivered","Cancelled" });
            cmbStatus.SelectedIndex = 0;

            var btnSet = new Button
            {
                Text = "✔ Update",
                Location = new Point(220, 10),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(56, 142, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSet.FlatAppearance.BorderSize = 0;
            btnSet.Click += UpdateStatus;
            toolbar.Controls.AddRange(new Control[] { cmbStatus, btnSet });

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };

            dgvOrders = DGV(Color.FromArgb(245, 124, 0),
                Color.FromArgb(255, 248, 225));
            dgvOrders.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    int oid = Convert.ToInt32(
                        dgvOrders.Rows[e.RowIndex].Cells["OrderID"].Value);
                    LoadItems(oid);
                }
            };

            dgvItems = DGV(Color.FromArgb(56, 142, 60),
                Color.FromArgb(232, 245, 233));

            split.Panel1.Controls.Add(dgvOrders);
            split.Panel2.Controls.Add(dgvItems);

            Controls.Add(split);
            Controls.Add(toolbar);
            Controls.Add(header);

            LoadOrders();
        }

        private void LoadOrders()
        {
            string sql =
                $@"SELECT DISTINCT o.OrderID, c.FullName AS Customer,
                     o.TotalAmount, o.Status,
                     CONVERT(VARCHAR,o.OrderDate,103) AS OrderDate
                   FROM Orders o
                   JOIN Customers c   ON c.CustomerID  = o.CustomerID
                   JOIN OrderItems oi ON oi.OrderID    = o.OrderID
                   JOIN Products p    ON p.ProductID   = oi.ProductID
                   WHERE p.VendorID = {Program.VendorID}
                   ORDER BY o.OrderDate DESC";
            dgvOrders.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void LoadItems(int oid)
        {
            string sql =
                $@"SELECT oi.OrderItemID, p.Name AS Product,
                     oi.Quantity, oi.Price,
                     (oi.Quantity * oi.Price) AS SubTotal
                   FROM OrderItems oi
                   JOIN Products p ON p.ProductID = oi.ProductID
                   WHERE oi.OrderID = {oid}
                     AND p.VendorID = {Program.VendorID}";
            dgvItems.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void UpdateStatus(object s, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0) return;
            int oid = Convert.ToInt32(
                dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Orders SET Status=@s WHERE OrderID=@id",
                new[]
                {
                    new SqlParameter("@s",  cmbStatus.SelectedItem.ToString()),
                    new SqlParameter("@id", oid)
                });
            LoadOrders();
        }

        private DataGridView DGV(Color hc, Color alt)
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
            d.AlternatingRowsDefaultCellStyle.BackColor = alt;
            return d;
        }
    }
}