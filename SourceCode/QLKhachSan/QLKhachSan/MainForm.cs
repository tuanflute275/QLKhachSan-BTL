using System;
using System.Linq;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.Forms.BaoCao;
using QLKhachSan.Forms.DangKy;
using QLKhachSan.Forms.DangNhap;
using QLKhachSan.Forms.DichVu;
using QLKhachSan.Forms.HoaDon;
using QLKhachSan.Forms.KhachHang;
using QLKhachSan.Forms.NhanVien;
using QLKhachSan.Forms.Phong;
using QLKhachSan.Forms.TraPhong;

namespace QLKhachSan
{
    /// <summary>
    /// Form chính (MDI Container): toàn bộ điều hướng chức năng qua MenuStrip/ToolStrip
    /// theo đúng yêu cầu đề bài (3.2 - giao tiếp qua hệ thống menu/toolbar).
    /// </summary>
    public partial class MainForm : Form
    {
        private MenuStrip menuStrip;
        private ToolStrip toolStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblUser;

        private ToolStripItem mnuNhanVienItem;
        private ToolStripItem mnuBaoCaoItem;
        private ToolStripButton btnBaoCaoItem;

        /// <summary>True nếu người dùng bấm "Đăng xuất" (Program.cs sẽ quay lại màn hình đăng nhập
        /// thay vì thoát hẳn ứng dụng khi form này đóng).</summary>
        public bool YeuCauDangXuat { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            ApDungPhanQuyen();
            KiemTraKetNoi(silent: true);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            IsMdiContainer = true;
            Text = "Phần mềm Quản lý thuê phòng khách sạn - QLKhachSan";
            WindowState = FormWindowState.Maximized;
            ClientSize = new System.Drawing.Size(1200, 700);

            BuildMenuStrip();
            BuildToolStrip();
            BuildStatusStrip();

            MainMenuStrip = menuStrip;
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private void BuildMenuStrip()
        {
            menuStrip = new MenuStrip { Dock = DockStyle.Top };

            var mnuDanhMuc = new ToolStripMenuItem("&Danh mục");
            mnuDanhMuc.DropDownItems.Add("Loại &phòng", null, (s, e) => MoFormDonLe<frmLoaiPhong>());
            mnuDanhMuc.DropDownItems.Add("&Phòng", null, (s, e) => MoFormDonLe<frmPhong>());
            mnuDanhMuc.DropDownItems.Add("&Dịch vụ", null, (s, e) => MoFormDonLe<frmDichVu>());
            mnuDanhMuc.DropDownItems.Add(new ToolStripSeparator());
            mnuDanhMuc.DropDownItems.Add("&Khách hàng", null, (s, e) => MoFormDonLe<frmKhachHang>());
            mnuNhanVienItem = mnuDanhMuc.DropDownItems.Add(
                "&Nhân viên", null, (s, e) => MoFormDonLe<frmNhanVien>());

            var mnuNghiepVu = new ToolStripMenuItem("&Nghiệp vụ");
            mnuNghiepVu.DropDownItems.Add("Đặt &phòng / Nhận phòng\tF6", null, (s, e) => MoFormDonLe<frmDangKy>())
                .Name = "mnuDangKy";
            mnuNghiepVu.DropDownItems.Add("&Sử dụng dịch vụ\tF7", null, (s, e) => MoFormDonLe<frmSuDungDichVu>())
                .Name = "mnuSuDungDichVu";
            mnuNghiepVu.DropDownItems.Add("&Trả phòng / Thanh toán\tF8", null, (s, e) => MoFormDonLe<frmTraPhong>())
                .Name = "mnuTraPhong";

            var mnuHoaDon = new ToolStripMenuItem("&Hóa đơn");
            mnuHoaDon.DropDownItems.Add("&Tra cứu / In lại hóa đơn", null, (s, e) => MoFormDonLe<frmTraCuuHoaDon>());

            var mnuBaoCao = new ToolStripMenuItem("&Báo cáo");
            mnuBaoCaoItem = mnuBaoCao.DropDownItems.Add(
                "Báo cáo &doanh thu", null, (s, e) => MoFormDonLe<frmBaoCaoDoanhThu>());

            var mnuHeThong = new ToolStripMenuItem("Hệ &thống");
            mnuHeThong.DropDownItems.Add("&Kiểm tra kết nối CSDL", null, (s, e) => KiemTraKetNoi(silent: false));
            mnuHeThong.DropDownItems.Add(new ToolStripSeparator());
            mnuHeThong.DropDownItems.Add("Đổi &mật khẩu", null, (s, e) => MoDoiMatKhau());
            mnuHeThong.DropDownItems.Add("Đăn&g xuất", null, (s, e) => DangXuat());
            mnuHeThong.DropDownItems.Add(new ToolStripSeparator());
            mnuHeThong.DropDownItems.Add("Th&oát\tAlt+F4", null, (s, e) => Close());

            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                mnuDanhMuc, mnuNghiepVu, mnuHoaDon, mnuBaoCao, mnuHeThong
            });

