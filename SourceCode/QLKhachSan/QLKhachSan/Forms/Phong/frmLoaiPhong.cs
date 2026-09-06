using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.Common;
using QLKhachSan.DataAccess;
using QLKhachSan.Entities;

namespace QLKhachSan.Forms.Phong
{
    /// <summary>Danh mục Loại phòng: thêm/sửa/xóa/danh sách - toàn bộ qua Stored Procedure.</summary>
    public class frmLoaiPhong : Form
    {
        private DataGridView dgv;
        private TextBox txtTen, txtMoTa;
        private NumericUpDown nudSucChua, nudDonGia;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnLamMoi;
        private ErrorProvider errorProvider;

        private bool _dangSua;
        private int _maLoaiPhong;

        public frmLoaiPhong()
        {
            BuildUi();
            LoadDanhSach();
            SetMode(false);
        }

        private void BuildUi()
        {
            Text = "Danh mục Loại phòng";
            Width = 720;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += Form_KeyDown;

            errorProvider = new ErrorProvider();

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

            var pnlInput = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            txtTen = new TextBox { Dock = DockStyle.Fill };
            txtTen.KeyDown += KeyboardHelper.EnterAsTab;

            nudSucChua = new NumericUpDown { Dock = DockStyle.Left, Width = 100, Minimum = 1, Maximum = 20, Value = 1 };
            nudSucChua.KeyDown += KeyboardHelper.EnterAsTab;

            nudDonGia = new NumericUpDown
            {
                Dock = DockStyle.Left, Width = 150, Minimum = 0, Maximum = 100000000,
                DecimalPlaces = 0, Increment = 50000, ThousandsSeparator = true
            };
            nudDonGia.KeyDown += KeyboardHelper.EnterAsTab;

            txtMoTa = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            txtMoTa.KeyDown += KeyboardHelper.EnterAsTab;

            pnlInput.Controls.Add(new Label { Text = "Tên loại phòng (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            pnlInput.Controls.Add(txtTen, 1, 0);
            pnlInput.Controls.Add(new Label { Text = "Sức chứa (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            pnlInput.Controls.Add(nudSucChua, 1, 1);
            pnlInput.Controls.Add(new Label { Text = "Đơn giá/đêm (*):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            pnlInput.Controls.Add(nudDonGia, 1, 2);
            pnlInput.Controls.Add(new Label { Text = "Mô tả:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 3);
            pnlInput.Controls.Add(txtMoTa, 1, 3);

            var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 45, FlowDirection = FlowDirection.LeftToRight };
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
        }

        private void LoadDanhSach()
        {
            dgv.DataSource = LoaiPhongDAL.DanhSach();
            if (dgv.Columns.Contains("MaLoaiPhong")) dgv.Columns["MaLoaiPhong"].Visible = false;
            if (dgv.Columns.Contains("TenLoaiPhong")) dgv.Columns["TenLoaiPhong"].HeaderText = "Tên loại phòng";
            if (dgv.Columns.Contains("SucChua")) dgv.Columns["SucChua"].HeaderText = "Sức chứa";
            if (dgv.Columns.Contains("DonGia")) dgv.Columns["DonGia"].HeaderText = "Đơn giá/đêm";
            if (dgv.Columns.Contains("MoTa")) dgv.Columns["MoTa"].HeaderText = "Mô tả";
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || _dangSua) return;
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            _maLoaiPhong = Convert.ToInt32(row["MaLoaiPhong"]);
            txtTen.Text = row["TenLoaiPhong"].ToString();
            nudSucChua.Value = Convert.ToInt32(row["SucChua"]);
            nudDonGia.Value = Convert.ToDecimal(row["DonGia"]);
            txtMoTa.Text = row["MoTa"] == DBNull.Value ? "" : row["MoTa"].ToString();
        }

        private bool Validate_()
        {
            bool ok = Validator.CheckRequired(txtTen, errorProvider, "Bắt buộc nhập tên loại phòng");
            ok &= Validator.CheckCondition(nudSucChua, errorProvider, nudSucChua.Value > 0, "Sức chứa phải lớn hơn 0");
            ok &= Validator.CheckCondition(nudDonGia, errorProvider, nudDonGia.Value > 0, "Đơn giá phải lớn hơn 0");
            return ok;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!Validate_()) return;

            var lp = new LoaiPhong
            {
                MaLoaiPhong = _maLoaiPhong,
                TenLoaiPhong = txtTen.Text.Trim(),
                SucChua = (int)nudSucChua.Value,
                DonGia = nudDonGia.Value,
                MoTa = string.IsNullOrWhiteSpace(txtMoTa.Text) ? null : txtMoTa.Text.Trim()
            };

            try
            {
                bool isEditing = _dangSua && _maLoaiPhong > 0;
                if (isEditing) LoaiPhongDAL.Sua(lp);
                else LoaiPhongDAL.Them(lp);

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
            if (MessageBox.Show("Bạn có chắc muốn xóa loại phòng này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
                LoaiPhongDAL.Xoa(Convert.ToInt32(row["MaLoaiPhong"]));
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
                _maLoaiPhong = 0;
                txtTen.Clear(); nudSucChua.Value = 1; nudDonGia.Value = 0; txtMoTa.Clear();
            }

            txtTen.Enabled = nudSucChua.Enabled = nudDonGia.Enabled = txtMoTa.Enabled = dangNhap;
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
        }
    }
}
