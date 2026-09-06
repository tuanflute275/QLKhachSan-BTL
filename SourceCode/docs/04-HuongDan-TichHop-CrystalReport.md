# 04 — Hướng dẫn tích hợp Crystal Report thật

## Vì sao có tài liệu này

Đề bài (mục 3.2) yêu cầu "Các báo cáo viết trên Crystal Report và được gọi qua chương trình". Crystal
Reports Designer là một **công cụ GUI tích hợp vào Visual Studio** (kéo-thả field lên khung thiết kế
`.rpt`) — không có cách nào tạo ra file `.rpt` hợp lệ chỉ bằng cách ghi text/code, và SAP Crystal
Reports for VS cũng không được cài trong môi trường build này để thử nghiệm. Vì vậy:

- Phần **in hóa đơn** và **báo cáo doanh thu** trong bản nộp hiện tại dùng `System.Drawing.Printing`
  (`PrintDocument` + `PrintPreviewDialog`) — xem [Reports/InvoicePrintDocument.cs](../QLKhachSan/QLKhachSan/Reports/InvoicePrintDocument.cs)
  và [Reports/RevenueReportPrintDocument.cs](../QLKhachSan/QLKhachSan/Reports/RevenueReportPrintDocument.cs).
  Đây là bản **chạy được ngay, không cần cài Crystal Reports**, và đã lấy đúng dữ liệu qua các
  Stored Procedure (`HoaDonDAL`, `BaoCaoDAL`) — chỉ khác phần trình bày cuối cùng.
- Tài liệu này hướng dẫn bạn thay thế 2 chỗ đó bằng Crystal Report thật, trên máy đã cài
  **SAP Crystal Reports, Developer for Visual Studio** (xem `HuongDanCaiDat.docx` mục 2.6).

## Bước 1 — Cài đặt (nếu chưa có)

Theo `HuongDanCaiDat.docx`: đóng hết Visual Studio → cài SAP Crystal Reports for VS → cài thêm
**SAP Crystal Reports Runtime Engine (64-bit)** → mở lại VS, kiểm tra `Add New Item` có mục
"Crystal Report".

## Bước 2 — Thêm project reference

Sau khi cài, trong `QLKhachSan.csproj` sẽ có sẵn các reference của Crystal Reports (VS tự thêm khi bạn
"Add New Item → Crystal Report"). Không cần tự tay sửa `.csproj` cho bước này — cứ để Visual Studio làm.

## Bước 3 — Tạo report hóa đơn (`rptHoaDon.rpt`)

1. Chuột phải project → **Add → New Item → Reporting → Crystal Report** → đặt tên `rptHoaDon.rpt`,
   chọn "As a Blank Report".
2. Report cần các trường: `MaHoadon, NgayLapHoadon, TenKhach, CMND, SoDienThoai, SoPhong,
   TenLoaiPhong, DonGia, NgayNhanThucTe, NgayTraThucTe, TenNhanVien, SoNgayO, TienPhong,
   TienDichVu, TienPhatSinh, TongTien, HinhThucThanhToan` — đúng các cột trả về từ
   `sp_Hoadon_ChiTiet` (xem `HoaDonDAL.ChiTiet`).
3. Cách nạp field nhanh nhất: **Add New Item → DataSet (.xsd)** đặt tên `dsHoaDon.xsd`, thêm 1
   DataTable tay với đúng các cột trên (khớp tên/kiểu dữ liệu với bảng ở bước 2). Khi thiết kế
   `rptHoaDon.rpt`, chọn Database Expert → Project Data → ADO.NET DataSets → `dsHoaDon` → kéo field
   vào khung thiết kế như bình thường.
4. Thêm 1 phần "Chi tiết dịch vụ" và "Chi phí phát sinh" dạng subreport hoặc thêm luôn 2 DataTable
   nữa vào `dsHoaDon.xsd` (khớp cột trả về từ `HoaDonChiTietDAL.DanhSachTheoDangky` và
   `ChiPhiPhatSinhDAL.DanhSachTheoDangky`) rồi dùng Subreport trỏ tới các bảng đó.

## Bước 4 — Form hiển thị report

Thêm 1 Form mới `frmXemHoaDonCrystal` (WinForms), kéo control **CrystalReportViewer** vào từ Toolbox.
Code gọi report (thay cho `InvoicePrintDocument.XemTruocHoaDon`):

```csharp
using CrystalDecisions.CrystalReports.Engine;
using QLKhachSan.DataAccess;

var report = new rptHoaDon(); // class được sinh ra từ rptHoaDon.rpt

var dsHoaDon = new dsHoaDon();
var hd = HoaDonDAL.ChiTiet(maHoadon);
dsHoaDon.HoaDon.Rows.Add(/* map các cột từ hd vào đúng thứ tự cột trong dsHoaDon.HoaDon */);
// tương tự nạp dsHoaDon.ChiTietDichVu và dsHoaDon.ChiPhiPhatSinh
// từ HoaDonChiTietDAL.DanhSachTheoDangky(maDangky) và ChiPhiPhatSinhDAL.DanhSachTheoDangky(maDangky)

report.SetDataSource(dsHoaDon);
crystalReportViewer1.ReportSource = report;
```

Gọi `new frmXemHoaDonCrystal(maHoadon).ShowDialog()` thay cho dòng
`InvoicePrintDocument.XemTruocHoaDon(maHoadon)` trong `frmTraPhong.cs` và `frmTraCuuHoaDon.cs`.

## Bước 5 — Report doanh thu (`rptDoanhThuDichVu.rpt`)

Làm tương tự với dữ liệu từ `BaoCaoDAL.DoanhThuDichVu`, `BaoCaoDAL.DoanhThuPhong`,
`BaoCaoDAL.TongHop` (thay cho `RevenueReportPrintDocument.XemTruoc`).

## Bước 6 — Đóng gói khi nộp bài

Máy chấm bài có thể **không cài Crystal Reports** → bắt buộc kèm theo bộ cài
**SAP Crystal Reports Runtime Engine (64-bit)** trong file nộp, hoặc dùng Visual Studio Installer
Projects để tạo file cài đặt tự động kèm runtime (mục "Đóng gói sản phẩm" trong
[03-KeHoach-Code.md](./03-KeHoach-Code.md)).

## Vì sao giữ lại cả 2 cách

Ngay cả sau khi tích hợp Crystal Report thật, nên **giữ lại** `InvoicePrintDocument` /
`RevenueReportPrintDocument` làm phương án dự phòng: nếu máy chấm/máy demo không cài được Crystal
Reports Runtime kịp lúc, bạn vẫn có bản in hoạt động để không bị mất điểm chức năng.
