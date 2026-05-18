using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MultiVendorEcommerce
{
    public class WriteReviewForm : Form
    {
        private ComboBox cmbProduct;
        private TrackBar trkRating;
        private Label lblRating;
        private TextBox txtComment;

        public WriteReviewForm()
        {
            Text = "Write a Review";
            Size = new Size(560, 430);
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
                Text = "⭐  Write a Product Review",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 13),
                Size = new Size(400, 32)
            });

            var fp = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.White
            };

            fp.Controls.Add(L("Select Product", 25, 25));
            cmbProduct = new ComboBox
            {
                Location = new Point(25, 50),
                Size = new Size(490, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            string sql =
                $@"SELECT DISTINCT p.ProductID, p.Name
                   FROM OrderItems oi
                   JOIN Orders o   ON o.OrderID   = oi.OrderID
                   JOIN Products p ON p.ProductID = oi.ProductID
                   WHERE o.CustomerID = {Program.CustomerID}";
            var dt = DatabaseHelper.ExecuteQuery(sql);
            cmbProduct.DataSource = dt;
            cmbProduct.DisplayMember = "Name";
            cmbProduct.ValueMember = "ProductID";

            fp.Controls.Add(L("Rating (1-5 Stars)", 25, 100));
            trkRating = new TrackBar
            {
                Location = new Point(25, 125),
                Size = new Size(300, 45),
                Minimum = 1,
                Maximum = 5,
                Value = 5,
                TickFrequency = 1
            };
            lblRating = new Label
            {
                Text = "⭐⭐⭐⭐⭐ (5)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 124, 0),
                Location = new Point(340, 130),
                Size = new Size(175, 30)
            };
            trkRating.ValueChanged += (s, e) =>
                lblRating.Text =
                    new string('⭐', trkRating.Value) +
                    " (" + trkRating.Value + ")";

            fp.Controls.Add(L("Your Comment", 25, 180));
            txtComment = new TextBox
            {
                Location = new Point(25, 205),
                Size = new Size(490, 80),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            var btnSave = new Button
            {
                Text = "⭐ Submit Review",
                Location = new Point(25, 305),
                Size = new Size(180, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(194, 24, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save;

            fp.Controls.AddRange(new Control[]
            {
                cmbProduct, trkRating, lblRating,
                txtComment, btnSave
            });

            Controls.Add(fp);
            Controls.Add(header);
        }

        private void Save(object s, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null) return;
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Reviews
                    (ProductID,CustomerID,Rating,Comment)
                  VALUES (@pid,@cid,@r,@c)",
                new[]
                {
                    new SqlParameter("@pid", cmbProduct.SelectedValue),
                    new SqlParameter("@cid", Program.CustomerID),
                    new SqlParameter("@r",   trkRating.Value),
                    new SqlParameter("@c",   txtComment.Text.Trim())
                });
            MessageBox.Show("✅ Review submitted! Thank you!",
                "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }

        private Label L(string t, int x, int y) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(x, y),
            Size = new Size(280, 22)
        };
    }
}