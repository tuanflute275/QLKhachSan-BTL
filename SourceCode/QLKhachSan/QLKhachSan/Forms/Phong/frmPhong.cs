using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.Phong
{
    /// <summary>Danh mục Phòng: thêm/sửa/xóa/tìm kiếm - toàn bộ qua Stored Procedure.</summary>
    public class frmPhong : Form
    {
        private DataGridView dgv;
        private TextBox txtSoPhong, txtGhiChu, txtTimKiem;
        private ComboBox cboLoaiPhong, cboTrangThai;
        private NumericUpDown nudTang;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi, btnTimKiem;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maPhong;

        public frmPhong()
        {
            BuildUi();
            LoadLoaiPhong();
            LoadDanhSach();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Danh mục Phòng";
            Width = 800;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;

            errorProvider = new ErrorProvider();

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 36 };
            var lblTim = new Label { Text = "Tìm số phòng:", Location = new System.Drawing.Point(6, 8), AutoSize = true };
            txtTimKiem = new TextBox { Location = new System.Drawing.Point(100, 5), Width = 180 };
            btnTimKiem = new Button { Text = "Tìm (Ctrl+F)", Location = new System.Drawing.Point(290, 3) };
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

            var pnlInput = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            txtSoPhong = new TextBox { Dock = DockStyle.Fill };
            txtSoPhong.KeyDown += KeyboardHelper.EnterAsTab;

            cboLoaiPhong = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiPhong.KeyDown += KeyboardHelper.EnterAsTab;

            nudTang = new NumericUpDown { Dock = DockStyle.Left, Width = 100, Minimum = 0, Maximum = 100 };
            nudTang.KeyDown += KeyboardHelper.EnterAsTab;

            cboTrangThai = new ComboBox { Dock = DockStyle.Left, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var ma in TrangThaiPhong.TatCa)
                cboTrangThai.Items.Add(new ComboItem(ma, TrangThaiPhong.HienThi(ma)));
            cboTrangThai.DisplayMember = "Text";
            cboTrangThai.ValueMember = "Value";
            cboTrangThai.KeyDown += KeyboardHelper.EnterAsTab;

            txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 50 };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            pnlInput.Controls.Add(new Label { Text = "Số phòng (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnlInput.Controls.Add(txtSoPhong, 1, 0);
            pnlInput.Controls.Add(new Label { Text = "Loại phòng (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnlInput.Controls.Add(cboLoaiPhong, 1, 1);
            pnlInput.Controls.Add(new Label { Text = "Tầng:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            pnlInput.Controls.Add(nudTang, 1, 2);
            pnlInput.Controls.Add(new Label { Text = "Trạng thái (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 3);
            pnlInput.Controls.Add(cboTrangThai, 1, 3);
            pnlInput.Controls.Add(new Label { Text = "Ghi chú:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 4);
            pnlInput.Controls.Add(txtGhiChu, 1, 4);

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

        private void LoadLoaiPhong()
        {
            var dt = LoaiPhongDAL.DanhSach();
            cboLoaiPhong.DisplayMember = "TenLoaiPhong";
            cboLoaiPhong.ValueMember = "MaLoaiPhong";
            cboLoaiPhong.DataSource = dt;
        }

        private void LoadDanhSach(string tuKhoa = null)
        {
            dgv.DataSource = PhongDAL.DanhSach(tuKhoa);
            if (dgv.Columns.Contains("MaPhong")) dgv.Columns["MaPhong"].Visible = false;
            if (dgv.Columns.Contains("MaLoaiPhong")) dgv.Columns["MaLoaiPhong"].Visible = false;
            if (dgv.Columns.Contains("SoPhong")) dgv.Columns["SoPhong"].HeaderText = "Số phòng";
            if (dgv.Columns.Contains("TenLoaiPhong")) dgv.Columns["TenLoaiPhong"].HeaderText = "Loại phòng";
            if (dgv.Columns.Contains("SucChua")) dgv.Columns["SucChua"].HeaderText = "Sức chứa";
            if (dgv.Columns.Contains("DonGia")) dgv.Columns["DonGia"].HeaderText = "Đơn giá";
            if (dgv.Columns.Contains("Tang")) dgv.Columns["Tang"].HeaderText = "Tầng";
            if (dgv.Columns.Contains("TrangThai")) dgv.Columns["TrangThai"].HeaderText = "Trạng thái";
            if (dgv.Columns.Contains("GhiChu")) dgv.Columns["GhiChu"].HeaderText = "Ghi chú";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maPhong = Convert.ToInt32(row["MaPhong"]);
            txtSoPhong.Text = row["SoPhong"].ToString();
            cboLoaiPhong.SelectedValue = Convert.ToInt32(row["MaLoaiPhong"]);
            nudTang.Value = row["Tang"] == DBNull.Value ? 0 : Convert.ToInt32(row["Tang"]);
            ChonTrangThai(row["TrangThai"].ToString());
            txtGhiChu.Text = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString();
        }

        private void ChonTrangThai(string ma)
        {
            foreach (ComboItem item in cboTrangThai.Items)
                if (item.Value == ma) { cboTrangThai.SelectedItem = item; return; }
        }

        private bool Validate_()
        {
            bool ok = Validator.CheckRequired(txtSoPhong, errorProvider, "Bắt buộc nhập số phòng");
            ok &= Validator.CheckCondition(cboLoaiPhong, errorProvider, cboLoaiPhong.SelectedValue != null, "Chọn loại phòng");
            ok &= Validator.CheckCondition(cboTrangThai, errorProvider, cboTrangThai.SelectedItem != null, "Chọn trạng thái");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var p = new Entities.Phong
            {
                MaPhong = _maPhong,
                SoPhong = txtSoPhong.Text.Trim(),
                MaLoaiPhong = Convert.ToInt32(cboLoaiPhong.SelectedValue),
                Tang = (int)nudTang.Value,
                TrangThai = ((ComboItem)cboTrangThai.SelectedItem).Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSua && _maPhong > 0) PhongDAL.Sua(p);
                else PhongDAL.Them(p);

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
            if (MessageBox.Show("Bạn có chắc muốn xóa phòng này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                PhongDAL.Xoa(Convert.ToInt32(row["MaPhong"]));
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
                _maPhong = 0;
                txtSoPhong.Clear(); nudTang.Value = 0; txtGhiChu.Clear();
                if (cboLoaiPhong.Items.Count > 0) cboLoaiPhong.SelectedIndex = 0;
                ChonTrangThai(TrangThaiPhong.Trong);
            }

            txtSoPhong.Enabled = cboLoaiPhong.Enabled = nudTang.Enabled = cboTrangThai.Enabled = txtGhiChu.Enabled = dangNhap;
            dgv.Enabled = !dangNhap;
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnLamMoi.Enabled = !dangNhap;
            btnLuu.Enabled = btnHuy.Enabled = dangNhap;

            if (dangNhap) txtSoPhong.Focus();
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
