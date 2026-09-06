/*
    Đề 05 - Quản lý thuê phòng khách sạn
    Toàn bộ Stored Procedure dùng cho thêm/sửa/xóa/truy vấn (theo yêu cầu 3.1 đề bài)
    Chạy sau 01_TaoDatabase_Bang.sql (và 02_DuLieuMau.sql nếu muốn có dữ liệu mẫu).

    Quy ước:
      - Toàn bộ SP dùng CREATE OR ALTER để chạy lại nhiều lần không lỗi khi chỉnh sửa.
      - SP thêm mới luôn có tham số OUTPUT trả về khóa chính vừa sinh (SCOPE_IDENTITY()).
      - Lỗi nghiệp vụ dùng THROW với mã lỗi > 50000 kèm thông điệp tiếng Việt,
        tầng C# (DbHelper) sẽ bắt SqlException và hiển thị Message cho người dùng.
      - tblPhong.TrangThai chỉ là "ảnh chụp nhanh" trạng thái hiện tại (Trong/DaDat/DangSD/BaoTri)
        để hiển thị danh sách phòng cho nhanh. Câu hỏi "phòng có trống trong khoảng ngày X-Y hay
        không" LUÔN được xác định bằng cách so sánh khoảng ngày với tblDangky
        (sp_Phong_KiemTraTrong / sp_Phong_DanhSachTrong), không dựa vào cột TrangThai.
*/

USE QLKhachSan;
GO

-- tblHoadon.TongTien và tblHoadonchitiet.ThanhTien là computed column PERSISTED: SQL Server
-- yêu cầu ANSI_NULLS/QUOTED_IDENTIFIER phải là ON tại thời điểm TẠO procedure (giá trị này được
-- "đóng băng" cùng procedure và dùng lại mỗi lần gọi, bất kể session gọi có bật hay không).
-- Thiếu 2 dòng này sẽ gây lỗi "INSERT failed because the following SET options have incorrect
-- settings: 'QUOTED_IDENTIFIER'" khi sp_Dangky_TraPhong_LapHoaDon / sp_HoaDonChiTiet_Them chạy.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =====================================================================================
-- A. tblLoaiPhong
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_LoaiPhong_Them
    @TenLoaiPhong   NVARCHAR(50),
    @SucChua        INT,
    @DonGia         DECIMAL(18,2),
    @MoTa           NVARCHAR(255) = NULL,
    @MaLoaiPhong    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblLoaiPhong (TenLoaiPhong, SucChua, DonGia, MoTa)
    VALUES (@TenLoaiPhong, @SucChua, @DonGia, @MoTa);
    SET @MaLoaiPhong = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPhong_Sua
    @MaLoaiPhong    INT,
    @TenLoaiPhong   NVARCHAR(50),
    @SucChua        INT,
    @DonGia         DECIMAL(18,2),
    @MoTa           NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tblLoaiPhong
       SET TenLoaiPhong = @TenLoaiPhong, SucChua = @SucChua, DonGia = @DonGia, MoTa = @MoTa
     WHERE MaLoaiPhong = @MaLoaiPhong;
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPhong_Xoa
    @MaLoaiPhong INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblPhong WHERE MaLoaiPhong = @MaLoaiPhong)
        THROW 51010, N'Không thể xóa: vẫn còn phòng thuộc loại phòng này.', 1;

    DELETE FROM tblLoaiPhong WHERE MaLoaiPhong = @MaLoaiPhong;
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPhong_DanhSach
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaLoaiPhong, TenLoaiPhong, SucChua, DonGia, MoTa
      FROM tblLoaiPhong
     ORDER BY TenLoaiPhong;
END
GO

-- =====================================================================================
-- B. tblPhong
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_Phong_Them
    @SoPhong        VARCHAR(10),
    @MaLoaiPhong    INT,
    @Tang           INT = NULL,
    @TrangThai      VARCHAR(20) = 'Trong',
    @GhiChu         NVARCHAR(255) = NULL,
    @MaPhong        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblPhong (SoPhong, MaLoaiPhong, Tang, TrangThai, GhiChu)
    VALUES (@SoPhong, @MaLoaiPhong, @Tang, @TrangThai, @GhiChu);
    SET @MaPhong = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_Phong_Sua
    @MaPhong        INT,
    @SoPhong        VARCHAR(10),
    @MaLoaiPhong    INT,
    @Tang           INT = NULL,
    @TrangThai      VARCHAR(20),
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tblPhong
       SET SoPhong = @SoPhong, MaLoaiPhong = @MaLoaiPhong, Tang = @Tang,
           TrangThai = @TrangThai, GhiChu = @GhiChu
     WHERE MaPhong = @MaPhong;
