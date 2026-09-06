using System;
using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class DangKyDAL
    {
        public static DataTable DanhSach(string trangThai = null)
        {
            return DbHelper.ExecuteDataTable("sp_Dangky_DanhSach", DbHelper.Param("@TrangThai", trangThai));
        }

        public static DataRow ChiTiet(int maDangky)
        {
            return DbHelper.ExecuteDataRow("sp_Dangky_ChiTiet", DbHelper.Param("@MaDangky", maDangky));
        }

        public static int Them(DangKy dk)
        {
            var pOut = DbHelper.OutputParam("@MaDangky", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_Dangky_Them",
                DbHelper.Param("@MaKH", dk.MaKH),
                DbHelper.Param("@MaPhong", dk.MaPhong),
                DbHelper.Param("@MaNVLap", dk.MaNVLap),
                DbHelper.Param("@NgayNhanDuKien", dk.NgayNhanDuKien.Date),
                DbHelper.Param("@NgayTraDuKien", dk.NgayTraDuKien.Date),
                DbHelper.Param("@SoKhach", dk.SoKhach),
                DbHelper.Param("@GhiChu", dk.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(DangKy dk)
        {
            DbHelper.ExecuteNonQuery("sp_Dangky_Sua",
                DbHelper.Param("@MaDangky", dk.MaDangky),
                DbHelper.Param("@MaKH", dk.MaKH),
                DbHelper.Param("@MaPhong", dk.MaPhong),
                DbHelper.Param("@NgayNhanDuKien", dk.NgayNhanDuKien.Date),
                DbHelper.Param("@NgayTraDuKien", dk.NgayTraDuKien.Date),
                DbHelper.Param("@SoKhach", dk.SoKhach),
                DbHelper.Param("@GhiChu", dk.GhiChu));
        }

        public static void Huy(int maDangky)
        {
            DbHelper.ExecuteNonQuery("sp_Dangky_Huy", DbHelper.Param("@MaDangky", maDangky));
        }

        public static void NhanPhong(int maDangky, DateTime? ngayNhanThucTe = null)
        {
            DbHelper.ExecuteNonQuery("sp_Dangky_NhanPhong",
                DbHelper.Param("@MaDangky", maDangky),
                DbHelper.Param("@NgayNhanThucTe", ngayNhanThucTe));
        }

        /// <summary>Trả phòng + lập hóa đơn. Trả về MaHoadon vừa sinh để mở màn hình in hóa đơn.</summary>
        public static int TraPhong_LapHoaDon(int maDangky, int maNVLap, DateTime? ngayTraThucTe,
            string hinhThucThanhToan)
        {
            var pOut = DbHelper.OutputParam("@MaHoadon", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_Dangky_TraPhong_LapHoaDon",
                DbHelper.Param("@MaDangky", maDangky),
                DbHelper.Param("@MaNVLap", maNVLap),
                DbHelper.Param("@NgayTraThucTe", ngayTraThucTe),
                DbHelper.Param("@HinhThucThanhToan", hinhThucThanhToan),
                pOut);
            return (int)pOut.Value;
        }
    }
}
