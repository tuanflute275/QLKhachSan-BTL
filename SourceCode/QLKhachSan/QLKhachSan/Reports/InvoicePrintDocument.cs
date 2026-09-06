using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QLKhachSan.DataAccess;

namespace QLKhachSan.Reports
{
    /// <summary>
    /// In hóa đơn thanh toán bằng System.Drawing.Printing (PrintDocument + PrintPreviewDialog).
    /// Đây là bản in "chạy được ngay" không phụ thuộc SAP Crystal Reports Runtime.
    /// Muốn thay bằng Crystal Report thật: xem hướng dẫn tại docs/04-HuongDan-TichHop-CrystalReport.md
    /// (tầng dữ liệu HoaDonDAL/HoaDonChiTietDAL/ChiPhiPhatSinhDAL bên dưới dùng chung cho cả 2 cách).
    /// </summary>
    public static class InvoicePrintDocument
    {
        public static void XemTruocHoaDon(int maHoadon)
        {
            var hd = HoaDonDAL.ChiTiet(maHoadon);
            if (hd == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var maDangky = Convert.ToInt32(hd["MaDangky"]);
            var dichVu = HoaDonChiTietDAL.DanhSachTheoDangky(maDangky);
            var phatSinh = ChiPhiPhatSinhDAL.DanhSachTheoDangky(maDangky);

            var doc = new PrintDocument();
            doc.PrintPage += (s, e) => VeHoaDon(e, hd, dichVu, phatSinh);

            using (var preview = new PrintPreviewDialog { Document = doc, Width = 950, Height = 750 })
            {
                preview.ShowDialog();
            }
        }

        private static void VeHoaDon(PrintPageEventArgs e, DataRow hd, DataTable dichVu, DataTable phatSinh)
        {
            var g = e.Graphics;
            var fontTitle = new Font("Arial", 16, FontStyle.Bold);
            var fontHeader = new Font("Arial", 11, FontStyle.Bold);
            var fontNormal = new Font("Arial", 10);
            var fontTong = new Font("Arial", 13, FontStyle.Bold);
            float x = 50, y = 40;

            g.DrawString("KHÁCH SẠN QLKhachSan", fontTitle, Brushes.Black, x, y); y += 30;
            g.DrawString("HÓA ĐƠN THANH TOÁN", fontHeader, Brushes.Black, x, y); y += 22;
            g.DrawString("Số hóa đơn: " + hd["MaHoadon"], fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Ngày lập: " + Convert.ToDateTime(hd["NgayLapHoadon"]).ToString("dd/MM/yyyy HH:mm"),
                fontNormal, Brushes.Black, x, y); y += 26;

            g.DrawString("Khách hàng: " + hd["TenKhach"], fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("CMND/CCCD: " + hd["CMND"], fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Điện thoại: " + hd["SoDienThoai"], fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Phòng: " + hd["SoPhong"] + " (" + hd["TenLoaiPhong"] + ")", fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Nhận phòng: " + Convert.ToDateTime(hd["NgayNhanThucTe"]).ToString("dd/MM/yyyy HH:mm"),
                fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Trả phòng: " + Convert.ToDateTime(hd["NgayTraThucTe"]).ToString("dd/MM/yyyy HH:mm"),
                fontNormal, Brushes.Black, x, y); y += 28;

            g.DrawString("Chi tiết dịch vụ đã sử dụng:", fontHeader, Brushes.Black, x, y); y += 20;
            if (dichVu.Rows.Count == 0)
            {
                g.DrawString("(Không sử dụng dịch vụ)", fontNormal, Brushes.Black, x + 10, y); y += 18;
            }
            else
            {
                foreach (DataRow row in dichVu.Rows)
                {
                    g.DrawString(
                        $"{Convert.ToDateTime(row["NgaySuDung"]):dd/MM} - {row["TenDichvu"]} x {row["SoLuong"]} = {Convert.ToDecimal(row["ThanhTien"]):N0} đ",
                        fontNormal, Brushes.Black, x + 10, y);
                    y += 16;
                }
            }
            y += 12;

            if (phatSinh.Rows.Count > 0)
            {
                g.DrawString("Chi phí phát sinh khác:", fontHeader, Brushes.Black, x, y); y += 20;
                foreach (DataRow row in phatSinh.Rows)
                {
                    g.DrawString($"{row["LoaiPhi"]} = {Convert.ToDecimal(row["SoTien"]):N0} đ",
                        fontNormal, Brushes.Black, x + 10, y);
                    y += 16;
                }
                y += 12;
            }

            g.DrawLine(Pens.Black, x, y, x + 500, y); y += 14;
            g.DrawString("Số đêm ở: " + hd["SoNgayO"], fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Tiền phòng: " + Convert.ToDecimal(hd["TienPhong"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Tiền dịch vụ: " + Convert.ToDecimal(hd["TienDichVu"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x, y); y += 18;
            g.DrawString("Tiền phát sinh: " + Convert.ToDecimal(hd["TienPhatSinh"]).ToString("N0") + " đ", fontNormal, Brushes.Black, x, y); y += 24;
            g.DrawString("TỔNG CỘNG: " + Convert.ToDecimal(hd["TongTien"]).ToString("N0") + " đ", fontTong, Brushes.Black, x, y); y += 28;
            g.DrawString("Hình thức thanh toán: " + Common.HinhThucThanhToan.HienThi(hd["HinhThucThanhToan"].ToString()),
                fontNormal, Brushes.Black, x, y); y += 32;

            g.DrawString("Nhân viên lập hóa đơn: " + hd["TenNhanVien"], fontNormal, Brushes.Black, x, y);
        }
    }
}