END
GO

CREATE OR ALTER PROCEDURE sp_Phong_Xoa
    @MaPhong INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblDangky WHERE MaPhong = @MaPhong)
        THROW 51011, N'Không thể xóa: phòng đã từng có đăng ký đặt phòng.', 1;

    DELETE FROM tblPhong WHERE MaPhong = @MaPhong;
END
GO

CREATE OR ALTER PROCEDURE sp_Phong_DanhSach
    @TuKhoa VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, lp.TenLoaiPhong, lp.SucChua, lp.DonGia,
           p.Tang, p.TrangThai, p.GhiChu
      FROM tblPhong p
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
     WHERE (@TuKhoa IS NULL OR p.SoPhong LIKE '%' + @TuKhoa + '%')
     ORDER BY p.SoPhong;
END
GO

-- Danh sách phòng còn trống trong khoảng [@NgayNhan, @NgayTra) - nguồn xác thực duy nhất
-- cho câu hỏi "phòng nào đang trống", KHÔNG dựa vào cột tblPhong.TrangThai.
CREATE OR ALTER PROCEDURE sp_Phong_DanhSachTrong
    @NgayNhan           DATE,
    @NgayTra            DATE,
    @MaLoaiPhong        INT = NULL,
    @MaDangkyBoQua      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, lp.TenLoaiPhong, lp.SucChua, lp.DonGia, p.Tang
      FROM tblPhong p
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
     WHERE p.TrangThai <> 'BaoTri'
       AND (@MaLoaiPhong IS NULL OR p.MaLoaiPhong = @MaLoaiPhong)
       AND NOT EXISTS (
            SELECT 1 FROM tblDangky d
             WHERE d.MaPhong = p.MaPhong
               AND d.TrangThai IN ('DaDat','DangO')
               AND (@MaDangkyBoQua IS NULL OR d.MaDangky <> @MaDangkyBoQua)
               AND NOT (@NgayTra <= d.NgayNhanDuKien OR @NgayNhan >= d.NgayTraDuKien)
       )
     ORDER BY p.SoPhong;
END
GO

-- Kiểm tra nhanh 1 phòng cụ thể có trống trong khoảng ngày hay không (dùng khi sửa đăng ký,
-- @MaDangkyBoQua để loại trừ chính đăng ký đang sửa ra khỏi phép so sánh overlap).
CREATE OR ALTER PROCEDURE sp_Phong_KiemTraTrong
    @MaPhong            INT,
    @NgayNhan           DATE,
    @NgayTra            DATE,
    @MaDangkyBoQua      INT = NULL,
    @KetQua             BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM tblDangky d
         WHERE d.MaPhong = @MaPhong
           AND d.TrangThai IN ('DaDat','DangO')
           AND (@MaDangkyBoQua IS NULL OR d.MaDangky <> @MaDangkyBoQua)
           AND NOT (@NgayTra <= d.NgayNhanDuKien OR @NgayNhan >= d.NgayTraDuKien)
    )
        SET @KetQua = 0;
    ELSE
        SET @KetQua = 1;
END
GO

-- =====================================================================================
-- C. tblDichvu
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_DichVu_Them
    @TenDichvu      NVARCHAR(100),
    @DonGia         DECIMAL(18,2),
    @DonViTinh      NVARCHAR(20) = NULL,
    @TrangThai      BIT = 1,
    @GhiChu         NVARCHAR(255) = NULL,
    @MaDichvu       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblDichvu (TenDichvu, DonGia, DonViTinh, TrangThai, GhiChu)
    VALUES (@TenDichvu, @DonGia, @DonViTinh, @TrangThai, @GhiChu);
    SET @MaDichvu = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_DichVu_Sua
    @MaDichvu       INT,
    @TenDichvu      NVARCHAR(100),
    @DonGia         DECIMAL(18,2),
    @DonViTinh      NVARCHAR(20) = NULL,
    @TrangThai      BIT = 1,
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tblDichvu
       SET TenDichvu = @TenDichvu, DonGia = @DonGia, DonViTinh = @DonViTinh,
           TrangThai = @TrangThai, GhiChu = @GhiChu
     WHERE MaDichvu = @MaDichvu;
