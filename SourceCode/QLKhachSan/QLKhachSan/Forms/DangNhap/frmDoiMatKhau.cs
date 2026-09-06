using System;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;

namespace QLKhachSan.Forms.DangNhap
{
    /// <summary>Đổi mật khẩu cho tài khoản đang đăng nhập.</summary>
    public class frmDoiMatKhau : Form
    {
        private TextBox txtMatKhauCu, txtMatKhauMoi, txtMatKhauXacNhan;
        private Button btnLuu, btnHuy;
        private ErrorProvider errorProvider;

        public frmDoiMatKhau()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Đổi mật khẩu";
            Width = 380;
            Height = 260;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            errorProvider = new ErrorProvider();

            var pnl = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Height = 120, ColumnCount = 2, RowCount = 3,
                Padding = new Padding(20, 10, 20, 0)
            };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34f));

            txtMatKhauCu = new TextBox { Dock = DockStyle.Fill, PasswordChar = '●' };
            txtMatKhauMoi = new TextBox { Dock = DockStyle.Fill, PasswordChar = '●' };
            txtMatKhauXacNhan = new TextBox { Dock = DockStyle.Fill, PasswordChar = '●' };
            txtMatKhauCu.KeyDown += KeyboardHelper.EnterAsTab;
            txtMatKhauMoi.KeyDown += KeyboardHelper.EnterAsTab;
            txtMatKhauXacNhan.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.Handled = true; e.SuppressKeyPress = true; BtnLuu_Click(s, e); }
            };

            pnl.Controls.Add(new Label { Text = "Mật khẩu hiện tại:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnl.Controls.Add(txtMatKhauCu, 1, 0);
            pnl.Controls.Add(new Label { Text = "Mật khẩu mới:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnl.Controls.Add(txtMatKhauMoi, 1, 1);
            pnl.Controls.Add(new Label { Text = "Xác nhận mật khẩu:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            pnl.Controls.Add(txtMatKhauXacNhan, 1, 2);

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, Height = 45, FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(20, 5, 20, 5)
            };
            btnHuy = new Button { Text = "Hủy (Esc)", Width = 100 };
            btnLuu = new Button { Text = "Lưu (Enter)", Width = 120 };
            btnLuu.Click += BtnLuu_Click;
            btnHuy.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            pnlButtons.Controls.Add(btnHuy);
            pnlButtons.Controls.Add(btnLuu);

            Controls.Add(pnlButtons);
            Controls.Add(pnl);

            AcceptButton = btnLuu;
            CancelButton = btnHuy;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            bool ok = Validator.CheckRequired(txtMatKhauCu, errorProvider, "Nhập mật khẩu hiện tại");
            ok &= Validator.CheckRequired(txtMatKhauMoi, errorProvider, "Nhập mật khẩu mới");
            ok &= Validator.CheckCondition(txtMatKhauMoi, errorProvider, txtMatKhauMoi.Text.Length >= 4,
                "Mật khẩu mới phải từ 4 ký tự trở lên");
            ok &= Validator.CheckCondition(txtMatKhauXacNhan, errorProvider,
                txtMatKhauXacNhan.Text == txtMatKhauMoi.Text, "Xác nhận mật khẩu không khớp");
            if (!ok) return;

            try
            {
                TaiKhoanDAL.DoiMatKhau(CurrentUser.MaTK, txtMatKhauCu.Text, txtMatKhauMoi.Text);
                MessageBox.Show("Đổi mật khẩu thành công. Lần đăng nhập sau dùng mật khẩu mới.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
