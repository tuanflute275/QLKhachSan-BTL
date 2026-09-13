using System;
using System.Windows.Forms;
using QLKhachSan.DataAccess;
using QLKhachSan.Reports;

namespace QLKhachSan.Forms.BaoCao
{
    /// <summary>Báo cáo doanh thu theo dịch vụ / theo phòng trong khoảng thời gian.</summary>
    public class frmBaoCaoDoanhThu : Form
    {
        private DateTimePicker dtpTu, dtpDen;
        private Button btnXem, btnIn;
        private Label lblTongHop;
        private DataGridView dgvDichVu, dgvPhong;

        public frmBaoCaoDoanhThu()
        {
            BuildUi();
            XemBaoCao();
        }

        private void BuildUi()
        {
            Text = "Báo cáo doanh thu";
            Width = 900;
            Height = 680;
            StartPosition = FormStartPosition.CenterParent;

            var pnlLoc = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblTu = new Label { Text = "Từ ngày:", Location = new System.Drawing.Point(6, 10), AutoSize = true };
            dtpTu = new DateTimePicker { Location = new System.Drawing.Point(70, 6), Width = 130, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
            var lblDen = new Label { Text = "Đến ngày:", Location = new System.Drawing.Point(210, 10), AutoSize = true };
            dtpDen = new DateTimePicker { Location = new System.Drawing.Point(276, 6), Width = 130, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            btnXem = new Button { Text = "Xem báo cáo", Location = new System.Drawing.Point(416, 4), Width = 110 };
            btnIn = new Button { Text = "In báo cáo", Location = new System.Drawing.Point(532, 4), Width = 110 };
            btnXem.Click += (s, e) => XemBaoCao();
            btnIn.Click += (s, e) => CrystalRevenueViewer.XemTruoc(dtpTu.Value.Date, dtpDen.Value.Date);
            dtpTu.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; XemBaoCao(); } };
            dtpDen.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; XemBaoCao(); } };

            pnlLoc.Controls.AddRange(new Control[] { lblTu, dtpTu, lblDen, dtpDen, btnXem, btnIn });

            lblTongHop = new Label
            {
                Dock = DockStyle.Top, Height = 80, Padding = new Padding(6),
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
            };

            var lblDichVu = new Label { Dock = DockStyle.Top, Height = 22, Text = "Doanh thu theo dịch vụ:", Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold) };
            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Top, Height = 200, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var lblPhong = new Label { Dock = DockStyle.Top, Height = 22, Text = "Doanh thu theo phòng:", Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold) };
            dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Controls.Add(dgvPhong);
            Controls.Add(lblPhong);
            Controls.Add(dgvDichVu);
            Controls.Add(lblDichVu);
            Controls.Add(lblTongHop);
            Controls.Add(pnlLoc);
        }

        private void XemBaoCao()
        {
            var tu = dtpTu.Value.Date;
            var den = dtpDen.Value.Date;

            var tongHop = BaoCaoDAL.TongHop(tu, den);
            if (tongHop != null)
            {
                lblTongHop.Text =
                    $"Số lượng hóa đơn: {tongHop["SoLuongHoaDon"]}\n" +
                    $"Tổng tiền phòng: {Convert.ToDecimal(tongHop["TongTienPhong"]):N0} đ   |   " +
                    $"Tổng tiền dịch vụ: {Convert.ToDecimal(tongHop["TongTienDichVu"]):N0} đ   |   " +
                    $"Tổng phụ phí: {Convert.ToDecimal(tongHop["TongTienPhatSinh"]):N0} đ\n" +
                    $"TỔNG DOANH THU: {Convert.ToDecimal(tongHop["TongDoanhThu"]):N0} đ";
            }

            dgvDichVu.DataSource = BaoCaoDAL.DoanhThuDichVu(tu, den);
            if (dgvDichVu.Columns.Contains("MaDichvu")) dgvDichVu.Columns["MaDichvu"].Visible = false;
            if (dgvDichVu.Columns.Contains("TenDichvu")) dgvDichVu.Columns["TenDichvu"].HeaderText = "Dịch vụ";
            if (dgvDichVu.Columns.Contains("TongSoLuong")) dgvDichVu.Columns["TongSoLuong"].HeaderText = "Tổng SL";
            if (dgvDichVu.Columns.Contains("TongDoanhThu")) dgvDichVu.Columns["TongDoanhThu"].HeaderText = "Doanh thu";

            dgvPhong.DataSource = BaoCaoDAL.DoanhThuPhong(tu, den);
            if (dgvPhong.Columns.Contains("SoPhong")) dgvPhong.Columns["SoPhong"].HeaderText = "Phòng";
            if (dgvPhong.Columns.Contains("TenLoaiPhong")) dgvPhong.Columns["TenLoaiPhong"].HeaderText = "Loại phòng";
            if (dgvPhong.Columns.Contains("SoLuotThue")) dgvPhong.Columns["SoLuotThue"].HeaderText = "Số lượt thuê";
            if (dgvPhong.Columns.Contains("TongSoDem")) dgvPhong.Columns["TongSoDem"].HeaderText = "Tổng số đêm";
            if (dgvPhong.Columns.Contains("TongDoanhThuPhong")) dgvPhong.Columns["TongDoanhThuPhong"].HeaderText = "Doanh thu";
        }
    }
}
