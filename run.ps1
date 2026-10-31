param(
    [switch]$NewWindow,
    [switch]$OpenBrowser
)

$ErrorActionPreference = "Stop"
$ScriptDir = $PSScriptRoot
if (-not $ScriptDir) { $ScriptDir = (Get-Location).Path }

$BackendProj = Join-Path $ScriptDir "backend\Academy.Api\Academy.Api.csproj"
$FrontendDir = Join-Path $ScriptDir "frontend"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  ENTERPRISE BACKEND ENGINEERING ACADEMY RUNNER   " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

# 1. Validasi Prasyarat (dotnet & npm)
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
    Write-Error "[ERROR] .NET SDK ('dotnet') tidak ditemukan di PATH. Pastikan .NET 8 SDK terinstal."
    exit 1
}

if (-not (Get-Command "npm" -ErrorAction SilentlyContinue)) {
    Write-Error "[ERROR] Node.js / npm tidak ditemukan di PATH. Pastikan Node.js terinstal."
    exit 1
}

# 2. Cek apakah node_modules sudah ada di frontend
$NodeModulesDir = Join-Path $FrontendDir "node_modules"
if (-not (Test-Path $NodeModulesDir)) {
    Write-Host "[SETUP] Direktori node_modules belum ditemukan di frontend. Menjalankan npm install..." -ForegroundColor Yellow
    Push-Location $FrontendDir
    npm install
    Pop-Location
}

# 3. Opsi Terpisah (Jendela Baru)
if ($NewWindow) {
    Write-Host "[1/2] Menjalankan Backend di jendela terminal baru..." -ForegroundColor Green
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host '=== BACKEND API (ASP.NET Core 8) ===' -ForegroundColor Cyan; cd '$ScriptDir'; dotnet run --project '$BackendProj' --urls 'http://localhost:5100'"

    Write-Host "[2/2] Menjalankan Frontend di jendela terminal baru..." -ForegroundColor Green
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host '=== FRONTEND (React 19 + Vite) ===' -ForegroundColor Magenta; cd '$FrontendDir'; npm run dev -- --port 3000"

    Write-Host "`nLayanan telah dibuka di dua jendela terminal terpisah." -ForegroundColor Cyan
    Write-Host " - Backend API : http://localhost:5100" -ForegroundColor White
    Write-Host " - Frontend UI : http://localhost:3000" -ForegroundColor White

    if ($OpenBrowser) {
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:3000"
    }
    exit 0
}

# 4. Mode Paralel di Terminal yang Sama (Default)
Write-Host "[1/2] Menjalankan ASP.NET Core Backend di http://localhost:5100..." -ForegroundColor Green
$backendProc = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --project `"$BackendProj`" --urls `"http://localhost:5100`"" `
    -WorkingDirectory $ScriptDir `
    -PassThru `
    -NoNewWindow

Write-Host "[2/2] Menjalankan Frontend Vite Dev Server di http://localhost:3000..." -ForegroundColor Green
$frontendProc = Start-Process -FilePath "cmd.exe" `
    -ArgumentList "/c npm run dev -- --port 3000" `
    -WorkingDirectory $FrontendDir `
    -PassThru `
    -NoNewWindow

Write-Host "`n==================================================" -ForegroundColor Cyan
Write-Host " Kedua layanan sedang berjalan secara paralel:" -ForegroundColor Cyan
Write-Host "  -> Backend API : http://localhost:5100" -ForegroundColor White
Write-Host "  -> Frontend UI : http://localhost:3000" -ForegroundColor White
Write-Host " Tekan Ctrl+C untuk menghentikan seluruh layanan." -ForegroundColor Yellow
Write-Host "==================================================`n" -ForegroundColor Cyan

if ($OpenBrowser) {
    Start-Job -ScriptBlock {
        Start-Sleep -Seconds 3
        Start-Process "http://localhost:3000"
    } | Out-Null
}

try {
    # Tunggu salah satu atau keduanya selesai / dihentikan
    while (-not $backendProc.HasExited -and -not $frontendProc.HasExited) {
        Start-Sleep -Milliseconds 500
    }
}
finally {
    Write-Host "`n[SHUTDOWN] Menghentikan proses Backend dan Frontend..." -ForegroundColor Yellow

    if ($backendProc -and -not $backendProc.HasExited) {
        & taskkill.exe /PID $backendProc.Id /T /F 2>$null | Out-Null
    }

    if ($frontendProc -and -not $frontendProc.HasExited) {
        & taskkill.exe /PID $frontendProc.Id /T /F 2>$null | Out-Null
    }

    Write-Host "[SHUTDOWN] Seluruh proses berhasil dihentikan dengan bersih. Sampai jumpa!" -ForegroundColor Green
}
