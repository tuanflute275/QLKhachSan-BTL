using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class NhanVienDAL
    {
        public static DataTable DanhSach(string tuKhoa = null)
        {
            return DbHelper.ExecuteDataTable("sp_NhanVien_DanhSach", DbHelper.Param("@TuKhoa", tuKhoa));
        }

        public static int Them(NhanVien nv)
        {
            var pOut = DbHelper.OutputParam("@MaNV", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_NhanVien_Them",
                DbHelper.Param("@HoTen", nv.HoTen),
                DbHelper.Param("@NgaySinh", nv.NgaySinh),
                DbHelper.Param("@GioiTinh", nv.GioiTinh),
                DbHelper.Param("@CMND", nv.CMND),
                DbHelper.Param("@SoDienThoai", nv.SoDienThoai),
                DbHelper.Param("@DiaChi", nv.DiaChi),
                DbHelper.Param("@ChucVu", nv.ChucVu),
                DbHelper.Param("@NgayVaoLam", nv.NgayVaoLam),
                DbHelper.Param("@TrangThai", nv.TrangThai),
                DbHelper.Param("@GhiChu", nv.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(NhanVien nv)
        {
            DbHelper.ExecuteNonQuery("sp_NhanVien_Sua",
                DbHelper.Param("@MaNV", nv.MaNV),
                DbHelper.Param("@HoTen", nv.HoTen),
                DbHelper.Param("@NgaySinh", nv.NgaySinh),
                DbHelper.Param("@GioiTinh", nv.GioiTinh),
                DbHelper.Param("@CMND", nv.CMND),
                DbHelper.Param("@SoDienThoai", nv.SoDienThoai),
                DbHelper.Param("@DiaChi", nv.DiaChi),
                DbHelper.Param("@ChucVu", nv.ChucVu),
                DbHelper.Param("@NgayVaoLam", nv.NgayVaoLam),
                DbHelper.Param("@TrangThai", nv.TrangThai),
                DbHelper.Param("@GhiChu", nv.GhiChu));
        }

        public static void Xoa(int maNV)
        {
            DbHelper.ExecuteNonQuery("sp_NhanVien_Xoa", DbHelper.Param("@MaNV", maNV));
        }
    }
}
