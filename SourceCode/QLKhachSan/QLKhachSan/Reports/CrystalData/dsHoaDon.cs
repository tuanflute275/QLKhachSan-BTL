using System;
using System.Data;

namespace QLKhachSan.Reports.CrystalData
{
    /// <summary>
    /// Nguồn dữ liệu ADO.NET DataSet cho rptHoaDon.rpt. Trong Crystal Reports Designer, chọn
    /// Database Expert -> Project Data -> ADO.NET Datasets -> dsHoaDon để kéo field vào report
    /// (xem docs/04-HuongDan-TichHop-CrystalReport.md, bước 3). Cột khớp với sp_Hoadon_ChiTiet,
    /// sp_HoaDonChiTiet_DanhSachTheoDangky, sp_ChiPhiPhatSinh_DanhSachTheoDangky.
    /// </summary>
    public class dsHoaDon : DataSet
    {
        public DataTable HoaDon { get; }
        public DataTable ChiTietDichVu { get; }
        public DataTable ChiPhiPhatSinh { get; }

        public dsHoaDon() : base("dsHoaDon")
        {
            HoaDon = Tables.Add("HoaDon");
            HoaDon.Columns.Add("MaHoadon", typeof(int));
            HoaDon.Columns.Add("MaDangky", typeof(int));
            HoaDon.Columns.Add("TenKhach", typeof(string));
            HoaDon.Columns.Add("CMND", typeof(string));
            HoaDon.Columns.Add("SoDienThoai", typeof(string));
            HoaDon.Columns.Add("SoPhong", typeof(string));
            HoaDon.Columns.Add("TenLoaiPhong", typeof(string));
            HoaDon.Columns.Add("DonGia", typeof(decimal));
            HoaDon.Columns.Add("NgayNhanThucTe", typeof(DateTime));
            HoaDon.Columns.Add("NgayTraThucTe", typeof(DateTime));
            HoaDon.Columns.Add("TenNhanVien", typeof(string));
            HoaDon.Columns.Add("NgayLapHoadon", typeof(DateTime));
            HoaDon.Columns.Add("SoNgayO", typeof(int));
            HoaDon.Columns.Add("TienPhong", typeof(decimal));
            HoaDon.Columns.Add("TienDichVu", typeof(decimal));
            HoaDon.Columns.Add("TienPhatSinh", typeof(decimal));
            HoaDon.Columns.Add("TongTien", typeof(decimal));
            HoaDon.Columns.Add("HinhThucThanhToan", typeof(string));

            ChiTietDichVu = Tables.Add("ChiTietDichVu");
            ChiTietDichVu.Columns.Add("MaChiTiet", typeof(int));
            ChiTietDichVu.Columns.Add("MaDangky", typeof(int));
            ChiTietDichVu.Columns.Add("TenDichvu", typeof(string));
            ChiTietDichVu.Columns.Add("NgaySuDung", typeof(DateTime));
            ChiTietDichVu.Columns.Add("SoLuong", typeof(int));
            ChiTietDichVu.Columns.Add("DonGia", typeof(decimal));
            ChiTietDichVu.Columns.Add("ThanhTien", typeof(decimal));

            ChiPhiPhatSinh = Tables.Add("ChiPhiPhatSinh");
            ChiPhiPhatSinh.Columns.Add("MaPhatSinh", typeof(int));
            ChiPhiPhatSinh.Columns.Add("MaDangky", typeof(int));
            ChiPhiPhatSinh.Columns.Add("LoaiPhi", typeof(string));
            ChiPhiPhatSinh.Columns.Add("SoTien", typeof(decimal));
            ChiPhiPhatSinh.Columns.Add("NgayPhatSinh", typeof(DateTime));

            Relations.Add("HoaDon_ChiTietDichVu", HoaDon.Columns["MaDangky"], ChiTietDichVu.Columns["MaDangky"], false);
            Relations.Add("HoaDon_ChiPhiPhatSinh", HoaDon.Columns["MaDangky"], ChiPhiPhatSinh.Columns["MaDangky"], false);
        }

        /// <summary>Nạp 1 hóa đơn (và dịch vụ/phát sinh kèm theo) từ kết quả HoaDonDAL/HoaDonChiTietDAL/ChiPhiPhatSinhDAL.</summary>
        public void Nap(DataRow hoaDon, DataTable dichVu, DataTable phatSinh)
        {
            var r = HoaDon.NewRow();
            r["MaHoadon"] = hoaDon["MaHoadon"];
            r["MaDangky"] = hoaDon["MaDangky"];
            r["TenKhach"] = hoaDon["TenKhach"];
            r["CMND"] = hoaDon["CMND"];
            r["SoDienThoai"] = hoaDon["SoDienThoai"];
            r["SoPhong"] = hoaDon["SoPhong"];
            r["TenLoaiPhong"] = hoaDon["TenLoaiPhong"];
            r["DonGia"] = hoaDon["DonGia"];
            r["NgayNhanThucTe"] = hoaDon["NgayNhanThucTe"];
            r["NgayTraThucTe"] = hoaDon["NgayTraThucTe"];
            r["TenNhanVien"] = hoaDon["TenNhanVien"];
            r["NgayLapHoadon"] = hoaDon["NgayLapHoadon"];
            r["SoNgayO"] = hoaDon["SoNgayO"];
            r["TienPhong"] = hoaDon["TienPhong"];
            r["TienDichVu"] = hoaDon["TienDichVu"];
            r["TienPhatSinh"] = hoaDon["TienPhatSinh"];
            r["TongTien"] = hoaDon["TongTien"];
            r["HinhThucThanhToan"] = Common.HinhThucThanhToan.HienThi(hoaDon["HinhThucThanhToan"].ToString());
            HoaDon.Rows.Add(r);

            foreach (DataRow row in dichVu.Rows)
            {
                var dr = ChiTietDichVu.NewRow();
                dr["MaChiTiet"] = row["MaChiTiet"];
                dr["MaDangky"] = row["MaDangky"];
                dr["TenDichvu"] = row["TenDichvu"];
                dr["NgaySuDung"] = row["NgaySuDung"];
                dr["SoLuong"] = row["SoLuong"];
                dr["DonGia"] = row["DonGia"];
                dr["ThanhTien"] = row["ThanhTien"];
                ChiTietDichVu.Rows.Add(dr);
            }

            foreach (DataRow row in phatSinh.Rows)
            {
                var pr = ChiPhiPhatSinh.NewRow();
                pr["MaPhatSinh"] = row["MaPhatSinh"];
                pr["MaDangky"] = row["MaDangky"];
                pr["LoaiPhi"] = row["LoaiPhi"];
                pr["SoTien"] = row["SoTien"];
                pr["NgayPhatSinh"] = row["NgayPhatSinh"];
                ChiPhiPhatSinh.Rows.Add(pr);
            }
        }
    }
}
