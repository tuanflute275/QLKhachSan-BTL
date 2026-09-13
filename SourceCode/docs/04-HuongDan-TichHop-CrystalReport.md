# 04 — Hướng dẫn tích hợp Crystal Report thật

## Hiện trạng

Máy này **đã cài** SAP Crystal Reports for Visual Studio (Crystal Reports 2011 runtime, "Crystal
Reports for .NET Framework 4.0"), và toàn bộ phần code phía C# đã được chuẩn bị sẵn:

- Project đã reference `CrystalDecisions.CrystalReports.Engine`, `CrystalDecisions.Shared`,
  `CrystalDecisions.Windows.Forms` (xem `QLKhachSan.csproj`), lấy DLL từ `SourceCode/lib/CrystalReports/`
  (copy sẵn trong repo để build không phụ thuộc đường dẫn cài đặt Crystal Reports trên từng máy).
- [Reports/CrystalData/dsHoaDon.cs](../QLKhachSan/QLKhachSan/Reports/CrystalData/dsHoaDon.cs) và
  [Reports/CrystalData/dsDoanhThu.cs](../QLKhachSan/QLKhachSan/Reports/CrystalData/dsDoanhThu.cs) là 2
  `DataSet` viết tay (không cần .xsd) — Crystal Reports Designer nhìn thấy chúng dưới
  **Database Expert → Project Data → ADO.NET Datasets** ngay sau khi project build xong.
- [Reports/CrystalInvoiceViewer.cs](../QLKhachSan/QLKhachSan/Reports/CrystalInvoiceViewer.cs) và
  [Reports/CrystalRevenueViewer.cs](../QLKhachSan/QLKhachSan/Reports/CrystalRevenueViewer.cs) tự
  tìm report class bằng reflection (`CrystalReportLoader.TryCreate("rptHoaDon")` /
  `"rptDoanhThu"`), nạp dữ liệu qua `HoaDonDAL`/`HoaDonChiTietDAL`/`ChiPhiPhatSinhDAL`/`BaoCaoDAL`,
  rồi hiển thị bằng `CrystalReportViewer`. **Nếu chưa tìm thấy report class (vì .rpt chưa được vẽ),
  tự động rơi về `InvoicePrintDocument`/`RevenueReportPrintDocument` (PrintDocument)** — app luôn
  chạy được, không bao giờ crash vì thiếu report.
- `frmTraPhong.cs`, `frmTraCuuHoaDon.cs`, `frmBaoCaoDoanhThu.cs` đã gọi `CrystalInvoiceViewer` /
  `CrystalRevenueViewer` thay vì gọi thẳng PrintDocument.

**Việc còn lại — chỉ làm được bằng thao tác kéo-thả trong Visual Studio Report Designer** (đây là một
canvas vẽ nhị phân độc quyền, không có cách nào tạo ra bằng text/code một cách đáng tin cậy): vẽ 2 file
`rptHoaDon.rpt` và `rptDoanhThu.rpt`.

## Bước 1 — Report hóa đơn (`rptHoaDon.rpt`)

1. Chuột phải project **QLKhachSan** → **Add → New Item… → Reporting → Crystal Report**.
2. Đặt tên chính xác **`rptHoaDon.rpt`** (tên class sinh ra phải là `rptHoaDon` — đây là tên mà
   `CrystalReportLoader` tìm bằng reflection). Chọn **"As a Blank Report"** → OK.
3. Cửa sổ Database Expert hiện ra → **Project Data → ADO.NET Datasets → dsHoaDon** → bấm mũi tên để
   thêm cả 3 bảng (`HoaDon`, `ChiTietDichVu`, `ChiPhiPhatSinh`) → Finish.
   - Nếu không thấy `dsHoaDon` trong danh sách: Build project trước (Ctrl+Shift+B), Crystal Reports
     chỉ nhận diện được DataSet sau khi assembly đã build.
4. Trong Field Explorer, kéo các field của bảng **HoaDon** vào Page Header / Details:
   `TenKhach, CMND, SoDienThoai, SoPhong, TenLoaiPhong, DonGia, NgayNhanThucTe, NgayTraThucTe,
   TenNhanVien, NgayLapHoadon, SoNgayO, TienPhong, TienDichVu, TienPhatSinh, TongTien,
   HinhThucThanhToan, MaHoadon`.
5. Thêm 1 khu vực danh sách dịch vụ: cách đơn giản nhất là insert một Subreport trỏ tới bảng
   `ChiTietDichVu` (Insert → Subreport, hoặc thêm 1 Group theo `MaDangky` ngay trên report chính vì
   dataset chỉ chứa đúng 1 hóa đơn mỗi lần in). Field cần: `TenDichvu, NgaySuDung, SoLuong, DonGia,
   ThanhTien`.
6. Tương tự cho `ChiPhiPhatSinh`: `LoaiPhi, SoTien, NgayPhatSinh`.
7. Save. Nhấn F5/Start hoặc mở lại form Trả phòng / Tra cứu hóa đơn để test — `CrystalInvoiceViewer`
   sẽ tự động dùng report này thay vì bản PrintDocument ngay khi tìm thấy class `rptHoaDon`.

## Bước 2 — Report doanh thu (`rptDoanhThu.rpt`)

1. Add New Item → Crystal Report → đặt tên chính xác **`rptDoanhThu.rpt`** → Blank Report.
2. Database Expert → Project Data → ADO.NET Datasets → **dsDoanhThu** → thêm cả 3 bảng (`TongHop`,
   `DoanhThuDichVu`, `DoanhThuPhong`).
3. Report chính đặt trên bảng `TongHop` (luôn có đúng 1 dòng): hiển thị `TuNgay, DenNgay,
   SoLuongHoaDon, TongTienPhong, TongTienDichVu, TongTienPhatSinh, TongDoanhThu` ở phần đầu.
4. Insert 2 Subreport: một trỏ tới `DoanhThuDichVu` (field `TenDichvu, TongSoLuong, TongDoanhThu`),
   một trỏ tới `DoanhThuPhong` (field `SoPhong, TenLoaiPhong, SoLuotThue, TongSoDem,
   TongDoanhThuPhong`).
5. Save. Mở form Báo cáo doanh thu → nút "In báo cáo" sẽ tự dùng report này.

## Đóng gói khi nộp bài

Máy chấm bài có thể không cài Crystal Reports Runtime → bắt buộc kèm theo bộ cài **SAP Crystal
Reports Runtime Engine (64-bit)** trong file nộp (mục "Đóng gói sản phẩm" trong
[03-KeHoach-Code.md](./03-KeHoach-Code.md)). Vì `CrystalInvoiceViewer`/`CrystalRevenueViewer` tự rơi
về PrintDocument khi thiếu report/runtime, app vẫn chạy được ngay cả khi máy chấm thiếu runtime —
chỉ mất phần trình bày Crystal Report thật.