END
GO

CREATE OR ALTER PROCEDURE sp_DichVu_Xoa
    @MaDichvu INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblHoadonchitiet WHERE MaDichvu = @MaDichvu)
        THROW 51020, N'Không thể xóa: dịch vụ đã được sử dụng trong hóa đơn chi tiết.', 1;

    DELETE FROM tblDichvu WHERE MaDichvu = @MaDichvu;
END
GO

CREATE OR ALTER PROCEDURE sp_DichVu_DanhSach
    @TuKhoa NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaDichvu, TenDichvu, DonGia, DonViTinh, TrangThai, GhiChu
      FROM tblDichvu
     WHERE (@TuKhoa IS NULL OR TenDichvu LIKE '%' + @TuKhoa + '%')
     ORDER BY TenDichvu;
END
GO

-- =====================================================================================
-- D. tblKhach
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_Khach_Them
    @CMND           VARCHAR(12),
    @HoTen          NVARCHAR(100),
    @NgaySinh       DATE,
    @GioiTinh       BIT = 1,
    @SoDienThoai    VARCHAR(15),
    @Email          VARCHAR(100) = NULL,
    @DiaChi         NVARCHAR(200) = NULL,
    @QuocTich       NVARCHAR(50) = N'Việt Nam',
    @GhiChu         NVARCHAR(255) = NULL,
    @MaKH           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblKhach WHERE CMND = @CMND)
        THROW 51030, N'Số CMND/CCCD đã tồn tại trong hệ thống.', 1;

    INSERT INTO tblKhach (CMND, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi, QuocTich, GhiChu)
    VALUES (@CMND, @HoTen, @NgaySinh, @GioiTinh, @SoDienThoai, @Email, @DiaChi, @QuocTich, @GhiChu);
    SET @MaKH = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_Khach_Sua
    @MaKH           INT,
    @CMND           VARCHAR(12),
    @HoTen          NVARCHAR(100),
    @NgaySinh       DATE,
    @GioiTinh       BIT = 1,
    @SoDienThoai    VARCHAR(15),
    @Email          VARCHAR(100) = NULL,
    @DiaChi         NVARCHAR(200) = NULL,
    @QuocTich       NVARCHAR(50) = N'Việt Nam',
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblKhach WHERE CMND = @CMND AND MaKH <> @MaKH)
        THROW 51031, N'Số CMND/CCCD đã tồn tại trong hệ thống.', 1;

    UPDATE tblKhach
       SET CMND = @CMND, HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh,
           SoDienThoai = @SoDienThoai, Email = @Email, DiaChi = @DiaChi,
           QuocTich = @QuocTich, GhiChu = @GhiChu
     WHERE MaKH = @MaKH;
END
GO

CREATE OR ALTER PROCEDURE sp_Khach_Xoa
    @MaKH INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblDangky WHERE MaKH = @MaKH)
        THROW 51032, N'Không thể xóa: khách hàng đã có lịch sử đăng ký đặt phòng.', 1;

    DELETE FROM tblKhach WHERE MaKH = @MaKH;
END
GO

CREATE OR ALTER PROCEDURE sp_Khach_DanhSach
    @TuKhoa NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaKH, CMND, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi, QuocTich, NgayTao, GhiChu
      FROM tblKhach
     WHERE (@TuKhoa IS NULL
            OR HoTen LIKE '%' + @TuKhoa + '%'
            OR CMND LIKE '%' + @TuKhoa + '%'
            OR SoDienThoai LIKE '%' + @TuKhoa + '%')
     ORDER BY HoTen;
END
GO

