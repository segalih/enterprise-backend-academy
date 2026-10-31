#!/usr/bin/env bash

# Enterprise Backend Engineering Academy Runner Script
# Menjalankan Backend ASP.NET Core 8 dan Frontend React 19 + Vite secara paralel.

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACKEND_PROJ="$SCRIPT_DIR/backend/Academy.Api/Academy.Api.csproj"
FRONTEND_DIR="$SCRIPT_DIR/frontend"

echo "=================================================="
echo "  ENTERPRISE BACKEND ENGINEERING ACADEMY RUNNER   "
echo "=================================================="

# 1. Validasi Prasyarat
if ! command -v dotnet >/dev/null 2>&1; then
    echo "[ERROR] .NET SDK ('dotnet') tidak ditemukan di PATH. Pastikan .NET 8 SDK terinstal." >&2
    exit 1
fi

if ! command -v npm >/dev/null 2>&1; then
    echo "[ERROR] Node.js / npm tidak ditemukan di PATH. Pastikan Node.js terinstal." >&2
    exit 1
fi

# 2. Cek dependensi frontend
if [ ! -d "$FRONTEND_DIR/node_modules" ]; then
    echo "[SETUP] Direktori node_modules belum ditemukan di frontend. Menjalankan npm install..."
    (cd "$FRONTEND_DIR" && npm install)
fi

BACKEND_PID=""
FRONTEND_PID=""

cleanup() {
    echo ""
    echo "[SHUTDOWN] Menghentikan Backend dan Frontend..."
    
    # Hentikan proses backend jika masih aktif
    if [ -n "$BACKEND_PID" ] && kill -0 "$BACKEND_PID" 2>/dev/null; then
        # Coba matikan child process tree jika di Linux/macOS/Windows
        pkill -P "$BACKEND_PID" 2>/dev/null || true
        kill -TERM "$BACKEND_PID" 2>/dev/null || true
    fi

    # Hentikan proses frontend jika masih aktif
    if [ -n "$FRONTEND_PID" ] && kill -0 "$FRONTEND_PID" 2>/dev/null; then
        pkill -P "$FRONTEND_PID" 2>/dev/null || true
        kill -TERM "$FRONTEND_PID" 2>/dev/null || true
    fi

    # Jika berjalan di Git Bash / Windows, pastikan port dilepas bersih
    if command -v taskkill.exe >/dev/null 2>&1; then
        if [ -n "$BACKEND_PID" ]; then taskkill.exe //PID "$BACKEND_PID" //T //F >/dev/null 2>&1 || true; fi
        if [ -n "$FRONTEND_PID" ]; then taskkill.exe //PID "$FRONTEND_PID" //T //F >/dev/null 2>&1 || true; fi
    fi

    echo "[SHUTDOWN] Seluruh proses berhasil dihentikan dengan bersih. Sampai jumpa!"
    exit 0
}

# Trap sinyal interupsi (Ctrl+C, SIGTERM)
trap cleanup SIGINT SIGTERM EXIT

# 3. Jalankan Backend
echo "[1/2] Menjalankan ASP.NET Core Backend di http://localhost:5100..."
dotnet run --project "$BACKEND_PROJ" --urls "http://localhost:5100" &
BACKEND_PID=$!

# 4. Jalankan Frontend
echo "[2/2] Menjalankan Frontend Vite Dev Server di http://localhost:3000..."
(cd "$FRONTEND_DIR" && npm run dev -- --port 3000) &
FRONTEND_PID=$!

echo ""
echo "=================================================="
echo " Kedua layanan sedang berjalan secara paralel:"
echo "  -> Backend API : http://localhost:5100"
echo "  -> Frontend UI : http://localhost:3000"
echo " Tekan Ctrl+C untuk menghentikan seluruh layanan."
echo "=================================================="
echo ""

# Tunggu child processes selesai
wait
