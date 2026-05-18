using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class AdminDashboard : Form
    {
        private Panel panelSidebar, panelContent, panelHeader;
        private Label lblClock;
        private Timer clock;

        public AdminDashboard()
        {
            BuildUI();
            ShowHome();
            StartClock();
        }

        private void BuildUI()
        {
            Text = "ShopHub - Admin Dashboard";
            Size = new Size(1280, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 250);

            // HEADER
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(26, 35, 126)
            };

            panelHeader.Controls.Add(new Label
            {
                Text = "🛒  ShopHub  -  Admin Panel",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(400, 34)
            });

            lblClock = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(197, 202, 233),
                Size = new Size(320, 30),
                Location = new Point(820, 15),
                TextAlign = ContentAlignment.MiddleRight
            };

            panelHeader.Controls.Add(new Label
            {
                Text = "👤 " + Program.LoggedInUser + " (" + Program.UserRole + ")",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightYellow,
                Location = new Point(600, 18),
                Size = new Size(300, 22)
            });

            var btnLogout = MkBtn("🚪 Logout", 1155, 11, 95, 36,
                Color.FromArgb(211, 47, 47));
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Close();
            };

            panelHeader.Controls.Add(lblClock);
            panelHeader.Controls.Add(btnLogout);

            // SIDEBAR
            panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.FromArgb(21, 27, 84)
            };

            panelSidebar.Controls.Add(new Label
            {
                Text = "NAVIGATION",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 160),
                Location = new Point(15, 15),
                Size = new Size(200, 22)
            });

            string[] menus =
            {
                "🏠  Home / Dashboard",
                "👥  Manage Users",
                "🏪  Manage Vendors",
                "📦  All Products",
                "🛒  All Orders",
                "🏷️  Categories",
                "⭐  Reviews",
                "📊  Reports"
            };

            for (int i = 0; i < menus.Length; i++)
            {
                int idx = i;
                var btn = new Button
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
                btn.FlatAppearance.BorderSize = 0;
                btn.MouseEnter += (s, e) =>
                    btn.BackColor = Color.FromArgb(40, 50, 130);
                btn.MouseLeave += (s, e) =>
                    btn.BackColor = Color.Transparent;
                btn.Click += (s, e) => HandleMenu(idx);
                panelSidebar.Controls.Add(btn);
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
                case 1: new ManageUsersForm().ShowDialog(); break;
                case 2: new ManageVendorsForm().ShowDialog(); break;
                case 3: new ManageProductsForm().ShowDialog(); break;
                case 4: new ManageOrdersForm().ShowDialog(); break;
                case 5: new ManageCategoriesForm().ShowDialog(); break;
                case 6: new ManageReviewsForm().ShowDialog(); break;
                case 7: ShowReports(); break;
            }
        }

        private void ShowHome()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "📊  Dashboard Overview",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Location = new Point(20, 18),
                Size = new Size(500, 42)
            });

            string[] sqls =
            {
                "SELECT COUNT(*) FROM Users",
                "SELECT COUNT(*) FROM Vendors WHERE IsApproved=1",
                "SELECT COUNT(*) FROM Products WHERE IsActive=1",
                "SELECT COUNT(*) FROM Orders",
                "SELECT ISNULL(SUM(TotalAmount),0) FROM Orders WHERE Status='Delivered'",
                "SELECT COUNT(*) FROM Orders WHERE Status='Pending'"
            };
            string[] titles =
            {
                "Total Users","Active Vendors","Live Products",
                "Total Orders","Revenue (BDT)","Pending Orders"
            };
            string[] icons =
                { "👥","🏪","📦","🛒","💰","⏳" };
            Color[] colors =
            {
                Color.FromArgb(26,35,126),  Color.FromArgb(0,150,136),
                Color.FromArgb(56,142,60),  Color.FromArgb(245,124,0),
                Color.FromArgb(194,24,91),  Color.FromArgb(21,101,192)
            };

            for (int i = 0; i < sqls.Length; i++)
            {
                object res = DatabaseHelper.ExecuteScalar(sqls[i]);
                string val = (i == 4)
                    ? "BDT" + Convert.ToDecimal(res ?? 0).ToString("N0")
                    : (res ?? 0).ToString();

                int col = i % 3;
                int row = i / 3;
                var card = StatCard(titles[i], val, icons[i], colors[i]);
                card.Location = new Point(20 + col * 310, 72 + row * 155);
                panelContent.Controls.Add(card);
            }

            panelContent.Controls.Add(new Label
            {
                Text = "🛒  Recent Orders",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Location = new Point(20, 400),
                Size = new Size(300, 35)
            });

            string sql =
                @"SELECT TOP 10
                    o.OrderID,
                    c.FullName  AS Customer,
                    o.TotalAmount,
                    o.Status,
                    CONVERT(VARCHAR,o.OrderDate,103) AS OrderDate
                  FROM Orders o
                  JOIN Customers c ON c.CustomerID = o.CustomerID
                  ORDER BY o.OrderDate DESC";

            var dgv = MakeDGV(new Point(20, 440), new Size(940, 220),
                Color.FromArgb(26, 35, 126));
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
            panelContent.Controls.Add(dgv);
        }

        private void ShowReports()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "📊  Sales Reports",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Location = new Point(20, 18),
                Size = new Size(500, 42)
            });

            panelContent.Controls.Add(new Label
            {
                Text = "💰  Revenue by Vendor",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Location = new Point(20, 70),
                Size = new Size(350, 30)
            });

            string sql1 =
                @"SELECT v.StoreName AS Vendor,
                    COUNT(DISTINCT o.OrderID)             AS Orders,
                    ISNULL(SUM(oi.Price * oi.Quantity),0) AS Revenue
                  FROM Vendors v
                  LEFT JOIN Products p  ON p.VendorID  = v.VendorID
                  LEFT JOIN OrderItems oi ON oi.ProductID = p.ProductID
                  LEFT JOIN Orders o    ON o.OrderID   = oi.OrderID
                  GROUP BY v.StoreName";

            var dgv1 = MakeDGV(new Point(20, 108), new Size(940, 220),
                Color.FromArgb(26, 35, 126));
            dgv1.DataSource = DatabaseHelper.ExecuteQuery(sql1);
            panelContent.Controls.Add(dgv1);

            panelContent.Controls.Add(new Label
            {
                Text = "🏆  Top Selling Products",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                Location = new Point(20, 350),
                Size = new Size(350, 30)
            });

            string sql2 =
                @"SELECT TOP 10
                    p.Name         AS Product,
                    v.StoreName    AS Vendor,
                    SUM(oi.Quantity)            AS UnitsSold,
                    SUM(oi.Price * oi.Quantity) AS Revenue
                  FROM OrderItems oi
                  JOIN Products p ON p.ProductID = oi.ProductID
                  JOIN Vendors  v ON v.VendorID  = p.VendorID
                  GROUP BY p.Name, v.StoreName
                  ORDER BY Revenue DESC";

            var dgv2 = MakeDGV(new Point(20, 388), new Size(940, 220),
                Color.FromArgb(26, 35, 126));
            dgv2.DataSource = DatabaseHelper.ExecuteQuery(sql2);
            panelContent.Controls.Add(dgv2);
        }

        private Panel StatCard(string title, string value,
            string icon, Color color)
        {
            var p = new Panel { Size = new Size(290, 130), BackColor = Color.White };
            var bar = new Panel { Size = new Size(290, 7), Location = new Point(0, 0), BackColor = color };
            var lI = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 26),
                Location = new Point(10, 14),
                Size = new Size(55, 55),
                ForeColor = color
            };
            var lV = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(75, 18),
                Size = new Size(205, 45),
                ForeColor = Color.FromArgb(30, 40, 60)
            };
            var lT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 95),
                Size = new Size(270, 25),
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
                Color.FromArgb(240, 242, 255);
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
                lblClock.Text = DateTime.Now.ToString("ddd MMM dd yyyy | hh:mm:ss tt");
            clock.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clock?.Stop();
            base.OnFormClosed(e);
        }
    }
}