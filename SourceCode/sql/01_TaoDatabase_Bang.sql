/*
    Đề 05 - Quản lý thuê phòng khách sạn
    Script tạo Database + Bảng + Khóa + Ràng buộc
    Xem giải thích thiết kế tại: ../docs/02-ThietKe-CSDL.md

    Chạy toàn bộ script này trên SSMS (kết nối .\SQLEXPRESS, Windows Authentication)
    trước khi chạy 02_DuLieuMau.sql
*/

IF DB_ID(N'QLKhachSan') IS NULL
BEGIN
    CREATE DATABASE QLKhachSan;
END
GO

USE QLKhachSan;
GO

-- =====================================================================
-- 1. tblLoaiPhong
-- =====================================================================
CREATE TABLE tblLoaiPhong
(
    MaLoaiPhong     INT IDENTITY(1,1)      NOT NULL,
    TenLoaiPhong    NVARCHAR(50)           NOT NULL,
    SucChua         INT                    NOT NULL,
    DonGia          DECIMAL(18,2)          NOT NULL,
    MoTa            NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblLoaiPhong PRIMARY KEY (MaLoaiPhong),
    CONSTRAINT UQ_tblLoaiPhong_TenLoaiPhong UNIQUE (TenLoaiPhong),
    CONSTRAINT CK_tblLoaiPhong_SucChua CHECK (SucChua > 0),
    CONSTRAINT CK_tblLoaiPhong_DonGia CHECK (DonGia > 0)
);
GO

-- =====================================================================
-- 2. tblPhong
-- =====================================================================
CREATE TABLE tblPhong
(
    MaPhong         INT IDENTITY(1,1)      NOT NULL,
    SoPhong         VARCHAR(10)            NOT NULL,
    MaLoaiPhong     INT                    NOT NULL,
    Tang            INT                    NULL,
    TrangThai       VARCHAR(20)            NOT NULL CONSTRAINT DF_tblPhong_TrangThai DEFAULT ('Trong'),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblPhong PRIMARY KEY (MaPhong),
    CONSTRAINT UQ_tblPhong_SoPhong UNIQUE (SoPhong),
    CONSTRAINT FK_tblPhong_tblLoaiPhong FOREIGN KEY (MaLoaiPhong)
        REFERENCES tblLoaiPhong (MaLoaiPhong) ON DELETE NO ACTION,
    CONSTRAINT CK_tblPhong_TrangThai CHECK (TrangThai IN ('Trong','DaDat','DangSD','BaoTri'))
);
GO

