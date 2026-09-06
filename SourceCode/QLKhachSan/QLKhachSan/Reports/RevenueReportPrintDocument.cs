using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QLKhachSan.DataAccess;

namespace QLKhachSan.Reports
{
    /// <summary>In báo cáo doanh thu bằng System.Drawing.Printing (xem ghi chú tại InvoicePrintDocument).</summary>
    public static class RevenueReportPrintDocument
    {
        public static void XemTruoc(DateTime tuNgay, DateTime denNgay)
        {
            var tongHop = BaoCaoDAL.TongHop(tuNgay, denNgay);
            var theoDichVu = BaoCaoDAL.DoanhThuDichVu(tuNgay, denNgay);
            var theoPhong = BaoCaoDAL.DoanhThuPhong(tuNgay, denNgay);

            var doc = new PrintDocument();
            doc.PrintPage += (s, e) => Ve(e, tuNgay, denNgay, tongHop, theoDichVu, theoPhong);

            using (var preview = new PrintPreviewDialog { Document = doc, Width = 950, Height = 750 })
            {
                preview.ShowDialog();
            }
        }

        private static void Ve(PrintPageEventArgs e, DateTime tuNgay, DateTime denNgay,
            DataRow tongHop, DataTable theoDichVu, DataTable theoPhong)
        {
            var g = e.Graphics;
            var fontTitle = new Font("Arial", 15, FontStyle.Bold);
            var fontHeader = new Font("Arial", 11, FontStyle.Bold);
            var fontNormal = new Font("Arial", 10);
            float x = 50, y = 40;

            g.DrawString("BÁO CÁO DOANH THU", fontTitle, Brushes.Black, x, y); y += 26;
            g.DrawString($"Từ ngày {tuNgay:dd/MM/yyyy} đến ngày {denNgay:dd/MM/yyyy}", fontNormal, Brushes.Black, x, y); y += 28;

            g.DrawString("TỔNG HỢP", fontHeader, Brushes.Black, x, y); y += 20;
            if (tongHop != null)
            {
                g.DrawString("Số lượng hóa đơn: " + tongHop["SoLuongHoaDon"], fontNormal, Brushes.Black, x + 10, y); y += 16;
                g.DrawString("Tổng tiền phòng: " + Convert.ToDecimal(tongHop["TongTienPhong"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x + 10, y); y += 16;
                g.DrawString("Tổng tiền dịch vụ: " + Convert.ToDecimal(tongHop["TongTienDichVu"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x + 10, y); y += 16;
                g.DrawString("Tổng tiền phát sinh: " + Convert.ToDecimal(tongHop["TongTienPhatSinh"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x + 10, y); y += 16;
                g.DrawString("TỔNG DOANH THU: " + Convert.ToDecimal(tongHop["TongDoanhThu"]).ToString("N0") + " đ", fontHeader, Brushes.Black, x + 10, y); y += 26;
            }

            g.DrawString("DOANH THU THEO DỊCH VỤ", fontHeader, Brushes.Black, x, y); y += 20;
            if (theoDichVu.Rows.Count == 0)
            {
                g.DrawString("(Không có dữ liệu)", fontNormal, Brushes.Black, x + 10, y); y += 16;
            }
            else
            {
                foreach (DataRow row in theoDichVu.Rows)
                {
                    g.DrawString(
                        $"{row["TenDichvu"]}: SL {row["TongSoLuong"]}, doanh thu {Convert.ToDecimal(row["TongDoanhThu"]):N0} đ",
                        fontNormal, Brushes.Black, x + 10, y);
                    y += 16;
                }
            }
            y += 12;

            g.DrawString("DOANH THU THEO PHÒNG", fontHeader, Brushes.Black, x, y); y += 20;
            if (theoPhong.Rows.Count == 0)
            {
                g.DrawString("(Không có dữ liệu)", fontNormal, Brushes.Black, x + 10, y); y += 16;
            }
            else
            {
                foreach (DataRow row in theoPhong.Rows)
                {
                    g.DrawString(
                        $"Phòng {row["SoPhong"]} ({row["TenLoaiPhong"]}): {row["SoLuotThue"]} lượt, {row["TongSoDem"]} đêm, doanh thu {Convert.ToDecimal(row["TongDoanhThuPhong"]):N0} đ",
                        fontNormal, Brushes.Black, x + 10, y);
                    y += 16;
                }
            }
        }
    }
}