-- =====================================================================================
-- E. tblNhanvien
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_NhanVien_Them
    @HoTen          NVARCHAR(100),
    @NgaySinh       DATE,
    @GioiTinh       BIT = 1,
    @CMND           VARCHAR(12),
    @SoDienThoai    VARCHAR(15),
    @DiaChi         NVARCHAR(200) = NULL,
    @ChucVu         NVARCHAR(50) = N'Lễ tân',
    @NgayVaoLam     DATE = NULL,
    @TrangThai      BIT = 1,
    @GhiChu         NVARCHAR(255) = NULL,
    @MaNV           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblNhanvien WHERE CMND = @CMND)
        THROW 51040, N'Số CMND/CCCD đã tồn tại trong hệ thống.', 1;

    INSERT INTO tblNhanvien (HoTen, NgaySinh, GioiTinh, CMND, SoDienThoai, DiaChi, ChucVu, NgayVaoLam, TrangThai, GhiChu)
    VALUES (@HoTen, @NgaySinh, @GioiTinh, @CMND, @SoDienThoai, @DiaChi, @ChucVu,
            ISNULL(@NgayVaoLam, CAST(GETDATE() AS DATE)), @TrangThai, @GhiChu);
    SET @MaNV = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_Sua
    @MaNV           INT,
    @HoTen          NVARCHAR(100),
    @NgaySinh       DATE,
    @GioiTinh       BIT = 1,
    @CMND           VARCHAR(12),
    @SoDienThoai    VARCHAR(15),
    @DiaChi         NVARCHAR(200) = NULL,
    @ChucVu         NVARCHAR(50) = N'Lễ tân',
    @NgayVaoLam     DATE,
    @TrangThai      BIT = 1,
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblNhanvien WHERE CMND = @CMND AND MaNV <> @MaNV)
        THROW 51041, N'Số CMND/CCCD đã tồn tại trong hệ thống.', 1;

    UPDATE tblNhanvien
       SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, CMND = @CMND,
           SoDienThoai = @SoDienThoai, DiaChi = @DiaChi, ChucVu = @ChucVu,
           NgayVaoLam = @NgayVaoLam, TrangThai = @TrangThai, GhiChu = @GhiChu
     WHERE MaNV = @MaNV;
END
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_Xoa
    @MaNV INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblDangky WHERE MaNVLap = @MaNV)
       OR EXISTS (SELECT 1 FROM tblHoadon WHERE MaNVLap = @MaNV)
        THROW 51042, N'Không thể xóa: nhân viên đã có lịch sử xử lý đăng ký/hóa đơn.', 1;

    DELETE FROM tblNhanvien WHERE MaNV = @MaNV;
END
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_DanhSach
    @TuKhoa NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNV, HoTen, NgaySinh, GioiTinh, CMND, SoDienThoai, DiaChi, ChucVu, NgayVaoLam, TrangThai, GhiChu
      FROM tblNhanvien
     WHERE (@TuKhoa IS NULL OR HoTen LIKE '%' + @TuKhoa + '%' OR CMND LIKE '%' + @TuKhoa + '%')
     ORDER BY HoTen;
END
GO

-- =====================================================================================
-- F. tblDangky (đăng ký đặt phòng - luồng nghiệp vụ trọng tâm)
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_Dangky_Them
    @MaKH               INT,
    @MaPhong            INT,
    @MaNVLap            INT,
    @NgayNhanDuKien     DATE,
    @NgayTraDuKien      DATE,
    @SoKhach            INT = 1,
    @GhiChu             NVARCHAR(255) = NULL,
    @MaDangky           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @NgayTraDuKien <= @NgayNhanDuKien
        THROW 51050, N'Ngày trả dự kiến phải sau ngày nhận dự kiến.', 1;

    IF EXISTS (
        SELECT 1 FROM tblDangky
         WHERE MaPhong = @MaPhong
           AND TrangThai IN ('DaDat','DangO')
           AND NOT (@NgayTraDuKien <= NgayNhanDuKien OR @NgayNhanDuKien >= NgayTraDuKien)
    )
        THROW 51051, N'Phòng đã có người đặt trong khoảng ngày này. Vui lòng chọn phòng/ngày khác.', 1;

    INSERT INTO tblDangky (MaKH, MaPhong, MaNVLap, NgayNhanDuKien, NgayTraDuKien, SoKhach, GhiChu)
    VALUES (@MaKH, @MaPhong, @MaNVLap, @NgayNhanDuKien, @NgayTraDuKien, @SoKhach, @GhiChu);

    SET @MaDangky = CAST(SCOPE_IDENTITY() AS INT);

    IF @NgayNhanDuKien = CAST(GETDATE() AS DATE)
        UPDATE tblPhong SET TrangThai = 'DaDat' WHERE MaPhong = @MaPhong AND TrangThai = 'Trong';
END
GO

