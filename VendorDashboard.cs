using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class VendorDashboard : Form
    {
        private Panel panelSidebar, panelContent, panelHeader;
        private Label lblClock;
        private Timer clock;

        public VendorDashboard()
        {
            BuildUI();
            ShowHome();
            StartClock();
        }

        private void BuildUI()
        {
            Text = "ShopHub - Vendor Dashboard";
            Size = new Size(1280, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 250);

            // HEADER
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(0, 150, 136)
            };
            panelHeader.Controls.Add(new Label
            {
                Text = "🏪  ShopHub  -  Vendor Panel",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(400, 34)
            });
            lblClock = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(178, 223, 219),
                Size = new Size(320, 30),
                Location = new Point(820, 15),
                TextAlign = ContentAlignment.MiddleRight
            };
            panelHeader.Controls.Add(new Label
            {
                Text = "👤 " + Program.LoggedInUser,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightYellow,
                Location = new Point(600, 18),
                Size = new Size(200, 22)
            });
            var btnOut = MkBtn("🚪 Logout", 1155, 11, 95, 36,
                Color.FromArgb(211, 47, 47));
            btnOut.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Close();
            };
            panelHeader.Controls.Add(lblClock);
            panelHeader.Controls.Add(btnOut);

            // SIDEBAR
            panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.FromArgb(0, 77, 64)
            };
            panelSidebar.Controls.Add(new Label
            {
                Text = "VENDOR MENU",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 180, 160),
                Location = new Point(15, 15),
                Size = new Size(200, 22)
            });

            string[] menus =
            {
                "🏠  Dashboard",
                "📦  My Products",
                "🛒  My Orders",
                "📊  Sales Report",
                "🏪  Store Settings"
            };

            for (int i = 0; i < menus.Length; i++)
            {
                int idx = i;
                var b = new Button
                {
                    Text = menus[i],
                    Size = new Size(230, 50),
                    Location = new Point(0, 45 + i * 52),
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(18, 0, 0, 0),
                    Cursor = Cursors.Hand
                };
                b.FlatAppearance.BorderSize = 0;
                b.MouseEnter += (s, e) =>
                    b.BackColor = Color.FromArgb(0, 121, 107);
                b.MouseLeave += (s, e) =>
                    b.BackColor = Color.Transparent;
                b.Click += (s, e) => HandleMenu(idx);
                panelSidebar.Controls.Add(b);
            }

            // CONTENT
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 246, 250),
                AutoScroll = true
            };

            Controls.Add(panelContent);
            Controls.Add(panelSidebar);
            Controls.Add(panelHeader);
        }

        private void HandleMenu(int idx)
        {
            switch (idx)
            {
                case 0: ShowHome(); break;
                case 1: new VendorProductsForm().ShowDialog(); break;
                case 2: new VendorOrdersForm().ShowDialog(); break;
                case 3: ShowSalesReport(); break;
                case 4: new VendorSettingsForm().ShowDialog(); break;
            }
        }

        private void ShowHome()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "📊  Vendor Dashboard",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 77, 64),
                Location = new Point(20, 18),
                Size = new Size(500, 42)
            });

            string[] sqls =
            {
                $"SELECT COUNT(*) FROM Products WHERE VendorID={Program.VendorID}",
                $@"SELECT COUNT(DISTINCT o.OrderID) FROM Orders o
                   JOIN OrderItems oi ON oi.OrderID=o.OrderID
                   JOIN Products p ON p.ProductID=oi.ProductID
                   WHERE p.VendorID={Program.VendorID}",
                $@"SELECT ISNULL(SUM(oi.Price*oi.Quantity),0)
                   FROM OrderItems oi
                   JOIN Products p ON p.ProductID=oi.ProductID
                   JOIN Orders o ON o.OrderID=oi.OrderID
                   WHERE p.VendorID={Program.VendorID} AND o.Status='Delivered'",
                $@"SELECT COUNT(*) FROM Reviews r
                   JOIN Products p ON p.ProductID=r.ProductID
                   WHERE p.VendorID={Program.VendorID}"
            };
            string[] titles =
                { "My Products","Total Orders","Revenue (BDT)","Reviews" };
            string[] icons =
                { "📦","🛒","💰","⭐" };
            Color[] colors =
            {
                Color.FromArgb(0,150,136), Color.FromArgb(245,124,0),
                Color.FromArgb(56,142,60), Color.FromArgb(194,24,91)
            };

            for (int i = 0; i < sqls.Length; i++)
            {
                object res = DatabaseHelper.ExecuteScalar(sqls[i]);
                string val = (i == 2)
                    ? "BDT" + Convert.ToDecimal(res ?? 0).ToString("N0")
                    : (res ?? 0).ToString();
                var card = StatCard(titles[i], val, icons[i], colors[i]);
                card.Location = new Point(20 + i * 240, 72);
                panelContent.Controls.Add(card);
            }

            panelContent.Controls.Add(new Label
            {
                Text = "🛒  Recent Orders",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 77, 64),
                Location = new Point(20, 240),
                Size = new Size(300, 35)
            });

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

            var dgv = MakeDGV(new Point(20, 280), new Size(940, 220),
                Color.FromArgb(0, 150, 136));
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
            panelContent.Controls.Add(dgv);
        }

        private void ShowSalesReport()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "📊  Sales Report",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 77, 64),
                Location = new Point(20, 18),
                Size = new Size(400, 42)
            });

            panelContent.Controls.Add(new Label
            {
                Text = "🏆  Top Products by Revenue",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 77, 64),
                Location = new Point(20, 68),
                Size = new Size(400, 30)
            });

            string sql =
                $@"SELECT p.Name AS Product,
                     SUM(oi.Quantity)            AS UnitsSold,
                     SUM(oi.Price * oi.Quantity) AS Revenue
                   FROM OrderItems oi
                   JOIN Products p ON p.ProductID = oi.ProductID
                   WHERE p.VendorID = {Program.VendorID}
                   GROUP BY p.Name
                   ORDER BY Revenue DESC";

            var dgv = MakeDGV(new Point(20, 105), new Size(940, 300),
                Color.FromArgb(0, 150, 136));
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
            panelContent.Controls.Add(dgv);
        }

        private Panel StatCard(string title, string value,
            string icon, Color color)
        {
            var p = new Panel { Size = new Size(220, 130), BackColor = Color.White };
            var bar = new Panel { Size = new Size(220, 7), Location = new Point(0, 0), BackColor = color };
            var lI = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                Location = new Point(8, 14),
                Size = new Size(52, 50),
                ForeColor = color
            };
            var lV = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(65, 18),
                Size = new Size(150, 42),
                ForeColor = Color.FromArgb(30, 40, 60)
            };
            var lT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                Location = new Point(8, 95),
                Size = new Size(204, 25),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            p.Controls.AddRange(new Control[] { bar, lI, lV, lT });
            return p;
        }

        private DataGridView MakeDGV(Point loc, Size sz, Color hc)
        {
            var d = new DataGridView
            {
                Location = loc,
                Size = sz,
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

        private Button MkBtn(string t, int x, int y,
            int w, int h, Color bg)
        {
            var b = new Button
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void StartClock()
        {
            clock = new Timer { Interval = 1000 };
            clock.Tick += (s, e) =>
                lblClock.Text =
                    DateTime.Now.ToString("ddd MMM dd yyyy | hh:mm:ss tt");
            clock.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clock?.Stop();
            base.OnFormClosed(e);
        }
    }
}