using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.KhachHang
{
    /// <summary>Danh mục Khách hàng: thêm/sửa/xóa/tìm kiếm - toàn bộ qua Stored Procedure.</summary>
    public class frmKhachHang : Form
    {
        private DataGridView dgv;
        private TextBox txtCMND, txtHoTen, txtSoDienThoai, txtEmail, txtDiaChi, txtQuocTich, txtGhiChu, txtTimKiem;
        private DateTimePicker dtpNgaySinh;
        private RadioButton rdoNam, rdoNu;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi, btnTimKiem;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maKH;

        public frmKhachHang()
        {
            BuildUi();
            LoadDanhSach();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Danh mục Khách hàng";
            Width = 820;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;

            errorProvider = new ErrorProvider();

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 36 };
            var lblTim = new Label { Text = "Tìm (họ tên/CMND/SĐT):", Location = new System.Drawing.Point(6, 8), AutoSize = true };
            txtTimKiem = new TextBox { Location = new System.Drawing.Point(160, 5), Width = 220 };
            btnTimKiem = new Button { Text = "Tìm (Ctrl+F)", Location = new System.Drawing.Point(390, 3) };
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
            txtEmail = NewTextBox();
            txtQuocTich = NewTextBox(); txtQuocTich.Text = "Việt Nam";
            txtDiaChi = new TextBox { Dock = DockStyle.Fill };
            txtDiaChi.KeyDown += KeyboardHelper.EnterAsTab;
            txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 40 };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            int r = 0;
            AddRow(pnlInput, r++, "Số CMND/CCCD (*):", txtCMND, "Họ tên (*):", txtHoTen);
            AddRow(pnlInput, r++, "Ngày sinh (*):", dtpNgaySinh, "Giới tính:", pnlGioiTinh);
            AddRow(pnlInput, r++, "Số điện thoại (*):", txtSoDienThoai, "Email:", txtEmail);
            AddRow(pnlInput, r++, "Quốc tịch:", txtQuocTich, "Địa chỉ:", txtDiaChi);
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
            dgv.DataSource = KhachHangDAL.DanhSach(tuKhoa);
            if (dgv.Columns.Contains("MaKH")) dgv.Columns["MaKH"].Visible = false;
            if (dgv.Columns.Contains("NgayTao")) dgv.Columns["NgayTao"].Visible = false;
            if (dgv.Columns.Contains("CMND")) dgv.Columns["CMND"].HeaderText = "Số CMND/CCCD";
            if (dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns.Contains("NgaySinh")) dgv.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            if (dgv.Columns.Contains("GioiTinh")) dgv.Columns["GioiTinh"].HeaderText = "Nam";
            if (dgv.Columns.Contains("SoDienThoai")) dgv.Columns["SoDienThoai"].HeaderText = "Điện thoại";
            if (dgv.Columns.Contains("QuocTich")) dgv.Columns["QuocTich"].HeaderText = "Quốc tịch";
            if (dgv.Columns.Contains("DiaChi")) dgv.Columns["DiaChi"].HeaderText = "Địa chỉ";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maKH = Convert.ToInt32(row["MaKH"]);
            txtCMND.Text = row["CMND"].ToString();
            txtHoTen.Text = row["HoTen"].ToString();
            dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);
            bool nam = Convert.ToBoolean(row["GioiTinh"]);
            rdoNam.Checked = nam; rdoNu.Checked = !nam;
            txtSoDienThoai.Text = row["SoDienThoai"].ToString();
            txtEmail.Text = row["Email"] == DBNull.Value ? "" : row["Email"].ToString();
            txtDiaChi.Text = row["DiaChi"] == DBNull.Value ? "" : row["DiaChi"].ToString();
            txtQuocTich.Text = row["QuocTich"].ToString();
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
            ok &= Validator.CheckCondition(txtEmail, errorProvider, Validator.IsValidEmail(txtEmail.Text.Trim()),
                "Email không hợp lệ");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var kh = new Entities.KhachHang
            {
                MaKH = _maKH,
                CMND = txtCMND.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value.Date,
                GioiTinh = rdoNam.Checked,
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(txtDiaChi.Text) ? null : txtDiaChi.Text.Trim(),
                QuocTich = string.IsNullOrWhiteSpace(txtQuocTich.Text) ? "Việt Nam" : txtQuocTich.Text.Trim(),
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSua && _maKH > 0) KhachHangDAL.Sua(kh);
                else KhachHangDAL.Them(kh);

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
            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                KhachHangDAL.Xoa(Convert.ToInt32(row["MaKH"]));
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
                _maKH = 0;
                txtCMND.Clear(); txtHoTen.Clear(); dtpNgaySinh.Value = DateTime.Today.AddYears(-18);
                rdoNam.Checked = true;
                txtSoDienThoai.Clear(); txtEmail.Clear(); txtDiaChi.Clear();
                txtQuocTich.Text = "Việt Nam"; txtGhiChu.Clear();
            }

            foreach (Control c in new Control[] { txtCMND, txtHoTen, dtpNgaySinh, rdoNam, rdoNu,
                txtSoDienThoai, txtEmail, txtDiaChi, txtQuocTich, txtGhiChu })
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
