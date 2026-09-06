/*
    Đề 05 - Quản lý thuê phòng khách sạn
    Dữ liệu mẫu để test - CHẠY SAU 01_TaoDatabase_Bang.sql, trên CSDL rỗng vừa tạo.

    Giả định: các bảng đang rỗng nên IDENTITY sinh ra lần lượt 1,2,3...
    -> các câu INSERT bên dưới cố ý tham chiếu thẳng số thứ tự (1,2,3..) cho FK,
       KHÔNG chạy script này 2 lần trên cùng 1 CSDL đã có dữ liệu.

    Kịch bản dữ liệu (xem giải thích tại ../docs/02-ThietKe-CSDL.md mục 6):
      - Đăng ký #1 (phòng VIP, KH Trần Văn Nam): đã DaTra, có đủ dịch vụ + phụ phí + hóa đơn
      - Đăng ký #2 (phòng đôi, KH Nguyễn Thị Hoa): đang DangO, có vài dòng dịch vụ, chưa hóa đơn
      - Đăng ký #3 (phòng đôi, KH Đỗ Minh Khôi): mới DaDat, ngày nhận trong tương lai
*/

USE QLKhachSan;
GO

-- =====================================================================
-- 1. tblLoaiPhong
-- =====================================================================
INSERT INTO tblLoaiPhong (TenLoaiPhong, SucChua, DonGia, MoTa) VALUES
    (N'Phòng Đơn', 1, 350000,  N'1 giường đơn, phù hợp khách đi công tác 1 mình'),
    (N'Phòng Đôi', 2, 550000,  N'1 giường đôi hoặc 2 giường đơn'),
    (N'Phòng VIP', 4, 1200000, N'Phòng cao cấp, view đẹp, tối đa 4 khách');
-- MaLoaiPhong: 1=Đơn, 2=Đôi, 3=VIP
GO

