using System;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;
using QLKhachSan.DataAccess;
using QLKhachSan.Reports.CrystalData;

namespace QLKhachSan.Reports
{
    /// <summary>
    /// Xem hóa đơn bằng Crystal Report thật (rptHoaDon.rpt, xem
    /// docs/04-HuongDan-TichHop-CrystalReport.md). Nếu report chưa được thiết kế xong trong Visual
    /// Studio, tự động rơi về <see cref="InvoicePrintDocument"/> làm phương án dự phòng.
    /// </summary>
    public static class CrystalInvoiceViewer
    {
        public static void XemTruocHoaDon(int maHoadon)
        {
            var hd = HoaDonDAL.ChiTiet(maHoadon);
            if (hd == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var report = CrystalReportLoader.TryCreate("rptHoaDon");
            if (report == null)
            {
                InvoicePrintDocument.XemTruocHoaDon(maHoadon);
                return;
            }

            var maDangky = Convert.ToInt32(hd["MaDangky"]);
            var dichVu = HoaDonChiTietDAL.DanhSachTheoDangky(maDangky);
            var phatSinh = ChiPhiPhatSinhDAL.DanhSachTheoDangky(maDangky);

            var ds = new dsHoaDon();
            ds.Nap(hd, dichVu, phatSinh);
            report.SetDataSource(ds);

            try
            {
                using (var form = new Form
                {
                    Text = "Hóa đơn #" + maHoadon,
                    Width = 950,
                    Height = 750,
                    StartPosition = FormStartPosition.CenterParent
                })
                {
                    var viewer = new CrystalReportViewer { Dock = DockStyle.Fill, ReportSource = report };
                    form.Controls.Add(viewer);
                    form.ShowDialog();
                }
            }
            finally
            {
                report.Close();
                report.Dispose();
            }
        }
    }
}
