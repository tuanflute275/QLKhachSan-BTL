using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.DichVu
{
    /// <summary>Danh mục Dịch vụ: thêm/sửa/xóa/tìm kiếm - toàn bộ qua Stored Procedure.</summary>
    public class frmDichVu : Form
    {
        private DataGridView dgv;
        private TextBox txtTen, txtDonViTinh, txtGhiChu, txtTimKiem;
        private NumericUpDown nudDonGia;
        private CheckBox chkTrangThai;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi, btnTimKiem;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maDichvu;

        public frmDichVu()
        {
            BuildUi();
            LoadDanhSach();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Danh mục Dịch vụ";
            Width = 760;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;

            errorProvider = new ErrorProvider();

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 36 };
            var lblTim = new Label { Text = "Tìm tên dịch vụ:", Location = new System.Drawing.Point(6, 8), AutoSize = true };
            txtTimKiem = new TextBox { Location = new System.Drawing.Point(110, 5), Width = 200 };
            btnTimKiem = new Button { Text = "Tìm (Ctrl+F)", Location = new System.Drawing.Point(320, 3) };
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

            txtTen = new TextBox { Dock = DockStyle.Fill };
            txtTen.KeyDown += KeyboardHelper.EnterAsTab;

            nudDonGia = new NumericUpDown { Dock = DockStyle.Left, Width = 150, Minimum = 0, Maximum = 100000000, DecimalPlaces = 0, Increment = 10000, ThousandsSeparator = true };
            nudDonGia.KeyDown += KeyboardHelper.EnterAsTab;

            txtDonViTinh = new TextBox { Dock = DockStyle.Fill };
            txtDonViTinh.KeyDown += KeyboardHelper.EnterAsTab;

            chkTrangThai = new CheckBox { Text = "Còn kinh doanh", Checked = true, Dock = DockStyle.Left, AutoSize = true };
            chkTrangThai.KeyDown += KeyboardHelper.EnterAsTab;

            txtGhiChu = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 50 };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            pnlInput.Controls.Add(new Label { Text = "Tên dịch vụ (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnlInput.Controls.Add(txtTen, 1, 0);
            pnlInput.Controls.Add(new Label { Text = "Đơn giá (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnlInput.Controls.Add(nudDonGia, 1, 1);
            pnlInput.Controls.Add(new Label { Text = "Đơn vị tính:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            pnlInput.Controls.Add(txtDonViTinh, 1, 2);
            pnlInput.Controls.Add(new Label { Text = "Trạng thái:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 3);
            pnlInput.Controls.Add(chkTrangThai, 1, 3);
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

        private void LoadDanhSach(string tuKhoa = null)
        {
            dgv.DataSource = DichVuDAL.DanhSach(tuKhoa);
            if (dgv.Columns.Contains("MaDichvu")) dgv.Columns["MaDichvu"].Visible = false;
            if (dgv.Columns.Contains("TenDichvu")) dgv.Columns["TenDichvu"].HeaderText = "Tên dịch vụ";
            if (dgv.Columns.Contains("DonGia")) dgv.Columns["DonGia"].HeaderText = "Đơn giá";
            if (dgv.Columns.Contains("DonViTinh")) dgv.Columns["DonViTinh"].HeaderText = "Đơn vị tính";
            if (dgv.Columns.Contains("TrangThai")) dgv.Columns["TrangThai"].HeaderText = "Còn KD";
            if (dgv.Columns.Contains("GhiChu")) dgv.Columns["GhiChu"].HeaderText = "Ghi chú";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maDichvu = Convert.ToInt32(row["MaDichvu"]);
            txtTen.Text = row["TenDichvu"].ToString();
            nudDonGia.Value = Convert.ToDecimal(row["DonGia"]);
            txtDonViTinh.Text = row["DonViTinh"] == DBNull.Value ? "" : row["DonViTinh"].ToString();
            chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
            txtGhiChu.Text = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString();
        }

        private bool Validate_()
        {
            bool ok = Validator.CheckRequired(txtTen, errorProvider, "Bắt buộc nhập tên dịch vụ");
            ok &= Validator.CheckCondition(nudDonGia, errorProvider, nudDonGia.Value > 0, "Đơn giá phải lớn hơn 0");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var dv = new Entities.DichVu
            {
                MaDichvu = _maDichvu,
                TenDichvu = txtTen.Text.Trim(),
                DonGia = nudDonGia.Value,
                DonViTinh = string.IsNullOrWhiteSpace(txtDonViTinh.Text) ? null : txtDonViTinh.Text.Trim(),
                TrangThai = chkTrangThai.Checked,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSua && _maDichvu > 0) DichVuDAL.Sua(dv);
                else DichVuDAL.Them(dv);

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
            if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                DichVuDAL.Xoa(Convert.ToInt32(row["MaDichvu"]));
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
                _maDichvu = 0;
                txtTen.Clear(); nudDonGia.Value = 0; txtDonViTinh.Clear();
                chkTrangThai.Checked = true; txtGhiChu.Clear();
            }

            txtTen.Enabled = nudDonGia.Enabled = txtDonViTinh.Enabled = chkTrangThai.Enabled = txtGhiChu.Enabled = dangNhap;
            dgv.Enabled = !dangNhap;
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnLamMoi.Enabled = !dangNhap;
            btnLuu.Enabled = btnHuy.Enabled = dangNhap;

            if (dangNhap) txtTen.Focus();
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
