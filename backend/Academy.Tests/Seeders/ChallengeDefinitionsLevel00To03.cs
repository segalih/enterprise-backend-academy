using System.Collections.Generic;
using Academy.Api.Models;

namespace Academy.Tests.Seeders
{
    public static class ChallengeDefinitionsLevel00To03
    {
        public static Dictionary<string, ChallengeDto> GetAll()
        {
            var dict = new Dictionary<string, ChallengeDto>();

            // lesson-00-04: Hex converter
            dict["lesson-00-04"] = ChallengeBuilder.Create(
                id: "ch-00-04",
                title: "Konverter Hex String ke Desimal 64-bit",
                difficulty: "Beginner",
                problemStatement: "Baca sebuah string heksadesimal dari stdin dan konversikan menjadi bilangan bulat desimal 64-bit (long).\nFormat string dapat diawali '0x' atau tanpa prefix, dan dapat menggunakan huruf kapital maupun kecil.\nJika memuat karakter non-heksadesimal, cetak 'INVALID'.",
                inputFormat: "Satu baris string berisi representasi bilangan heksadesimal.",
                outputFormat: "Satu baris bilangan bulat desimal 64-bit, atau 'INVALID'.",
                constraints: "Panjang string 1 s/d 18 karakter. Nilai tidak negatif.",
                examples: new[] {
                    ("0x1A", "26", "Nilai 0x1A = (1 * 16) + 10 = 26."),
                    ("FF", "255", "Nilai FF tanpa prefix 0x = (15 * 16) + 15 = 255.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""INVALID""); return; }
        // TODO: Baca string, hilangkan prefix 0x (bila ada), dan parse ke long.
        Console.WriteLine(0);
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""INVALID""); return; }
        string clean = line.Trim();
        if (clean.StartsWith(""0x"", StringComparison.OrdinalIgnoreCase)) clean = clean.Substring(2);
        if (clean.Length > 0 && long.TryParse(clean, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val) && val >= 0)
        {
            Console.WriteLine(val);
        }
        else
        {
            Console.WriteLine(""INVALID"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Prefix 0x)", "0x1A", "26", false, "Example"),
                    ("Contoh 2 (Tanpa Prefix)", "FF", "255", false, "Example"),
                    ("Kasus Normal Huruf Kecil", "0x2a", "42", false, "Normal"),
                    ("Edge Case Karakter Invalid", "0xG12", "INVALID", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan StartsWith(\"0x\", StringComparison.OrdinalIgnoreCase) untuk mendeteksi prefix.",
                    "Gunakan long.TryParse dengan NumberStyles.HexNumber untuk konversi aman."
                },
                explanation: "Tipe data bilangan biner dan heksadesimal dapat dikonversi ke long secara deterministik menggunakan System.Globalization.NumberStyles.HexNumber."
            );

            // lesson-00-05: Two's complement safe addition
            dict["lesson-00-05"] = ChallengeBuilder.Create(
                id: "ch-00-05",
                title: "Deteksi Overflow Aritmatika Two's Complement",
                difficulty: "Beginner",
                problemStatement: "Baca dua buah bilangan bulat 32-bit bertanda (int) A dan B dari stdin yang dipisahkan oleh spasi.\nLakukan operasi penjumlahan A + B.\nJika operasi menyebabkan integer overflow (melampaui int.MaxValue atau di bawah int.MinValue), cetak 'OVERFLOW'.\nJika aman, cetak hasil penjumlahannya.",
                inputFormat: "Satu baris memuat dua bilangan bulat A dan B dipisahkan oleh spasi.",
                outputFormat: "Satu baris berisi hasil penjumlahan atau kata 'OVERFLOW'.",
                constraints: "A dan B berada dalam rentang int 32-bit [-2147483648, 2147483647].",
                examples: new[] {
                    ("100 200", "300", "100 + 200 = 300 aman tanpa overflow."),
                    ("2147483640 10", "OVERFLOW", "2147483640 + 10 melampaui int.MaxValue (2147483647).")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        // TODO: Gunakan blok checked { ... } untuk mendeteksi OverflowException.
        Console.WriteLine(0);
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out int a) || !int.TryParse(parts[1], out int b)) return;
        try
        {
            checked
            {
                int sum = a + b;
                Console.WriteLine(sum);
            }
        }
        catch (OverflowException)
        {
            Console.WriteLine(""OVERFLOW"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Penjumlahan Normal)", "100 200", "300", false, "Example"),
                    ("Contoh 2 (Overflow Positif)", "2147483640 10", "OVERFLOW", false, "Example"),
                    ("Underflow Negatif", "-2147483640 -10", "OVERFLOW", true, "EdgeCase"),
                    ("Penjumlahan Negatif Aman", "-50 30", "-20", false, "Normal")
                },
                hints: new[] {
                    "Di C#, kata kunci `checked { ... }` mengaktifkan pengecekan overflow aritmatika saat runtime.",
                    "Tangkap OverflowException untuk mencetak 'OVERFLOW'."
                },
                explanation: "Representasi two's complement memiliki batas nilai tetap. Kata kunci checked di C# memastikan overflow memicu OverflowException alih-alih silent wrap-around."
            );

            // lesson-00-06: UTF-8 Header Validator
            dict["lesson-00-06"] = ChallengeBuilder.Create(
                id: "ch-00-06",
                title: "Validator Leading Byte Header Karakter UTF-8",
                difficulty: "Beginner",
                problemStatement: "Baca sebuah byte (dalam format hex, misal '0x41' atau '41') dari stdin yang merepresentasikan byte pertama (leading byte) suatu karakter UTF-8.\nTentukan kategori panjang sequence karakter tersebut:\n- '1-BYTE' bila bit pola adalah 0xxxxxxx (0x00 - 0x7F)\n- '2-BYTE' bila bit pola adalah 110xxxxx (0xC0 - 0xDF)\n- '3-BYTE' bila bit pola adalah 1110xxxx (0xE0 - 0xEF)\n- '4-BYTE' bila bit pola adalah 11110xxx (0xF0 - 0xF7)\n- 'INVALID' untuk nilai lainnya (misal byte kelanjutan 10xxxxxx atau di atas 0xF7).",
                inputFormat: "Satu baris string berisi representasi heksadesimal byte (0x00..0xFF).",
                outputFormat: "Satu baris berisi kategori UTF-8 ('1-BYTE', '2-BYTE', '3-BYTE', '4-BYTE', atau 'INVALID').",
                constraints: "Input adalah byte 8-bit [0x00, 0xFF].",
                examples: new[] {
                    ("0x41", "1-BYTE", "0x41 (ASCII 'A') bernilai biner 01000001 diawali bit 0."),
                    ("0xC3", "2-BYTE", "0xC3 diawali bit 110xxxxx.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""INVALID""); return; }
        // TODO: Parse byte dan uji bitwise prefix UTF-8.
        Console.WriteLine(""1-BYTE"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""INVALID""); return; }
        string clean = line.Trim();
        if (clean.StartsWith(""0x"", StringComparison.OrdinalIgnoreCase)) clean = clean.Substring(2);
        if (!byte.TryParse(clean, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
        {
            Console.WriteLine(""INVALID"");
            return;
        }

        if ((b & 0x80) == 0x00) Console.WriteLine(""1-BYTE"");
        else if ((b & 0xE0) == 0xC0) Console.WriteLine(""2-BYTE"");
        else if ((b & 0xF0) == 0xE0) Console.WriteLine(""3-BYTE"");
        else if ((b & 0xF8) == 0xF0) Console.WriteLine(""4-BYTE"");
        else Console.WriteLine(""INVALID"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (ASCII)", "0x41", "1-BYTE", false, "Example"),
                    ("Contoh 2 (2-Byte UTF-8)", "0xC3", "2-BYTE", false, "Example"),
                    ("3-Byte UTF-8", "0xE2", "3-BYTE", false, "Normal"),
                    ("Continuation Byte Invalid Header", "0x80", "INVALID", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan operator bitwise AND: (b & 0x80) == 0x00 untuk 1 byte.",
                    "Untuk 2-byte: (b & 0xE0) == 0xC0."
                },
                explanation: "Spesifikasi UTF-8 RFC 3629 mengkodekan jumlah byte urutan karakter pada bit-bit paling signifikan (MSB) dari byte pembuka."
            );

            // lesson-01-03: Prime Detection
            dict["lesson-01-03"] = ChallengeBuilder.Create(
                id: "ch-01-03",
                title: "Deteksi Bilangan Prima Efisien O(sqrt(N))",
                difficulty: "Beginner",
                problemStatement: "Baca sebuah bilangan bulat N dari stdin.\nTentukan apakah N merupakan bilangan prima.\nCetak 'PRIMA' jika prima, atau 'BUKAN PRIMA' jika bukan prima.",
                inputFormat: "Satu baris bilangan bulat N.",
                outputFormat: "Satu baris: 'PRIMA' atau 'BUKAN PRIMA'.",
                constraints: "N adalah bilangan bulat 64-bit [-10^12, 10^12].",
                examples: new[] {
                    ("29", "PRIMA", "29 hanya dapat dibagi oleh 1 dan 29."),
                    ("1", "BUKAN PRIMA", "1 secara definisi matematis bukan bilangan prima.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (!long.TryParse(line, out long n)) return;
        // TODO: Periksa keprimaan bilangan N dengan efisiensi O(sqrt(N)).
        Console.WriteLine(""PRIMA"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (!long.TryParse(line, out long n)) return;

        if (n <= 1) { Console.WriteLine(""BUKAN PRIMA""); return; }
        if (n <= 3) { Console.WriteLine(""PRIMA""); return; }
        if (n % 2 == 0 || n % 3 == 0) { Console.WriteLine(""BUKAN PRIMA""); return; }

        bool isPrime = true;
        for (long i = 5; i * i <= n; i += 6)
        {
            if (n % i == 0 || n % (i + 2) == 0)
            {
                isPrime = false;
                break;
            }
        }
        Console.WriteLine(isPrime ? ""PRIMA"" : ""BUKAN PRIMA"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Prima)", "29", "PRIMA", false, "Example"),
                    ("Contoh 2 (Bukan Prima Angka 1)", "1", "BUKAN PRIMA", false, "Example"),
                    ("Prima Genap 2", "2", "PRIMA", false, "Normal"),
                    ("Bilangan Negatif", "-7", "BUKAN PRIMA", true, "EdgeCase")
                },
                hints: new[] {
                    "Bilangan <= 1 bukan prima.",
                    "Cukup uji pembagi hingga akar kuadrat i * i <= n dengan langkah 6k +/- 1."
                },
                explanation: "Uji pembagian hingga sqrt(N) memberikan kompleksitas O(sqrt(N)), sangat efisien untuk memproses input hingga 10^12."
            );

            // lesson-01-04: Credit Scoring Evaluator
            dict["lesson-01-04"] = ChallengeBuilder.Create(
                id: "ch-01-04",
                title: "Evaluator Kelayakan Kredit Perbankan Berbasis Logika Proposisi",
                difficulty: "Beginner",
                problemStatement: "Baca tiga nilai dari stdin dipisahkan spasi: PendapatanBulanan (int), SkorKredit (int), dan RasioUtang (persen desimal float).\nSyarat disetujui ('APPROVED'):\n(Pendapatan >= 5000000 AND SkorKredit >= 700 AND RasioUtang <= 0.40)\nATAU\n(Pendapatan >= 15000000 AND SkorKredit >= 650 AND RasioUtang <= 0.50).\nJika tidak memenuhi, cetak 'REJECTED'.",
                inputFormat: "Satu baris: Pendapatan (int) SkorKredit (int) RasioUtang (float).",
                outputFormat: "Satu baris: 'APPROVED' atau 'REJECTED'.",
                constraints: "Pendapatan > 0, SkorKredit 300..850, RasioUtang 0.0..1.0.",
                examples: new[] {
                    ("6000000 720 0.35", "APPROVED", "Memenuhi aturan pertama: Pendapatan >= 5jt, Skor >= 700, Rasio <= 40%."),
                    ("4000000 750 0.20", "REJECTED", "Pendapatan di bawah 5 juta.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        // TODO: Evaluasi proposisi logika perbankan.
        Console.WriteLine(""APPROVED"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3) return;

        int income = int.Parse(parts[0]);
        int score = int.Parse(parts[1]);
        double dti = double.Parse(parts[2], CultureInfo.InvariantCulture);

        bool cond1 = (income >= 5000000 && score >= 700 && dti <= 0.40001);
        bool cond2 = (income >= 15000000 && score >= 650 && dti <= 0.50001);

        Console.WriteLine((cond1 || cond2) ? ""APPROVED"" : ""REJECTED"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Lolos Aturan 1)", "6000000 720 0.35", "APPROVED", false, "Example"),
                    ("Contoh 2 (Gagal Pendapatan)", "4000000 750 0.20", "REJECTED", false, "Example"),
                    ("Lolos Aturan High Income", "20000000 660 0.45", "APPROVED", false, "Normal"),
                    ("DTI Terlalu Tinggi", "10000000 750 0.45", "REJECTED", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan operator logika C# && (AND) dan || (OR).",
                    "Gunakan CultureInfo.InvariantCulture saat membaca angka desimal titik."
                },
                explanation: "Aturan bisnis enterprise perbankan dibangun dari proposisi logika majemuk deterministik untuk memitigasi risiko kredit nasabah."
            );

            // lesson-01-05: Rupiah currency string parser
            dict["lesson-01-05"] = ChallengeBuilder.Create(
                id: "ch-01-05",
                title: "Parser & Normalizer String Nominal Rupiah",
                difficulty: "Beginner",
                problemStatement: "Baca sebuah string nominal uang dari stdin yang mungkin memuat prefix 'Rp', tanda titik sebagai pemisah ribuan, dan koma desimal.\nKonversikan nominal tersebut menjadi bilangan bulat murni (long) tanpa desimal (dibulatkan ke bawah / truncate).\nJika string kosong atau tidak memuat digit angka yang valid, cetak '0'.",
                inputFormat: "Satu baris string format nominal (misal 'Rp 1.500.000,50' atau '750000').",
                outputFormat: "Satu baris bilangan long hasil ekstraksi.",
                constraints: "Nominal tidak negatif, muat dalam rentang long.",
                examples: new[] {
                    ("Rp 1.500.000,00", "1500000", "Prefix Rp, titik, dan sen desimal dibersihkan."),
                    ("Rp250.000", "250000", "Format tanpa spasi tetap terurai dengan benar.")
                },
                starterCode: @"using System;
using System.Text;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Ekstrak digit angka utama sebelum pemisah koma desimal.
        Console.WriteLine(0);
    }
}",
                solutionCode: @"using System;
using System.Text;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(0); return; }

        string s = line.Trim();
        int commaIdx = s.IndexOf(',');
        if (commaIdx >= 0) s = s.Substring(0, commaIdx);

        var sb = new StringBuilder();
        foreach (char c in s)
        {
            if (char.IsDigit(c)) sb.Append(c);
        }

        if (sb.Length == 0) { Console.WriteLine(0); return; }
        if (long.TryParse(sb.ToString(), out long val)) Console.WriteLine(val);
        else Console.WriteLine(0);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Format Lengkap)", "Rp 1.500.000,00", "1500000", false, "Example"),
                    ("Contoh 2 (Tanpa Spasi)", "Rp250.000", "250000", false, "Example"),
                    ("Angka Polos", "12500", "12500", false, "Normal"),
                    ("String Tanpa Angka", "Rp -", "0", true, "EdgeCase")
                },
                hints: new[] {
                    "Potong teks sebelum koma jika terdapat nilai sen desimal.",
                    "Kumpulkan hanya karakter digit dengan char.IsDigit(c)."
                },
                explanation: "Parsing data finansial enterprise memerlukan dekomposisi pembersihan format visual (currency symbol & thousands separator) sebelum diproses logika internal."
            );

            // lesson-02-01: CLI argument parser
            dict["lesson-02-01"] = ChallengeBuilder.Create(
                id: "ch-02-01",
                title: "Parser Argumen CLI Key-Value Pair",
                difficulty: "Beginner",
                problemStatement: "Baca satu baris string argumen command-line dari stdin (misal '--port=5000 --env=Production --verbose').\nCari nilai dari kunci yang diawali prefix '--port='.\nJika ditemukan, cetak nilainya. Jika tidak ditemukan, cetak 'DEFAULT_PORT'.",
                inputFormat: "Satu baris string berisi daftar flag argumen CLI.",
                outputFormat: "Satu baris string nilai port atau 'DEFAULT_PORT'.",
                constraints: "Panjang baris <= 500 karakter.",
                examples: new[] {
                    ("--port=5100 --env=Development", "5100", "Ditemukan kunci --port dengan nilai 5100."),
                    ("--env=Production --name=ApiGateway", "DEFAULT_PORT", "Kunci --port tidak disediakan.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Cari parameter --port= dan ekstrak nilainya.
        Console.WriteLine(""DEFAULT_PORT"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""DEFAULT_PORT""); return; }
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string result = ""DEFAULT_PORT"";
        foreach (var p in parts)
        {
            if (p.StartsWith(""--port="", StringComparison.OrdinalIgnoreCase))
            {
                result = p.Substring(7).Trim();
                break;
            }
        }
        Console.WriteLine(result);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Port Ada)", "--port=5100 --env=Development", "5100", false, "Example"),
                    ("Contoh 2 (Port Absen)", "--env=Production --name=ApiGateway", "DEFAULT_PORT", false, "Example"),
                    ("Port Huruf Kecil", "--port=8080", "8080", false, "Normal"),
                    ("String Kosong", "", "DEFAULT_PORT", true, "EdgeCase")
                },
                hints: new[] {
                    "Pecah string dengan Split(' ', StringSplitOptions.RemoveEmptyEntries).",
                    "Gunakan StartsWith(\"--port=\", StringComparison.OrdinalIgnoreCase)."
                },
                explanation: "Parsing konfigurasi CLI merupakan fondasi dalam bootstrapping aplikasi ASP.NET Core saat membaca parameter startup sistem."
            );

            // lesson-02-02: Structured Log Formatter
            dict["lesson-02-02"] = ChallengeBuilder.Create(
                id: "ch-02-02",
                title: "Pemformat Log Entry Point Berstruktur",
                difficulty: "Beginner",
                problemStatement: "Baca dua baris dari stdin:\nBaris 1: Level Log ('INFO', 'WARN', 'ERROR')\nBaris 2: Pesan Log.\nCetak string log terstruktur dengan format persis: '[<LEVEL>] <PESAN>'.\nJika Level Log di luar ketiga kategori tersebut, ubah level menjadi 'INFO'.",
                inputFormat: "Dua baris stdin: baris pertama level, baris kedua pesan.",
                outputFormat: "Satu baris string terformat '[<LEVEL>] <PESAN>'.",
                constraints: "Level case-insensitive. Pesan tidak kosong.",
                examples: new[] {
                    ("ERROR\nKoneksi database terputus.", "[ERROR] Koneksi database terputus.", "Level ERROR valid."),
                    ("DEBUG\nAplikasi dimulai.", "[INFO] Aplikasi dimulai.", "Level DEBUG dinormalisasi menjadi INFO.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? level = Console.ReadLine();
        string? msg = Console.ReadLine();
        // TODO: Format string log terstruktur.
        Console.WriteLine(""[INFO] Ready"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? level = Console.ReadLine()?.Trim().ToUpperInvariant();
        string? msg = Console.ReadLine()?.Trim();
        if (level != ""INFO"" && level != ""WARN"" && level != ""ERROR"")
        {
            level = ""INFO"";
        }
        Console.WriteLine($""[{level}] {msg}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Level ERROR)", "ERROR\nKoneksi database terputus.", "[ERROR] Koneksi database terputus.", false, "Example"),
                    ("Contoh 2 (Level Default)", "DEBUG\nAplikasi dimulai.", "[INFO] Aplikasi dimulai.", false, "Example"),
                    ("Level WARN Huruf Kecil", "warn\nPenyimpanan hampir penuh.", "[WARN] Penyimpanan hampir penuh.", false, "Normal"),
                    ("Level Kosong", "\nSistem berjalan normal.", "[INFO] Sistem berjalan normal.", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan ToUpperInvariant() untuk menyamakan level log.",
                    "Gunakan string interpolation $[{level}] {msg}."
                },
                explanation: "Standardisasi penulisan log entry point mempermudah agregasi log pada backend monitoring tools seperti ELK atau Seq."
            );

            // lesson-02-03: Decimal Precision Compound Interest
            dict["lesson-02-03"] = ChallengeBuilder.Create(
                id: "ch-02-03",
                title: "Kalkulator Bunga Majemuk Finansial Presisi Decimal",
                difficulty: "Beginner",
                problemStatement: "Baca tiga nilai dari stdin: Pokok (decimal), SukuBungaTahunanDalamPersen (decimal), dan JumlahTahun (int).\nHitung total saldo akhir bunga majemuk tahunan menggunakan tipe 'decimal' dengan rumus:\nSaldo = Pokok * (1 + SukuBunga / 100)^JumlahTahun.\nBulatkan hasil akhir ke 2 digit desimal (banker's rounding MidpointRounding.ToEven) dan cetak hasilnya.",
                inputFormat: "Satu baris: Pokok SukuBunga Tahun (misal: '1000000 10 2').",
                outputFormat: "Satu baris angka desimal 2 digit di belakang koma (misal: '1210000.00').",
                constraints: "Pokok >= 0, Bunga >= 0, Tahun >= 0.",
                examples: new[] {
                    ("1000000 10 2", "1210000.00", "1.000.000 * (1.1)^2 = 1.210.000,00."),
                    ("500000 5 1", "525000.00", "500.000 * 1.05 = 525.000,00.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        // TODO: Hitung akumulasi saldo dengan tipe decimal dan format N2.
        Console.WriteLine(""0.00"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 3) return;

        decimal principal = decimal.Parse(p[0], CultureInfo.InvariantCulture);
        decimal rate = decimal.Parse(p[1], CultureInfo.InvariantCulture) / 100m;
        int years = int.Parse(p[2]);

        decimal total = principal;
        for (int i = 0; i < years; i++)
        {
            total *= (1m + rate);
        }

        total = Math.Round(total, 2, MidpointRounding.ToEven);
        Console.WriteLine(total.ToString(""0.00"", CultureInfo.InvariantCulture));
    }
}",
                testCases: new[] {
                    ("Contoh 1 (2 Tahun 10%)", "1000000 10 2", "1210000.00", false, "Example"),
                    ("Contoh 2 (1 Tahun 5%)", "500000 5 1", "525000.00", false, "Example"),
                    ("0 Tahun (Pokok Tetap)", "750000 12 0", "750000.00", false, "Normal"),
                    ("Bunga Desimal", "10000 7.5 3", "12423.00", true, "EdgeCase")
                },
                hints: new[] {
                    "Selalu gunakan tipe data decimal untuk komputasi finansial demi mencegah rounding error biner float/double.",
                    "Format string desimal persis dengan ToString(\"0.00\", CultureInfo.InvariantCulture)."
                },
                explanation: "Tipe decimal di C# memiliki presisi 128-bit berbasis desimal, wajib digunakan untuk sistem perbankan guna menghindari selisih floating point."
            );

            // lesson-02-04: Immutable Money Struct
            dict["lesson-02-04"] = ChallengeBuilder.Create(
                id: "ch-02-04",
                title: "Operasi Penjumlahan Aman Money Value Object Struct",
                difficulty: "Intermediate",
                problemStatement: "Baca dua baris dari stdin, masing-masing merepresentasikan objek Money:\nBaris 1: '<Amount1> <Currency1>'\nBaris 2: '<Amount2> <Currency2>'\nJika Currency1 dan Currency2 sama, cetak hasil penjumlahannya dalam format '<TotalAmount> <Currency>'.\nJika mata uang berbeda, cetak 'CURRENCY_MISMATCH'.",
                inputFormat: "Dua baris stdin berisi amount (decimal) dan currency code (string 3 huruf).",
                outputFormat: "Satu baris: '<Total> <Currency>' atau 'CURRENCY_MISMATCH'.",
                constraints: "Currency code 3 huruf kapital (IDR, USD, EUR).",
                examples: new[] {
                    ("15000 IDR\n25000 IDR", "40000 IDR", "Mata uang sama IDR, jumlah 40000."),
                    ("100 USD\n50 EUR", "CURRENCY_MISMATCH", "USD dan EUR berbeda.")
                },
                starterCode: @"using System;
using System.Globalization;

public readonly struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }
    public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }
}

public class Solution
{
    public static void Main()
    {
        // TODO: Baca dua baris dan operasikan value object Money.
        Console.WriteLine(""0 IDR"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public readonly struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }
    public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }

    public static bool TryAdd(Money a, Money b, out Money result)
    {
        if (!string.Equals(a.Currency, b.Currency, StringComparison.OrdinalIgnoreCase))
        {
            result = default;
            return false;
        }
        result = new Money(a.Amount + b.Amount, a.Currency.ToUpperInvariant());
        return true;
    }
}

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2)) return;

        var p1 = l1.Trim().Split(' ');
        var p2 = l2.Trim().Split(' ');

        var m1 = new Money(decimal.Parse(p1[0], CultureInfo.InvariantCulture), p1[1]);
        var m2 = new Money(decimal.Parse(p2[0], CultureInfo.InvariantCulture), p2[1]);

        if (Money.TryAdd(m1, m2, out var res))
        {
            Console.WriteLine($""{res.Amount} {res.Currency}"");
        }
        else
        {
            Console.WriteLine(""CURRENCY_MISMATCH"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Sama IDR)", "15000 IDR\n25000 IDR", "40000 IDR", false, "Example"),
                    ("Contoh 2 (Beda Currency)", "100 USD\n50 EUR", "CURRENCY_MISMATCH", false, "Example"),
                    ("Mata Uang Huruf Kecil", "500 idr\n200 IDR", "700 IDR", false, "Normal"),
                    ("Nilai 0", "0 USD\n100 USD", "100 USD", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan readonly struct untuk mengimplementasikan Value Object yang hemat alokasi heap.",
                    "Pastikan pembandingan mata uang case-insensitive."
                },
                explanation: "Value Object immutable berbasis readonly struct menjamin integritas data domain finansial tanpa alokasi memori berlebih di GC Managed Heap."
            );

            // lesson-02-05: Safe Unboxing Extractor
            dict["lesson-02-05"] = ChallengeBuilder.Create(
                id: "ch-02-05",
                title: "Ekstraktor Nilai Aman Objek Ter-Boxing",
                difficulty: "Beginner",
                problemStatement: "Diberikan sebuah string input dari stdin. Simulasikan penerimaan objek generik ter-boxing (tipe `object`).\nUji tipe nilai tersebut:\n- Jika string dapat di-parse sebagai int 32-bit: simulasikan boxing ke `object`, unbox kembali ke int, lalu cetak 'INT: <nilai*2>'\n- Jika dapat di-parse sebagai boolean: simulasikan boxing ke `object`, unbox, lalu cetak 'BOOL: <NOT nilai>'\n- Jika bukan keduanya: cetak 'UNKNOWN'.",
                inputFormat: "Satu baris string berisi teks atau angka.",
                outputFormat: "Satu baris hasil unboxing yang telah diproses.",
                constraints: "Input 1 baris.",
                examples: new[] {
                    ("42", "INT: 84", "42 di-unbox ke int lalu dikalikan 2."),
                    ("true", "BOOL: False", "true di-unbox ke bool lalu dibalik menjadi False.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? input = Console.ReadLine();
        // TODO: Simulasikan boxing dan unboxing yang aman dari runtime invalid cast.
        Console.WriteLine(""UNKNOWN"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) { Console.WriteLine(""UNKNOWN""); return; }

        object boxed;
        if (int.TryParse(input, out int iVal))
        {
            boxed = iVal; // boxing
        }
        else if (bool.TryParse(input, out bool bVal))
        {
            boxed = bVal; // boxing
        }
        else
        {
            Console.WriteLine(""UNKNOWN"");
            return;
        }

        // Safe unboxing pattern matching
        if (boxed is int unboxedInt)
        {
            Console.WriteLine($""INT: {unboxedInt * 2}"");
        }
        else if (boxed is bool unboxedBool)
        {
            Console.WriteLine($""BOOL: {!unboxedBool}"");
        }
        else
        {
            Console.WriteLine(""UNKNOWN"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Angka Int)", "42", "INT: 84", false, "Example"),
                    ("Contoh 2 (Boolean True)", "true", "BOOL: False", false, "Example"),
                    ("Boolean False", "False", "BOOL: True", false, "Normal"),
                    ("Teks Sembarang", "HaloDunia", "UNKNOWN", true, "EdgeCase")
                },
                hints: new[] {
                    "Boxing terjadi saat value type dimasukkan ke variabel bertipe object.",
                    "Gunakan pattern matching `boxed is int val` untuk unboxing aman tanpa memicu InvalidCastException."
                },
                explanation: "Unboxing langsung dengan explicit cast `(int)boxed` akan melempar InvalidCastException jika tipe underlying tidak cocok persis. Pattern matching memastikan keamanan tipe saat unboxing."
            );

            // lesson-02-06: Pattern Matching Progressive Tax
            dict["lesson-02-06"] = ChallengeBuilder.Create(
                id: "ch-02-06",
                title: "Kalkulator Tarif Pajak Progresif C# Switch Expression",
                difficulty: "Beginner",
                problemStatement: "Baca nilai Penghasilan Kena Pajak tahunan (decimal) dari stdin.\nGunakan C# relational pattern matching (switch expression) untuk menentukan persentase tarif pajak:\n- <= 60000000: tarif 5%\n- > 60000000 dan <= 250000000: tarif 15%\n- > 250000000 dan <= 500000000: tarif 25%\n- > 500000000: tarif 35%.\nCetak persentase tarif dalam format '<X>%'.",
                inputFormat: "Satu baris angka penghasilan bertipe decimal.",
                outputFormat: "Satu baris string persentase ('5%', '15%', '25%', atau '35%').",
                constraints: "Penghasilan >= 0.",
                examples: new[] {
                    ("50000000", "5%", "Di bawah 60 juta tarif 5%."),
                    ("100000000", "15%", "Di antara 60 juta dan 250 juta tarif 15%.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (!decimal.TryParse(line, CultureInfo.InvariantCulture, out decimal income)) return;
        // TODO: Gunakan switch expression relational pattern (<=, >, dll).
        Console.WriteLine(""5%"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (!decimal.TryParse(line, CultureInfo.InvariantCulture, out decimal income)) return;

        string rate = income switch
        {
            <= 60_000_000m => ""5%"",
            <= 250_000_000m => ""15%"",
            <= 500_000_000m => ""25%"",
            _ => ""35%""
        };

        Console.WriteLine(rate);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Tier 1)", "50000000", "5%", false, "Example"),
                    ("Contoh 2 (Tier 2)", "100000000", "15%", false, "Example"),
                    ("Tier 3 Tepat Batas", "500000000", "25%", false, "Normal"),
                    ("Tier Tertinggi", "1000000000", "35%", true, "EdgeCase")
                },
                hints: new[] {
                    "C# 9+ mendukung relational pattern dalam switch expression, misal: `<= 60_000_000m => \"5%\"`.",
                    "Gunakan underscore digit separator untuk keterbacaan angka nominal besar."
                },
                explanation: "Switch expression dengan relational pattern memberikan ekspresi logika kondisional yang jauh lebih bersih, deklaratif, dan aman dibanding rantai if-else konvensional."
            );

            // lesson-02-07: TryParse Bank Account Checksum
            dict["lesson-02-07"] = ChallengeBuilder.Create(
                id: "ch-02-07",
                title: "Implementasi Pola TryParse Rekening Bank Modulo-10",
                difficulty: "Intermediate",
                problemStatement: "Buat method dengan pola `bool TryParseAccount(string input, out string branch, out string accountNum)`.\nNomor rekening valid memiliki format: '<3 digit cabang>-<6 digit rekening>-<1 digit checksum>'.\nChecksum dihitung dari: (Jumlah semua digit cabang dan digit rekening) modulo 10.\nJika format valid dan digit checksum cocok, kembalikan true dan cetak 'VALID: Branch=<branch>, Account=<accountNum>'.\nJika tidak valid, cetak 'INVALID_ACCOUNT'.",
                inputFormat: "Satu baris string nomor rekening (misal '101-123456-7').",
                outputFormat: "Satu baris status validitas atau rincian cabang dan akun.",
                constraints: "Panjang string persis 12 karakter termasuk tanda minus.",
                examples: new[] {
                    ("101-123456-8", "VALID: Branch=101, Account=123456", "Jumlah digit 1+0+1 + 1+2+3+4+5+6 = 23. 23 mod 10 = 3 (Mismatch jika checksum != 3)."),
                    ("101-123456-3", "VALID: Branch=101, Account=123456", "23 mod 10 = 3 cocok dengan checksum.")
                },
                starterCode: @"using System;

public class Solution
{
    public static bool TryParseAccount(string? input, out string branch, out string accountNum)
    {
        branch = string.Empty;
        accountNum = string.Empty;
        // TODO: Validasi format dan kalkulasi modulo 10 checksum.
        return false;
    }

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (TryParseAccount(line, out var b, out var acc))
        {
            Console.WriteLine($""VALID: Branch={b}, Account={acc}"");
        }
        else
        {
            Console.WriteLine(""INVALID_ACCOUNT"");
        }
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static bool TryParseAccount(string? input, out string branch, out string accountNum)
    {
        branch = string.Empty;
        accountNum = string.Empty;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var parts = input.Trim().Split('-');
        if (parts.Length != 3) return false;
        if (parts[0].Length != 3 || parts[1].Length != 6 || parts[2].Length != 1) return false;

        int sum = 0;
        foreach (char c in parts[0])
        {
            if (!char.IsDigit(c)) return false;
            sum += c - '0';
        }
        foreach (char c in parts[1])
        {
            if (!char.IsDigit(c)) return false;
            sum += c - '0';
        }

        if (!char.IsDigit(parts[2][0])) return false;
        int expectedChecksum = sum % 10;
        int actualChecksum = parts[2][0] - '0';

        if (expectedChecksum != actualChecksum) return false;

        branch = parts[0];
        accountNum = parts[1];
        return true;
    }

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (TryParseAccount(line, out var b, out var acc))
        {
            Console.WriteLine($""VALID: Branch={b}, Account={acc}"");
        }
        else
        {
            Console.WriteLine(""INVALID_ACCOUNT"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Valid Checksum 3)", "101-123456-3", "VALID: Branch=101, Account=123456", false, "Example"),
                    ("Contoh 2 (Salah Checksum)", "101-123456-8", "INVALID_ACCOUNT", false, "Example"),
                    ("Format Kurang Segmen", "101-123456", "INVALID_ACCOUNT", false, "Normal"),
                    ("Semua Nol", "000-000000-0", "VALID: Branch=000, Account=000000", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola TryParse menggunakan out parameter untuk mengembalikan data hasil parsing tanpa melempar exception saat format salah.",
                    "Jumlahkan digit 1+0+1+1+2+3+4+5+6 = 23, 23 % 10 = 3."
                },
                explanation: "Pola TryParse adalah konvensi resmi .NET Runtime untuk parsing data yang rentan kesalahan input pengguna tanpa membebani performa akibat overhead throw exception."
            );

            // lesson-02-08: Token Deduplicator HashSet
            dict["lesson-02-08"] = ChallengeBuilder.Create(
                id: "ch-02-08",
                title: "Pembersih Token Duplikat Menggunakan HashSet",
                difficulty: "Beginner",
                problemStatement: "Baca satu baris string daftar token yang dipisahkan koma dari stdin.\nBersihkan token dari spasi di ujung, abaikan perbedaan huruf besar/kecil (case-insensitive deduplication), dan cetak daftar token unik sesuai urutan kemunculan pertama kali (FIFO) dipisahkan oleh tanda koma dan spasi.",
                inputFormat: "Satu baris string berisi token dipisahkan koma.",
                outputFormat: "Satu baris token unik dipisahkan ', ' (koma spasi).",
                constraints: "Panjang baris <= 1000 karakter.",
                examples: new[] {
                    ("apple, BANANA, Apple, orange, banana", "apple, BANANA, orange", "'Apple' dan 'banana' duplikat diabaikan, format awal dipertahankan."),
                    ("token1, token2, token1", "token1, token2", "Duplikat token1 dihapus.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Gunakan HashSet<string> dengan StringComparer.OrdinalIgnoreCase.
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""""); return; }

        var tokens = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var orderedUnique = new List<string>();

        foreach (var t in tokens)
        {
            string clean = t.Trim();
            if (clean.Length > 0 && seen.Add(clean))
            {
                orderedUnique.Add(clean);
            }
        }

        Console.WriteLine(string.Join("", "", orderedUnique));
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Case Insensitive)", "apple, BANANA, Apple, orange, banana", "apple, BANANA, orange", false, "Example"),
                    ("Contoh 2 (Dua Token)", "token1, token2, token1", "token1, token2", false, "Example"),
                    ("Satu Token", "UniqueToken", "UniqueToken", false, "Normal"),
                    ("Semua Duplikat", "A, a, A, a", "A", true, "EdgeCase")
                },
                hints: new[] {
                    "HashSet.Add(item) mengembalikan boolean true hanya jika item belum pernah ada di dalam set.",
                    "Inisialisasi HashSet dengan StringComparer.OrdinalIgnoreCase."
                },
                explanation: "HashSet<T> menyediakan pencarian O(1) berbasis hash table, ideal untuk deduplikasi data skala besar secara performan."
            );

            // lesson-02-09: Custom Financial Exception
            dict["lesson-02-09"] = ChallengeBuilder.Create(
                id: "ch-02-09",
                title: "Pemroses Transaksi Aman dengan Custom FinancialException",
                difficulty: "Intermediate",
                problemStatement: "Baca dua angka dari stdin: Saldo (decimal) dan Penarikan (decimal).\nJika Penarikan > Saldo, lempar custom exception `InsufficientFundsException` yang menyimpan properti `Deficit` (Penarikan - Saldo).\nTangkap exception tersebut dan cetak 'GAGAL: Defisit <Deficit>'.\nJika saldo mencukupi, cetak 'SUKSES: Sisa <Saldo - Penarikan>'.",
                inputFormat: "Satu baris berisi dua angka Saldo dan Penarikan dipisahkan spasi.",
                outputFormat: "Satu baris status 'SUKSES: Sisa <X>' atau 'GAGAL: Defisit <Y>'.",
                constraints: "Saldo dan Penarikan >= 0.",
                examples: new[] {
                    ("100000 30000", "SUKSES: Sisa 70000", "Saldo 100rb cukup untuk penarikan 30rb."),
                    ("50000 80000", "GAGAL: Defisit 30000", "Saldo kurang 30rb.")
                },
                starterCode: @"using System;
using System.Globalization;

public class InsufficientFundsException : Exception
{
    public decimal Deficit { get; }
    public InsufficientFundsException(decimal deficit) : base(""Saldo tidak mencukupi"") { Deficit = deficit; }
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Lempar dan tangkap InsufficientFundsException.
        Console.WriteLine(""SUKSES: Sisa 0"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class InsufficientFundsException : Exception
{
    public decimal Deficit { get; }
    public InsufficientFundsException(decimal deficit) : base(""Saldo tidak mencukupi"")
    {
        Deficit = deficit;
    }
}

public class Solution
{
    public static void Withdraw(decimal balance, decimal amount)
    {
        if (amount > balance)
        {
            throw new InsufficientFundsException(amount - balance);
        }
        Console.WriteLine($""SUKSES: Sisa {balance - amount}"");
    }

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 2) return;

        decimal bal = decimal.Parse(p[0], CultureInfo.InvariantCulture);
        decimal amt = decimal.Parse(p[1], CultureInfo.InvariantCulture);

        try
        {
            Withdraw(bal, amt);
        }
        catch (InsufficientFundsException ex)
        {
            Console.WriteLine($""GAGAL: Defisit {ex.Deficit}"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Sukses)", "100000 30000", "SUKSES: Sisa 70000", false, "Example"),
                    ("Contoh 2 (Defisit)", "50000 80000", "GAGAL: Defisit 30000", false, "Example"),
                    ("Penarikan Pas Saldo", "50000 50000", "SUKSES: Sisa 0", false, "Normal"),
                    ("Penarikan Nol", "100000 0", "SUKSES: Sisa 100000", true, "EdgeCase")
                },
                hints: new[] {
                    "Buat class exception yang mewarisi System.Exception.",
                    "Tangkap InsufficientFundsException secara spesifik sebelum catch umum."
                },
                explanation: "Custom exception domain memungkinkan penangkapan error terstruktur yang memuat metadata spesifik bisnis (seperti nominal defisit) untuk penanganan terisolasi."
            );

            // lesson-03-01: Encapsulation BankAccount
            dict["lesson-03-01"] = ChallengeBuilder.Create(
                id: "ch-03-01",
                title: "Perancangan Entitas Saldo Finansial Terenkapsulasi",
                difficulty: "Beginner",
                problemStatement: "Buat kelas `BankAccount` dengan field privat saldo (`_balance`).\nSediakan method `Deposit(decimal amount)` dan `Withdraw(decimal amount)`.\nInvarian:\n- Deposit hanya menerima angka > 0.\n- Withdraw hanya memproses jika saldo mencukupi dan amount > 0.\nBaca daftar transaksi dari stdin: 'DEP 50000', 'WIT 20000', dll dipisahkan titik koma (';').\nCetak saldo akhir: 'SALDO_AKHIR: <Balance>'.",
                inputFormat: "Satu baris instruksi transaksi dipisahkan ';'.",
                outputFormat: "Satu baris: 'SALDO_AKHIR: <Balance>'.",
                constraints: "Saldo awal adalah 0.",
                examples: new[] {
                    ("DEP 100000; WIT 40000", "SALDO_AKHIR: 60000", "Setor 100rb, tarik 40rb = 60rb."),
                    ("DEP 50000; WIT 70000", "SALDO_AKHIR: 50000", "Penarikan 70rb ditolak karena saldo kurang.")
                },
                starterCode: @"using System;
using System.Globalization;

public class BankAccount
{
    private decimal _balance = 0;
    public decimal Balance => _balance;
    // TODO: Implementasikan Deposit dan Withdraw dengan invariant guard.
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        Console.WriteLine(""SALDO_AKHIR: 0"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public class BankAccount
{
    private decimal _balance = 0;
    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        if (amount > 0) _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > 0 && _balance >= amount) _balance -= amount;
    }
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""SALDO_AKHIR: 0""); return; }

        var acc = new BankAccount();
        var txs = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var t in txs)
        {
            var p = t.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (p.Length < 2) continue;
            string op = p[0].ToUpperInvariant();
            if (decimal.TryParse(p[1], CultureInfo.InvariantCulture, out decimal val))
            {
                if (op == ""DEP"") acc.Deposit(val);
                else if (op == ""WIT"") acc.Withdraw(val);
            }
        }
        Console.WriteLine($""SALDO_AKHIR: {acc.Balance}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Setor & Tarik)", "DEP 100000; WIT 40000", "SALDO_AKHIR: 60000", false, "Example"),
                    ("Contoh 2 (Overdraft Ditolak)", "DEP 50000; WIT 70000", "SALDO_AKHIR: 50000", false, "Example"),
                    ("Deposit Negatif Ditolak", "DEP -20000; DEP 30000", "SALDO_AKHIR: 30000", false, "Normal"),
                    ("Banyak Transaksi", "DEP 10000; DEP 20000; WIT 15000; WIT 5000", "SALDO_AKHIR: 10000", true, "EdgeCase")
                },
                hints: new[] {
                    "Enkapsulasi menyembunyikan state internal _balance dari manipulasi langsung di luar kelas.",
                    "Pastikan operasi penarikan hanya memotong saldo bila amount <= _balance."
                },
                explanation: "Enkapsulasi memastikan objek bertanggung jawab penuh menjaga keabsahan invariant bisnis internalnya."
            );

            // lesson-03-02: Composition Notifier
            dict["lesson-03-02"] = ChallengeBuilder.Create(
                id: "ch-03-02",
                title: "Perancangan Komposisi Notifikasi Multi-Channel",
                difficulty: "Beginner",
                problemStatement: "Buat sistem pengirim notifikasi menggunakan komposisi. Terdapat antarmuka `IMessageSender` dengan method `Send(string message)`.\nImplementasikan `EmailSender` (mencetak 'EMAIL: <msg>') dan `SmsSender` (mencetak 'SMS: <msg>').\nKelas `NotificationService` menerima kumpulan sender melalui konstruktor komposisi.\nBaca satu baris teks pesan dari stdin dan kirimkan ke kedua sender secara berurutan.",
                inputFormat: "Satu baris pesan notifikasi.",
                outputFormat: "Dua baris hasil pengiriman: pertama EMAIL, kedua SMS.",
                constraints: "Pesan tidak kosong.",
                examples: new[] {
                    ("Tagihan Anda jatuh tempo", "EMAIL: Tagihan Anda jatuh tempo\nSMS: Tagihan Anda jatuh tempo", "Kedua channel mengirimkan pesan yang sama."),
                    ("OTP: 123456", "EMAIL: OTP: 123456\nSMS: OTP: 123456", "Notifikasi OTP terkirim ke multi-channel.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public interface IMessageSender { void Send(string message); }
// TODO: Implementasikan EmailSender, SmsSender, dan NotificationService dengan komposisi.

public class Solution
{
    public static void Main()
    {
        string? msg = Console.ReadLine();
        Console.WriteLine($""EMAIL: {msg}"");
        Console.WriteLine($""SMS: {msg}"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public interface IMessageSender
{
    void Send(string message);
}

public class EmailSender : IMessageSender
{
    public void Send(string message) => Console.WriteLine($""EMAIL: {message}"");
}

public class SmsSender : IMessageSender
{
    public void Send(string message) => Console.WriteLine($""SMS: {message}"");
}

public class NotificationService
{
    private readonly List<IMessageSender> _senders = new();
    public NotificationService(IEnumerable<IMessageSender> senders)
    {
        _senders.AddRange(senders);
    }
    public void NotifyAll(string message)
    {
        foreach (var s in _senders) s.Send(message);
    }
}

public class Solution
{
    public static void Main()
    {
        string? msg = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(msg)) return;

        var service = new NotificationService(new IMessageSender[] { new EmailSender(), new SmsSender() });
        service.NotifyAll(msg);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Tagihan)", "Tagihan Anda jatuh tempo", "EMAIL: Tagihan Anda jatuh tempo\nSMS: Tagihan Anda jatuh tempo", false, "Example"),
                    ("Contoh 2 (OTP)", "OTP: 123456", "EMAIL: OTP: 123456\nSMS: OTP: 123456", false, "Example"),
                    ("Pesan Singkat", "Halo", "EMAIL: Halo\nSMS: Halo", false, "Normal"),
                    ("Pesan Simbol", "Trans $100 -> OK!", "EMAIL: Trans $100 -> OK!\nSMS: Trans $100 -> OK!", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola komposisi menyatukan perilaku dengan menggabungkan objek-objek kecil independen alih-alih inheritance hierarki ketat.",
                    "Iterasi daftar IMessageSender untuk memanggil Send(msg)."
                },
                explanation: "Prinsip Composition over Inheritance memungkinkan penambahan channel komunikasi baru (misal WhatsAppSender) tanpa memodifikasi kode NotificationService yang sudah ada (Open/Closed Principle)."
            );

            // lesson-03-03: Polymorphic Fee Calculator
            dict["lesson-03-03"] = ChallengeBuilder.Create(
                id: "ch-03-03",
                title: "Kalkulator Potongan Biaya Transaksi Polimorfik",
                difficulty: "Intermediate",
                problemStatement: "Buat kelas dasar abstrak `Account` dengan method virtual/abstract `decimal CalculateFee(decimal amount)`.\nAturan tier:\n- `StandardAccount`: Biaya tetap 5000.\n- `GoldAccount`: Biaya 1% dari nominal (amount * 0.01).\n- `PlatinumAccount`: Biaya bebas (0).\nBaca satu baris dari stdin: '<TIER> <AMOUNT>'.\nHitung dan cetak biaya transaksi dengan pemanggilan polimorfik.",
                inputFormat: "Satu baris: '<TIER> <AMOUNT>' (misal: 'Gold 1000000').",
                outputFormat: "Satu baris angka biaya bertipe decimal.",
                constraints: "Tier: 'Standard', 'Gold', 'Platinum'. Amount >= 0.",
                examples: new[] {
                    ("Standard 500000", "5000", "StandardAccount selalu berbiaya 5000."),
                    ("Gold 1000000", "10000", "GoldAccount berbiaya 1% = 10000.")
                },
                starterCode: @"using System;
using System.Globalization;

public abstract class Account
{
    public abstract decimal CalculateFee(decimal amount);
}
// TODO: Buat subclass StandardAccount, GoldAccount, PlatinumAccount.

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        Console.WriteLine(0);
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public abstract class Account
{
    public abstract decimal CalculateFee(decimal amount);
}

public class StandardAccount : Account
{
    public override decimal CalculateFee(decimal amount) => 5000m;
}

public class GoldAccount : Account
{
    public override decimal CalculateFee(decimal amount) => amount * 0.01m;
}

public class PlatinumAccount : Account
{
    public override decimal CalculateFee(decimal amount) => 0m;
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 2) return;

        string tier = p[0];
        decimal amount = decimal.Parse(p[1], CultureInfo.InvariantCulture);

        Account account = tier.ToLowerInvariant() switch
        {
            ""gold"" => new GoldAccount(),
            ""platinum"" => new PlatinumAccount(),
            _ => new StandardAccount()
        };

        decimal fee = account.CalculateFee(amount);
        Console.WriteLine((long)fee);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Standard)", "Standard 500000", "5000", false, "Example"),
                    ("Contoh 2 (Gold)", "Gold 1000000", "10000", false, "Example"),
                    ("Platinum Bebas Biaya", "Platinum 5000000", "0", false, "Normal"),
                    ("Gold Nominal Kecil", "Gold 50000", "500", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan kata kunci abstract pada kelas Account dan override pada kelas turunan.",
                    "Eksekusi method CalculateFee terjadi melalui vtable dispatching sesuai tipe instansiasi aktual runtime."
                },
                explanation: "Polimorfisme dinamis di CLR mendispatch pemanggilan method melalui pointer Virtual Method Table (vtable), memungkinkan eksekusi algoritma berbeda tanpa percabangan if/else di level pemanggil."
            );

            // lesson-03-04: IPaymentGateway Interface
            dict["lesson-03-04"] = ChallengeBuilder.Create(
                id: "ch-03-04",
                title: "Perancangan Multi-Saluran Pembayaran via Interface",
                difficulty: "Intermediate",
                problemStatement: "Buat antarmuka `IPaymentGateway` dengan method `string Process(decimal amount)`.\nImplementasikan dua gateway:\n- `QrisGateway`: mengembalikan 'QRIS_<amount>_PAID'\n- `CreditCardGateway`: mengembalikan 'CC_<amount>_AUTHORIZED'\nBaca satu baris dari stdin: '<GATEWAY> <AMOUNT>'.\nCetak hasil pemrosesan pembayaran.",
                inputFormat: "Satu baris: '<GATEWAY> <AMOUNT>'.",
                outputFormat: "Satu baris string response gateway.",
                constraints: "Gateway: 'QRIS' atau 'CC'. Amount >= 0.",
                examples: new[] {
                    ("QRIS 50000", "QRIS_50000_PAID", "Channel QRIS memproses pembayaran."),
                    ("CC 150000", "CC_150000_AUTHORIZED", "Channel Credit Card mengotorisasi transaksi.")
                },
                starterCode: @"using System;
using System.Globalization;

public interface IPaymentGateway
{
    string Process(decimal amount);
}
// TODO: Implementasikan QrisGateway dan CreditCardGateway.

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        Console.WriteLine(""PAID"");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public interface IPaymentGateway
{
    string Process(decimal amount);
}

public class QrisGateway : IPaymentGateway
{
    public string Process(decimal amount) => $""QRIS_{amount}_PAID"";
}

public class CreditCardGateway : IPaymentGateway
{
    public string Process(decimal amount) => $""CC_{amount}_AUTHORIZED"";
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 2) return;

        string channel = p[0].ToUpperInvariant();
        decimal amount = decimal.Parse(p[1], CultureInfo.InvariantCulture);

        IPaymentGateway gateway = channel == ""CC"" ? new CreditCardGateway() : new QrisGateway();
        Console.WriteLine(gateway.Process(amount));
    }
}",
                testCases: new[] {
                    ("Contoh 1 (QRIS)", "QRIS 50000", "QRIS_50000_PAID", false, "Example"),
                    ("Contoh 2 (Credit Card)", "CC 150000", "CC_150000_AUTHORIZED", false, "Example"),
                    ("QRIS Desimal", "QRIS 25000.50", "QRIS_25000.50_PAID", false, "Normal"),
                    ("CC Nol", "CC 0", "CC_0_AUTHORIZED", true, "EdgeCase")
                },
                hints: new[] {
                    "Interface mendefinisikan kontrak tanpa implementasi.",
                    "Instansiasi gateway yang bersesuaian ke variabel bertipe IPaymentGateway."
                },
                explanation: "Desain berbasis antarmuka memisahkan konsumen layanan pembayaran dari vendor konkret, memudahkan mocking saat unit testing dan swapping vendor di produksi."
            );

            // lesson-03-05: Record With Expression
            dict["lesson-03-05"] = ChallengeBuilder.Create(
                id: "ch-03-05",
                title: "Pembaruan Status Pesanan Menggunakan Record dan With-Expression",
                difficulty: "Beginner",
                problemStatement: "Buat positional record `Order(string Id, decimal Total, string Status)`.\nBaca satu baris dari stdin: '<ID> <TOTAL> <STATUS_AWAL> <STATUS_BARU>'.\nBuat record pesanan awal, lalu buat pesanan baru yang diperbarui statusnya menggunakan C# `with` expression.\nCetak record baru dalam format: '<Id>|<Total>|<Status>'.",
                inputFormat: "Satu baris memuat empat token dipisahkan spasi.",
                outputFormat: "Satu baris: '<Id>|<Total>|<Status>'.",
                constraints: "Id string tanpa spasi, Total decimal.",
                examples: new[] {
                    ("ORD-101 250000 PENDING PAID", "ORD-101|250000|PAID", "Status diperbarui dari PENDING menjadi PAID."),
                    ("ORD-999 50000 CREATED CANCELLED", "ORD-999|50000|CANCELLED", "Status diperbarui menjadi CANCELLED.")
                },
                starterCode: @"using System;
using System.Globalization;

public record Order(string Id, decimal Total, string Status);

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Gunakan record with { Status = ... }
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;
using System.Globalization;

public record Order(string Id, decimal Total, string Status);

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 4) return;

        string id = p[0];
        decimal total = decimal.Parse(p[1], CultureInfo.InvariantCulture);
        string initStatus = p[2];
        string newStatus = p[3];

        var initialOrder = new Order(id, total, initStatus);
        var updatedOrder = initialOrder with { Status = newStatus };

        Console.WriteLine($""{updatedOrder.Id}|{updatedOrder.Total}|{updatedOrder.Status}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Pending ke Paid)", "ORD-101 250000 PENDING PAID", "ORD-101|250000|PAID", false, "Example"),
                    ("Contoh 2 (Cancelled)", "ORD-999 50000 CREATED CANCELLED", "ORD-999|50000|CANCELLED", false, "Example"),
                    ("Status Tetap", "ORD-001 10000 PAID PAID", "ORD-001|10000|PAID", false, "Normal"),
                    ("Total Desimal", "ORD-002 99.99 NEW PROCESSING", "ORD-002|99.99|PROCESSING", true, "EdgeCase")
                },
                hints: new[] {
                    "Record C# 9+ mendukung non-destructive mutation menggunakan kata kunci `with`.",
                    "Sintaks: `var updated = original with { Status = newStatus };`"
                },
                explanation: "C# Records menerapkan konsep immutability secara elegan melalui copy-and-update with-expression tanpa mengubah objek aslinya."
            );

            // lesson-03-06: Generic Repository Constraints
            dict["lesson-03-06"] = ChallengeBuilder.Create(
                id: "ch-03-06",
                title: "Penyaring Repositori Berbasis Generic Constraints",
                difficulty: "Intermediate",
                problemStatement: "Buat antarmuka `IEntity { int Id { get; } }` dan kelas generic `Repository<T> where T : class, IEntity`.\nBuat kelas `User : IEntity` dengan properti `Id (int)` dan `Name (string)`.\nBaca data user dari stdin berformat 'ID:Name' dipisahkan koma, lalu baca target ID di baris berikutnya.\nCari dan cetak 'FOUND: <Name>' jika ada, atau 'NOT_FOUND'.",
                inputFormat: "Dua baris stdin: baris 1 kumpulan user ('1:Alice, 2:Bob'), baris 2 target ID.",
                outputFormat: "Satu baris: 'FOUND: <Name>' atau 'NOT_FOUND'.",
                constraints: "ID bilangan bulat positif.",
                examples: new[] {
                    ("1:Alice, 2:Bob, 3:Charlie\n2", "FOUND: Bob", "ID 2 ditemukan dengan nama Bob."),
                    ("1:Alice, 2:Bob\n5", "NOT_FOUND", "ID 5 tidak ada di repositori.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public interface IEntity { int Id { get; } }
// TODO: Buat Repository<T> where T : class, IEntity dan method FindById(int id).

public class Solution
{
    public static void Main()
    {
        Console.WriteLine(""NOT_FOUND"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;

public interface IEntity
{
    int Id { get; }
}

public class User : IEntity
{
    public int Id { get; }
    public string Name { get; }
    public User(int id, string name) { Id = id; Name = name; }
}

public class Repository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();
    public void Add(T item) => _items.Add(item);
    public T? FindById(int id) => _items.FirstOrDefault(x => x.Id == id);
}

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || !int.TryParse(l2, out int targetId))
        {
            Console.WriteLine(""NOT_FOUND"");
            return;
        }

        var repo = new Repository<User>();
        var pairs = l1.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var p in pairs)
        {
            var parts = p.Trim().Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out int id))
            {
                repo.Add(new User(id, parts[1].Trim()));
            }
        }

        var found = repo.FindById(targetId);
        Console.WriteLine(found != null ? $""FOUND: {found.Name}"" : ""NOT_FOUND"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (User Ditemukan)", "1:Alice, 2:Bob, 3:Charlie\n2", "FOUND: Bob", false, "Example"),
                    ("Contoh 2 (User Tidak Ada)", "1:Alice, 2:Bob\n5", "NOT_FOUND", false, "Example"),
                    ("Item Pertama", "10:ServerAdmin, 20:Guest\n10", "FOUND: ServerAdmin", false, "Normal"),
                    ("Koleksi Kosong", "\n1", "NOT_FOUND", true, "EdgeCase")
                },
                hints: new[] {
                    "Terapkan constraint `where T : class, IEntity` agar compiler mengetahui bahwa properti Id tersedia pada tipe T.",
                    "Gunakan LINQ FirstOrDefault(x => x.Id == id)."
                },
                explanation: "Generic type constraints memastikan type-safety pada saat kompilasi tanpa perlu runtime reflection atau casting berbahaya."
            );

            // lesson-03-07: Delegates & Predicate Filter Pipeline
            dict["lesson-03-07"] = ChallengeBuilder.Create(
                id: "ch-03-07",
                title: "Pipeline Filter Data Finansial Menggunakan Predicate<T>",
                difficulty: "Intermediate",
                problemStatement: "Buat pipeline penyaringan daftar transaksi bertipe decimal menggunakan `Predicate<decimal>`.\nSaring daftar angka dari stdin (dipisahkan spasi) dengan dua predikat berurutan:\n1. Transaksi positif (> 0)\n2. Transaksi bernilai besar (>= 100000)\nCetak total transaksi yang lolos kedua kriteria dalam format: 'LOLOS: <Count> TRANSAKSI, TOTAL: <Sum>'.",
                inputFormat: "Satu baris berisi nominal transaksi dipisahkan spasi.",
                outputFormat: "Satu baris: 'LOLOS: <Count> TRANSAKSI, TOTAL: <Sum>'.",
                constraints: "Nominal decimal positif/negatif.",
                examples: new[] {
                    ("50000 150000 -20000 200000 75000", "LOLOS: 2 TRANSAKSI, TOTAL: 350000", "Hanya 150rb dan 200rb yang > 0 dan >= 100rb."),
                    ("10000 20000", "LOLOS: 0 TRANSAKSI, TOTAL: 0", "Tidak ada yang mencapai 100rb.")
                },
                starterCode: @"using System;
using System.Collections.Generic;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Gunakan Predicate<decimal> untuk memfilter transaksi.
        Console.WriteLine(""LOLOS: 0 TRANSAKSI, TOTAL: 0"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""LOLOS: 0 TRANSAKSI, TOTAL: 0""); return; }

        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var amounts = new List<decimal>();
        foreach (var p in parts)
        {
            if (decimal.TryParse(p, CultureInfo.InvariantCulture, out decimal val))
            {
                amounts.Add(val);
            }
        }

        Predicate<decimal> isPositive = x => x > 0;
        Predicate<decimal> isLarge = x => x >= 100000m;

        int count = 0;
        decimal sum = 0;
        foreach (var a in amounts)
        {
            if (isPositive(a) && isLarge(a))
            {
                count++;
                sum += a;
            }
        }

        Console.WriteLine($""LOLOS: {count} TRANSAKSI, TOTAL: {sum}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Campuran)", "50000 150000 -20000 200000 75000", "LOLOS: 2 TRANSAKSI, TOTAL: 350000", false, "Example"),
                    ("Contoh 2 (Nol Lolos)", "10000 20000", "LOLOS: 0 TRANSAKSI, TOTAL: 0", false, "Example"),
                    ("Tepat 100rb", "100000", "LOLOS: 1 TRANSAKSI, TOTAL: 100000", false, "Normal"),
                    ("Semua Negatif", "-100000 -500000", "LOLOS: 0 TRANSAKSI, TOTAL: 0", true, "EdgeCase")
                },
                hints: new[] {
                    "Delegates Predicate<T> setara dengan Func<T, bool> yang menerima satu argumen dan mengembalikan boolean.",
                    "Kombinasikan predikat dalam loop atau LINQ Where."
                },
                explanation: "Delegates dan Predicate<T> memungkinkan logika filter dikomposisi secara fleksibel dan dipassing sebagai first-class citizen di C#."
            );

            return dict;
        }
    }
}
