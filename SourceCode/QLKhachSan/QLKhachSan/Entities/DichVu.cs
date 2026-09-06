namespace QLKhachSan.Entities
{
    public class DichVu
    {
        public int MaDichvu { get; set; }
        public string TenDichvu { get; set; }
        public decimal DonGia { get; set; }
        public string DonViTinh { get; set; }
        public bool TrangThai { get; set; } = true;
        public string GhiChu { get; set; }
    }
}
