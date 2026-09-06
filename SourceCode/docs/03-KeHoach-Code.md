# 03 — Kế hoạch code ứng dụng (C#.NET WinForms)

> **Cập nhật:** phần code đã được triển khai đầy đủ theo kế hoạch dưới đây (xem
> [`../QLKhachSan/`](../QLKhachSan/)). Giữ nguyên tài liệu này làm hồ sơ kiến trúc cho quyển báo cáo BTL.
> Sai khác duy nhất so với kế hoạch ban đầu: dùng **.NET Framework 4.7.2** (không phải .NET 8) vì đây
> là target mặc định khi tạo project Windows Forms App có tích hợp Crystal Reports trong Visual Studio
> 2022, và phần báo cáo dùng `System.Drawing.Printing` làm phương án chạy-được-ngay thay vì Crystal
> Report thật (xem [04-HuongDan-TichHop-CrystalReport.md](./04-HuongDan-TichHop-CrystalReport.md)).

## 1. Lựa chọn công nghệ

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Ngôn ngữ / UI | C#, Windows Forms App (**.NET Framework 4.7.2**) | Đề bài yêu cầu VB.NET/C#.NET + menu/toolbar; nhóm chọn C#; 4.7.2 là target mặc định tương thích Crystal Reports for VS |
| Truy cập dữ liệu | ADO.NET thuần (`SqlConnection`/`SqlCommand`), **không dùng Entity Framework** | Đề bài yêu cầu thao tác ghi phải qua Stored Procedure — ADO.NET gọi SP trực tiếp, kiểm soát rõ ràng, đúng tinh thần đề bài |
| Báo cáo | `System.Drawing.Printing` (chạy ngay, không cần cài thêm gì) + hướng dẫn thay bằng Crystal Reports thật khi cần | Đề bài yêu cầu Crystal Report (3.2); Crystal Report Designer là công cụ GUI trong VS, xem hướng dẫn tích hợp tại tài liệu 04 |
| Cấu trúc project | 1 solution — 1 project Windows Forms App duy nhất, chia theo **folder** (không tách nhiều class library) | 4 người, 10 ngày — tách nhiều project làm tăng chi phí ghép nối; 1 project + quy ước thư mục rõ ràng dễ merge tay hơn |

## 2. Cấu trúc thư mục project

```
QLKhachSan/
├─ QLKhachSan.sln
└─ QLKhachSan/                      (Windows Forms App, .NET Framework 4.7.2)
   ├─ Common/
   │  ├─ DbHelper.cs                 // wrapper SqlConnection dùng chung, đọc connection string từ App.config
   │  ├─ AppConstants.cs             // hằng số trạng thái: "DaDat","DangO","DaTra","DaHuy","Trong","DaDat","DangSD","BaoTri"
   │  └─ Validator.cs                // hàm kiểm tra input dùng chung (CMND, SĐT, ngày...)
   ├─ Entities/                      // POCO map 1-1 với bảng, KHÔNG chứa logic truy vấn
   │  ├─ KhachHang.cs
   │  ├─ NhanVien.cs
   │  ├─ LoaiPhong.cs
   │  ├─ Phong.cs
   │  ├─ DichVu.cs
   │  ├─ DangKy.cs
   │  ├─ HoaDonChiTiet.cs
   │  ├─ ChiPhiPhatSinh.cs
   │  └─ HoaDon.cs
   ├─ DataAccess/                    // mỗi entity 1 class, các method gọi thẳng Stored Procedure tương ứng ở tài liệu 02
   │  ├─ KhachHangDAL.cs             // GetAll(), TimKiem(), Them(), Sua(), Xoa()
   │  ├─ NhanVienDAL.cs
   │  ├─ PhongDAL.cs                 // + DanhSachPhongTrong(ngayNhan, ngayTra)
   │  ├─ DichVuDAL.cs
   │  ├─ DangKyDAL.cs                // + NhanPhong(), TraPhong_LapHoaDon()
   │  ├─ HoaDonChiTietDAL.cs
   │  └─ ChiPhiPhatSinhDAL.cs
   ├─ Forms/
   │  ├─ MainForm.cs                 // MDI parent: MenuStrip + ToolStrip, mở các form con
   │  ├─ KhachHang/frmKhachHang.cs
   │  ├─ NhanVien/frmNhanVien.cs
   │  ├─ Phong/frmLoaiPhong.cs
   │  ├─ Phong/frmPhong.cs
   │  ├─ DichVu/frmDichVu.cs
   │  ├─ DangKy/frmDangKy.cs         // form đặt phòng, chỉ hiện phòng trống trong combobox
   │  ├─ DangKy/frmSuDungDichVu.cs   // nhập dịch vụ theo ngày cho 1 phiếu đăng ký
   │  ├─ TraPhong/frmTraPhong.cs     // check-out + tính tiền + xem trước hóa đơn
   │  ├─ HoaDon/frmTraCuuHoaDon.cs   // tra cứu / in lại hóa đơn đã lập
   │  └─ BaoCao/frmBaoCaoDoanhThu.cs
   ├─ Reports/
   │  ├─ InvoicePrintDocument.cs     // in hóa đơn qua System.Drawing.Printing (chạy ngay, không cần Crystal Report)
   │  └─ RevenueReportPrintDocument.cs
   └─ App.config                    // connection string
```

