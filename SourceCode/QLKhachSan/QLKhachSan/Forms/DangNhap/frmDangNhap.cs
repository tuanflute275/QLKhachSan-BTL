using System;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;

namespace QLKhachSan.Forms.DangNhap
{
    /// <summary>Đăng nhập bằng tài khoản nhân viên (FR12 - tùy chọn). Chặn ứng dụng cho tới khi
    /// đăng nhập thành công hoặc người dùng bấm Thoát.</summary>
    public class frmDangNhap : Form
    {
        private TextBox txtTenDangNhap, txtMatKhau;
        private Button btnDangNhap, btnThoat;
        private Label lblLoi;

        public frmDangNhap()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Đăng nhập - QLKhachSan";
            Width = 400;
            Height = 280;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            var lblTieuDe = new Label
            {
                Text = "PHẦN MỀM QUẢN LÝ THUÊ PHÒNG KHÁCH SẠN",
                Dock = DockStyle.Top, Height = 50, TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
            };

            var pnl = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Height = 80, ColumnCount = 2, RowCount = 2,
                Padding = new Padding(30, 5, 30, 5)
            };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            txtTenDangNhap = new TextBox { Dock = DockStyle.Fill };
            txtMatKhau = new TextBox { Dock = DockStyle.Fill, PasswordChar = '●' };
            txtTenDangNhap.KeyDown += KeyboardHelper.EnterAsTab;
            txtMatKhau.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.Handled = true; e.SuppressKeyPress = true; BtnDangNhap_Click(s, e); }
            };

            pnl.Controls.Add(new Label { Text = "Tên đăng nhập:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnl.Controls.Add(txtTenDangNhap, 1, 0);
            pnl.Controls.Add(new Label { Text = "Mật khẩu:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnl.Controls.Add(txtMatKhau, 1, 1);

            lblLoi = new Label
            {
                Dock = DockStyle.Top, Height = 30, ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, Height = 45, FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(30, 5, 30, 5)
            };
            btnThoat = new Button { Text = "Thoát", Width = 90 };
            btnDangNhap = new Button { Text = "Đăng nhập (Enter)", Width = 150 };
            btnDangNhap.Click += BtnDangNhap_Click;
            btnThoat.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            pnlButtons.Controls.Add(btnThoat);
            pnlButtons.Controls.Add(btnDangNhap);

            var lblHint = new Label
            {
                Text = "Demo: admin / annv / binhlt - mật khẩu: 123456",
                Dock = DockStyle.Bottom, Height = 24, ForeColor = System.Drawing.Color.Gray,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Controls.Add(pnlButtons);
            Controls.Add(lblLoi);
            Controls.Add(pnl);
            Controls.Add(lblTieuDe);
            Controls.Add(lblHint);

            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                lblLoi.Text = "Nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                var row = TaiKhoanDAL.DangNhap(tenDangNhap, matKhau);
                if (row == null)
                {
                    lblLoi.Text = "Sai tên đăng nhập hoặc mật khẩu.";
                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                    return;
                }

                CurrentUser.DangNhap(
                    Convert.ToInt32(row["MaTK"]), row["TenDangNhap"].ToString(),
                    Convert.ToInt32(row["MaNV"]), row["HoTen"].ToString(), row["Quyen"].ToString());

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblLoi.Text = "Lỗi kết nối CSDL: " + ex.Message;
            }
        }
    }
}