CREATE OR ALTER PROCEDURE sp_Dangky_Sua
    @MaDangky           INT,
    @MaKH               INT,
    @MaPhong            INT,
    @NgayNhanDuKien     DATE,
    @NgayTraDuKien      DATE,
    @SoKhach            INT = 1,
    @GhiChu             NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM tblDangky WHERE MaDangky = @MaDangky AND TrangThai = 'DaDat')
        THROW 51052, N'Chỉ có thể sửa đăng ký đang ở trạng thái Đã đặt (chưa nhận phòng).', 1;

    IF @NgayTraDuKien <= @NgayNhanDuKien
        THROW 51053, N'Ngày trả dự kiến phải sau ngày nhận dự kiến.', 1;

    IF EXISTS (
        SELECT 1 FROM tblDangky
         WHERE MaPhong = @MaPhong
           AND MaDangky <> @MaDangky
           AND TrangThai IN ('DaDat','DangO')
           AND NOT (@NgayTraDuKien <= NgayNhanDuKien OR @NgayNhanDuKien >= NgayTraDuKien)
    )
        THROW 51054, N'Phòng đã có người đặt trong khoảng ngày này. Vui lòng chọn phòng/ngày khác.', 1;

    UPDATE tblDangky
       SET MaKH = @MaKH, MaPhong = @MaPhong, NgayNhanDuKien = @NgayNhanDuKien,
           NgayTraDuKien = @NgayTraDuKien, SoKhach = @SoKhach, GhiChu = @GhiChu
     WHERE MaDangky = @MaDangky;
END
GO

CREATE OR ALTER PROCEDURE sp_Dangky_Huy
    @MaDangky INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaPhong INT;

    SELECT @MaPhong = MaPhong FROM tblDangky WHERE MaDangky = @MaDangky AND TrangThai = 'DaDat';
    IF @MaPhong IS NULL
        THROW 51055, N'Chỉ có thể hủy đăng ký đang ở trạng thái Đã đặt (chưa nhận phòng).', 1;

    UPDATE tblDangky SET TrangThai = 'DaHuy' WHERE MaDangky = @MaDangky;
    UPDATE tblPhong SET TrangThai = 'Trong' WHERE MaPhong = @MaPhong AND TrangThai = 'DaDat';
END
GO

-- Check-in: chuyển DaDat -> DangO
CREATE OR ALTER PROCEDURE sp_Dangky_NhanPhong
    @MaDangky           INT,
    @NgayNhanThucTe     DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaPhong INT;

    SELECT @MaPhong = MaPhong FROM tblDangky WHERE MaDangky = @MaDangky AND TrangThai = 'DaDat';
    IF @MaPhong IS NULL
        THROW 51056, N'Chỉ có thể nhận phòng cho đăng ký đang ở trạng thái Đã đặt.', 1;

    UPDATE tblDangky
       SET TrangThai = 'DangO', NgayNhanThucTe = ISNULL(@NgayNhanThucTe, GETDATE())
     WHERE MaDangky = @MaDangky;

    UPDATE tblPhong SET TrangThai = 'DangSD' WHERE MaPhong = @MaPhong;
END
GO

