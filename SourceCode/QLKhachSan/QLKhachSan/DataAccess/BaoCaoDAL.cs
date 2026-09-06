using System;
using System.Data;
using QLKhachSan.Common;

namespace QLKhachSan.DataAccess
{
    public static class BaoCaoDAL
    {
        public static DataTable DoanhThuDichVu(DateTime tuNgay, DateTime denNgay)
        {
            return DbHelper.ExecuteDataTable("sp_BaoCao_DoanhThuDichVu",
                DbHelper.Param("@TuNgay", tuNgay.Date),
                DbHelper.Param("@DenNgay", denNgay.Date));
        }

        public static DataTable DoanhThuPhong(DateTime tuNgay, DateTime denNgay)
        {
            return DbHelper.ExecuteDataTable("sp_BaoCao_DoanhThuPhong",
                DbHelper.Param("@TuNgay", tuNgay.Date),
                DbHelper.Param("@DenNgay", denNgay.Date));
        }

        public static DataRow TongHop(DateTime tuNgay, DateTime denNgay)
        {
            return DbHelper.ExecuteDataRow("sp_BaoCao_TongHop",
                DbHelper.Param("@TuNgay", tuNgay.Date),
                DbHelper.Param("@DenNgay", denNgay.Date));
        }
    }
}
