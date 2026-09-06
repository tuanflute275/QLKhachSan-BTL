using System;

namespace QLKhachSan.Entities
{
    public class HoaDon
    {
        public int MaHoadon { get; set; }
        public int MaDangky { get; set; }
        public string TenKhach { get; set; }
        public string CMND { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public decimal DonGiaPhong { get; set; }
        public DateTime? NgayNhanThucTe { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public int MaNVLap { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayLapHoadon { get; set; }
        public int SoNgayO { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TienPhatSinh { get; set; }
        public decimal TongTien { get; set; }
        public string HinhThucThanhToan { get; set; }
        public bool TrangThaiThanhToan { get; set; }
    }
}
