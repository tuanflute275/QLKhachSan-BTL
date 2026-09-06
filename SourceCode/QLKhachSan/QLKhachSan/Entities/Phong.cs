namespace QLKhachSan.Entities
{
    public class Phong
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }   // cột join hiển thị, không phải cột thật của tblPhong
        public int SucChua { get; set; }           // cột join hiển thị
        public decimal DonGia { get; set; }        // cột join hiển thị
        public int? Tang { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