-- =====================================================================
-- 2. tblPhong
-- =====================================================================
INSERT INTO tblPhong (SoPhong, MaLoaiPhong, Tang, TrangThai) VALUES
    ('101', 1, 1, 'Trong'),
    ('102', 1, 1, 'Trong'),
    ('103', 2, 1, 'DaDat'),    -- giữ chỗ cho Đăng ký #3
    ('201', 2, 2, 'DangSD'),   -- đang có khách ở (Đăng ký #2)
    ('202', 2, 2, 'Trong'),
    ('301', 3, 3, 'Trong');    -- đã trả phòng xong (Đăng ký #1)
-- MaPhong: 1=101, 2=102, 3=103, 4=201, 5=202, 6=301
GO

-- =====================================================================
-- 3. tblKhach
-- =====================================================================
INSERT INTO tblKhach (CMND, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi) VALUES
    ('012345678901', N'Trần Văn Nam',    '1990-05-12', 1, '0912345678', N'nam.tran@email.com',  N'Hà Nội'),
    ('012345678902', N'Nguyễn Thị Hoa',  '1995-11-03', 0, '0912345679', N'hoa.nguyen@email.com', N'Hải Phòng'),
    ('012345678903', N'Đỗ Minh Khôi',    '1988-02-20', 1, '0912345680', N'khoi.do@email.com',    N'Đà Nẵng'),
    ('012345678904', N'Vũ Thị Lan',      '1992-07-15', 0, '0912345681', N'lan.vu@email.com',     N'TP.HCM'),
    ('012345678905', N'Hoàng Văn Phúc',  '1985-09-30', 1, '0912345682', N'phuc.hoang@email.com', N'Huế');
-- MaKH: 1=Trần Văn Nam, 2=Nguyễn Thị Hoa, 3=Đỗ Minh Khôi, 4=Vũ Thị Lan, 5=Hoàng Văn Phúc
GO

-- =====================================================================
-- 4. tblNhanvien
-- =====================================================================
INSERT INTO tblNhanvien (HoTen, NgaySinh, GioiTinh, CMND, SoDienThoai, ChucVu, NgayVaoLam) VALUES
    (N'Nguyễn Văn An',   '1993-03-10', 1, '098765432101', '0987654321', N'Lễ tân',        '2023-01-10'),
    (N'Lê Thị Bình',     '1996-06-22', 0, '098765432102', '0987654322', N'Lễ tân',        '2023-05-15'),
    (N'Phạm Văn Cường',  '1988-12-01', 1, '098765432103', '0987654323', N'Lễ tân trưởng', '2021-08-01'),
    (N'Đỗ Thị Hương',    '1997-04-18', 0, '098765432104', '0987654324', N'Lễ tân',        '2024-02-01'),
    (N'Vũ Minh Đức',     '1994-09-05', 1, '098765432105', '0987654325', N'Lễ tân',        '2024-02-01');
-- MaNV: 1=Nguyễn Văn An, 2=Lê Thị Bình, 3=Phạm Văn Cường, 4=Đỗ Thị Hương, 5=Vũ Minh Đức
GO

-- =====================================================================
-- 5. tblDichvu
-- =====================================================================
INSERT INTO tblDichvu (TenDichvu, DonGia, DonViTinh) VALUES
    (N'Ăn sáng',            80000,  N'suất'),
    (N'Giặt ủi',             50000,  N'lượt'),
    (N'Đưa đón sân bay',    300000, N'lượt'),
    (N'Spa',                500000, N'lượt'),
    (N'Minibar',            100000, N'lượt');
-- MaDichvu: 1=Ăn sáng, 2=Giặt ủi, 3=Đưa đón sân bay, 4=Spa, 5=Minibar
GO

-- =====================================================================
-- 6. tblDangky
-- =====================================================================
INSERT INTO tblDangky
    (MaKH, MaPhong, MaNVLap, NgayDangKy, NgayNhanDuKien, NgayTraDuKien, NgayNhanThucTe, NgayTraThucTe, SoKhach, TrangThai)
VALUES
    -- Đăng ký #1: đã trả phòng (phòng 301 - VIP)
    (1, 6, 1, '2026-08-20 09:00', '2026-08-22', '2026-08-25', '2026-08-22 14:00', '2026-08-25 11:00', 2, 'DaTra'),
    -- Đăng ký #2: đang ở (phòng 201 - Đôi)
    (2, 4, 2, '2026-09-01 10:30', '2026-09-02', '2026-09-08', '2026-09-02 13:00', NULL, 2, 'DangO'),
    -- Đăng ký #3: đã đặt, chưa tới ngày nhận (phòng 103 - Đôi)
    (3, 3, 1, '2026-09-05 15:00', '2026-09-10', '2026-09-12', NULL, NULL, 1, 'DaDat');
-- MaDangky: 1, 2, 3 (đúng theo thứ tự trên)
GO

-- =====================================================================
-- 7. tblHoadonchitiet (chỉ Đăng ký #1 và #2 có phát sinh dịch vụ)
-- =====================================================================
INSERT INTO tblHoadonchitiet (MaDangky, MaDichvu, NgaySuDung, SoLuong, DonGia) VALUES
    -- Đăng ký #1 (đã trả phòng): 3 ngày ở, dùng ăn sáng + spa + giặt ủi
    (1, 1, '2026-08-22', 2, 80000),   -- ăn sáng ngày nhận phòng
    (1, 1, '2026-08-23', 2, 80000),   -- ăn sáng ngày 2
    (1, 4, '2026-08-23', 1, 500000),  -- spa ngày 2
    (1, 2, '2026-08-24', 1, 50000),   -- giặt ủi ngày 3
    (1, 1, '2026-08-24', 2, 80000),   -- ăn sáng ngày 3
    -- Đăng ký #2 (đang ở): mới dùng dịch vụ 2 ngày đầu
    (2, 1, '2026-09-02', 2, 80000),   -- ăn sáng
    (2, 3, '2026-09-03', 1, 300000);  -- đưa đón sân bay
GO
-- Tổng tiền dịch vụ Đăng ký #1 = 160000+160000+500000+50000+160000 = 1.030.000

-- =====================================================================
-- 8. tblChiphiphatsinh (chỉ Đăng ký #1 có phát sinh, vì đã trả phòng)
-- =====================================================================
INSERT INTO tblChiphiphatsinh (MaDangky, LoaiPhi, SoTien, NgayPhatSinh, GhiChu) VALUES
    (1, N'Đền bù vỡ ly cốc minibar', 50000, '2026-08-25', N'Phát hiện khi dọn phòng trả khách');
GO

-- =====================================================================
-- 9. tblHoadon (chỉ Đăng ký #1 đã trả phòng nên có hóa đơn)
-- =====================================================================
-- SoNgayO = DATEDIFF(day, '2026-08-22 14:00', '2026-08-25 11:00') = 3 ngày
-- TienPhong = 3 x 1.200.000 (đơn giá phòng VIP) = 3.600.000
-- TienDichVu = 1.030.000 (tổng ở bước 7)
-- TienPhatSinh = 50.000 (bước 8)
-- TongTien (cột tính toán) = 3.600.000 + 1.030.000 + 50.000 = 4.680.000
INSERT INTO tblHoadon
    (MaDangky, MaNVLap, NgayLapHoadon, SoNgayO, TienPhong, TienDichVu, TienPhatSinh, HinhThucThanhToan, TrangThaiThanhToan) VALUES
    (1, 3, '2026-08-25 11:15', 3, 3600000, 1030000, 50000, 'TienMat', 1);
GO

-- =====================================================================
-- 10. tblTaikhoan (tuỳ chọn - mật khẩu mẫu "123456", băm SHA-256 ngay khi insert)
-- =====================================================================
INSERT INTO tblTaikhoan (TenDangNhap, MatKhau, MaNV, Quyen) VALUES
    ('admin',   HASHBYTES('SHA2_256', N'123456'), 3, 'Admin'),
    ('annv',    HASHBYTES('SHA2_256', N'123456'), 1, 'LeTan'),
    ('binhlt',  HASHBYTES('SHA2_256', N'123456'), 2, 'LeTan'),
    ('huongdt', HASHBYTES('SHA2_256', N'123456'), 4, 'LeTan'),
    ('ducvm',   HASHBYTES('SHA2_256', N'123456'), 5, 'LeTan');
GO

PRINT N'Đã chèn xong dữ liệu mẫu.';
GO

-- =====================================================================
-- Kiểm tra nhanh sau khi insert
-- =====================================================================
SELECT * FROM tblLoaiPhong;
SELECT * FROM tblPhong;
SELECT * FROM tblKhach;
SELECT * FROM tblNhanvien;
SELECT * FROM tblDichvu;
SELECT * FROM tblDangky;
SELECT * FROM tblHoadonchitiet;
SELECT * FROM tblChiphiphatsinh;
SELECT * FROM tblHoadon;
SELECT * FROM tblTaikhoan;
