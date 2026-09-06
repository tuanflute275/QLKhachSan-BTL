using System;

namespace QLKhachSan.Entities
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string CMND { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; } = true;
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string QuocTich { get; set; } = "Việt Nam";
        public DateTime NgayTao { get; set; }
        public string GhiChu { get; set; }
    }
}
