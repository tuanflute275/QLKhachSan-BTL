namespace QLKhachSan.Common
{
    /// <summary>Thông tin tài khoản đang đăng nhập, dùng chung cho toàn bộ ứng dụng (đăng xuất, phân quyền,
    /// tự chọn sẵn "nhân viên lập" trong các form nghiệp vụ theo người đang đăng nhập).</summary>
    public static class CurrentUser
    {
        public static int MaTK { get; private set; }
        public static string TenDangNhap { get; private set; }
        public static int MaNV { get; private set; }
        public static string HoTen { get; private set; }
        public static string Quyen { get; private set; }

        public static bool DaDangNhap => TenDangNhap != null;
        public static bool IsAdmin => Quyen == "Admin";

        public static void DangNhap(int maTK, string tenDangNhap, int maNV, string hoTen, string quyen)
        {
            MaTK = maTK;
            TenDangNhap = tenDangNhap;
            MaNV = maNV;
            HoTen = hoTen;
            Quyen = quyen;
        }

        public static void DangXuat()
        {
            MaTK = 0;
            TenDangNhap = null;
            MaNV = 0;
            HoTen = null;
            Quyen = null;
        }
    }
}
