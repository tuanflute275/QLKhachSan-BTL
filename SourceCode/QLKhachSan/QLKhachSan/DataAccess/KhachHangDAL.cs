using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class KhachHangDAL
    {
        public static DataTable DanhSach(string tuKhoa = null)
        {
            return DbHelper.ExecuteDataTable("sp_Khach_DanhSach", DbHelper.Param("@TuKhoa", tuKhoa));
        }

        public static int Them(KhachHang kh)
        {
            var pOut = DbHelper.OutputParam("@MaKH", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_Khach_Them",
                DbHelper.Param("@CMND", kh.CMND),
                DbHelper.Param("@HoTen", kh.HoTen),
                DbHelper.Param("@NgaySinh", kh.NgaySinh),
                DbHelper.Param("@GioiTinh", kh.GioiTinh),
                DbHelper.Param("@SoDienThoai", kh.SoDienThoai),
                DbHelper.Param("@Email", kh.Email),
                DbHelper.Param("@DiaChi", kh.DiaChi),
                DbHelper.Param("@QuocTich", kh.QuocTich),
                DbHelper.Param("@GhiChu", kh.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(KhachHang kh)
        {
            DbHelper.ExecuteNonQuery("sp_Khach_Sua",
                DbHelper.Param("@MaKH", kh.MaKH),
                DbHelper.Param("@CMND", kh.CMND),
                DbHelper.Param("@HoTen", kh.HoTen),
                DbHelper.Param("@NgaySinh", kh.NgaySinh),
                DbHelper.Param("@GioiTinh", kh.GioiTinh),
                DbHelper.Param("@SoDienThoai", kh.SoDienThoai),
                DbHelper.Param("@Email", kh.Email),
                DbHelper.Param("@DiaChi", kh.DiaChi),
                DbHelper.Param("@QuocTich", kh.QuocTich),
                DbHelper.Param("@GhiChu", kh.GhiChu));
        }

        public static void Xoa(int maKH)
        {
            DbHelper.ExecuteNonQuery("sp_Khach_Xoa", DbHelper.Param("@MaKH", maKH));
        }
    }
}