-- Check-out + lập hóa đơn: nghiệp vụ tính tiền trọng tâm của đề bài.
CREATE OR ALTER PROCEDURE sp_Dangky_TraPhong_LapHoaDon
    @MaDangky           INT,
    @MaNVLap            INT,
    @NgayTraThucTe      DATETIME = NULL,
    @HinhThucThanhToan  VARCHAR(20) = 'TienMat',
    @MaHoadon           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaPhong INT, @NgayNhanThucTe DATETIME, @DonGiaPhong DECIMAL(18,2),
            @SoNgayO INT, @TienPhong DECIMAL(18,2), @TienDichVu DECIMAL(18,2),
            @TienPhatSinh DECIMAL(18,2), @NgayTra DATETIME;

    SELECT @MaPhong = d.MaPhong, @NgayNhanThucTe = d.NgayNhanThucTe, @DonGiaPhong = lp.DonGia
      FROM tblDangky d
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
     WHERE d.MaDangky = @MaDangky AND d.TrangThai = 'DangO';

    IF @MaPhong IS NULL
        THROW 51060, N'Chỉ có thể trả phòng cho đăng ký đang ở trạng thái Đang ở.', 1;

    SET @NgayTra = ISNULL(@NgayTraThucTe, GETDATE());

    SET @SoNgayO = DATEDIFF(DAY, @NgayNhanThucTe, @NgayTra);
    IF @SoNgayO < 1 SET @SoNgayO = 1;

    SET @TienPhong = @SoNgayO * @DonGiaPhong;

    SELECT @TienDichVu = ISNULL(SUM(ThanhTien), 0)
      FROM tblHoadonchitiet WHERE MaDangky = @MaDangky;

    SELECT @TienPhatSinh = ISNULL(SUM(SoTien), 0)
      FROM tblChiphiphatsinh WHERE MaDangky = @MaDangky;

    UPDATE tblDangky
       SET TrangThai = 'DaTra', NgayTraThucTe = @NgayTra
     WHERE MaDangky = @MaDangky;

    UPDATE tblPhong SET TrangThai = 'Trong' WHERE MaPhong = @MaPhong;

    INSERT INTO tblHoadon (MaDangky, MaNVLap, NgayLapHoadon, SoNgayO, TienPhong, TienDichVu,
                            TienPhatSinh, HinhThucThanhToan, TrangThaiThanhToan)
    VALUES (@MaDangky, @MaNVLap, @NgayTra, @SoNgayO, @TienPhong, @TienDichVu,
            @TienPhatSinh, @HinhThucThanhToan, 1);

    SET @MaHoadon = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_Dangky_DanhSach
    @TrangThai VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.MaDangky, d.MaKH, k.HoTen AS TenKhach, k.SoDienThoai,
           d.MaPhong, p.SoPhong, lp.TenLoaiPhong, lp.DonGia,
           d.MaNVLap, nv.HoTen AS TenNhanVien,
           d.NgayDangKy, d.NgayNhanDuKien, d.NgayTraDuKien,
           d.NgayNhanThucTe, d.NgayTraThucTe, d.SoKhach, d.TrangThai, d.GhiChu
      FROM tblDangky d
      JOIN tblKhach k ON k.MaKH = d.MaKH
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
      JOIN tblNhanvien nv ON nv.MaNV = d.MaNVLap
     WHERE (@TrangThai IS NULL OR d.TrangThai = @TrangThai)
     ORDER BY d.NgayDangKy DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Dangky_ChiTiet
    @MaDangky INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.MaDangky, d.MaKH, k.HoTen AS TenKhach, k.SoDienThoai, k.CMND,
           d.MaPhong, p.SoPhong, p.MaLoaiPhong, lp.TenLoaiPhong, lp.DonGia,
           d.MaNVLap, nv.HoTen AS TenNhanVien,
           d.NgayDangKy, d.NgayNhanDuKien, d.NgayTraDuKien,
           d.NgayNhanThucTe, d.NgayTraThucTe, d.SoKhach, d.TrangThai, d.GhiChu
      FROM tblDangky d
      JOIN tblKhach k ON k.MaKH = d.MaKH
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
      JOIN tblNhanvien nv ON nv.MaNV = d.MaNVLap
     WHERE d.MaDangky = @MaDangky;
END
GO

-- =====================================================================================
-- G. tblHoadonchitiet (sử dụng dịch vụ theo ngày)
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_HoaDonChiTiet_Them
    @MaDangky       INT,
    @MaDichvu       INT,
    @NgaySuDung     DATE,
    @SoLuong        INT = 1,
    @GhiChu         NVARCHAR(255) = NULL,
    @MaChiTiet      INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonGia DECIMAL(18,2);

    IF NOT EXISTS (SELECT 1 FROM tblDangky WHERE MaDangky = @MaDangky AND TrangThai = 'DangO')
        THROW 51070, N'Chỉ được ghi nhận dịch vụ cho đăng ký đang ở trạng thái Đang ở.', 1;

    SELECT @DonGia = DonGia FROM tblDichvu WHERE MaDichvu = @MaDichvu;

    INSERT INTO tblHoadonchitiet (MaDangky, MaDichvu, NgaySuDung, SoLuong, DonGia, GhiChu)
    VALUES (@MaDangky, @MaDichvu, @NgaySuDung, @SoLuong, @DonGia, @GhiChu);

    SET @MaChiTiet = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_HoaDonChiTiet_Sua
    @MaChiTiet      INT,
    @MaDichvu       INT,
    @NgaySuDung     DATE,
    @SoLuong        INT = 1,
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DonGia DECIMAL(18,2);
    SELECT @DonGia = DonGia FROM tblDichvu WHERE MaDichvu = @MaDichvu;

    UPDATE tblHoadonchitiet
       SET MaDichvu = @MaDichvu, NgaySuDung = @NgaySuDung, SoLuong = @SoLuong,
           DonGia = @DonGia, GhiChu = @GhiChu
     WHERE MaChiTiet = @MaChiTiet;
END
GO

