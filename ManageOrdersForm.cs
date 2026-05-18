using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageOrdersForm : Form
    {
        private DataGridView dgvOrders, dgvItems;
        private ComboBox cmbStatus;

        public ManageOrdersForm()
        {
            Text = "All Orders";
            Size = new Size(1100, 700);
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
                Text = "🛒  All Orders",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(350, 32)
            });

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(255, 243, 224)
            };

            toolbar.Controls.Add(new Label
            {
                Text = "Set Status:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 15),
                Size = new Size(80, 22)
            });

            cmbStatus = new ComboBox
            {
                Location = new Point(95, 12),
                Size = new Size(150, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[]
                { "Pending","Processing","Shipped","Delivered","Cancelled" });
            cmbStatus.SelectedIndex = 0;

            var btnSet = new Button
            {
                Text = "✔ Apply",
                Location = new Point(255, 10),
                Size = new Size(90, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(56, 142, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSet.FlatAppearance.BorderSize = 0;
            btnSet.Click += UpdateStatus;

            var btnRef = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(360, 10),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRef.FlatAppearance.BorderSize = 0;
            btnRef.Click += (s, e) => LoadOrders();

            toolbar.Controls.AddRange(new Control[]
                { cmbStatus, btnSet, btnRef });

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320
            };

            dgvOrders = MakeDGV(Color.FromArgb(245, 124, 0),
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

            dgvItems = MakeDGV(Color.FromArgb(56, 142, 60),
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
                @"SELECT o.OrderID, c.FullName AS Customer,
                    o.TotalAmount, o.Status, o.PaymentMethod,
                    CONVERT(VARCHAR,o.OrderDate,103) AS OrderDate,
                    o.ShippingAddress
                  FROM Orders o
                  JOIN Customers c ON c.CustomerID = o.CustomerID
                  ORDER BY o.OrderDate DESC";
            dgvOrders.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void LoadItems(int oid)
        {
            string sql =
                @"SELECT oi.OrderItemID, p.Name AS Product,
                    v.StoreName AS Vendor,
                    oi.Quantity, oi.Price,
                    (oi.Quantity * oi.Price) AS SubTotal
                  FROM OrderItems oi
                  JOIN Products p ON p.ProductID = oi.ProductID
                  JOIN Vendors  v ON v.VendorID  = p.VendorID
                  WHERE oi.OrderID = @oid";
            dgvItems.DataSource = DatabaseHelper.ExecuteQuery(sql,
                new[] { new SqlParameter("@oid", oid) });
        }

        private void UpdateStatus(object sender, EventArgs e)
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

        private DataGridView MakeDGV(Color hc, Color alt)
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