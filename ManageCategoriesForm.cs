using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class ManageCategoriesForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName, txtDesc;
        private Button btnSave, btnUpdate, btnDelete, btnClear;
        private int selID = 0;
        private TabControl tabs;

        public ManageCategoriesForm()
        {
            Text = "Manage Categories";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // TAB 1
            var t1 = new TabPage("🏷️  List") { BackColor = Color.White };
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
                Color.FromArgb(123, 31, 162);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(243, 229, 245);
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var row = dgv.Rows[e.RowIndex];
                selID = Convert.ToInt32(row.Cells["CategoryID"].Value);
                txtName.Text = row.Cells["CategoryName"].Value?.ToString();
                txtDesc.Text = row.Cells["Description"].Value?.ToString();
                btnSave.Enabled = false;
                btnUpdate.Enabled = btnDelete.Enabled = true;
                tabs.SelectedIndex = 1;
            };
            t1.Controls.Add(dgv);

            // TAB 2
            var t2 = new TabPage("➕  Add / Edit") { BackColor = Color.White };
            var fp = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.White
            };

            var l1 = L("Category Name *", 25, 30);
            txtName = TB(25, 55, 400);
            var l2 = L("Description", 25, 100);
            txtDesc = new TextBox
            {
                Location = new Point(25, 125),
                Size = new Size(400, 80),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            btnSave = B("💾 Save", 25, 230, Color.FromArgb(56, 142, 60));
            btnUpdate = B("✏️ Update", 160, 230, Color.FromArgb(26, 35, 126));
            btnDelete = B("🗑️ Delete", 295, 230, Color.FromArgb(211, 47, 47));
            btnClear = B("🗒️ Clear", 430, 230, Color.Gray);

            btnSave.Click += Save;
            btnUpdate.Click += Update;
            btnDelete.Click += Delete;
            btnClear.Click += (s, e) => Clear();
            btnUpdate.Enabled = btnDelete.Enabled = false;

            fp.Controls.AddRange(new Control[]
            {
                l1, txtName, l2, txtDesc,
                btnSave, btnUpdate, btnDelete, btnClear
            });
            t2.Controls.Add(fp);

            tabs.TabPages.AddRange(new TabPage[] { t1, t2 });
            Controls.Add(tabs);

            LoadCats();
        }

        private void LoadCats()
        {
            dgv.DataSource = DatabaseHelper.ExecuteQuery(
                "SELECT CategoryID, CategoryName, Description FROM Categories ORDER BY CategoryName");
        }

        private void Save(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO Categories (CategoryName,Description) VALUES (@n,@d)",
                new[]
                {
                    new SqlParameter("@n", txtName.Text.Trim()),
                    new SqlParameter("@d", txtDesc.Text.Trim())
                });
            MessageBox.Show("✅ Category saved!");
            Clear(); LoadCats(); tabs.SelectedIndex = 0;
        }

        private void Update(object s, EventArgs e)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Categories SET CategoryName=@n,Description=@d WHERE CategoryID=@id",
                new[]
                {
                    new SqlParameter("@n",  txtName.Text.Trim()),
                    new SqlParameter("@d",  txtDesc.Text.Trim()),
                    new SqlParameter("@id", selID)
                });
            MessageBox.Show("✅ Category updated!");
            Clear(); LoadCats();
        }

        private void Delete(object s, EventArgs e)
        {
            if (MessageBox.Show("Delete?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM Categories WHERE CategoryID=@id",
                    new[] { new SqlParameter("@id", selID) });
                Clear(); LoadCats(); tabs.SelectedIndex = 0;
            }
        }

        private void Clear()
        {
            selID = 0; txtName.Clear(); txtDesc.Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = btnDelete.Enabled = false;
        }

        private Label L(string t, int x, int y)
        {
            return new Label
            {
                Text = t,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                Size = new Size(250, 22)
            };
        }
        private TextBox TB(int x, int y, int w)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 28),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
        }
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