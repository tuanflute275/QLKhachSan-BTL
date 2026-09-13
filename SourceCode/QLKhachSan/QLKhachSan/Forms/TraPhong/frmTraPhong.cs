using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;
using QLKhachSan.Reports;

namespace QLKhachSan.Forms.TraPhong
{
    /// <summary>
    /// Trả phòng + lập hóa đơn: nghiệp vụ trọng tâm của đề bài.
    /// TongTien = TienPhong (số đêm x đơn giá) + TienDichVu (tblHoadonchitiet) + TienPhatSinh (tblChiphiphatsinh).
    /// </summary>
    public class frmTraPhong : Form
    {
        private ComboBox cboDangKy;
        private Label lblThongTinPhong;
        private DataGridView dgvDichVu;
        private Label lblTienDichVu;

        private DataGridView dgvPhatSinh;
        private TextBox txtLoaiPhi;
        private NumericUpDown nudSoTien;
        private Button btnThemPhatSinh, btnXoaPhatSinh;

        private DateTimePicker dtpNgayTra;
        private ComboBox cboHinhThucTT, cboNhanVien;
        private Label lblTongHop;
        private Button btnTinhTien, btnTraPhong;
        private ErrorProvider errorProvider;

        public frmTraPhong()
        {
            BuildUi();
            LoadDanhSachDangKy();
            LoadDanhSachNhanVien();
        }

        private void BuildUi()
        {
            Text = "Trả phòng / Thanh toán";
            Width = 980;
            Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            errorProvider = new ErrorProvider();

            var pnlChon = new Panel { Dock = DockStyle.Top, Height = 66 };
            var lblChon = new Label { Text = "Phiếu đăng ký (đang ở):", Location = new System.Drawing.Point(6, 10), AutoSize = true };
            cboDangKy = new ComboBox { Location = new System.Drawing.Point(180, 6), Width = 420, DropDownStyle = ComboBoxStyle.DropDownList, FormattingEnabled = true };
            cboDangKy.Format += (s, e) =>
            {
                var row = e.ListItem as DataRowView;
                if (row != null) e.Value = $"#{row["MaDangky"]} - {row["TenKhach"]} - Phòng {row["SoPhong"]}";
            };
            cboDangKy.SelectedIndexChanged += (s, e) => NapThongTin();
            lblThongTinPhong = new Label { Location = new System.Drawing.Point(6, 36), AutoSize = true, Text = "" };
            pnlChon.Controls.AddRange(new Control[] { lblChon, cboDangKy, lblThongTinPhong });

            // ----- Dịch vụ đã dùng (chỉ xem) -----
            var lblDichVu = new Label { Dock = DockStyle.Top, Height = 22, Text = "Dịch vụ đã sử dụng:", Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold) };
            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Top, Height = 130, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            lblTienDichVu = new Label { Dock = DockStyle.Top, Height = 22, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Text = "Tổng tiền dịch vụ: 0 đ" };

