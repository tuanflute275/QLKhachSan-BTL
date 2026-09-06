/*
    Đề 05 - Quản lý thuê phòng khách sạn
    Xóa toàn bộ bảng để reset lại CSDL từ đầu.

    Thứ tự xóa: bảng con trước, bảng cha sau (đúng chiều ngược của FK) để không bị lỗi
    "cannot drop table because it is referenced by a FOREIGN KEY constraint".

    Sau khi chạy file này, chạy lại theo thứ tự:
      1. 01_TaoDatabase_Bang.sql   (tạo lại toàn bộ bảng)
      2. 02_DuLieuMau.sql          (chèn lại dữ liệu mẫu)
    Không cần chạy lại 03_StoredProcedures.sql - các Stored Procedure vẫn còn nguyên
    (chỉ thất bại lúc EXEC nếu bảng chưa tồn tại, không bị xóa theo bảng).
*/

USE QLKhachSan;
GO

DROP TABLE IF EXISTS tblTaikhoan;
DROP TABLE IF EXISTS tblHoadon;
DROP TABLE IF EXISTS tblChiphiphatsinh;
DROP TABLE IF EXISTS tblHoadonchitiet;
DROP TABLE IF EXISTS tblDangky;
DROP TABLE IF EXISTS tblPhong;
DROP TABLE IF EXISTS tblDichvu;
DROP TABLE IF EXISTS tblNhanvien;
DROP TABLE IF EXISTS tblKhach;
DROP TABLE IF EXISTS tblLoaiPhong;
GO

PRINT N'Đã xóa toàn bộ bảng. Chạy tiếp 01_TaoDatabase_Bang.sql rồi 02_DuLieuMau.sql để reset dữ liệu.';
GO
