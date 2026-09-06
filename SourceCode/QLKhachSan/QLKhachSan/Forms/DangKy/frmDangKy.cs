using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.DangKy
{
    /// <summary>
    /// Đặt phòng / Nhận phòng / Hủy đăng ký. Nghiệp vụ then chốt: chỉ được chọn phòng
    /// nằm trong danh sách "phòng trống" (kết quả của sp_Phong_DanhSachTrong) cho khoảng ngày yêu cầu.
    /// </summary>
    public class frmDangKy : Form
    {
        private ComboBox cboTrangThaiLoc;
        private DataGridView dgvDangKy;
        private Button btnNhanPhong, btnHuyDangKy, btnSuaDangKy, btnLamMoi;

        private ComboBox cboKhachHang, cboNhanVien;
        private DateTimePicker dtpNhan, dtpTra;
        private NumericUpDown nudSoKhach;
        private Button btnTimPhongTrong, btnDatPhong, btnHuyNhap;
        private DataGridView dgvPhongTrong;
        private TextBox txtGhiChu;
        private Label lblTieuDeDatPhong;
        private ErrorProvider errorProvider;

        private bool _dangSuaDangKy;
        private int _maDangkyDangSua;

        public frmDangKy()
        {
            BuildUi();
            LoadDanhSachKhach();
            LoadDanhSachNhanVien();
            LoadDanhSachDangKy();
        }

        private void BuildUi()
        {
            Text = "Đặt phòng / Nhận phòng";
            Width = 1000;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;
            errorProvider = new ErrorProvider();

            // ----- Khu vực danh sách đăng ký -----
            var pnlLoc = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblLoc = new Label { Text = "Lọc trạng thái:", Location = new System.Drawing.Point(6, 10), AutoSize = true };
            cboTrangThaiLoc = new ComboBox { Location = new System.Drawing.Point(110, 6), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThaiLoc.Items.Add(new ComboItem(null, "Tất cả"));
            cboTrangThaiLoc.Items.Add(new ComboItem(TrangThaiDangKy.DaDat, TrangThaiDangKy.HienThi(TrangThaiDangKy.DaDat)));
            cboTrangThaiLoc.Items.Add(new ComboItem(TrangThaiDangKy.DangO, TrangThaiDangKy.HienThi(TrangThaiDangKy.DangO)));
            cboTrangThaiLoc.Items.Add(new ComboItem(TrangThaiDangKy.DaTra, TrangThaiDangKy.HienThi(TrangThaiDangKy.DaTra)));
            cboTrangThaiLoc.Items.Add(new ComboItem(TrangThaiDangKy.DaHuy, TrangThaiDangKy.HienThi(TrangThaiDangKy.DaHuy)));
            cboTrangThaiLoc.SelectedIndex = 0;
            cboTrangThaiLoc.SelectedIndexChanged += (s, e) => LoadDanhSachDangKy();

            btnNhanPhong = new Button { Text = "Nhận phòng", Location = new System.Drawing.Point(280, 4), Width = 100 };
            btnHuyDangKy = new Button { Text = "Hủy đăng ký", Location = new System.Drawing.Point(386, 4), Width = 100 };
            btnSuaDangKy = new Button { Text = "Sửa đăng ký", Location = new System.Drawing.Point(492, 4), Width = 100 };
            btnLamMoi = new Button { Text = "Làm mới (F5)", Location = new System.Drawing.Point(598, 4), Width = 100 };
            btnNhanPhong.Click += BtnNhanPhong_Click;
            btnHuyDangKy.Click += BtnHuyDangKy_Click;
            btnSuaDangKy.Click += BtnSuaDangKy_Click;
            btnLamMoi.Click += (s, e) => LoadDanhSachDangKy();

            pnlLoc.Controls.AddRange(new Control[] { lblLoc, cboTrangThaiLoc, btnNhanPhong, btnHuyDangKy, btnSuaDangKy, btnLamMoi });

            dgvDangKy = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 140,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // ----- Khu vực đặt phòng mới -----
            var pnlDatPhong = new Panel { Dock = DockStyle.Fill };
            lblTieuDeDatPhong = new Label
            {
                Text = "ĐẶT PHÒNG MỚI", Dock = DockStyle.Top, Height = 26,
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
            };

            var pnlInput = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, RowCount = 3, ColumnCount = 4, Padding = new Padding(6) };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlInput.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            pnlInput.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            pnlInput.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34f));

            cboKhachHang = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, FormattingEnabled = true };
            cboKhachHang.Format += (s, e) =>
            {
                var row = e.ListItem as DataRowView;
                if (row != null) e.Value = row["HoTen"] + " - CMND:" + row["CMND"];
            };
            cboKhachHang.KeyDown += KeyboardHelper.EnterAsTab;

            cboNhanVien = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboNhanVien.KeyDown += KeyboardHelper.EnterAsTab;

            dtpNhan = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today, Value = DateTime.Today };
            dtpTra = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today.AddDays(1), Value = DateTime.Today.AddDays(1) };
            dtpNhan.ValueChanged += (s, e) => { if (dtpTra.Value <= dtpNhan.Value) dtpTra.Value = dtpNhan.Value.AddDays(1); };
            dtpNhan.KeyDown += KeyboardHelper.EnterAsTab;
            dtpTra.KeyDown += KeyboardHelper.EnterAsTab;

            nudSoKhach = new NumericUpDown { Dock = DockStyle.Left, Width = 100, Minimum = 1, Maximum = 10, Value = 1 };
            nudSoKhach.KeyDown += KeyboardHelper.EnterAsTab;

            txtGhiChu = new TextBox { Dock = DockStyle.Fill };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            int r = 0;
            AddRow(pnlInput, r++, "Khách hàng (*):", cboKhachHang, "Nhân viên lập (*):", cboNhanVien);
            AddRow(pnlInput, r++, "Ngày nhận (*):", dtpNhan, "Ngày trả dự kiến (*):", dtpTra);
            AddRow(pnlInput, r++, "Số khách:", nudSoKhach, "Ghi chú:", txtGhiChu);

            var pnlTim = new Panel { Dock = DockStyle.Top, Height = 36 };
            btnTimPhongTrong = new Button { Text = "Tìm phòng trống", Location = new System.Drawing.Point(6, 4), Width = 140 };
            btnTimPhongTrong.Click += BtnTimPhongTrong_Click;
            var lblDsPhongTrong = new Label
            {
                Text = "Danh sách phòng trống (chọn 1 dòng để đặt):",
                Location = new System.Drawing.Point(156, 10), AutoSize = true,
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
            };
            pnlTim.Controls.AddRange(new Control[] { btnTimPhongTrong, lblDsPhongTrong });

            dgvPhongTrong = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var pnlActionButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42 };
            btnDatPhong = new Button { Text = "Đặt phòng (F2)", Width = 130 };
            btnHuyNhap = new Button { Text = "Hủy nhập (Esc)", Width = 130 };
            btnDatPhong.Click += BtnDatPhong_Click;
            btnHuyNhap.Click += (s, e) => ThoatCheDoSua();
            pnlActionButtons.Controls.AddRange(new Control[] { btnDatPhong, btnHuyNhap });

            pnlDatPhong.Controls.Add(dgvPhongTrong);
            pnlDatPhong.Controls.Add(pnlActionButtons);
            pnlDatPhong.Controls.Add(pnlTim);
            pnlDatPhong.Controls.Add(pnlInput);
            pnlDatPhong.Controls.Add(lblTieuDeDatPhong);

            Controls.Add(pnlDatPhong);
            Controls.Add(dgvDangKy);
            Controls.Add(pnlLoc);
        }

        private void AddRow(TableLayoutPanel panel, int row, string label1, Control control1, string label2, Control control2)
        {
            panel.Controls.Add(new Label { Text = label1, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
            panel.Controls.Add(control1, 1, row);
            panel.Controls.Add(new Label { Text = label2, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, row);
            panel.Controls.Add(control2, 3, row);
        }

        private void LoadDanhSachKhach()
        {
            var dt = KhachHangDAL.DanhSach();
            cboKhachHang.DataSource = dt;
            cboKhachHang.ValueMember = "MaKH";
            cboKhachHang.DisplayMember = "HoTen";
        }

        private void LoadDanhSachNhanVien()
        {
            var dt = NhanVienDAL.DanhSach();
            cboNhanVien.DataSource = dt;
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.DisplayMember = "HoTen";

            if (CurrentUser.DaDangNhap) cboNhanVien.SelectedValue = CurrentUser.MaNV;
        }

        private void LoadDanhSachDangKy()
        {
            string trangThai = ((ComboItem)cboTrangThaiLoc.SelectedItem)?.Value;
            dgvDangKy.DataSource = DangKyDAL.DanhSach(trangThai);

            if (dgvDangKy.Columns.Contains("MaKH")) dgvDangKy.Columns["MaKH"].Visible = false;
            if (dgvDangKy.Columns.Contains("MaPhong")) dgvDangKy.Columns["MaPhong"].Visible = false;
            if (dgvDangKy.Columns.Contains("MaNVLap")) dgvDangKy.Columns["MaNVLap"].Visible = false;
            if (dgvDangKy.Columns.Contains("MaDangky")) dgvDangKy.Columns["MaDangky"].HeaderText = "Mã ĐK";
            if (dgvDangKy.Columns.Contains("TenKhach")) dgvDangKy.Columns["TenKhach"].HeaderText = "Khách hàng";
            if (dgvDangKy.Columns.Contains("SoPhong")) dgvDangKy.Columns["SoPhong"].HeaderText = "Phòng";
            if (dgvDangKy.Columns.Contains("TenLoaiPhong")) dgvDangKy.Columns["TenLoaiPhong"].HeaderText = "Loại phòng";
            if (dgvDangKy.Columns.Contains("TenNhanVien")) dgvDangKy.Columns["TenNhanVien"].HeaderText = "NV lập";
            if (dgvDangKy.Columns.Contains("TrangThai")) dgvDangKy.Columns["TrangThai"].HeaderText = "Trạng thái";
        }

        private void BtnTimPhongTrong_Click(object sender, EventArgs e)
        {
            if (dtpTra.Value.Date <= dtpNhan.Value.Date)
            {
                MessageBox.Show("Ngày trả dự kiến phải sau ngày nhận.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? boQua = _dangSuaDangKy ? (int?)_maDangkyDangSua : null;
            dgvPhongTrong.DataSource = PhongDAL.DanhSachTrong(dtpNhan.Value, dtpTra.Value, null, boQua);

            if (dgvPhongTrong.Columns.Contains("MaPhong")) dgvPhongTrong.Columns["MaPhong"].Visible = false;
            if (dgvPhongTrong.Columns.Contains("MaLoaiPhong")) dgvPhongTrong.Columns["MaLoaiPhong"].Visible = false;
            if (dgvPhongTrong.Columns.Contains("SoPhong")) dgvPhongTrong.Columns["SoPhong"].HeaderText = "Số phòng";
            if (dgvPhongTrong.Columns.Contains("TenLoaiPhong")) dgvPhongTrong.Columns["TenLoaiPhong"].HeaderText = "Loại phòng";
            if (dgvPhongTrong.Columns.Contains("SucChua")) dgvPhongTrong.Columns["SucChua"].HeaderText = "Sức chứa";
            if (dgvPhongTrong.Columns.Contains("DonGia")) dgvPhongTrong.Columns["DonGia"].HeaderText = "Đơn giá";
            if (dgvPhongTrong.Columns.Contains("Tang")) dgvPhongTrong.Columns["Tang"].HeaderText = "Tầng";

            if (dgvPhongTrong.Rows.Count == 0)
                MessageBox.Show("Không còn phòng trống trong khoảng ngày đã chọn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateDatPhong()
        {
            bool ok = Validator.CheckCondition(cboKhachHang, errorProvider, cboKhachHang.SelectedValue != null, "Chọn khách hàng");
            ok &= Validator.CheckCondition(cboNhanVien, errorProvider, cboNhanVien.SelectedValue != null, "Chọn nhân viên lập");
            ok &= Validator.CheckCondition(dtpTra, errorProvider, dtpTra.Value.Date > dtpNhan.Value.Date, "Ngày trả phải sau ngày nhận");
            ok &= Validator.CheckCondition(dgvPhongTrong, errorProvider, dgvPhongTrong.CurrentRow != null,
                "Bấm 'Tìm phòng trống' và chọn 1 phòng trong danh sách");
            return ok;
        }

        private void BtnDatPhong_Click(object sender, EventArgs e)
        {
            if (!ValidateDatPhong()) return;

            var rowPhong = (DataRowView)dgvPhongTrong.CurrentRow.DataBoundItem;
            var dk = new Entities.DangKy
            {
                MaDangky = _maDangkyDangSua,
                MaKH = Convert.ToInt32(cboKhachHang.SelectedValue),
                MaPhong = Convert.ToInt32(rowPhong["MaPhong"]),
                MaNVLap = Convert.ToInt32(cboNhanVien.SelectedValue),
                NgayNhanDuKien = dtpNhan.Value.Date,
                NgayTraDuKien = dtpTra.Value.Date,
                SoKhach = (int)nudSoKhach.Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSuaDangKy) DangKyDAL.Sua(dk);
                else DangKyDAL.Them(dk);

                MessageBox.Show(_dangSuaDangKy ? "Cập nhật đăng ký thành công." : "Đặt phòng thành công.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ThoatCheDoSua();
                LoadDanhSachDangKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRowView SelectedDangKyRow()
        {
            if (dgvDangKy.CurrentRow == null)
            {
                MessageBox.Show("Chọn 1 đăng ký trong danh sách trước.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return (DataRowView)dgvDangKy.CurrentRow.DataBoundItem;
        }

        private void BtnNhanPhong_Click(object sender, EventArgs e)
        {
            var row = SelectedDangKyRow();
            if (row == null) return;
            if (row["TrangThai"].ToString() != TrangThaiDangKy.DaDat)
            {
                MessageBox.Show("Chỉ nhận phòng cho đăng ký đang ở trạng thái Đã đặt.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DangKyDAL.NhanPhong(Convert.ToInt32(row["MaDangky"]));
                LoadDanhSachDangKy();
                MessageBox.Show("Nhận phòng thành công. Khách có thể bắt đầu sử dụng dịch vụ.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnHuyDangKy_Click(object sender, EventArgs e)
        {
            var row = SelectedDangKyRow();
            if (row == null) return;
            if (row["TrangThai"].ToString() != TrangThaiDangKy.DaDat)
            {
                MessageBox.Show("Chỉ hủy được đăng ký đang ở trạng thái Đã đặt.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn hủy đăng ký này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                DangKyDAL.Huy(Convert.ToInt32(row["MaDangky"]));
                LoadDanhSachDangKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSuaDangKy_Click(object sender, EventArgs e)
        {
            var row = SelectedDangKyRow();
            if (row == null) return;
            if (row["TrangThai"].ToString() != TrangThaiDangKy.DaDat)
            {
                MessageBox.Show("Chỉ sửa được đăng ký đang ở trạng thái Đã đặt (chưa nhận phòng).", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _dangSuaDangKy = true;
            _maDangkyDangSua = Convert.ToInt32(row["MaDangky"]);
            lblTieuDeDatPhong.Text = "SỬA ĐĂNG KÝ #" + _maDangkyDangSua;

            cboKhachHang.SelectedValue = Convert.ToInt32(row["MaKH"]);
            cboNhanVien.SelectedValue = Convert.ToInt32(row["MaNVLap"]);
            dtpNhan.Value = Convert.ToDateTime(row["NgayNhanDuKien"]);
            dtpTra.Value = Convert.ToDateTime(row["NgayTraDuKien"]);
            nudSoKhach.Value = Convert.ToInt32(row["SoKhach"]);
            txtGhiChu.Text = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString();

            // Tự tìm lại phòng trống (loại trừ chính đăng ký này) và chọn sẵn phòng đang đặt,
            // để người dùng không phải tự nhớ và tìm lại đúng phòng cũ.
            int maPhongHienTai = Convert.ToInt32(row["MaPhong"]);
            BtnTimPhongTrong_Click(this, EventArgs.Empty);
            ChonPhongTrongGrid(maPhongHienTai);

            btnDatPhong.Text = "Lưu sửa (F2)";
        }

        private void ChonPhongTrongGrid(int maPhong)
        {
            foreach (DataGridViewRow gridRow in dgvPhongTrong.Rows)
            {
                var rv = (DataRowView)gridRow.DataBoundItem;
                if (Convert.ToInt32(rv["MaPhong"]) == maPhong)
                {
                    dgvPhongTrong.CurrentCell = gridRow.Cells[0];
                    break;
                }
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) { BtnDatPhong_Click(sender, e); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { ThoatCheDoSua(); e.Handled = true; }
            else if (e.KeyCode == Keys.F5) { LoadDanhSachDangKy(); e.Handled = true; }
        }

        private void ThoatCheDoSua()
        {
            _dangSuaDangKy = false;
            _maDangkyDangSua = 0;
            lblTieuDeDatPhong.Text = "ĐẶT PHÒNG MỚI";
            btnDatPhong.Text = "Đặt phòng (F2)";
            dtpNhan.Value = DateTime.Today;
            dtpTra.Value = DateTime.Today.AddDays(1);
            nudSoKhach.Value = 1;
            txtGhiChu.Clear();
            dgvPhongTrong.DataSource = null;
            if (CurrentUser.DaDangNhap) cboNhanVien.SelectedValue = CurrentUser.MaNV;
        }
    }
}
