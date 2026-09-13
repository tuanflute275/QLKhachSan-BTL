using System;
using System.Data;
using System.Windows.Forms;
using QLKhachSan.DataAccess;
using QLKhachSan.Reports;

namespace QLKhachSan.Forms.HoaDon
{
    /// <summary>Tra cứu và in lại hóa đơn đã lập.</summary>
    public class frmTraCuuHoaDon : Form
    {
        private DateTimePicker dtpTu, dtpDen;
        private Button btnLoc, btnInLai;
        private DataGridView dgv;

        public frmTraCuuHoaDon()
        {
            BuildUi();
            Loc();
        }

        private void BuildUi()
        {
            Text = "Tra cứu / In lại hóa đơn";
            Width = 900;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;

            var pnlLoc = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblTu = new Label { Text = "Từ ngày:", Location = new System.Drawing.Point(6, 10), AutoSize = true };
            dtpTu = new DateTimePicker { Location = new System.Drawing.Point(70, 6), Width = 130, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
            var lblDen = new Label { Text = "Đến ngày:", Location = new System.Drawing.Point(210, 10), AutoSize = true };
            dtpDen = new DateTimePicker { Location = new System.Drawing.Point(276, 6), Width = 130, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            btnLoc = new Button { Text = "Lọc", Location = new System.Drawing.Point(416, 4), Width = 80 };
            btnInLai = new Button { Text = "In lại hóa đơn", Location = new System.Drawing.Point(500, 4), Width = 130 };
            btnLoc.Click += (s, e) => Loc();
            btnInLai.Click += BtnInLai_Click;
            dtpTu.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; Loc(); } };
            dtpDen.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; Loc(); } };

            pnlLoc.Controls.AddRange(new Control[] { lblTu, dtpTu, lblDen, dtpDen, btnLoc, btnInLai });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellDoubleClick += (s, e) => InLaiHoaDonDangChon();

            Controls.Add(dgv);
            Controls.Add(pnlLoc);
        }

        private void Loc()
        {
            dgv.DataSource = HoaDonDAL.DanhSach(dtpTu.Value.Date, dtpDen.Value.Date);
            if (dgv.Columns.Contains("MaDangky")) dgv.Columns["MaDangky"].Visible = false;
            if (dgv.Columns.Contains("MaNVLap")) dgv.Columns["MaNVLap"].Visible = false;
            if (dgv.Columns.Contains("MaHoadon")) dgv.Columns["MaHoadon"].HeaderText = "Số HĐ";
            if (dgv.Columns.Contains("TenKhach")) dgv.Columns["TenKhach"].HeaderText = "Khách hàng";
            if (dgv.Columns.Contains("SoPhong")) dgv.Columns["SoPhong"].HeaderText = "Phòng";
            if (dgv.Columns.Contains("TenNhanVien")) dgv.Columns["TenNhanVien"].HeaderText = "NV lập";
            if (dgv.Columns.Contains("NgayLapHoadon")) dgv.Columns["NgayLapHoadon"].HeaderText = "Ngày lập";
            if (dgv.Columns.Contains("TongTien")) dgv.Columns["TongTien"].HeaderText = "Tổng tiền";
        }

        private void BtnInLai_Click(object sender, EventArgs e) => InLaiHoaDonDangChon();

        private void InLaiHoaDonDangChon()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Chọn 1 hóa đơn trong danh sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var row = (DataRowView)dgv.CurrentRow.DataBoundItem;
            CrystalInvoiceViewer.XemTruocHoaDon(Convert.ToInt32(row["MaHoadon"]));
        }
    }
}
