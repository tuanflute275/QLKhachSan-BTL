using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class HoaDonChiTietDAL
    {
        public static DataTable DanhSachTheoDangky(int maDangky)
        {
            return DbHelper.ExecuteDataTable("sp_HoaDonChiTiet_DanhSachTheoDangky",
                DbHelper.Param("@MaDangky", maDangky));
        }

        public static int Them(HoaDonChiTiet ct)
        {
            var pOut = DbHelper.OutputParam("@MaChiTiet", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_HoaDonChiTiet_Them",
                DbHelper.Param("@MaDangky", ct.MaDangky),
                DbHelper.Param("@MaDichvu", ct.MaDichvu),
                DbHelper.Param("@NgaySuDung", ct.NgaySuDung.Date),
                DbHelper.Param("@SoLuong", ct.SoLuong),
                DbHelper.Param("@GhiChu", ct.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(HoaDonChiTiet ct)
        {
            DbHelper.ExecuteNonQuery("sp_HoaDonChiTiet_Sua",
                DbHelper.Param("@MaChiTiet", ct.MaChiTiet),
                DbHelper.Param("@MaDichvu", ct.MaDichvu),
                DbHelper.Param("@NgaySuDung", ct.NgaySuDung.Date),
                DbHelper.Param("@SoLuong", ct.SoLuong),
                DbHelper.Param("@GhiChu", ct.GhiChu));
        }

        public static void Xoa(int maChiTiet)
        {
            DbHelper.ExecuteNonQuery("sp_HoaDonChiTiet_Xoa", DbHelper.Param("@MaChiTiet", maChiTiet));
        }
    }
}