CREATE OR ALTER PROCEDURE sp_HoaDonChiTiet_Xoa
    @MaChiTiet INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM tblHoadonchitiet WHERE MaChiTiet = @MaChiTiet;
END
GO

CREATE OR ALTER PROCEDURE sp_HoaDonChiTiet_DanhSachTheoDangky
    @MaDangky INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.MaChiTiet, ct.MaDangky, ct.MaDichvu, dv.TenDichvu, ct.NgaySuDung,
           ct.SoLuong, ct.DonGia, ct.ThanhTien, ct.GhiChu
      FROM tblHoadonchitiet ct
      JOIN tblDichvu dv ON dv.MaDichvu = ct.MaDichvu
     WHERE ct.MaDangky = @MaDangky
     ORDER BY ct.NgaySuDung, ct.MaChiTiet;
END
GO

-- =====================================================================================
-- H. tblChiphiphatsinh
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_ChiPhiPhatSinh_Them
    @MaDangky       INT,
    @LoaiPhi        NVARCHAR(100),
    @SoTien         DECIMAL(18,2),
    @NgayPhatSinh   DATE = NULL,
    @GhiChu         NVARCHAR(255) = NULL,
    @MaPhatSinh     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblChiphiphatsinh (MaDangky, LoaiPhi, SoTien, NgayPhatSinh, GhiChu)
    VALUES (@MaDangky, @LoaiPhi, @SoTien, ISNULL(@NgayPhatSinh, CAST(GETDATE() AS DATE)), @GhiChu);

    SET @MaPhatSinh = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_ChiPhiPhatSinh_Sua
    @MaPhatSinh     INT,
    @LoaiPhi        NVARCHAR(100),
    @SoTien         DECIMAL(18,2),
    @NgayPhatSinh   DATE,
    @GhiChu         NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tblChiphiphatsinh
       SET LoaiPhi = @LoaiPhi, SoTien = @SoTien, NgayPhatSinh = @NgayPhatSinh, GhiChu = @GhiChu
     WHERE MaPhatSinh = @MaPhatSinh;
END
GO

CREATE OR ALTER PROCEDURE sp_ChiPhiPhatSinh_Xoa
    @MaPhatSinh INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM tblChiphiphatsinh WHERE MaPhatSinh = @MaPhatSinh;
END
GO

CREATE OR ALTER PROCEDURE sp_ChiPhiPhatSinh_DanhSachTheoDangky
    @MaDangky INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaPhatSinh, MaDangky, LoaiPhi, SoTien, NgayPhatSinh, GhiChu
      FROM tblChiphiphatsinh
     WHERE MaDangky = @MaDangky
     ORDER BY NgayPhatSinh, MaPhatSinh;
END
GO

-- =====================================================================================
-- I. tblHoadon (tra cứu / in lại hóa đơn)
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_Hoadon_DanhSach
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.MaHoadon, h.MaDangky, k.HoTen AS TenKhach, p.SoPhong,
           h.MaNVLap, nv.HoTen AS TenNhanVien, h.NgayLapHoadon, h.SoNgayO,
           h.TienPhong, h.TienDichVu, h.TienPhatSinh, h.TongTien,
           h.HinhThucThanhToan, h.TrangThaiThanhToan
      FROM tblHoadon h
      JOIN tblDangky d ON d.MaDangky = h.MaDangky
      JOIN tblKhach k ON k.MaKH = d.MaKH
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblNhanvien nv ON nv.MaNV = h.MaNVLap
     WHERE (@TuNgay IS NULL OR CAST(h.NgayLapHoadon AS DATE) >= @TuNgay)
       AND (@DenNgay IS NULL OR CAST(h.NgayLapHoadon AS DATE) <= @DenNgay)
     ORDER BY h.NgayLapHoadon DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Hoadon_ChiTiet
    @MaHoadon INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.MaHoadon, h.MaDangky, k.HoTen AS TenKhach, k.CMND, k.SoDienThoai,
           p.SoPhong, lp.TenLoaiPhong, lp.DonGia,
           d.NgayNhanThucTe, d.NgayTraThucTe,
           h.MaNVLap, nv.HoTen AS TenNhanVien, h.NgayLapHoadon, h.SoNgayO,
           h.TienPhong, h.TienDichVu, h.TienPhatSinh, h.TongTien,
           h.HinhThucThanhToan, h.TrangThaiThanhToan
      FROM tblHoadon h
      JOIN tblDangky d ON d.MaDangky = h.MaDangky
      JOIN tblKhach k ON k.MaKH = d.MaKH
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
      JOIN tblNhanvien nv ON nv.MaNV = h.MaNVLap
     WHERE h.MaHoadon = @MaHoadon;
