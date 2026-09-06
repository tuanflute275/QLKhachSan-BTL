using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.NhanVien
{
    /// <summary>Danh mục Nhân viên: thêm/sửa/xóa/tìm kiếm - toàn bộ qua Stored Procedure.</summary>
    public class frmNhanVien : Form
    {
        private DataGridView dgv;
        private TextBox txtCMND, txtHoTen, txtSoDienThoai, txtDiaChi, txtChucVu, txtGhiChu, txtTimKiem;
        private DateTimePicker dtpNgaySinh, dtpNgayVaoLam;
        private RadioButton rdoNam, rdoNu;
        private CheckBox chkTrangThai;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi, btnTimKiem;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maNV;

        public frmNhanVien()
        {
            BuildUi();
            LoadDanhSach();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Danh mục Nhân viên";
            Width = 820;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;

            errorProvider = new ErrorProvider();

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 36 };
            var lblTim = new Label { Text = "Tìm (họ tên/CMND):", Location = new System.Drawing.Point(6, 8), AutoSize = true };
            txtTimKiem = new TextBox { Location = new System.Drawing.Point(150, 5), Width = 220 };
            btnTimKiem = new Button { Text = "Tìm (Ctrl+F)", Location = new System.Drawing.Point(380, 3) };
            btnTimKiem.Click += (s, e) => LoadDanhSach(txtTimKiem.Text.Trim());
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { LoadDanhSach(txtTimKiem.Text.Trim()); e.Handled = true; } };
            pnlSearch.Controls.AddRange(new Control[] { lblTim, txtTimKiem, btnTimKiem });

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 240,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            var pnlInput = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, Padding = new Padding(10) };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            txtCMND = NewTextBox();
            txtHoTen = NewTextBox();
            dtpNgaySinh = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today };
            dtpNgaySinh.KeyDown += KeyboardHelper.EnterAsTab;

            var pnlGioiTinh = new FlowLayoutPanel { Dock = DockStyle.Fill };
            rdoNam = new RadioButton { Text = "Nam", Checked = true, AutoSize = true };
            rdoNu = new RadioButton { Text = "Nữ", AutoSize = true };
            pnlGioiTinh.Controls.AddRange(new Control[] { rdoNam, rdoNu });

            txtSoDienThoai = NewTextBox();
            txtChucVu = NewTextBox(); txtChucVu.Text = "Lễ tân";
            dtpNgayVaoLam = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            dtpNgayVaoLam.KeyDown += KeyboardHelper.EnterAsTab;
            chkTrangThai = new CheckBox { Text = "Đang làm việc", Checked = true, Dock = DockStyle.Fill, AutoSize = true };

            txtDiaChi = new TextBox { Dock = DockStyle.Fill };
            txtDiaChi.KeyDown += KeyboardHelper.EnterAsTab;
            txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 40 };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            int r = 0;
            AddRow(pnlInput, r++, "Số CMND/CCCD (*):", txtCMND, "Họ tên (*):", txtHoTen);
            AddRow(pnlInput, r++, "Ngày sinh (*):", dtpNgaySinh, "Giới tính:", pnlGioiTinh);
            AddRow(pnlInput, r++, "Số điện thoại (*):", txtSoDienThoai, "Chức vụ:", txtChucVu);
            AddRow(pnlInput, r++, "Ngày vào làm:", dtpNgayVaoLam, "Trạng thái:", chkTrangThai);
            pnlInput.Controls.Add(new Label { Text = "Địa chỉ:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, r);
            pnlInput.Controls.Add(txtDiaChi, 1, r);
            pnlInput.SetColumnSpan(txtDiaChi, 3);
            r++;
            pnlInput.Controls.Add(new Label { Text = "Ghi chú:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, r);
            pnlInput.Controls.Add(txtGhiChu, 1, r);
            pnlInput.SetColumnSpan(txtGhiChu, 3);

            var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 45 };
            btnThem = new Button { Text = "&Thêm (F2)", Width = 100 };
            btnSua = new Button { Text = "&Sửa (F3)", Width = 100 };
            btnXoa = new Button { Text = "&Xóa (Del)", Width = 100 };
            btnLuu = new Button { Text = "&Lưu (Enter)", Width = 100 };
            btnHuy = new Button { Text = "H&ủy (Esc)", Width = 100 };
            btnLamMoi = new Button { Text = "Là&m mới (F5)", Width = 110 };

            btnThem.Click += (s, e) => SetMode(true, false);
            btnSua.Click += (s, e) => SetMode(true, true);
            btnXoa.Click += BtnXoa_Click;
            btnLuu.Click += BtnLuu_Click;
            btnHuy.Click += (s, e) => SetMode(false);
            btnLamMoi.Click += (s, e) => LoadDanhSach();

            pnlButtons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi });

            var pnlBottom = new Panel { Dock = DockStyle.Fill };
            pnlBottom.Controls.Add(pnlInput);
            pnlBottom.Controls.Add(pnlButtons);

            Controls.Add(pnlBottom);
            Controls.Add(dgv);
            Controls.Add(pnlSearch);
        }

        private TextBox NewTextBox()
        {
            var tb = new TextBox { Dock = DockStyle.Fill };
            tb.KeyDown += KeyboardHelper.EnterAsTab;
            return tb;
        }

        private void AddRow(TableLayoutPanel panel, int row, string label1, Control control1, string label2, Control control2)
        {
            panel.Controls.Add(new Label { Text = label1, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
            panel.Controls.Add(control1, 1, row);
            panel.Controls.Add(new Label { Text = label2, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, row);
            panel.Controls.Add(control2, 3, row);
        }

        private void LoadDanhSach(string tuKhoa = null)
        {
            dgv.DataSource = NhanVienDAL.DanhSach(tuKhoa);
            if (dgv.Columns.Contains("MaNV")) dgv.Columns["MaNV"].Visible = false;
            if (dgv.Columns.Contains("CMND")) dgv.Columns["CMND"].HeaderText = "Số CMND/CCCD";
            if (dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns.Contains("NgaySinh")) dgv.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            if (dgv.Columns.Contains("GioiTinh")) dgv.Columns["GioiTinh"].HeaderText = "Nam";
            if (dgv.Columns.Contains("SoDienThoai")) dgv.Columns["SoDienThoai"].HeaderText = "Điện thoại";
            if (dgv.Columns.Contains("ChucVu")) dgv.Columns["ChucVu"].HeaderText = "Chức vụ";
            if (dgv.Columns.Contains("NgayVaoLam")) dgv.Columns["NgayVaoLam"].HeaderText = "Ngày vào làm";
            if (dgv.Columns.Contains("TrangThai")) dgv.Columns["TrangThai"].HeaderText = "Đang làm việc";
            if (dgv.Columns.Contains("DiaChi")) dgv.Columns["DiaChi"].HeaderText = "Địa chỉ";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maNV = Convert.ToInt32(row["MaNV"]);
            txtCMND.Text = row["CMND"].ToString();
            txtHoTen.Text = row["HoTen"].ToString();
            dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);
            bool nam = Convert.ToBoolean(row["GioiTinh"]);
            rdoNam.Checked = nam; rdoNu.Checked = !nam;
            txtSoDienThoai.Text = row["SoDienThoai"].ToString();
            txtChucVu.Text = row["ChucVu"].ToString();
            dtpNgayVaoLam.Value = Convert.ToDateTime(row["NgayVaoLam"]);
            chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
            txtDiaChi.Text = row["DiaChi"] == DBNull.Value ? "" : row["DiaChi"].ToString();
            txtGhiChu.Text = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString();
        }

        private bool Validate_()
        {
            bool ok = Validator.CheckRequired(txtCMND, errorProvider, "Bắt buộc nhập số CMND/CCCD");
            ok &= Validator.CheckCondition(txtCMND, errorProvider, Validator.IsValidCmnd(txtCMND.Text.Trim()),
                "CMND/CCCD phải gồm 9 hoặc 12 chữ số");
            ok &= Validator.CheckRequired(txtHoTen, errorProvider, "Bắt buộc nhập họ tên");
            ok &= Validator.CheckRequired(txtSoDienThoai, errorProvider, "Bắt buộc nhập số điện thoại");
            ok &= Validator.CheckCondition(txtSoDienThoai, errorProvider, Validator.IsValidPhone(txtSoDienThoai.Text.Trim()),
                "Số điện thoại không hợp lệ (9-11 chữ số)");
            ok &= Validator.CheckRequired(txtChucVu, errorProvider, "Bắt buộc nhập chức vụ");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var nv = new Entities.NhanVien
            {
                MaNV = _maNV,
                CMND = txtCMND.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value.Date,
                GioiTinh = rdoNam.Checked,
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim(),
                NgayVaoLam = dtpNgayVaoLam.Value.Date,
                TrangThai = chkTrangThai.Checked,
                DiaChi = string.IsNullOrWhiteSpace(txtDiaChi.Text) ? null : txtDiaChi.Text.Trim(),
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSua && _maNV > 0) NhanVienDAL.Sua(nv);
                else NhanVienDAL.Them(nv);

                SetMode(false);
                LoadDanhSach();
                MessageBox.Show("Lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                NhanVienDAL.Xoa(Convert.ToInt32(row["MaNV"]));
                LoadDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetMode(bool dangNhap, bool sua = false)
        {
            _dangSua = dangNhap && sua;
            if (dangNhap && !sua)
            {
                _maNV = 0;
                txtCMND.Clear(); txtHoTen.Clear(); dtpNgaySinh.Value = DateTime.Today.AddYears(-22);
                rdoNam.Checked = true;
                txtSoDienThoai.Clear(); txtChucVu.Text = "Lễ tân";
                dtpNgayVaoLam.Value = DateTime.Today; chkTrangThai.Checked = true;
                txtDiaChi.Clear(); txtGhiChu.Clear();
            }

            foreach (Control c in new Control[] { txtCMND, txtHoTen, dtpNgaySinh, rdoNam, rdoNu,
                txtSoDienThoai, txtChucVu, dtpNgayVaoLam, chkTrangThai, txtDiaChi, txtGhiChu })
                c.Enabled = dangNhap;

            dgv.Enabled = !dangNhap;
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnLamMoi.Enabled = !dangNhap;
            btnLuu.Enabled = btnHuy.Enabled = dangNhap;

            if (dangNhap) txtCMND.Focus();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && btnThem.Enabled) { SetMode(true, false); e.Handled = true; }
            else if (e.KeyCode == Keys.F3 && btnSua.Enabled) { SetMode(true, true); e.Handled = true; }
            else if (e.KeyCode == Keys.Delete && btnXoa.Enabled) { BtnXoa_Click(sender, e); e.Handled = true; }
            else if (e.KeyCode == Keys.F5 && btnLamMoi.Enabled) { LoadDanhSach(); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape && btnHuy.Enabled) { SetMode(false); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.F) { txtTimKiem.Focus(); e.Handled = true; }
        }
    }
}
