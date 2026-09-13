using System;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;
using QLKhachSan.DataAccess;
using QLKhachSan.Reports.CrystalData;

namespace QLKhachSan.Reports
{
    /// <summary>
    /// Xem báo cáo doanh thu bằng Crystal Report thật (rptDoanhThu.rpt, xem
    /// docs/04-HuongDan-TichHop-CrystalReport.md). Nếu report chưa được thiết kế xong trong Visual
    /// Studio, tự động rơi về <see cref="RevenueReportPrintDocument"/> làm phương án dự phòng.
    /// </summary>
    public static class CrystalRevenueViewer
    {
        public static void XemTruoc(DateTime tuNgay, DateTime denNgay)
        {
            var report = CrystalReportLoader.TryCreate("rptDoanhThu");
            if (report == null)
            {
                RevenueReportPrintDocument.XemTruoc(tuNgay, denNgay);
                return;
            }

            var tongHop = BaoCaoDAL.TongHop(tuNgay, denNgay);
            var theoDichVu = BaoCaoDAL.DoanhThuDichVu(tuNgay, denNgay);
            var theoPhong = BaoCaoDAL.DoanhThuPhong(tuNgay, denNgay);

            var ds = new dsDoanhThu();
            ds.Nap(tuNgay, denNgay, tongHop, theoDichVu, theoPhong);
            report.SetDataSource(ds);

            try
            {
                using (var form = new Form
                {
                    Text = "Báo cáo doanh thu",
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
