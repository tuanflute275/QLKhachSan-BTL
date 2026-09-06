using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.DangKy
{
    /// <summary>Ghi nhận sử dụng dịch vụ theo ngày cho phiếu đăng ký đang ở trạng thái Đang ở.</summary>
    public class frmSuDungDichVu : Form
    {
        private ComboBox cboDangKy, cboDichVu;
        private DataGridView dgv;
        private DateTimePicker dtpNgaySuDung;
        private NumericUpDown nudSoLuong;
        private TextBox txtGhiChu;
        private Label lblTongTien;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maChiTiet;

        public frmSuDungDichVu()
        {
            BuildUi();
            LoadDanhSachDangKy();
            LoadDanhSachDichVu();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Sử dụng dịch vụ theo ngày";
            Width = 900;
            Height = 640;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;
            errorProvider = new ErrorProvider();

            var pnlChon = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblChon = new Label { Text = "Phiếu đăng ký (đang ở):", Location = new System.Drawing.Point(6, 10), AutoSize = true };
            cboDangKy = new ComboBox { Location = new System.Drawing.Point(180, 6), Width = 420, DropDownStyle = ComboBoxStyle.DropDownList, FormattingEnabled = true };
            cboDangKy.Format += (s, e) =>
            {
                var row = e.ListItem as DataRowView;
                if (row != null) e.Value = $"#{row["MaDangky"]} - {row["TenKhach"]} - Phòng {row["SoPhong"]}";
            };
            cboDangKy.SelectedIndexChanged += (s, e) => { LoadChiTiet(); SetMode(false); };
            pnlChon.Controls.AddRange(new Control[] { lblChon, cboDangKy });

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 260,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionChanged += Dgv_SelectionChanged;

            lblTongTien = new Label
            {
                Dock = DockStyle.Top, Height = 28, TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold), Text = "Tổng tiền dịch vụ: 0 đ"
            };

            var pnlInput = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cboDichVu = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboDichVu.KeyDown += KeyboardHelper.EnterAsTab;

            dtpNgaySuDung = new DateTimePicker { Dock = DockStyle.Left, Width = 150, Format = DateTimePickerFormat.Short };
            dtpNgaySuDung.KeyDown += KeyboardHelper.EnterAsTab;

            nudSoLuong = new NumericUpDown { Dock = DockStyle.Left, Width = 100, Minimum = 1, Maximum = 100, Value = 1 };
            nudSoLuong.KeyDown += KeyboardHelper.EnterAsTab;

            txtGhiChu = new TextBox { Dock = DockStyle.Fill };
            txtGhiChu.KeyDown += KeyboardHelper.EnterAsTab;

            pnlInput.Controls.Add(new Label { Text = "Dịch vụ (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnlInput.Controls.Add(cboDichVu, 1, 0);
            pnlInput.Controls.Add(new Label { Text = "Ngày sử dụng (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnlInput.Controls.Add(dtpNgaySuDung, 1, 1);
            pnlInput.Controls.Add(new Label { Text = "Số lượng (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            pnlInput.Controls.Add(nudSoLuong, 1, 2);
            pnlInput.Controls.Add(new Label { Text = "Ghi chú:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 3);
            pnlInput.Controls.Add(txtGhiChu, 1, 3);

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
            btnLamMoi.Click += (s, e) => LoadChiTiet();

            pnlButtons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi });

            var pnlBottom = new Panel { Dock = DockStyle.Fill };
            pnlBottom.Controls.Add(pnlInput);
            pnlBottom.Controls.Add(pnlButtons);

            Controls.Add(pnlBottom);
            Controls.Add(lblTongTien);
            Controls.Add(dgv);
            Controls.Add(pnlChon);
        }

        private void LoadDanhSachDangKy()
        {
            var dt = DangKyDAL.DanhSach(TrangThaiDangKy.DangO);
            cboDangKy.DataSource = dt;
            cboDangKy.ValueMember = "MaDangky";
            cboDangKy.DisplayMember = "MaDangky";
        }

        private void LoadDanhSachDichVu()
        {
            var dt = DichVuDAL.DanhSach();
            cboDichVu.DataSource = dt;
            cboDichVu.ValueMember = "MaDichvu";
            cboDichVu.DisplayMember = "TenDichvu";
        }

        private int? MaDangkyDangChon()
        {
            return cboDangKy.SelectedValue == null ? (int?)null : Convert.ToInt32(cboDangKy.SelectedValue);
        }

        private void LoadChiTiet()
        {
            var maDangky = MaDangkyDangChon();
            if (maDangky == null) { dgv.DataSource = null; return; }

            dgv.DataSource = HoaDonChiTietDAL.DanhSachTheoDangky(maDangky.Value);
            if (dgv.Columns.Contains("MaDangky")) dgv.Columns["MaDangky"].Visible = false;
            if (dgv.Columns.Contains("MaDichvu")) dgv.Columns["MaDichvu"].Visible = false;
            if (dgv.Columns.Contains("MaChiTiet")) dgv.Columns["MaChiTiet"].HeaderText = "Mã CT";
            if (dgv.Columns.Contains("TenDichvu")) dgv.Columns["TenDichvu"].HeaderText = "Dịch vụ";
            if (dgv.Columns.Contains("NgaySuDung")) dgv.Columns["NgaySuDung"].HeaderText = "Ngày dùng";
            if (dgv.Columns.Contains("SoLuong")) dgv.Columns["SoLuong"].HeaderText = "SL";
            if (dgv.Columns.Contains("DonGia")) dgv.Columns["DonGia"].HeaderText = "Đơn giá";
            if (dgv.Columns.Contains("ThanhTien")) dgv.Columns["ThanhTien"].HeaderText = "Thành tiền";

            decimal tong = 0;
            var table = dgv.DataSource as DataTable;
            if (table != null)
                foreach (DataRow row in table.Rows) tong += Convert.ToDecimal(row["ThanhTien"]);
            lblTongTien.Text = $"Tổng tiền dịch vụ: {tong:N0} đ";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maChiTiet = Convert.ToInt32(row["MaChiTiet"]);
            cboDichVu.SelectedValue = Convert.ToInt32(row["MaDichvu"]);
            dtpNgaySuDung.Value = Convert.ToDateTime(row["NgaySuDung"]);
            nudSoLuong.Value = Convert.ToInt32(row["SoLuong"]);
            txtGhiChu.Text = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString();
        }

        private bool Validate_()
        {
            bool ok = Validator.CheckCondition(cboDangKy, errorProvider, MaDangkyDangChon() != null,
                "Chọn phiếu đăng ký đang ở");
            ok &= Validator.CheckCondition(cboDichVu, errorProvider, cboDichVu.SelectedValue != null, "Chọn dịch vụ");
            ok &= Validator.CheckCondition(nudSoLuong, errorProvider, nudSoLuong.Value > 0, "Số lượng phải lớn hơn 0");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var ct = new HoaDonChiTiet
            {
                MaChiTiet = _maChiTiet,
                MaDangky = MaDangkyDangChon().Value,
                MaDichvu = Convert.ToInt32(cboDichVu.SelectedValue),
                NgaySuDung = dtpNgaySuDung.Value.Date,
                SoLuong = (int)nudSoLuong.Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            try
            {
                if (_dangSua && _maChiTiet > 0) HoaDonChiTietDAL.Sua(ct);
                else HoaDonChiTietDAL.Them(ct);

                SetMode(false);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            if (MessageBox.Show("Bạn có chắc muốn xóa dòng sử dụng dịch vụ này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                HoaDonChiTietDAL.Xoa(Convert.ToInt32(row["MaChiTiet"]));
                LoadChiTiet();
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
                _maChiTiet = 0;
                if (cboDichVu.Items.Count > 0) cboDichVu.SelectedIndex = 0;
                dtpNgaySuDung.Value = DateTime.Today;
                nudSoLuong.Value = 1;
                txtGhiChu.Clear();
            }

            bool coDangKy = MaDangkyDangChon() != null;
            cboDichVu.Enabled = dtpNgaySuDung.Enabled = nudSoLuong.Enabled = txtGhiChu.Enabled = dangNhap;
            dgv.Enabled = cboDangKy.Enabled = !dangNhap;
            btnThem.Enabled = !dangNhap && coDangKy;
            btnSua.Enabled = btnXoa.Enabled = !dangNhap && dgv.CurrentRow != null;
            btnLamMoi.Enabled = !dangNhap;
            btnLuu.Enabled = btnHuy.Enabled = dangNhap;

            if (dangNhap) cboDichVu.Focus();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && btnThem.Enabled) { SetMode(true, false); e.Handled = true; }
            else if (e.KeyCode == Keys.F3 && btnSua.Enabled) { SetMode(true, true); e.Handled = true; }
            else if (e.KeyCode == Keys.Delete && btnXoa.Enabled) { BtnXoa_Click(sender, e); e.Handled = true; }
            else if (e.KeyCode == Keys.F5 && btnLamMoi.Enabled) { LoadChiTiet(); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape && btnHuy.Enabled) { SetMode(false); e.Handled = true; }
        }
    }
}
