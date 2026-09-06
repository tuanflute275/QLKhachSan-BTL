using System.Data;
using QLKhachSan.Common;
using QLKhachSan.Entities;

namespace QLKhachSan.DataAccess
{
    public static class ChiPhiPhatSinhDAL
    {
        public static DataTable DanhSachTheoDangky(int maDangky)
        {
            return DbHelper.ExecuteDataTable("sp_ChiPhiPhatSinh_DanhSachTheoDangky",
                DbHelper.Param("@MaDangky", maDangky));
        }

        public static int Them(ChiPhiPhatSinh cp)
        {
            var pOut = DbHelper.OutputParam("@MaPhatSinh", SqlDbType.Int);
            DbHelper.ExecuteNonQuery("sp_ChiPhiPhatSinh_Them",
                DbHelper.Param("@MaDangky", cp.MaDangky),
                DbHelper.Param("@LoaiPhi", cp.LoaiPhi),
                DbHelper.Param("@SoTien", cp.SoTien),
                DbHelper.Param("@NgayPhatSinh", cp.NgayPhatSinh.Date),
                DbHelper.Param("@GhiChu", cp.GhiChu),
                pOut);
            return (int)pOut.Value;
        }

        public static void Sua(ChiPhiPhatSinh cp)
        {
            DbHelper.ExecuteNonQuery("sp_ChiPhiPhatSinh_Sua",
                DbHelper.Param("@MaPhatSinh", cp.MaPhatSinh),
                DbHelper.Param("@LoaiPhi", cp.LoaiPhi),
                DbHelper.Param("@SoTien", cp.SoTien),
                DbHelper.Param("@NgayPhatSinh", cp.NgayPhatSinh.Date),
                DbHelper.Param("@GhiChu", cp.GhiChu));
        }

        public static void Xoa(int maPhatSinh)
        {
            DbHelper.ExecuteNonQuery("sp_ChiPhiPhatSinh_Xoa", DbHelper.Param("@MaPhatSinh", maPhatSinh));
        }
    }
}
