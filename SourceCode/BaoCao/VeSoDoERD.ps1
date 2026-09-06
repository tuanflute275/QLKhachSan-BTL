Add-Type -AssemblyName System.Drawing

$width = 1400
$height = 900
$bmp = New-Object System.Drawing.Bitmap $width, $height
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.Clear([System.Drawing.Color]::White)

$fontBox   = New-Object System.Drawing.Font("Times New Roman", 13, [System.Drawing.FontStyle]::Bold)
$fontLabel = New-Object System.Drawing.Font("Times New Roman", 10, [System.Drawing.FontStyle]::Italic)
$penBox    = New-Object System.Drawing.Pen([System.Drawing.Color]::Black, 1.5)
$penLine   = New-Object System.Drawing.Pen([System.Drawing.Color]::DimGray, 1.3)
$brushBox  = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(235,244,255))
$brushText = [System.Drawing.Brushes]::Black
$sf = New-Object System.Drawing.StringFormat
$sf.Alignment = [System.Drawing.StringAlignment]::Center
$sf.LineAlignment = [System.Drawing.StringAlignment]::Center

function Draw-Box($x, $y, $w, $h, $text) {
    $rect = New-Object System.Drawing.Rectangle $x, $y, $w, $h
    $g.FillRectangle($brushBox, $rect)
    $g.DrawRectangle($penBox, $rect)
    $g.DrawString($text, $fontBox, $brushText, [System.Drawing.RectangleF]$rect, $sf)
    return $rect
}

function Draw-Line($fromRect, $fromEdge, $toRect, $toEdge, $label) {
    $p1 = Get-EdgePoint $fromRect $fromEdge
    $p2 = Get-EdgePoint $toRect $toEdge
    $g.DrawLine($penLine, $p1, $p2)
    if ($label) {
        $mx = [int](($p1.X + $p2.X) / 2)
        $my = [int](($p1.Y + $p2.Y) / 2) - 12
        $g.DrawString($label, $fontLabel, [System.Drawing.Brushes]::DimGray, $mx, $my)
    }
}

function Get-EdgePoint($rect, $edge) {
    switch ($edge) {
        "top"    { return New-Object System.Drawing.Point ([int]($rect.X + $rect.Width/2)), $rect.Y }
        "bottom" { return New-Object System.Drawing.Point ([int]($rect.X + $rect.Width/2)), ($rect.Y + $rect.Height) }
        "left"   { return New-Object System.Drawing.Point $rect.X, ([int]($rect.Y + $rect.Height/2)) }
        "right"  { return New-Object System.Drawing.Point ($rect.X + $rect.Width), ([int]($rect.Y + $rect.Height/2)) }
    }
}

$bw = 190; $bh = 55

$rLoaiPhong = Draw-Box 40   30  $bw $bh "tblLoaiPhong"
$rPhong     = Draw-Box 40  160  $bw $bh "tblPhong"
$rKhach     = Draw-Box 40  400  $bw $bh "tblKhach"
$rNhanvien  = Draw-Box 40  650  $bw $bh "tblNhanvien"

$rDangky    = Draw-Box 330 400  $bw $bh "tblDangky"

$rDichvu    = Draw-Box 640  30  $bw $bh "tblDichvu"
$rHDCT      = Draw-Box 640 280  $bw $bh "tblHoadonchitiet"
$rPhatSinh  = Draw-Box 640 550  $bw $bh "tblChiphiphatsinh"
$rHoadon    = Draw-Box 940 400  $bw $bh "tblHoadon"
$rTaikhoan  = Draw-Box 330 650  $bw $bh "tblTaikhoan"

Draw-Line $rLoaiPhong "bottom" $rPhong "top" "1-n"
Draw-Line $rPhong "right" $rDangky "left" "1-n"
Draw-Line $rKhach "right" $rDangky "left" "1-n"
Draw-Line $rNhanvien "top" $rDangky "left" "1-n"
Draw-Line $rNhanvien "top" $rTaikhoan "left" "1-1"

Draw-Line $rDangky "top" $rHDCT "bottom" "1-n"
Draw-Line $rDichvu "bottom" $rHDCT "top" "1-n"
Draw-Line $rDangky "bottom" $rPhatSinh "top" "1-n"
Draw-Line $rDangky "right" $rHoadon "left" "1-1"
Draw-Line $rNhanvien "right" $rHoadon "bottom" "1-n"

$g.Flush()
$outPath = "C:\Users\Delll\Desktop\DeThiEHOU\BTL-laptrinh-huongsukien-29082026\Done\SourceCode\BaoCao\Hinh_ERD.png"
$bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp.Dispose()
Write-Output "Saved: $outPath"
