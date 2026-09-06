@echo off
setlocal

set SLN=%~dp0QLKhachSan.sln
set CONFIG=Release
set OUTDIR=%~dp0publish\
set OUTPUT=%OUTDIR%QLKhachSan.exe

echo === Building QLKhachSan ===

for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set MSBUILD=%%i

if not defined MSBUILD (
    echo MSBuild khong tim thay. Hay cai Visual Studio 2022 voi Workload .NET desktop development.
    pause
    exit /b 1
)

"%MSBUILD%" "%SLN%" /t:Rebuild /p:Configuration=%CONFIG% /p:OutputPath=%OUTDIR% /nologo /v:minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build FAILED!
    pause
    exit /b 1
)

echo.
echo Build OK! Output: %OUTPUT%
pause
