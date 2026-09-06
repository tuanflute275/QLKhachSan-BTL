using System;

namespace QLKhachSan.Entities
{
    public class HoaDonChiTiet
    {
        public int MaChiTiet { get; set; }
        public int MaDangky { get; set; }
        public int MaDichvu { get; set; }
        public string TenDichvu { get; set; }
        public DateTime NgaySuDung { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
    }
}
