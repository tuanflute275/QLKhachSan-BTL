using System;

namespace QLKhachSan.Entities
{
    public class DangKy
    {
        public int MaDangky { get; set; }
        public int MaKH { get; set; }
        public string TenKhach { get; set; }
        public string SoDienThoaiKhach { get; set; }
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public decimal DonGiaPhong { get; set; }
        public int MaNVLap { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime NgayNhanDuKien { get; set; }
        public DateTime NgayTraDuKien { get; set; }
        public DateTime? NgayNhanThucTe { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public int SoKhach { get; set; } = 1;
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
