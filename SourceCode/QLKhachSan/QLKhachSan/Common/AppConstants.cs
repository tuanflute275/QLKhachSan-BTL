namespace QLKhachSan.Common
{
    /// <summary>Mã trạng thái phòng - phải khớp CHECK constraint CK_tblPhong_TrangThai trong CSDL.</summary>
    public static class TrangThaiPhong
    {
        public const string Trong = "Trong";
        public const string DaDat = "DaDat";
        public const string DangSD = "DangSD";
        public const string BaoTri = "BaoTri";

        public static readonly string[] TatCa = { Trong, DaDat, DangSD, BaoTri };

        public static string HienThi(string ma)
        {
            switch (ma)
            {
                case Trong: return "Trống";
                case DaDat: return "Đã đặt";
                case DangSD: return "Đang sử dụng";
                case BaoTri: return "Bảo trì";
                default: return ma;
            }
        }
    }

    /// <summary>Mã trạng thái đăng ký - phải khớp CHECK constraint CK_tblDangky_TrangThai trong CSDL.</summary>
    public static class TrangThaiDangKy
    {
        public const string DaDat = "DaDat";
        public const string DangO = "DangO";
        public const string DaTra = "DaTra";
        public const string DaHuy = "DaHuy";

        public static string HienThi(string ma)
        {
            switch (ma)
            {
                case DaDat: return "Đã đặt";
                case DangO: return "Đang ở";
                case DaTra: return "Đã trả phòng";
                case DaHuy: return "Đã hủy";
                default: return ma;
            }
        }
    }

    /// <summary>Mã hình thức thanh toán - phải khớp CHECK constraint CK_tblHoadon_HinhThucThanhToan.</summary>
    public static class HinhThucThanhToan
    {
        public const string TienMat = "TienMat";
        public const string ChuyenKhoan = "ChuyenKhoan";
        public const string The = "The";

        public static readonly string[] TatCa = { TienMat, ChuyenKhoan, The };

        public static string HienThi(string ma)
        {
            switch (ma)
            {
                case TienMat: return "Tiền mặt";
                case ChuyenKhoan: return "Chuyển khoản";
                case The: return "Thẻ";
                default: return ma;
            }
        }
    }
}
