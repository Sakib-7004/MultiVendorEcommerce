using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class CustomerDashboard : Form
    {
        private Panel panelSidebar, panelContent, panelHeader;
        private Label lblClock;
        private Timer clock;
        private DataTable cart;

        public CustomerDashboard()
        {
            InitCart();
            BuildUI();
            ShowShop();
            StartClock();
        }

        private void InitCart()
        {
            cart = new DataTable();
            cart.Columns.Add("ProductID", typeof(int));
            cart.Columns.Add("Name", typeof(string));
            cart.Columns.Add("Price", typeof(decimal));
            cart.Columns.Add("Quantity", typeof(int));
            cart.Columns.Add("SubTotal", typeof(decimal));
        }

        private void BuildUI()
        {
            Text = "ShopHub - Customer Store";
            Size = new Size(1280, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 250);

            // HEADER
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(194, 24, 91)
            };
            panelHeader.Controls.Add(new Label
            {
                Text = "🛒  ShopHub  -  Online Store",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(420, 34)
            });
            lblClock = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(248, 187, 208),
                Size = new Size(300, 30),
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
                Color.FromArgb(136, 14, 79));
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
                BackColor = Color.FromArgb(136, 14, 79)
            };
            panelSidebar.Controls.Add(new Label
            {
                Text = "CUSTOMER MENU",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 130, 170),
                Location = new Point(15, 15),
                Size = new Size(200, 22)
            });

            string[] menus =
            {
                "🏠  Shop / Browse",
                "🛒  My Cart",
                "📋  My Orders",
                "⭐  Write Review",
                "👤  My Profile"
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
                    b.BackColor = Color.FromArgb(173, 20, 87);
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
                case 0: ShowShop(); break;
                case 1: ShowCart(); break;
                case 2: new CustomerOrdersForm().ShowDialog(); break;
                case 3: new WriteReviewForm().ShowDialog(); break;
                case 4: new CustomerProfileForm().ShowDialog(); break;
            }
        }

        private void ShowShop()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "🏪  Browse Products",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(136, 14, 79),
                Location = new Point(20, 15),
                Size = new Size(400, 42)
            });

            var txtSrch = new TextBox
            {
                Location = new Point(20, 65),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var cmbCat = new ComboBox
            {
                Location = new Point(315, 65),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            var dtCat = DatabaseHelper.ExecuteQuery(
                "SELECT CategoryID,CategoryName FROM Categories ORDER BY CategoryName");
            var allRow = dtCat.NewRow();
            allRow["CategoryID"] = 0;
            allRow["CategoryName"] = "All Categories";
            dtCat.Rows.InsertAt(allRow, 0);
            cmbCat.DataSource = dtCat;
            cmbCat.DisplayMember = "CategoryName";
            cmbCat.ValueMember = "CategoryID";

            var btnSrch = new Button
            {
                Text = "🔍 Search",
                Location = new Point(508, 63),
                Size = new Size(110, 33),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(194, 24, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSrch.FlatAppearance.BorderSize = 0;

            var dgv = new DataGridView
            {
                Location = new Point(20, 108),
                Size = new Size(920, 400),
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

            Action loadProducts = () =>
            {
                string catFilter =
                    (cmbCat.SelectedValue != null &&
                     Convert.ToInt32(cmbCat.SelectedValue) != 0)
                    ? $" AND p.CategoryID={cmbCat.SelectedValue}"
                    : "";

                string sql =
                    $@"SELECT p.ProductID, p.Name,
                         c.CategoryName AS Category,
                         v.StoreName    AS Vendor,
                         p.Price, p.Stock,
                         ISNULL((SELECT AVG(CAST(Rating AS FLOAT))
                                 FROM Reviews WHERE ProductID=p.ProductID),0)
                           AS Rating
                       FROM Products p
                       JOIN Categories c ON c.CategoryID = p.CategoryID
                       JOIN Vendors v    ON v.VendorID   = p.VendorID
                       WHERE p.IsActive=1 AND p.Stock>0
                         AND p.Name LIKE @s {catFilter}
                       ORDER BY p.ProductID DESC";

                dgv.DataSource = DatabaseHelper.ExecuteQuery(sql,
                    new[] { new SqlParameter("@s",
                        "%" + txtSrch.Text.Trim() + "%") });
            };

            btnSrch.Click += (s, e) => loadProducts();
            txtSrch.TextChanged += (s, e) => loadProducts();
            cmbCat.SelectedIndexChanged += (s, e) => loadProducts();

            var btnAdd = new Button
            {
                Text = "🛒 Add to Cart",
                Location = new Point(20, 520),
                Size = new Size(160, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(194, 24, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) return;
                var row = dgv.SelectedRows[0];
                int pid = Convert.ToInt32(row.Cells["ProductID"].Value);
                string nm = row.Cells["Name"].Value?.ToString();
                decimal pr = Convert.ToDecimal(row.Cells["Price"].Value);

                foreach (DataRow cr in cart.Rows)
                {
                    if (Convert.ToInt32(cr["ProductID"]) == pid)
                    {
                        cr["Quantity"] = Convert.ToInt32(cr["Quantity"]) + 1;
                        cr["SubTotal"] = Convert.ToDecimal(cr["Price"])
                                       * Convert.ToInt32(cr["Quantity"]);
                        MessageBox.Show("✅ " + nm + " quantity updated!");
                        return;
                    }
                }
                cart.Rows.Add(pid, nm, pr, 1, pr);
                MessageBox.Show("✅ " + nm + " added to cart!");
            };

            panelContent.Controls.AddRange(new Control[]
                { txtSrch, cmbCat, btnSrch, dgv, btnAdd });

            loadProducts();
        }

        private void ShowCart()
        {
            panelContent.Controls.Clear();

            panelContent.Controls.Add(new Label
            {
                Text = "🛒  My Cart",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(136, 14, 79),
                Location = new Point(20, 15),
                Size = new Size(400, 42)
            });

            var dgv = new DataGridView
            {
                Location = new Point(20, 65),
                Size = new Size(920, 280),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
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
            dgv.DataSource = cart;

            decimal total = 0;
            foreach (DataRow r in cart.Rows)
                total += Convert.ToDecimal(r["SubTotal"]);

            panelContent.Controls.Add(new Label
            {
                Text = "Total: BDT" + total.ToString("N2"),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(136, 14, 79),
                Location = new Point(20, 360),
                Size = new Size(300, 35)
            });

            var cmbPay = new ComboBox
            {
                Location = new Point(20, 408),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPay.Items.AddRange(new object[]
                { "Cash on Delivery","Credit Card","Debit Card","UPI","Net Banking" });
            cmbPay.SelectedIndex = 0;

            var txtAddr = new TextBox
            {
                Location = new Point(240, 408),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnRem = MkBtn("🗑️ Remove", 20, 460, 120, 38,
                Color.FromArgb(211, 47, 47));
            var btnOrd = MkBtn("✔ Place Order", 160, 460, 150, 38,
                Color.FromArgb(56, 142, 60));
            var btnClr = MkBtn("🗒️ Clear Cart", 320, 460, 130, 38,
                Color.Gray);

            btnRem.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count > 0 &&
                    dgv.SelectedRows[0].Index < cart.Rows.Count)
                {
                    cart.Rows.RemoveAt(dgv.SelectedRows[0].Index);
                    ShowCart();
                }
            };
            btnOrd.Click += (s, e) =>
                PlaceOrder(cmbPay, txtAddr, total);
            btnClr.Click += (s, e) =>
            { cart.Clear(); ShowCart(); };

            panelContent.Controls.AddRange(new Control[]
            {
                dgv, cmbPay, txtAddr,
                btnRem, btnOrd, btnClr
            });
        }

        private void PlaceOrder(ComboBox cmbPay,
            TextBox txtAddr, decimal total)
        {
            if (cart.Rows.Count == 0)
            { MessageBox.Show("Cart is empty!"); return; }
            if (string.IsNullOrWhiteSpace(txtAddr.Text))
            { MessageBox.Show("Please enter shipping address!"); return; }

            string sqlO =
                @"INSERT INTO Orders
                    (CustomerID,TotalAmount,Status,PaymentMethod,ShippingAddress)
                  VALUES (@cid,@tot,'Pending',@pay,@addr);
                  SELECT SCOPE_IDENTITY();";

            object oid = DatabaseHelper.ExecuteScalar(sqlO, new[]
            {
                new SqlParameter("@cid",  Program.CustomerID),
                new SqlParameter("@tot",  total),
                new SqlParameter("@pay",  cmbPay.SelectedItem.ToString()),
                new SqlParameter("@addr", txtAddr.Text.Trim())
            });

            if (oid != null)
            {
                int orderID = Convert.ToInt32(oid);
                foreach (DataRow r in cart.Rows)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO OrderItems
                            (OrderID,ProductID,Quantity,Price)
                          VALUES (@oid,@pid,@qty,@pr)",
                        new[]
                        {
                            new SqlParameter("@oid", orderID),
                            new SqlParameter("@pid", r["ProductID"]),
                            new SqlParameter("@qty", r["Quantity"]),
                            new SqlParameter("@pr",  r["Price"])
                        });
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE Products SET Stock=Stock-@qty WHERE ProductID=@pid",
                        new[]
                        {
                            new SqlParameter("@qty", r["Quantity"]),
                            new SqlParameter("@pid", r["ProductID"])
                        });
                }
                cart.Clear();
                MessageBox.Show(
                    "✅ Order #" + orderID + " placed!\nThank you!",
                    "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ShowShop();
            }
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