            // ----- Chi phí phát sinh (CRUD) -----
            var lblPhatSinh = new Label { Dock = DockStyle.Top, Height = 22, Text = "Chi phí phát sinh khác:", Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold) };
            dgvPhatSinh = new DataGridView
            {
                Dock = DockStyle.Top, Height = 100, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var pnlPhatSinh = new Panel { Dock = DockStyle.Top, Height = 34 };
            var lblLoaiPhi = new Label { Text = "Loại phí:", Location = new System.Drawing.Point(0, 8), AutoSize = true };
            txtLoaiPhi = new TextBox { Location = new System.Drawing.Point(60, 5), Width = 260 };
            txtLoaiPhi.KeyDown += KeyboardHelper.EnterAsTab;
            var lblSoTien = new Label { Text = "Số tiền:", Location = new System.Drawing.Point(326, 8), AutoSize = true };
            nudSoTien = new NumericUpDown { Location = new System.Drawing.Point(380, 5), Width = 120, Maximum = 100000000, ThousandsSeparator = true };
            nudSoTien.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; e.SuppressKeyPress = true; BtnThemPhatSinh_Click(s, e); } };
            btnThemPhatSinh = new Button { Text = "Thêm phí (Enter)", Location = new System.Drawing.Point(506, 3), Width = 110 };
            btnXoaPhatSinh = new Button { Text = "Xóa phí", Location = new System.Drawing.Point(622, 3), Width = 90 };
            btnThemPhatSinh.Click += BtnThemPhatSinh_Click;
            btnXoaPhatSinh.Click += BtnXoaPhatSinh_Click;
            pnlPhatSinh.Controls.AddRange(new Control[] { lblLoaiPhi, txtLoaiPhi, lblSoTien, nudSoTien, btnThemPhatSinh, btnXoaPhatSinh });

            // ----- Thanh toán -----
            var pnlThanhToan = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, RowCount = 2, ColumnCount = 4, Padding = new Padding(6) };
            pnlThanhToan.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnlThanhToan.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlThanhToan.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnlThanhToan.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlThanhToan.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            pnlThanhToan.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            dtpNgayTra = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Value = DateTime.Now };
            dtpNgayTra.KeyDown += KeyboardHelper.EnterAsTab;

            cboHinhThucTT = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var ma in HinhThucThanhToan.TatCa)
                cboHinhThucTT.Items.Add(new ComboItem(ma, HinhThucThanhToan.HienThi(ma)));
            cboHinhThucTT.DisplayMember = "Text";
            cboHinhThucTT.SelectedIndex = 0;
            cboHinhThucTT.KeyDown += KeyboardHelper.EnterAsTab;

            cboNhanVien = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboNhanVien.KeyDown += KeyboardHelper.EnterAsTab;

            pnlThanhToan.Controls.Add(new Label { Text = "Ngày trả thực tế:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnlThanhToan.Controls.Add(dtpNgayTra, 1, 0);
            pnlThanhToan.Controls.Add(new Label { Text = "Hình thức TT:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, 0);
            pnlThanhToan.Controls.Add(cboHinhThucTT, 3, 0);
            pnlThanhToan.Controls.Add(new Label { Text = "Nhân viên lập HĐ (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnlThanhToan.Controls.Add(cboNhanVien, 1, 1);

            lblTongHop = new Label
            {
                Dock = DockStyle.Top, Height = 70, TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font(Font.FontFamily, 11, System.Drawing.FontStyle.Bold),
                Text = "Bấm 'Tính tiền' để xem trước hóa đơn."
            };

            var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 45 };
            btnTinhTien = new Button { Text = "Tính tiền / Xem trước", Width = 160 };
            btnTraPhong = new Button { Text = "Trả phòng && Lập hóa đơn", Width = 200 };
            btnTinhTien.Click += BtnTinhTien_Click;
            btnTraPhong.Click += BtnTraPhong_Click;
            pnlButtons.Controls.AddRange(new Control[] { btnTinhTien, btnTraPhong });

            var pnlAll = new Panel { Dock = DockStyle.Fill };
            pnlAll.Controls.Add(lblTongHop);
            pnlAll.Controls.Add(pnlThanhToan);
            pnlAll.Controls.Add(pnlPhatSinh);
            pnlAll.Controls.Add(dgvPhatSinh);
            pnlAll.Controls.Add(lblPhatSinh);
            pnlAll.Controls.Add(lblTienDichVu);
            pnlAll.Controls.Add(dgvDichVu);
            pnlAll.Controls.Add(lblDichVu);

            Controls.Add(pnlButtons);
            Controls.Add(pnlAll);
            Controls.Add(pnlChon);

            SetTrangThaiSan(false);
        }

        private void LoadDanhSachDangKy()
        {
            var dt = DangKyDAL.DanhSach(TrangThaiDangKy.DangO);
            cboDangKy.DataSource = dt;
            cboDangKy.ValueMember = "MaDangky";
            cboDangKy.DisplayMember = "MaDangky";
        }

        private void LoadDanhSachNhanVien()
        {
            var dt = NhanVienDAL.DanhSach();
            cboNhanVien.DataSource = dt;
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.DisplayMember = "HoTen";

            if (CurrentUser.DaDangNhap) cboNhanVien.SelectedValue = CurrentUser.MaNV;
        }

        private int? MaDangkyDangChon() =>
            cboDangKy.SelectedValue == null ? (int?)null : Convert.ToInt32(cboDangKy.SelectedValue);

        private void NapThongTin()
        {
            var maDangky = MaDangkyDangChon();
            SetTrangThaiSan(maDangky != null);
            lblTongHop.Text = "Bấm 'Tính tiền' để xem trước hóa đơn.";

            if (maDangky == null)
            {
                lblThongTinPhong.Text = "";
                dgvDichVu.DataSource = null;
                dgvPhatSinh.DataSource = null;
                return;
            }

            var chiTiet = DangKyDAL.ChiTiet(maDangky.Value);
            var ngayNhan = Convert.ToDateTime(chiTiet["NgayNhanThucTe"]);
            lblThongTinPhong.Text =
                $"Khách: {chiTiet["TenKhach"]}   |   Phòng: {chiTiet["SoPhong"]} ({chiTiet["TenLoaiPhong"]}, {Convert.ToDecimal(chiTiet["DonGia"]):N0} đ/đêm)   |   Nhận phòng lúc: {ngayNhan:dd/MM/yyyy HH:mm}";

            NapDichVu(maDangky.Value);
            NapPhatSinh(maDangky.Value);
        }

        private void NapDichVu(int maDangky)
        {
            var dt = HoaDonChiTietDAL.DanhSachTheoDangky(maDangky);
            dgvDichVu.DataSource = dt;
            if (dgvDichVu.Columns.Contains("MaChiTiet")) dgvDichVu.Columns["MaChiTiet"].Visible = false;
            if (dgvDichVu.Columns.Contains("MaDangky")) dgvDichVu.Columns["MaDangky"].Visible = false;
            if (dgvDichVu.Columns.Contains("MaDichvu")) dgvDichVu.Columns["MaDichvu"].Visible = false;
            if (dgvDichVu.Columns.Contains("TenDichvu")) dgvDichVu.Columns["TenDichvu"].HeaderText = "Dịch vụ";
            if (dgvDichVu.Columns.Contains("NgaySuDung")) dgvDichVu.Columns["NgaySuDung"].HeaderText = "Ngày dùng";
            if (dgvDichVu.Columns.Contains("SoLuong")) dgvDichVu.Columns["SoLuong"].HeaderText = "SL";
            if (dgvDichVu.Columns.Contains("DonGia")) dgvDichVu.Columns["DonGia"].HeaderText = "Đơn giá";
            if (dgvDichVu.Columns.Contains("ThanhTien")) dgvDichVu.Columns["ThanhTien"].HeaderText = "Thành tiền";

            decimal tong = 0;
            foreach (DataRow row in dt.Rows) tong += Convert.ToDecimal(row["ThanhTien"]);
            lblTienDichVu.Text = $"Tổng tiền dịch vụ: {tong:N0} đ";
        }

        private void NapPhatSinh(int maDangky)
        {
            var dt = ChiPhiPhatSinhDAL.DanhSachTheoDangky(maDangky);
            dgvPhatSinh.DataSource = dt;
            if (dgvPhatSinh.Columns.Contains("MaPhatSinh")) dgvPhatSinh.Columns["MaPhatSinh"].Visible = false;
            if (dgvPhatSinh.Columns.Contains("MaDangky")) dgvPhatSinh.Columns["MaDangky"].Visible = false;
            if (dgvPhatSinh.Columns.Contains("LoaiPhi")) dgvPhatSinh.Columns["LoaiPhi"].HeaderText = "Loại phí";
            if (dgvPhatSinh.Columns.Contains("SoTien")) dgvPhatSinh.Columns["SoTien"].HeaderText = "Số tiền";
            if (dgvPhatSinh.Columns.Contains("NgayPhatSinh")) dgvPhatSinh.Columns["NgayPhatSinh"].HeaderText = "Ngày";
        }

        private decimal TongTienPhatSinh()
        {
            decimal tong = 0;
            var dt = dgvPhatSinh.DataSource as DataTable;
            if (dt != null) foreach (DataRow row in dt.Rows) tong += Convert.ToDecimal(row["SoTien"]);
            return tong;
        }

        private decimal TongTienDichVu()
        {
            decimal tong = 0;
            var dt = dgvDichVu.DataSource as DataTable;
            if (dt != null) foreach (DataRow row in dt.Rows) tong += Convert.ToDecimal(row["ThanhTien"]);
            return tong;
        }

        private void BtnThemPhatSinh_Click(object sender, EventArgs e)
        {
            var maDangky = MaDangkyDangChon();
            if (maDangky == null) return;
            if (string.IsNullOrWhiteSpace(txtLoaiPhi.Text) || nudSoTien.Value <= 0)
            {
                MessageBox.Show("Nhập loại phí và số tiền hợp lệ (> 0).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ChiPhiPhatSinhDAL.Them(new ChiPhiPhatSinh
                {
                    MaDangky = maDangky.Value,
                    LoaiPhi = txtLoaiPhi.Text.Trim(),
                    SoTien = nudSoTien.Value,
                    NgayPhatSinh = DateTime.Today
                });
                txtLoaiPhi.Clear();
                nudSoTien.Value = 0;
                NapPhatSinh(maDangky.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoaPhatSinh_Click(object sender, EventArgs e)
        {
            if (dgvPhatSinh.CurrentRow == null) return;
            var row = (DataRowView)dgvPhatSinh.CurrentRow.DataBoundItem;
            try
            {
                ChiPhiPhatSinhDAL.Xoa(Convert.ToInt32(row["MaPhatSinh"]));
                NapPhatSinh(MaDangkyDangChon().Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTinhTien_Click(object sender, EventArgs e)
        {
            var maDangky = MaDangkyDangChon();
            if (maDangky == null) return;

            var chiTiet = DangKyDAL.ChiTiet(maDangky.Value);
            var ngayNhan = Convert.ToDateTime(chiTiet["NgayNhanThucTe"]);
            var donGia = Convert.ToDecimal(chiTiet["DonGia"]);

            int soNgay = (dtpNgayTra.Value.Date - ngayNhan.Date).Days;
            if (soNgay < 1) soNgay = 1;

            decimal tienPhong = soNgay * donGia;
            decimal tienDichVu = TongTienDichVu();
            decimal tienPhatSinh = TongTienPhatSinh();
            decimal tongCong = tienPhong + tienDichVu + tienPhatSinh;

            lblTongHop.Text =
                $"Số đêm: {soNgay}   |   Tiền phòng: {tienPhong:N0} đ   |   Tiền dịch vụ: {tienDichVu:N0} đ   |   " +
                $"Phụ phí: {tienPhatSinh:N0} đ   |   TỔNG CỘNG: {tongCong:N0} đ";
        }

        private void BtnTraPhong_Click(object sender, EventArgs e)
        {
            var maDangky = MaDangkyDangChon();
            bool ok = Validator.CheckCondition(cboDangKy, errorProvider, maDangky != null, "Chọn phiếu đăng ký");
            ok &= Validator.CheckCondition(cboNhanVien, errorProvider, cboNhanVien.SelectedValue != null, "Chọn nhân viên lập hóa đơn");
            if (!ok) return;

            if (MessageBox.Show("Xác nhận trả phòng và lập hóa đơn thanh toán?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                string hinhThuc = ((ComboItem)cboHinhThucTT.SelectedItem).Value;
                int maHoadon = DangKyDAL.TraPhong_LapHoaDon(maDangky.Value,
                    Convert.ToInt32(cboNhanVien.SelectedValue), dtpNgayTra.Value, hinhThuc);

                MessageBox.Show("Trả phòng thành công! Đã lập hóa đơn #" + maHoadon, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CrystalInvoiceViewer.XemTruocHoaDon(maHoadon);

                LoadDanhSachDangKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTrangThaiSan(bool coChonDangKy)
        {
            btnTinhTien.Enabled = coChonDangKy;
            btnTraPhong.Enabled = coChonDangKy;
            btnThemPhatSinh.Enabled = coChonDangKy;
            btnXoaPhatSinh.Enabled = coChonDangKy;
        }
    }
}