-- =====================================================================
-- 3. tblKhach
-- =====================================================================
CREATE TABLE tblKhach
(
    MaKH            INT IDENTITY(1,1)      NOT NULL,
    CMND            VARCHAR(12)            NOT NULL,
    HoTen           NVARCHAR(100)          NOT NULL,
    NgaySinh        DATE                   NOT NULL,
    GioiTinh        BIT                    NOT NULL CONSTRAINT DF_tblKhach_GioiTinh DEFAULT (1),
    SoDienThoai     VARCHAR(15)            NOT NULL,
    Email           VARCHAR(100)           NULL,
    DiaChi          NVARCHAR(200)          NULL,
    QuocTich        NVARCHAR(50)           NOT NULL CONSTRAINT DF_tblKhach_QuocTich DEFAULT (N'Việt Nam'),
    NgayTao         DATETIME               NOT NULL CONSTRAINT DF_tblKhach_NgayTao DEFAULT (GETDATE()),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblKhach PRIMARY KEY (MaKH),
    CONSTRAINT UQ_tblKhach_CMND UNIQUE (CMND),
    CONSTRAINT CK_tblKhach_CMND CHECK (CMND NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_tblKhach_SoDienThoai CHECK (SoDienThoai NOT LIKE '%[^0-9]%')
);
GO

-- =====================================================================
-- 4. tblNhanvien
-- =====================================================================
CREATE TABLE tblNhanvien
(
    MaNV            INT IDENTITY(1,1)      NOT NULL,
    HoTen           NVARCHAR(100)          NOT NULL,
    NgaySinh        DATE                   NOT NULL,
    GioiTinh        BIT                    NOT NULL CONSTRAINT DF_tblNhanvien_GioiTinh DEFAULT (1),
    CMND            VARCHAR(12)            NOT NULL,
    SoDienThoai     VARCHAR(15)            NOT NULL,
    DiaChi          NVARCHAR(200)          NULL,
    ChucVu          NVARCHAR(50)           NOT NULL CONSTRAINT DF_tblNhanvien_ChucVu DEFAULT (N'Lễ tân'),
    NgayVaoLam      DATE                   NOT NULL CONSTRAINT DF_tblNhanvien_NgayVaoLam DEFAULT (CAST(GETDATE() AS DATE)),
    TrangThai       BIT                    NOT NULL CONSTRAINT DF_tblNhanvien_TrangThai DEFAULT (1),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblNhanvien PRIMARY KEY (MaNV),
    CONSTRAINT UQ_tblNhanvien_CMND UNIQUE (CMND),
    CONSTRAINT CK_tblNhanvien_SoDienThoai CHECK (SoDienThoai NOT LIKE '%[^0-9]%')
);
GO

-- =====================================================================
-- 5. tblDichvu
-- =====================================================================
CREATE TABLE tblDichvu
(
    MaDichvu        INT IDENTITY(1,1)      NOT NULL,
    TenDichvu       NVARCHAR(100)          NOT NULL,
    DonGia          DECIMAL(18,2)          NOT NULL,
    DonViTinh       NVARCHAR(20)           NULL,
    TrangThai       BIT                    NOT NULL CONSTRAINT DF_tblDichvu_TrangThai DEFAULT (1),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblDichvu PRIMARY KEY (MaDichvu),
    CONSTRAINT UQ_tblDichvu_TenDichvu UNIQUE (TenDichvu),
    CONSTRAINT CK_tblDichvu_DonGia CHECK (DonGia > 0)
);
GO

-- =====================================================================
-- 6. tblDangky
-- =====================================================================
CREATE TABLE tblDangky
(
    MaDangky        INT IDENTITY(1,1)      NOT NULL,
    MaKH            INT                    NOT NULL,
    MaPhong         INT                    NOT NULL,
    MaNVLap         INT                    NOT NULL,
    NgayDangKy      DATETIME               NOT NULL CONSTRAINT DF_tblDangky_NgayDangKy DEFAULT (GETDATE()),
    NgayNhanDuKien  DATE                   NOT NULL,
    NgayTraDuKien   DATE                   NOT NULL,
    NgayNhanThucTe  DATETIME               NULL,
    NgayTraThucTe   DATETIME               NULL,
    SoKhach         INT                    NOT NULL CONSTRAINT DF_tblDangky_SoKhach DEFAULT (1),
    TrangThai       VARCHAR(20)            NOT NULL CONSTRAINT DF_tblDangky_TrangThai DEFAULT ('DaDat'),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblDangky PRIMARY KEY (MaDangky),
    CONSTRAINT FK_tblDangky_tblKhach FOREIGN KEY (MaKH)
        REFERENCES tblKhach (MaKH) ON DELETE NO ACTION,
    CONSTRAINT FK_tblDangky_tblPhong FOREIGN KEY (MaPhong)
        REFERENCES tblPhong (MaPhong) ON DELETE NO ACTION,
    CONSTRAINT FK_tblDangky_tblNhanvien FOREIGN KEY (MaNVLap)
        REFERENCES tblNhanvien (MaNV) ON DELETE NO ACTION,
    CONSTRAINT CK_tblDangky_NgayTra CHECK (NgayTraDuKien > NgayNhanDuKien),
    CONSTRAINT CK_tblDangky_SoKhach CHECK (SoKhach > 0),
    CONSTRAINT CK_tblDangky_TrangThai CHECK (TrangThai IN ('DaDat','DangO','DaTra','DaHuy'))
);
GO

CREATE INDEX IX_Dangky_MaPhong ON tblDangky (MaPhong);
CREATE INDEX IX_Dangky_MaKH ON tblDangky (MaKH);
CREATE INDEX IX_Dangky_TrangThai ON tblDangky (TrangThai);
GO

-- =====================================================================
-- 7. tblHoadonchitiet
-- =====================================================================
CREATE TABLE tblHoadonchitiet
(
    MaChiTiet       INT IDENTITY(1,1)      NOT NULL,
    MaDangky        INT                    NOT NULL,
    MaDichvu        INT                    NOT NULL,
    NgaySuDung      DATE                   NOT NULL,
    SoLuong         INT                    NOT NULL CONSTRAINT DF_tblHoadonchitiet_SoLuong DEFAULT (1),
    DonGia          DECIMAL(18,2)          NOT NULL,
    ThanhTien       AS (SoLuong * DonGia) PERSISTED,
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblHoadonchitiet PRIMARY KEY (MaChiTiet),
    CONSTRAINT FK_tblHoadonchitiet_tblDangky FOREIGN KEY (MaDangky)
        REFERENCES tblDangky (MaDangky) ON DELETE CASCADE,
    CONSTRAINT FK_tblHoadonchitiet_tblDichvu FOREIGN KEY (MaDichvu)
        REFERENCES tblDichvu (MaDichvu) ON DELETE NO ACTION,
    CONSTRAINT CK_tblHoadonchitiet_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_tblHoadonchitiet_DonGia CHECK (DonGia > 0)
);
GO

CREATE INDEX IX_Hoadonchitiet_MaDangky ON tblHoadonchitiet (MaDangky);
GO

-- =====================================================================
-- 8. tblChiphiphatsinh (bảng thêm)
-- =====================================================================
CREATE TABLE tblChiphiphatsinh
(
    MaPhatSinh      INT IDENTITY(1,1)      NOT NULL,
    MaDangky        INT                    NOT NULL,
    LoaiPhi         NVARCHAR(100)          NOT NULL,
    SoTien          DECIMAL(18,2)          NOT NULL,
    NgayPhatSinh    DATE                   NOT NULL CONSTRAINT DF_tblChiphiphatsinh_NgayPhatSinh DEFAULT (CAST(GETDATE() AS DATE)),
    GhiChu          NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblChiphiphatsinh PRIMARY KEY (MaPhatSinh),
    CONSTRAINT FK_tblChiphiphatsinh_tblDangky FOREIGN KEY (MaDangky)
        REFERENCES tblDangky (MaDangky) ON DELETE CASCADE,
    CONSTRAINT CK_tblChiphiphatsinh_SoTien CHECK (SoTien >= 0)
);
GO

CREATE INDEX IX_Chiphiphatsinh_MaDangky ON tblChiphiphatsinh (MaDangky);
GO

-- =====================================================================
-- 9. tblHoadon (bảng thêm - hóa đơn thanh toán khi trả phòng)
-- =====================================================================
CREATE TABLE tblHoadon
(
    MaHoadon            INT IDENTITY(1,1)      NOT NULL,
    MaDangky             INT                    NOT NULL,
    MaNVLap              INT                    NOT NULL,
    NgayLapHoadon        DATETIME               NOT NULL CONSTRAINT DF_tblHoadon_NgayLapHoadon DEFAULT (GETDATE()),
    SoNgayO              INT                    NOT NULL,
    TienPhong            DECIMAL(18,2)          NOT NULL,
    TienDichVu           DECIMAL(18,2)          NOT NULL CONSTRAINT DF_tblHoadon_TienDichVu DEFAULT (0),
    TienPhatSinh         DECIMAL(18,2)          NOT NULL CONSTRAINT DF_tblHoadon_TienPhatSinh DEFAULT (0),
    TongTien             AS (TienPhong + TienDichVu + TienPhatSinh) PERSISTED,
    HinhThucThanhToan    VARCHAR(20)            NOT NULL CONSTRAINT DF_tblHoadon_HinhThucThanhToan DEFAULT ('TienMat'),
    TrangThaiThanhToan   BIT                    NOT NULL CONSTRAINT DF_tblHoadon_TrangThaiThanhToan DEFAULT (0),
    GhiChu               NVARCHAR(255)          NULL,
    CONSTRAINT PK_tblHoadon PRIMARY KEY (MaHoadon),
    CONSTRAINT UQ_tblHoadon_MaDangky UNIQUE (MaDangky),
    CONSTRAINT FK_tblHoadon_tblDangky FOREIGN KEY (MaDangky)
        REFERENCES tblDangky (MaDangky) ON DELETE CASCADE,
    CONSTRAINT FK_tblHoadon_tblNhanvien FOREIGN KEY (MaNVLap)
        REFERENCES tblNhanvien (MaNV) ON DELETE NO ACTION,
    CONSTRAINT CK_tblHoadon_SoNgayO CHECK (SoNgayO > 0),
    CONSTRAINT CK_tblHoadon_HinhThucThanhToan CHECK (HinhThucThanhToan IN ('TienMat','ChuyenKhoan','The'))
);
GO

-- =====================================================================
-- 10. tblTaikhoan (tuỳ chọn - Phase 2, đăng nhập/phân quyền)
-- =====================================================================
CREATE TABLE tblTaikhoan
(
    MaTK             INT IDENTITY(1,1)      NOT NULL,
    TenDangNhap      VARCHAR(50)            NOT NULL,
    MatKhau          VARBINARY(32)          NOT NULL,
    MaNV             INT                    NOT NULL,
    Quyen            VARCHAR(20)            NOT NULL CONSTRAINT DF_tblTaikhoan_Quyen DEFAULT ('LeTan'),
    TrangThai        BIT                    NOT NULL CONSTRAINT DF_tblTaikhoan_TrangThai DEFAULT (1),
    LanDangNhapCuoi  DATETIME               NULL,
    CONSTRAINT PK_tblTaikhoan PRIMARY KEY (MaTK),
    CONSTRAINT UQ_tblTaikhoan_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT UQ_tblTaikhoan_MaNV UNIQUE (MaNV),
    CONSTRAINT FK_tblTaikhoan_tblNhanvien FOREIGN KEY (MaNV)
        REFERENCES tblNhanvien (MaNV) ON DELETE CASCADE,
    CONSTRAINT CK_tblTaikhoan_Quyen CHECK (Quyen IN ('Admin','LeTan'))
);
GO

PRINT N'Đã tạo xong CSDL QLKhachSan và toàn bộ bảng.';
GO
