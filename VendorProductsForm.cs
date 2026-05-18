using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class VendorProductsForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName, txtDesc, txtPrice, txtStock;
        private ComboBox cmbCat;
        private Button btnSave, btnUpdate, btnDelete, btnClear;
        private Label lblID;
        private int selID = 0;
        private TabControl tabs;

        public VendorProductsForm()
        {
            Text = "My Products";
            Size = new Size(1050, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // TAB 1 - LIST
            var t1 = new TabPage("📦  My Products") { BackColor = Color.White };
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(0, 150, 136)
            };
            header.Controls.Add(new Label
            {
                Text = "📦  My Products",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 13),
                Size = new Size(300, 28)
            });

            var btnRef = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(880, 11),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRef.FlatAppearance.BorderSize = 0;
            btnRef.Click += (s, e) => LoadProducts();
            header.Controls.Add(btnRef);

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
                Color.FromArgb(0, 150, 136);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(224, 242, 241);
            dgv.CellClick += RowClick;

            t1.Controls.Add(dgv);
            t1.Controls.Add(header);

            // TAB 2 - ADD/EDIT
            var t2 = new TabPage("➕  Add / Edit") { BackColor = Color.White };
            var fp = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            lblID = new Label
            {
                Text = "Product ID: (Auto)",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(20, 15),
                Size = new Size(250, 22)
            };

            var l1 = L("Product Name *", 20, 45); txtName = TB(20, 68, 450);
            var l2 = L("Category *", 490, 45);

            cmbCat = new ComboBox
            {
                Location = new Point(490, 68),
                Size = new Size(320, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadCategories();

            var l3 = L("Price (BDT) *", 20, 115); txtPrice = TB(20, 138, 200);
            var l4 = L("Stock *", 240, 115); txtStock = TB(240, 138, 200);
            var l5 = L("Description", 20, 185);
            txtDesc = new TextBox
            {
                Location = new Point(20, 208),
                Size = new Size(790, 90),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            btnSave = B("💾 Save", 20, 320, Color.FromArgb(56, 142, 60));
            btnUpdate = B("✏️ Update", 160, 320, Color.FromArgb(0, 150, 136));
            btnDelete = B("🗑️ Delete", 295, 320, Color.FromArgb(211, 47, 47));
            btnClear = B("🗒️ Clear", 430, 320, Color.Gray);

            btnSave.Click += Save;
            btnUpdate.Click += Update;
            btnDelete.Click += Delete;
            btnClear.Click += (s, e) => Clear();
            btnUpdate.Enabled = btnDelete.Enabled = false;

            fp.Controls.AddRange(new Control[]
            {
                lblID, l1, txtName, l2, cmbCat,
                l3, txtPrice, l4, txtStock, l5, txtDesc,
                btnSave, btnUpdate, btnDelete, btnClear
            });
            t2.Controls.Add(fp);

            tabs.TabPages.AddRange(new TabPage[] { t1, t2 });
            Controls.Add(tabs);

            LoadProducts();
        }

        private void LoadProducts()
        {
            string sql =
                $@"SELECT p.ProductID, p.Name,
                     c.CategoryName AS Category,
                     p.Price, p.Stock,
                     CASE p.IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                     p.Description
                   FROM Products p
                   JOIN Categories c ON c.CategoryID = p.CategoryID
                   WHERE p.VendorID = {Program.VendorID}
                   ORDER BY p.ProductID DESC";
            dgv.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void LoadCategories()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName");
            cmbCat.DataSource = dt;
            cmbCat.DisplayMember = "CategoryName";
            cmbCat.ValueMember = "CategoryID";
        }

        private void RowClick(object s, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            selID = Convert.ToInt32(row.Cells["ProductID"].Value);
            lblID.Text = "Product ID: " + selID;
            txtName.Text = row.Cells["Name"].Value?.ToString();
            txtPrice.Text = row.Cells["Price"].Value?.ToString();
            txtStock.Text = row.Cells["Stock"].Value?.ToString();
            txtDesc.Text = row.Cells["Description"].Value?.ToString();

            var catID = DatabaseHelper.ExecuteScalar(
                "SELECT CategoryID FROM Products WHERE ProductID=@id",
                new[] { new SqlParameter("@id", selID) });
            cmbCat.SelectedValue = catID;

            btnSave.Enabled = false;
            btnUpdate.Enabled = btnDelete.Enabled = true;
            tabs.SelectedIndex = 1;
        }

        private void Save(object s, EventArgs e)
        {
            if (!Validate2()) return;
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Products
                    (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
                  VALUES (@vid,@cid,@n,@d,@p,@st,1)",
                new[]
                {
                    new SqlParameter("@vid", Program.VendorID),
                    new SqlParameter("@cid", cmbCat.SelectedValue),
                    new SqlParameter("@n",   txtName.Text.Trim()),
                    new SqlParameter("@d",   txtDesc.Text.Trim()),
                    new SqlParameter("@p",   decimal.Parse(txtPrice.Text.Trim())),
                    new SqlParameter("@st",  int.Parse(txtStock.Text.Trim()))
                });
            MessageBox.Show("✅ Product saved!");
            Clear(); LoadProducts(); tabs.SelectedIndex = 0;
        }

        private void Update(object s, EventArgs e)
        {
            if (!Validate2()) return;
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Products
                  SET CategoryID=@cid, Name=@n, Description=@d,
                      Price=@p, Stock=@st
                  WHERE ProductID=@id",
                new[]
                {
                    new SqlParameter("@cid", cmbCat.SelectedValue),
                    new SqlParameter("@n",   txtName.Text.Trim()),
                    new SqlParameter("@d",   txtDesc.Text.Trim()),
                    new SqlParameter("@p",   decimal.Parse(txtPrice.Text.Trim())),
                    new SqlParameter("@st",  int.Parse(txtStock.Text.Trim())),
                    new SqlParameter("@id",  selID)
                });
            MessageBox.Show("✅ Product updated!");
            Clear(); LoadProducts();
        }

        private void Delete(object s, EventArgs e)
        {
            if (MessageBox.Show("Delete product?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM Products WHERE ProductID=@id",
                    new[] { new SqlParameter("@id", selID) });
                Clear(); LoadProducts(); tabs.SelectedIndex = 0;
            }
        }

        private bool Validate2()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Name, Price and Stock are required!",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out _) ||
                !int.TryParse(txtStock.Text, out _))
            {
                MessageBox.Show("Enter valid Price and Stock!",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void Clear()
        {
            selID = 0; lblID.Text = "Product ID: (Auto)";
            txtName.Clear(); txtDesc.Clear();
            txtPrice.Clear(); txtStock.Clear();
            if (cmbCat.Items.Count > 0) cmbCat.SelectedIndex = 0;
            btnSave.Enabled = true;
            btnUpdate.Enabled = btnDelete.Enabled = false;
        }

        private Label L(string t, int x, int y) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(x, y),
            Size = new Size(250, 22)
        };
        private TextBox TB(int x, int y, int w) => new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(w, 28),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle
        };
        private Button B(string t, int x, int y, Color c)
        {
            var b = new Button
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(120, 38),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = c,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}