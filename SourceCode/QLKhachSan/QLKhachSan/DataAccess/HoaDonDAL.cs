using System;
using System.Data;
using QLKhachSan.Common;

namespace QLKhachSan.DataAccess
{
    public static class HoaDonDAL
    {
        public static DataTable DanhSach(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            return DbHelper.ExecuteDataTable("sp_Hoadon_DanhSach",
                DbHelper.Param("@TuNgay", tuNgay?.Date),
                DbHelper.Param("@DenNgay", denNgay?.Date));
        }

        public static DataRow ChiTiet(int maHoadon)
        {
            return DbHelper.ExecuteDataRow("sp_Hoadon_ChiTiet", DbHelper.Param("@MaHoadon", maHoadon));
        }
    }
}
