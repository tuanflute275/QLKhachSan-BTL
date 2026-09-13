using System;
using System.Data;

namespace QLKhachSan.Reports.CrystalData
{
    /// <summary>
    /// Nguồn dữ liệu ADO.NET DataSet cho rptDoanhThu.rpt (báo cáo doanh thu). Trong Crystal Reports
    /// Designer, chọn Database Expert -> Project Data -> ADO.NET Datasets -> dsDoanhThu. Cột khớp với
    /// sp_BaoCao_TongHop, sp_BaoCao_DoanhThuDichVu, sp_BaoCao_DoanhThuPhong.
    /// </summary>
    public class dsDoanhThu : DataSet
    {
        public DataTable TongHop { get; }
        public DataTable DoanhThuDichVu { get; }
        public DataTable DoanhThuPhong { get; }

        public dsDoanhThu() : base("dsDoanhThu")
        {
            TongHop = Tables.Add("TongHop");
            TongHop.Columns.Add("TuNgay", typeof(DateTime));
            TongHop.Columns.Add("DenNgay", typeof(DateTime));
            TongHop.Columns.Add("SoLuongHoaDon", typeof(int));
            TongHop.Columns.Add("TongTienPhong", typeof(decimal));
            TongHop.Columns.Add("TongTienDichVu", typeof(decimal));
            TongHop.Columns.Add("TongTienPhatSinh", typeof(decimal));
            TongHop.Columns.Add("TongDoanhThu", typeof(decimal));

            DoanhThuDichVu = Tables.Add("DoanhThuDichVu");
            DoanhThuDichVu.Columns.Add("MaDichvu", typeof(int));
            DoanhThuDichVu.Columns.Add("TenDichvu", typeof(string));
            DoanhThuDichVu.Columns.Add("TongSoLuong", typeof(int));
            DoanhThuDichVu.Columns.Add("TongDoanhThu", typeof(decimal));

            DoanhThuPhong = Tables.Add("DoanhThuPhong");
            DoanhThuPhong.Columns.Add("SoPhong", typeof(string));
            DoanhThuPhong.Columns.Add("TenLoaiPhong", typeof(string));
            DoanhThuPhong.Columns.Add("SoLuotThue", typeof(int));
            DoanhThuPhong.Columns.Add("TongSoDem", typeof(int));
            DoanhThuPhong.Columns.Add("TongDoanhThuPhong", typeof(decimal));
        }

        /// <summary>Nạp dữ liệu từ kết quả BaoCaoDAL.TongHop/DoanhThuDichVu/DoanhThuPhong.</summary>
        public void Nap(DateTime tuNgay, DateTime denNgay, DataRow tongHop, DataTable theoDichVu, DataTable theoPhong)
        {
            var r = TongHop.NewRow();
            r["TuNgay"] = tuNgay;
            r["DenNgay"] = denNgay;
            r["SoLuongHoaDon"] = tongHop?["SoLuongHoaDon"] ?? 0;
            r["TongTienPhong"] = tongHop?["TongTienPhong"] ?? 0m;
            r["TongTienDichVu"] = tongHop?["TongTienDichVu"] ?? 0m;
            r["TongTienPhatSinh"] = tongHop?["TongTienPhatSinh"] ?? 0m;
            r["TongDoanhThu"] = tongHop?["TongDoanhThu"] ?? 0m;
            TongHop.Rows.Add(r);

            foreach (DataRow row in theoDichVu.Rows)
            {
                var dr = DoanhThuDichVu.NewRow();
                dr["MaDichvu"] = row["MaDichvu"];
                dr["TenDichvu"] = row["TenDichvu"];
                dr["TongSoLuong"] = row["TongSoLuong"];
                dr["TongDoanhThu"] = row["TongDoanhThu"];
                DoanhThuDichVu.Rows.Add(dr);
            }

            foreach (DataRow row in theoPhong.Rows)
            {
                var pr = DoanhThuPhong.NewRow();
                pr["SoPhong"] = row["SoPhong"];
                pr["TenLoaiPhong"] = row["TenLoaiPhong"];
                pr["SoLuotThue"] = row["SoLuotThue"];
                pr["TongSoDem"] = row["TongSoDem"];
                pr["TongDoanhThuPhong"] = row["TongDoanhThuPhong"];
                DoanhThuPhong.Rows.Add(pr);
            }
        }
    }
}
