using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class CustomerOrdersForm : Form
    {
        private DataGridView dgvOrders, dgvItems;

        public CustomerOrdersForm()
        {
            Text = "My Orders";
            Size = new Size(1050, 660);
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
                Text = "📋  My Orders",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(300, 32)
            });

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };

            dgvOrders = DGV(Color.FromArgb(194, 24, 91),
                Color.FromArgb(252, 228, 236));
            dgvOrders.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    int oid = Convert.ToInt32(
                        dgvOrders.Rows[e.RowIndex].Cells["OrderID"].Value);
                    string sql =
                        @"SELECT oi.OrderItemID,
                             p.Name       AS Product,
                             v.StoreName  AS Vendor,
                             oi.Quantity, oi.Price,
                             (oi.Quantity * oi.Price) AS SubTotal
                           FROM OrderItems oi
                           JOIN Products p ON p.ProductID = oi.ProductID
                           JOIN Vendors  v ON v.VendorID  = p.VendorID
                           WHERE oi.OrderID = @oid";
                    dgvItems.DataSource = DatabaseHelper.ExecuteQuery(sql,
                        new[] { new SqlParameter("@oid", oid) });
                }
            };

            dgvItems = DGV(Color.FromArgb(56, 142, 60),
                Color.FromArgb(232, 245, 233));

            split.Panel1.Controls.Add(dgvOrders);
            split.Panel2.Controls.Add(dgvItems);

            Controls.Add(split);
            Controls.Add(header);

            string sqlO =
                $@"SELECT OrderID, TotalAmount, Status,
                     PaymentMethod, ShippingAddress,
                     CONVERT(VARCHAR,OrderDate,103) AS OrderDate
                   FROM Orders
                   WHERE CustomerID = {Program.CustomerID}
                   ORDER BY OrderDate DESC";
            dgvOrders.DataSource = DatabaseHelper.ExecuteQuery(sqlO);
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