            // Phím tắt F6/F7/F8 hoạt động toàn cục trong MainForm (không cần focus vào menu)
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
        }

        private void BuildToolStrip()
        {
            toolStrip = new ToolStrip { Dock = DockStyle.Top };

            var btnKhachHang = new ToolStripButton("Khách hàng") { ToolTipText = "Danh mục khách hàng" };
            btnKhachHang.Click += (s, e) => MoFormDonLe<frmKhachHang>();

            var btnPhong = new ToolStripButton("Phòng") { ToolTipText = "Danh mục phòng" };
            btnPhong.Click += (s, e) => MoFormDonLe<frmPhong>();

            var btnDichVu = new ToolStripButton("Dịch vụ") { ToolTipText = "Danh mục dịch vụ" };
            btnDichVu.Click += (s, e) => MoFormDonLe<frmDichVu>();

            var btnDangKy = new ToolStripButton("Đặt phòng (F6)") { ToolTipText = "Đăng ký đặt phòng / Nhận phòng" };
            btnDangKy.Click += (s, e) => MoFormDonLe<frmDangKy>();

            var btnSuDungDichVu = new ToolStripButton("Dịch vụ đã dùng (F7)") { ToolTipText = "Ghi nhận dịch vụ theo ngày" };
            btnSuDungDichVu.Click += (s, e) => MoFormDonLe<frmSuDungDichVu>();

            var btnTraPhong = new ToolStripButton("Trả phòng (F8)") { ToolTipText = "Trả phòng, tính tiền, lập hóa đơn" };
            btnTraPhong.Click += (s, e) => MoFormDonLe<frmTraPhong>();

            var btnTraCuuHoaDon = new ToolStripButton("Tra cứu hóa đơn");
            btnTraCuuHoaDon.Click += (s, e) => MoFormDonLe<frmTraCuuHoaDon>();

            btnBaoCaoItem = new ToolStripButton("Báo cáo doanh thu");
            btnBaoCaoItem.Click += (s, e) => MoFormDonLe<frmBaoCaoDoanhThu>();

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                btnKhachHang, btnPhong, btnDichVu, new ToolStripSeparator(),
                btnDangKy, btnSuDungDichVu, btnTraPhong, new ToolStripSeparator(),
                btnTraCuuHoaDon, btnBaoCaoItem
            });
        }

        private void BuildStatusStrip()
        {
            statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            lblStatus = new ToolStripStatusLabel { Text = "Sẵn sàng", Spring = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            lblUser = new ToolStripStatusLabel { TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(lblUser);
        }

        /// <summary>Nhân viên (LeTan) không được vào quản lý Nhân viên / xem báo cáo doanh thu -
        /// 2 chức năng này chỉ dành cho Admin.</summary>
        private void ApDungPhanQuyen()
        {
            bool isAdmin = CurrentUser.IsAdmin;
            mnuNhanVienItem.Enabled = isAdmin;
            mnuBaoCaoItem.Enabled = isAdmin;
            btnBaoCaoItem.Enabled = isAdmin;

            lblUser.Text = $"Xin chào: {CurrentUser.HoTen} ({(isAdmin ? "Quản trị" : "Lễ tân")})";
        }

        private void MoDoiMatKhau()
        {
            using (var frm = new frmDoiMatKhau())
            {
                frm.ShowDialog(this);
            }
        }

        private void DangXuat()
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            YeuCauDangXuat = true;
            CurrentUser.DangXuat();
            Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6) MoFormDonLe<frmDangKy>();
            else if (e.KeyCode == Keys.F7) MoFormDonLe<frmSuDungDichVu>();
            else if (e.KeyCode == Keys.F8) MoFormDonLe<frmTraPhong>();
        }

        /// <summary>
        /// Mở 1 form quản lý dạng MDI child. Chỉ hiển thị DUY NHẤT 1 form tại 1 thời điểm
        /// (đóng hết các form đang mở khác trước khi mở form mới) để tránh chồng chéo cửa sổ
        /// khi người dùng bấm qua lại nhiều menu/toolbar.
        /// </summary>
        private void MoFormDonLe<T>() where T : Form, new()
        {
            var existed = MdiChildren.FirstOrDefault(f => f is T);

            foreach (var f in MdiChildren)
                if (f != existed) f.Close();

            if (existed != null)
            {
                existed.Activate();
                return;
            }

            var frm = new T { MdiParent = this, WindowState = FormWindowState.Maximized };
            frm.Show();
        }

        private void KiemTraKetNoi(bool silent)
        {
            try
            {
                DbHelper.TestConnection();
                lblStatus.Text = "Đã kết nối CSDL QLKhachSan.";
                if (!silent)
                    MessageBox.Show("Kết nối CSDL thành công!", "Kiểm tra kết nối",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Chưa kết nối được CSDL.";
                MessageBox.Show("Không thể kết nối CSDL:\n" + ex.Message +
                    "\n\nKiểm tra lại chuỗi kết nối trong App.config (Data Source, Initial Catalog).",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