END
GO

-- =====================================================================================
-- J. Báo cáo doanh thu
-- =====================================================================================
CREATE OR ALTER PROCEDURE sp_BaoCao_DoanhThuDichVu
    @TuNgay DATE,
    @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dv.MaDichvu, dv.TenDichvu,
           SUM(ct.SoLuong) AS TongSoLuong,
           SUM(ct.ThanhTien) AS TongDoanhThu
      FROM tblHoadonchitiet ct
      JOIN tblDichvu dv ON dv.MaDichvu = ct.MaDichvu
     WHERE ct.NgaySuDung BETWEEN @TuNgay AND @DenNgay
     GROUP BY dv.MaDichvu, dv.TenDichvu
     ORDER BY TongDoanhThu DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_BaoCao_DoanhThuPhong
    @TuNgay DATE,
    @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.SoPhong, lp.TenLoaiPhong,
           COUNT(h.MaHoadon) AS SoLuotThue,
           SUM(h.SoNgayO) AS TongSoDem,
           SUM(h.TienPhong) AS TongDoanhThuPhong
      FROM tblHoadon h
      JOIN tblDangky d ON d.MaDangky = h.MaDangky
      JOIN tblPhong p ON p.MaPhong = d.MaPhong
      JOIN tblLoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
     WHERE CAST(h.NgayLapHoadon AS DATE) BETWEEN @TuNgay AND @DenNgay
     GROUP BY p.SoPhong, lp.TenLoaiPhong
     ORDER BY TongDoanhThuPhong DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_BaoCao_TongHop
    @TuNgay DATE,
    @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS SoLuongHoaDon,
           ISNULL(SUM(TienPhong), 0)    AS TongTienPhong,
           ISNULL(SUM(TienDichVu), 0)   AS TongTienDichVu,
           ISNULL(SUM(TienPhatSinh), 0) AS TongTienPhatSinh,
           ISNULL(SUM(TongTien), 0)     AS TongDoanhThu
      FROM tblHoadon
     WHERE CAST(NgayLapHoadon AS DATE) BETWEEN @TuNgay AND @DenNgay;
END
GO

-- =====================================================================================
-- K. tblTaikhoan (đăng nhập - tùy chọn, FR12)
-- =====================================================================================
-- So khớp mật khẩu bằng HASHBYTES ngay trong SQL Server, giống hệt cách dữ liệu mẫu được
-- chèn ở 02_DuLieuMau.sql (HASHBYTES('SHA2_256', N'123456')) - tầng C# không cần tự làm
-- crypto, chỉ gửi mật khẩu dạng chữ (plain text) qua tham số cho 1 lần gọi SP.
CREATE OR ALTER PROCEDURE sp_TaiKhoan_DangNhap
    @TenDangNhap    VARCHAR(50),
    @MatKhau        NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT tk.MaTK, tk.TenDangNhap, tk.Quyen, tk.MaNV, nv.HoTen, nv.ChucVu
      FROM tblTaikhoan tk
      JOIN tblNhanvien nv ON nv.MaNV = tk.MaNV
     WHERE tk.TenDangNhap = @TenDangNhap
       AND tk.MatKhau = HASHBYTES('SHA2_256', @MatKhau)
       AND tk.TrangThai = 1;

    IF @@ROWCOUNT = 1
        UPDATE tblTaikhoan SET LanDangNhapCuoi = GETDATE() WHERE TenDangNhap = @TenDangNhap;
END
GO

CREATE OR ALTER PROCEDURE sp_TaiKhoan_DoiMatKhau
    @MaTK           INT,
    @MatKhauCu      NVARCHAR(100),
    @MatKhauMoi     NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM tblTaikhoan
         WHERE MaTK = @MaTK AND MatKhau = HASHBYTES('SHA2_256', @MatKhauCu)
    )
        THROW 51080, N'Mật khẩu hiện tại không đúng.', 1;

    UPDATE tblTaikhoan
       SET MatKhau = HASHBYTES('SHA2_256', @MatKhauMoi)
     WHERE MaTK = @MaTK;
END
GO

PRINT N'Đã tạo xong toàn bộ Stored Procedure.';
GO
