# Tao Bao cao BTL (.pdf) tu file HTML noi dung, in bang Microsoft Edge (Chromium) headless.
#
# GHI CHU: ban dau dung Word COM automation (mo file, chen Section Break + Muc luc that,
# roi SaveAs2/ExportAsFixedFormat) nhung bi TREO VO HAN trong moi truong nay ngay khi tai
# lieu co ca Muc luc (TOC field) lan nhieu bang - da thu nhieu cach (bo SaveAs2, bo restart
# so trang theo section...) van treo o buoc tinh phan trang. Edge --headless --print-to-pdf
# dung engine Chromium (rat on dinh, hang ty luot dung moi ngay cho "in ra PDF") nen chuyen
# hoan toan sang cach nay: nhanh, khong treo, tuan thu CSS @page (le trang), page-break-before
# (ngat chuong), font, bang, anh y het nhu thiet ke trong NoiDungBaoCao.html.
#
# Muc luc trong file HTML la muc luc TINH (tu go san so trang uoc luong) vi Edge headless
# khong tu dong tinh so trang thuc te cho tung de muc nhu truong TOC that cua Word. Neu can
# so trang chinh xac 100%, mo file PDF ket qua, doi chieu lai so trang tung chuong va sua tay
# bang Muc luc trong NoiDungBaoCao.html roi chay lai script nay.

$ErrorActionPreference = "Stop"

$baseDir  = "C:\Users\Delll\Desktop\DeThiEHOU\BTL-laptrinh-huongsukien-29082026\Done\SourceCode\BaoCao"
$htmlPath = Join-Path $baseDir "NoiDungBaoCao.html"
$pdfPath  = Join-Path $baseDir "BaoCao_BTL_QuanLyThuePhongKhachSan.pdf"
$edgePath = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"

if (-not (Test-Path $edgePath)) {
    $edgePath = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}
if (-not (Test-Path $edgePath)) {
    Write-Output "Khong tim thay Microsoft Edge. Hay cai Edge hoac sua duong dan trong script nay."
    exit 1
}

if (Test-Path $pdfPath) { Remove-Item $pdfPath -Force }

$fileUrl = "file:///" + ($htmlPath -replace '\\','/')

Write-Output "Dang in PDF bang Microsoft Edge (headless)..."
$prevPref = $ErrorActionPreference
$ErrorActionPreference = "Continue"
& $edgePath --headless --disable-gpu --no-pdf-header-footer "--print-to-pdf=$pdfPath" $fileUrl *> $null
$ErrorActionPreference = $prevPref

# Edge ghi file bat dong bo - cho toi da 15s, kiem tra file da xuat hien VA da ngung tang dung luong
# (thay vi Start-Sleep 1 co dinh, hay bao loi gia "khong tao duoc PDF" du file thuc ra van duoc tao).
$maxWait = 15
$lastSize = -1
for ($i = 0; $i -lt $maxWait; $i++) {
    Start-Sleep -Seconds 1
    if (Test-Path $pdfPath) {
        $curSize = (Get-Item $pdfPath).Length
        if ($curSize -eq $lastSize -and $curSize -gt 0) { break }
        $lastSize = $curSize
    }
}

if (Test-Path $pdfPath) {
    $size = (Get-Item $pdfPath).Length
    Write-Output "HOAN TAT."
    Write-Output "  Pdf : $pdfPath ($size bytes)"
} else {
    Write-Output "LOI: khong tao duoc file PDF."
    exit 1
}
