using System.Data;
using QLKhachSan.Common;

namespace QLKhachSan.DataAccess
{
    public static class TaiKhoanDAL
    {
        /// <summary>Trả về null nếu sai tên đăng nhập/mật khẩu hoặc tài khoản đã khóa.</summary>
        public static DataRow DangNhap(string tenDangNhap, string matKhau)
        {
            return DbHelper.ExecuteDataRow("sp_TaiKhoan_DangNhap",
                DbHelper.Param("@TenDangNhap", tenDangNhap),
                DbHelper.Param("@MatKhau", matKhau));
        }

        public static void DoiMatKhau(int maTK, string matKhauCu, string matKhauMoi)
        {
            DbHelper.ExecuteNonQuery("sp_TaiKhoan_DoiMatKhau",
                DbHelper.Param("@MaTK", maTK),
                DbHelper.Param("@MatKhauCu", matKhauCu),
                DbHelper.Param("@MatKhauMoi", matKhauMoi));
        }
    }
}