## 3. Ánh xạ module ↔ người phụ trách (khớp `PhanCongNhiemVu.docx`)

| Người | Thư mục/Form phụ trách | Phụ thuộc vào |
|---|---|---|
| **Nguyễn Văn Tuấn (bạn)** | `Common/`, `Entities/` (khung sườn), toàn bộ Stored Procedure, `DangKyDAL.NhanPhong/TraPhong_LapHoaDon`, thiết kế + tích hợp Crystal Report, `MainForm`, tích hợp cuối cùng, đóng gói | — (làm trước, người khác phụ thuộc vào) |
| Trần Quốc Trung | `frmKhachHang`, `frmDangKy`, `KhachHangDAL`, `DangKyDAL` (phần CRUD cơ bản) | Cần `DbHelper` + Entities + SP tương ứng xong trước |
| Đậu Phi Tuấn | `frmLoaiPhong`, `frmPhong`, `frmDichVu`, `frmSuDungDichVu`, `PhongDAL`, `DichVuDAL`, `HoaDonChiTietDAL` | Cần `DbHelper` + Entities + SP tương ứng xong trước |
| Trần Anh Tú | `frmNhanVien`, `frmTraPhong`, `NhanVienDAL`, `ChiPhiPhatSinhDAL` | Cần `DbHelper` + Entities + SP tương ứng xong trước |

**Điểm găng (critical path)**: `Common/DbHelper.cs` + toàn bộ `Entities/` + các Stored Procedure phải xong **trước 09/09** để 3 thành viên còn lại code song song 09/09–12/09 như lịch GĐ4. Đây là lý do bạn (Tuấn) cần làm CSDL + kiến trúc trước tiên — đúng như bạn đang làm ở bước này.

## 4. Quy ước code chung (để 4 người ghép nối không xung đột)

- Tên form: tiền tố `frm` (VD `frmKhachHang`). Tên class DAL: hậu tố `DAL` (VD `KhachHangDAL`).
- Tên property trong Entity **trùng chính xác** tên cột CSDL (VD `Entities.KhachHang.HoTen` ↔ cột `HoTen`) — tránh nhầm lẫn khi map tham số SP.
- Tên tham số SqlParameter **trùng tên cột**, có tiền tố `@` (VD `@HoTen`) — copy-paste giữa DAL và SP không cần đổi tên.
- Mọi thao tác ghi dữ liệu (thêm/sửa/xóa) **bắt buộc gọi Stored Procedure** qua `DbHelper.ExecuteNonQuery(spName, params)` — không viết chuỗi SQL nối tay trong C# (tránh SQL Injection, đúng yêu cầu đề bài).
- Đọc dữ liệu (list/tìm kiếm) cũng ưu tiên qua SP + `DbHelper.ExecuteReader`/`ExecuteDataTable`, trả về `DataTable` để bind thẳng vào `DataGridView`.
- Toàn bộ ngày giờ nhập từ UI dùng `DateTimePicker`, không cho gõ tay chuỗi ngày tự do (giảm lỗi input theo yêu cầu "kiểm tra chặt chẽ dữ liệu" của đề bài).

## 5. Chuẩn thao tác bàn phím (đề bài yêu cầu hạn chế dùng chuột)

Áp dụng đồng nhất cho **mọi form CRUD** để tăng tốc độ nhập liệu:

| Phím | Hành động |
|---|---|
| `Enter` | Di chuyển sang control kế tiếp trong form (xử lý qua `KeyDown` chung hoặc `KeyPreview` ở form) |
| `F2` | Thêm mới |
| `F3` | Sửa |
| `Delete` / `F4` | Xóa (có xác nhận `MessageBox`) |
| `F5` | Làm mới danh sách |
| `Esc` | Hủy thao tác đang nhập / đóng form con |
| `Ctrl+F` | Focus vào ô tìm kiếm |

Gợi ý triển khai: viết 1 class `BaseForm` hoặc `KeyboardHelper` dùng chung trong `Common/`, xử lý phím tắt tập trung thay vì lặp code ở từng form.

## 6. Kiểm tra dữ liệu đầu vào (validation)

