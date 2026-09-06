using System;
using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class PhongDAL
    {
        public static DataTable DanhSach(string tuKhoa = null)
        {
            return DbHelper.ExecuteDataTable("sp_Phong_DanhSach", DbHelper.Param("@TuKhoa", tuKhoa));
        }

        /// <summary>Nguồn xác thực duy nhất cho "phòng nào đang trống" trong khoảng ngày - luôn dựa
        /// vào tblDangky (xem sp_Phong_DanhSachTrong), không dựa vào cột TrangThai.</summary>
        public static DataTable DanhSachTrong(DateTime ngayNhan, DateTime ngayTra, int? maLoaiPhong = null,
            int? maDangkyBoQua = null)
        {
            return DbHelper.ExecuteDataTable("sp_Phong_DanhSachTrong",
                DbHelper.Param("@NgayNhan", ngayNhan.Date),
                DbHelper.Param("@NgayTra", ngayTra.Date),
                DbHelper.Param("@MaLoaiPhong", maLoaiPhong),
                DbHelper.Param("@MaDangkyBoQua", maDangkyBoQua));
        }

        public static bool KiemTraTrong(int maPhong, DateTime ngayNhan, DateTime ngayTra, int? maDangkyBoQua = null)
        {
            var pOut = DbHelper.OutputParam("@KetQua", SqlDbType.Bit);
            DbHelper.ExecuteNonQuery("sp_Phong_KiemTraTrong",
                DbHelper.Param("@MaPhong", maPhong),
                DbHelper.Param("@NgayNhan", ngayNhan.Date),
                DbHelper.Param("@NgayTra", ngayTra.Date),
                DbHelper.Param("@MaDangkyBoQua", maDangkyBoQua),
                pOut);
            return (bool)pOut.Value;
        }

        public static int Them(Phong p)
        {
            var pOut = DbHelper.OutputParam("@MaPhong", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_Phong_Them",
                DbHelper.Param("@SoPhong", p.SoPhong),
                DbHelper.Param("@MaLoaiPhong", p.MaLoaiPhong),
                DbHelper.Param("@Tang", p.Tang),
                DbHelper.Param("@TrangThai", p.TrangThai),
                DbHelper.Param("@GhiChu", p.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(Phong p)
        {
            DbHelper.ExecuteNonQuery("sp_Phong_Sua",
                DbHelper.Param("@MaPhong", p.MaPhong),
                DbHelper.Param("@SoPhong", p.SoPhong),
                DbHelper.Param("@MaLoaiPhong", p.MaLoaiPhong),
                DbHelper.Param("@Tang", p.Tang),
                DbHelper.Param("@TrangThai", p.TrangThai),
                DbHelper.Param("@GhiChu", p.GhiChu));
        }

        public static void Xoa(int maPhong)
        {
            DbHelper.ExecuteNonQuery("sp_Phong_Xoa", DbHelper.Param("@MaPhong", maPhong));
        }
    }
}
