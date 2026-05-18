using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class VendorSettingsForm : Form
    {
        private TextBox txtStore, txtDesc, txtPhone, txtAddr;

        public VendorSettingsForm()
        {
            Text = "Store Settings";
            Size = new Size(600, 460);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(0, 150, 136)
            };
            header.Controls.Add(new Label
            {
                Text = "🏪  Store Settings",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(300, 32)
            });

            var fp = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.White
            };

            var l1 = L("Store Name", 25, 30); txtStore = TB(25, 55, 520);
            var l2 = L("Phone", 25, 100); txtPhone = TB(25, 125, 520);
            var l3 = L("Address", 25, 170); txtAddr = TB(25, 195, 520);
            var l4 = L("Description", 25, 240);
            txtDesc = new TextBox
            {
                Location = new Point(25, 265),
                Size = new Size(520, 80),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            var btnSave = new Button
            {
                Text = "💾 Save Settings",
                Location = new Point(25, 365),
                Size = new Size(180, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save;

            fp.Controls.AddRange(new Control[]
            {
                l1, txtStore, l2, txtPhone,
                l3, txtAddr,  l4, txtDesc, btnSave
            });

            Controls.Add(fp);
            Controls.Add(header);

            LoadData();
        }

        private void LoadData()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Vendors WHERE VendorID=@id",
                new[] { new SqlParameter("@id", Program.VendorID) });
            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                txtStore.Text = r["StoreName"]?.ToString();
                txtPhone.Text = r["Phone"]?.ToString();
                txtAddr.Text = r["Address"]?.ToString();
                txtDesc.Text = r["Description"]?.ToString();
            }
        }

        private void Save(object s, EventArgs e)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Vendors
                  SET StoreName=@sn, Phone=@ph, Address=@addr, Description=@d
                  WHERE VendorID=@id",
                new[]
                {
                    new SqlParameter("@sn",   txtStore.Text.Trim()),
                    new SqlParameter("@ph",   txtPhone.Text.Trim()),
                    new SqlParameter("@addr", txtAddr.Text.Trim()),
                    new SqlParameter("@d",    txtDesc.Text.Trim()),
                    new SqlParameter("@id",   Program.VendorID)
                });
            MessageBox.Show("✅ Store settings saved!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}