using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class DichVuDAL
    {
        public static DataTable DanhSach(string tuKhoa = null)
        {
            return DbHelper.ExecuteDataTable("sp_DichVu_DanhSach", DbHelper.Param("@TuKhoa", tuKhoa));
        }

        public static int Them(DichVu dv)
        {
            var pOut = DbHelper.OutputParam("@MaDichvu", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_DichVu_Them",
                DbHelper.Param("@TenDichvu", dv.TenDichvu),
                DbHelper.Param("@DonGia", dv.DonGia),
                DbHelper.Param("@DonViTinh", dv.DonViTinh),
                DbHelper.Param("@TrangThai", dv.TrangThai),
                DbHelper.Param("@GhiChu", dv.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(DichVu dv)
        {
            DbHelper.ExecuteNonQuery("sp_DichVu_Sua",
                DbHelper.Param("@MaDichvu", dv.MaDichvu),
                DbHelper.Param("@TenDichvu", dv.TenDichvu),
                DbHelper.Param("@DonGia", dv.DonGia),
                DbHelper.Param("@DonViTinh", dv.DonViTinh),
                DbHelper.Param("@TrangThai", dv.TrangThai),
                DbHelper.Param("@GhiChu", dv.GhiChu));
        }

        public static void Xoa(int maDichvu)
        {
            DbHelper.ExecuteNonQuery("sp_DichVu_Xoa", DbHelper.Param("@MaDichvu", maDichvu));
        }
    }
}
