# Xuat ban .docx (co the chinh sua) tu NoiDungBaoCao.html, dung Word COM (SaveAs2).
# GHI CHU: buoc nay THINH THOANG bi treo trong moi truong sandbox cua cong cu terminal
# (nghi do trang thai COM/Word con sot lai tu lan chay truoc). Neu chay bi treo qua ~1 phut:
# mo Task Manager, tat het tien trinh WINWORD.EXE, roi chay lai script nay 1 lan nua.
# Ban PDF chinh (dang tin cay, khong bi loi nay) nam o TaoBaoCao.ps1 (dung Edge, khong dung Word).
$ErrorActionPreference = "Stop"
$htmlPath = "C:\Users\Delll\Desktop\DeThiEHOU\BTL-laptrinh-huongsukien-29082026\Done\SourceCode\BaoCao\NoiDungBaoCao.html"
$docxPath = "C:\Users\Delll\Desktop\DeThiEHOU\BTL-laptrinh-huongsukien-29082026\Done\SourceCode\BaoCao\BaoCao_BTL_QuanLyThuePhongKhachSan.docx"
if (Test-Path $docxPath) { Remove-Item $docxPath -Force }

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    Write-Output ("[" + (Get-Date -Format "HH:mm:ss") + "] Opening...")
    $doc = $word.Documents.Open($htmlPath, $false, $false, $false)
    Write-Output ("[" + (Get-Date -Format "HH:mm:ss") + "] Opened. Saving as docx (this may take a long time)...")
    $doc.SaveAs2($docxPath, 16)
    Write-Output ("[" + (Get-Date -Format "HH:mm:ss") + "] Saved OK: $docxPath")
}
finally {
    if ($doc) { $doc.Close([ref]$false) }
    $word.Quit()
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($word) | Out-Null
}