- CMND/CCCD: đúng độ dài (9 hoặc 12 số), chỉ chứa chữ số — validate ở C# **trước** khi gọi SP (UX tốt hơn), CSDL vẫn giữ `CHECK` constraint làm lớp bảo vệ cuối.
- Ngày trả dự kiến > ngày nhận dự kiến — validate ở form trước khi enable nút Lưu.
- Số lượng dịch vụ, đơn giá > 0.
- Dùng `ErrorProvider` để hiển thị lỗi ngay tại control, không chỉ `MessageBox`.

## 7. Lộ trình theo tiến độ (đối chiếu `PhanCongNhiemVu.docx`)

| Giai đoạn | Ngày | Việc chính (liên quan code) | Trạng thái |
|---|---|---|---|
| GĐ2 | 07/09–08/09 | Chốt thiết kế CSDL (tài liệu 02 này) | ✅ Xong — đã tạo bảng + test trên SQL Server thật |
| GĐ3 | 09/09 | Viết Stored Procedure (mục 5, tài liệu 02) | ✅ Xong — 42 SP, đã test qua `sqlcmd` (kể cả rollback) |
| GĐ4 | 09/09–12/09 | Code song song 3 module UI + `DangKyDAL` nghiệp vụ tính tiền | ✅ Xong — toàn bộ 10 form + DAL/Entities |
| GĐ5 | 12/09–13/09 | Ghép nối module, tích hợp Crystal Report | ⚠️ Ghép nối xong; Crystal Report thật **chưa** tích hợp (xem tài liệu 04) — báo cáo/hóa đơn đang chạy bằng `PrintDocument` |
| GĐ6 | 14/09 | Kiểm thử toàn bộ, lập & sửa danh sách lỗi | ⏳ Đã build sạch 0 lỗi/0 warning + test SP; **chưa** kiểm thử toàn bộ UI thủ công trên Visual Studio thật |
| GĐ7 | 14/09–15/09 | Viết báo cáo BTL (dùng lại nội dung 3 file docs/ này làm gốc) | ❌ Chưa làm — cần nhóm tự viết theo `QuidinhTrinhbayBaocao_v4_5_2_1.pdf` |
| GĐ8 | 16/09 | Build release, đóng gói kèm Crystal Report Runtime, nén ZIP | ❌ Chưa làm |

## 8. Ghi chú đóng gói (GĐ8)

- Build cấu hình `Release`.
- Kèm theo **SAP Crystal Reports Runtime Engine (64-bit)** installer trong gói nộp (máy chấm có thể không cài Crystal Report) — đã ghi rõ trong `HuongDanCaiDat.docx` mục 5.
- File ZIP nộp gồm: mã nguồn (`.sln` + toàn bộ project) + file backup CSDL (`.bak` hoặc script `.sql` trong `docs/`, `sql/`) + báo cáo PDF — đúng mục 4 đề bài.

## 9. Việc còn thiếu trước khi nộp bài

- **Kiểm thử thủ công toàn bộ UI** trên máy có Visual Studio + SQL Server thật (F5 chạy thử từng form, thử các luồng lỗi: nhập sai định dạng, hủy giữa chừng...). Đã test kỹ tầng Stored Procedure qua `sqlcmd`, nhưng chưa có ai bấm thử giao diện thật.
- **Crystal Report thật** — hiện dùng `PrintDocument` chạy được ngay; muốn đúng 100% yêu cầu đề bài cần làm theo [04-HuongDan-TichHop-CrystalReport.md](./04-HuongDan-TichHop-CrystalReport.md) trên máy đã cài Crystal Reports for VS.
- **Quyển báo cáo BTL (PDF)** theo `QuidinhTrinhbayBaocao_v4_5_2_1.pdf` — chưa viết; có thể dùng lại nội dung phân tích/thiết kế trong `docs/01`, `docs/02` làm gốc.
- **Đóng gói nộp bài** (GĐ8): build Release, kèm Crystal Report Runtime (nếu tích hợp Crystal Report thật), nén ZIP gồm mã nguồn + backup CSDL + báo cáo PDF.

*(Đã xong, không còn thiếu)* Form đăng nhập/phân quyền (`frmDangNhap`, `frmDoiMatKhau`) — tuy đề bài và
`PhanCongNhiemVu.docx` không yêu cầu, nhưng đã làm theo yêu cầu bổ sung của nhóm: đăng nhập bằng
`tblTaikhoan`, phân quyền Admin/Lễ tân (Lễ tân không vào được Quản lý nhân viên & Báo cáo doanh thu),
tự chọn sẵn "nhân viên lập" theo người đang đăng nhập trong `frmDangKy`/`frmTraPhong`. Tài khoản demo:
`admin`/`annv`/`binhlt`, mật khẩu `123456` (xem `sql/02_DuLieuMau.sql`).
