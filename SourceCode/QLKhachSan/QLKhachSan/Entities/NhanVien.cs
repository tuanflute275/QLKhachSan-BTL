using System;

namespace QLKhachSan.Entities
{
    public class NhanVien
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; } = true;
        public string CMND { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string ChucVu { get; set; } = "Lễ tân";
        public DateTime NgayVaoLam { get; set; }
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; }
    }
}
