using System;

namespace QLKhachSan.Entities
{
    public class ChiPhiPhatSinh
    {
        public int MaPhatSinh { get; set; }
        public int MaDangky { get; set; }
        public string LoaiPhi { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayPhatSinh { get; set; }
        public string GhiChu { get; set; }
    }
}
