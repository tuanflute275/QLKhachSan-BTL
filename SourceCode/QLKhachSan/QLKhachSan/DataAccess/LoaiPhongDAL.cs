using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class LoaiPhongDAL
    {
        public static DataTable DanhSach()
        {
            return DbHelper.ExecuteDataTable("sp_LoaiPhong_DanhSach");
        }

        public static int Them(LoaiPhong lp)
        {
            var pOut = DbHelper.OutputParam("@MaLoaiPhong", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_LoaiPhong_Them",
                DbHelper.Param("@TenLoaiPhong", lp.TenLoaiPhong),
                DbHelper.Param("@SucChua", lp.SucChua),
                DbHelper.Param("@DonGia", lp.DonGia),
                DbHelper.Param("@MoTa", lp.MoTa),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(LoaiPhong lp)
        {
            DbHelper.ExecuteNonQuery("sp_LoaiPhong_Sua",
                DbHelper.Param("@MaLoaiPhong", lp.MaLoaiPhong),
                DbHelper.Param("@TenLoaiPhong", lp.TenLoaiPhong),
                DbHelper.Param("@SucChua", lp.SucChua),
                DbHelper.Param("@DonGia", lp.DonGia),
                DbHelper.Param("@MoTa", lp.MoTa));
        }

        public static void Xoa(int maLoaiPhong)
        {
            DbHelper.ExecuteNonQuery("sp_LoaiPhong_Xoa", DbHelper.Param("@MaLoaiPhong", maLoaiPhong));
        }
    }
}
