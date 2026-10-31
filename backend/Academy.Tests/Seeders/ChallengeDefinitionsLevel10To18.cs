using System.Collections.Generic;
using Academy.Api.Models;

namespace Academy.Tests.Seeders
{
    public static class ChallengeDefinitionsLevel10To18
    {
        public static Dictionary<string, ChallengeDto> GetAll()
        {
            var dict = new Dictionary<string, ChallengeDto>();

            // lesson-10-04: Unit of Work Simulator
            dict["lesson-10-04"] = ChallengeBuilder.Create(
                id: "ch-10-04",
                title: "Simulator Transaksi Unit of Work Berbasis Status Entitas",
                difficulty: "Intermediate",
                problemStatement: "Buat simulator Unit of Work yang melacak entitas dalam status: 'NEW', 'DIRTY', atau 'REMOVED'.\nBaca daftar instruksi dari stdin dipisahkan koma: 'REGISTER_NEW:Order1, REGISTER_DIRTY:User2, REMOVE:Item3, COMMIT' atau '... ROLLBACK'.\n- Jika diakhiri 'COMMIT': cetak 'COMMITTED: Added=<N>, Updated=<U>, Deleted=<D>'\n- Jika diakhiri 'ROLLBACK': batalkan seluruh perubahan dan cetak 'ROLLED_BACK: All changes discarded'.",
                inputFormat: "Satu baris string perintah Unit of Work dipisahkan koma.",
                outputFormat: "Satu baris status commit atau rollback.",
                constraints: "Operasi diakhiri persis oleh COMMIT atau ROLLBACK.",
                examples: new[] {
                    ("REGISTER_NEW:Order1, REGISTER_DIRTY:User2, COMMIT", "COMMITTED: Added=1, Updated=1, Deleted=0", "Perubahan baru dan modifikasi berhasil di-commit."),
                    ("REGISTER_NEW:Order1, ROLLBACK", "ROLLED_BACK: All changes discarded", "Transaksi dibatalkan.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Lacak entitas dalam HashSet/List dan evaluasi COMMIT/ROLLBACK.
        Console.WriteLine(""COMMITTED: Added=0, Updated=0, Deleted=0"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        var tokens = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var added = new HashSet<string>();
        var updated = new HashSet<string>();
        var deleted = new HashSet<string>();

        foreach (var tok in tokens)
        {
            string t = tok.Trim();
            if (t.Equals(""ROLLBACK"", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(""ROLLED_BACK: All changes discarded"");
                return;
            }
            if (t.Equals(""COMMIT"", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($""COMMITTED: Added={added.Count}, Updated={updated.Count}, Deleted={deleted.Count}"");
                return;
            }

            var parts = t.Split(':');
            if (parts.Length == 2)
            {
                string op = parts[0].Trim().ToUpperInvariant();
                string entity = parts[1].Trim();
                if (op == ""REGISTER_NEW"") added.Add(entity);
                else if (op == ""REGISTER_DIRTY"") updated.Add(entity);
                else if (op == ""REMOVE"") deleted.Add(entity);
            }
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Commit)", "REGISTER_NEW:Order1, REGISTER_DIRTY:User2, COMMIT", "COMMITTED: Added=1, Updated=1, Deleted=0", false, "Example"),
                    ("Contoh 2 (Rollback)", "REGISTER_NEW:Order1, ROLLBACK", "ROLLED_BACK: All changes discarded", false, "Example"),
                    ("Semua Status Ada", "REGISTER_NEW:O1, REGISTER_DIRTY:U1, REMOVE:D1, COMMIT", "COMMITTED: Added=1, Updated=1, Deleted=1", false, "Normal"),
                    ("Commit Kosong", "COMMIT", "COMMITTED: Added=0, Updated=0, Deleted=0", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola Unit of Work mengumpulkan seluruh perubahan database dalam satu sesi bisnis sebelum dieksekusi atomik saat SaveChanges/Commit.",
                    "Gunakan HashSet untuk melacak entitas unik yang berubah."
                },
                explanation: "Pola Unit of Work menjaga konsistensi transaksional dengan mencatat mutasi entitas di memory tracker dan menembakkannya dalam satu transaksi database tunggal."
            );

            // lesson-11-01: ASP.NET Core Middleware Pipeline Builder
            dict["lesson-11-01"] = ChallengeBuilder.Create(
                id: "cc-11-01",
                title: "Simulasi Pipeline Middleware ASP.NET Core (Russian Doll Pattern)",
                difficulty: "Intermediate",
                problemStatement: "Simulasikan eksekusi middleware ASP.NET Core bergaya Russian Doll (bawang).\nSetiap middleware mencetak '<Nama>:IN' sebelum meneruskan ke middleware berikutnya (`next()`), lalu mencetak '<Nama>:OUT' saat eksekusi kembali.\nBaca daftar nama middleware dari stdin dipisahkan koma (misal 'Auth, Logging, Cors').\nDi tengah pipeline (terminal endpoint), cetak 'ENDPOINT:HANDLED'.\nCetak seluruh urutan logging pipeline.",
                inputFormat: "Satu baris daftar nama middleware dipisahkan koma.",
                outputFormat: "Urutan baris log eksekusi pipeline IN, ENDPOINT:HANDLED, dan OUT.",
                constraints: "Middleware >= 1.",
                examples: new[] {
                    ("Auth, Logging", "Auth:IN\nLogging:IN\nENDPOINT:HANDLED\nLogging:OUT\nAuth:OUT", "Urutan IN berurutan maju, OUT berurutan mundur (LIFO)."),
                    ("Cors", "Cors:IN\nENDPOINT:HANDLED\nCors:OUT", "Satu middleware membungkus endpoint.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Simulasikan pemanggilan rekursif / delegasi next() pipeline middleware.
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public static void ExecutePipeline(List<string> middlewares, int index)
    {
        if (index >= middlewares.Count)
        {
            Console.WriteLine(""ENDPOINT:HANDLED"");
            return;
        }

        string current = middlewares[index];
        Console.WriteLine($""{current}:IN"");
        ExecutePipeline(middlewares, index + 1);
        Console.WriteLine($""{current}:OUT"");
    }

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        var list = line.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        ExecutePipeline(list, 0);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Auth & Logging)", "Auth, Logging", "Auth:IN\nLogging:IN\nENDPOINT:HANDLED\nLogging:OUT\nAuth:OUT", false, "Example"),
                    ("Tiga Middleware", "Cors, Auth, Cache", "Cors:IN\nAuth:IN\nCache:IN\nENDPOINT:HANDLED\nCache:OUT\nAuth:OUT\nCors:OUT", false, "Normal"),
                    ("Satu Middleware", "RateLimit", "RateLimit:IN\nENDPOINT:HANDLED\nRateLimit:OUT", false, "Normal"),
                    ("Banyak Middleware", "A, B, C, D", "A:IN\nB:IN\nC:IN\nD:IN\nENDPOINT:HANDLED\nD:OUT\nC:OUT\nB:OUT\nA:OUT", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola middleware ASP.NET Core membentuk call-stack rekursif dua arah: Request Phase (IN) dan Response Phase (OUT).",
                    "Panggil fungsi secara rekursif ExecutePipeline(list, index + 1) sebagai representasi next()."
                },
                explanation: "Arsitektur middleware ASP.NET Core menghubungkan komponen pemroses HTTP request secara berantai, memungkinkan inspeksi, autentikasi, dan modifikasi response secara modular."
            );

            // lesson-11-04: REST Idempotency Store
            dict["lesson-11-04"] = ChallengeBuilder.Create(
                id: "cc-11-04",
                title: "Implementasi Penyimpanan Idempotency-Key pada REST API",
                difficulty: "Intermediate",
                problemStatement: "Untuk mencegah transaksi duplikat akibat network retry, REST API enterprise menggunakan header `Idempotency-Key`.\nBaca daftar request dari stdin: masing-masing berformat '<KEY>:<PAYLOAD>'.\nJika Idempotency-Key baru pertama kali diterima: simpan payload dan cetak 'PROCESSED: <Payload>'\nJika Idempotency-Key sudah pernah diproses sebelumnya: kembalikan payload lama dengan prefix 'CACHED: <Payload>'.",
                inputFormat: "Daftar request dipisahkan titik koma (';').",
                outputFormat: "Baris status proses atau cached per request.",
                constraints: "Key berupa UUID/string unik.",
                examples: new[] {
                    ("key-1:Order#100; key-1:Order#100", "PROCESSED: Order#100\nCACHED: Order#100", "Request kedua dengan key yang sama mengembalikan cache."),
                    ("k-1:PayA; k-2:PayB", "PROCESSED: PayA\nPROCESSED: PayB", "Dua idempotency key berbeda diproses sebagai dua transaksi baru.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Simpan request dalam Dictionary<string, string> untuk verifikasi idempotency.
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        var requests = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var store = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var req in requests)
        {
            var parts = req.Trim().Split(new[] { ':' }, 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string payload = parts[1].Trim();

                if (store.TryGetValue(key, out var cachedPayload))
                {
                    Console.WriteLine($""CACHED: {cachedPayload}"");
                }
                else
                {
                    store[key] = payload;
                    Console.WriteLine($""PROCESSED: {payload}"");
                }
            }
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Duplikat Idempotent)", "key-1:Order#100; key-1:Order#100", "PROCESSED: Order#100\nCACHED: Order#100", false, "Example"),
                    ("Dua Key Berbeda", "k1:Bayar 50rb; k2:Bayar 100rb", "PROCESSED: Bayar 50rb\nPROCESSED: Bayar 100rb", false, "Normal"),
                    ("Retry Berulang", "x:A; x:B; x:C", "PROCESSED: A\nCACHED: A\nCACHED: A", false, "Normal"),
                    ("Payload Identik Beda Key", "k1:Data; k2:Data", "PROCESSED: Data\nPROCESSED: Data", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan Dictionary<string, string> untuk menyimpan payload pertama berdasarkan key.",
                    "Gunakan TryGetValue(key, out var cached) untuk memeriksa keberadaan key."
                },
                explanation: "Header Idempotency-Key adalah standar industri pembayaran (seperti Stripe atau Payment Gateway perbankan) untuk menjamin pemrosesan mutasi dana tepat satu kali (exactly-once semantics)."
            );

            // lesson-12-02: Async CancellationToken Propagation
            dict["lesson-12-02"] = ChallengeBuilder.Create(
                id: "cc-12-02",
                title: "Propagasi Graceful Cancellation CancellationToken",
                difficulty: "Intermediate",
                problemStatement: "Simulasikan eksekusi task berantai yang memantau sinyal pembatalan `CancellationToken`.\nBaca dua angka dari stdin: TotalLangkah (int) dan BatasLangkahBatal (int).\nLakukan perulangan dari step 1 s/d TotalLangkah:\n- Jika step == BatasLangkahBatal, panggil `cancellationTokenSource.Cancel()`.\n- Pada setiap langkah, periksa `token.ThrowIfCancellationRequested()`.\nTangkap `OperationCanceledException` dan cetak 'CANCELLED_AT_STEP: <step>'.\nJika seluruh langkah selesai tanpa pembatalan, cetak 'COMPLETED_SUCCESSFULLY'.",
                inputFormat: "Satu baris berisi TotalLangkah dan BatasLangkahBatal (angka -1 jika tidak ada pembatalan).",
                outputFormat: "Satu baris status eksekusi task.",
                constraints: "TotalLangkah >= 1.",
                examples: new[] {
                    ("10 4", "CANCELLED_AT_STEP: 4", "Pembatalan terpicu tepat pada langkah 4."),
                    ("5 -1", "COMPLETED_SUCCESSFULLY", "Tidak ada sinyal pembatalan, task selesai.")
                },
                starterCode: @"using System;
using System.Threading;

public class Solution
{
    public static void Main()
    {
        // TODO: Gunakan CancellationTokenSource dan ThrowIfCancellationRequested.
        Console.WriteLine(""COMPLETED_SUCCESSFULLY"");
    }
}",
                solutionCode: @"using System;
using System.Threading;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 2) return;

        int totalSteps = int.Parse(p[0]);
        int cancelAt = int.Parse(p[1]);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        int currentStep = 0;

        try
        {
            for (int i = 1; i <= totalSteps; i++)
            {
                currentStep = i;
                if (i == cancelAt)
                {
                    cts.Cancel();
                }
                token.ThrowIfCancellationRequested();
            }
            Console.WriteLine(""COMPLETED_SUCCESSFULLY"");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($""CANCELLED_AT_STEP: {currentStep}"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Batal Langkah 4)", "10 4", "CANCELLED_AT_STEP: 4", false, "Example"),
                    ("Contoh 2 (Selesai Sukses)", "5 -1", "COMPLETED_SUCCESSFULLY", false, "Example"),
                    ("Batal Langkah 1", "5 1", "CANCELLED_AT_STEP: 1", false, "Normal"),
                    ("Batal di Langkah Terakhir", "10 10", "CANCELLED_AT_STEP: 10", true, "EdgeCase")
                },
                hints: new[] {
                    "CancellationToken adalah pola kooperatif: pemanggil memicu Cancel(), dan target mengecek ThrowIfCancellationRequested().",
                    "Tangkap OperationCanceledException secara eksplisit."
                },
                explanation: "Propagasi CancellationToken yang disiplin ke seluruh method asinkron mencegah kebocoran thread dan pemborosan CPU saat klien membatalkan HTTP request."
            );

            // lesson-12-03: Interlocked Thread-Safe Counter Rate Limiter
            dict["lesson-12-03"] = ChallengeBuilder.Create(
                id: "cc-12-03",
                title: "Implementasi Thread-Safe Rate Limiter dengan Interlocked",
                difficulty: "Intermediate",
                problemStatement: "Buat rate limiter fixed-window menggunakan `Interlocked.Increment`.\nBaca dua baris dari stdin:\nBaris 1: BatasMaksimumRequest (int, misal 5)\nBaris 2: Daftar request ID dipisahkan spasi ('req1 req2 req3 ...').\nUntuk setiap request, lakukan penambahan counter:\n- Jika counter <= Batas: cetak 'ACCEPTED: <reqId>'\n- Jika counter > Batas: cetak 'REJECTED: <reqId>'.",
                inputFormat: "Dua baris stdin: baris 1 kuota batas, baris 2 daftar ID request.",
                outputFormat: "Status ACCEPTED atau REJECTED per baris.",
                constraints: "Batas >= 1.",
                examples: new[] {
                    ("2\nr1 r2 r3", "ACCEPTED: r1\nACCEPTED: r2\nREJECTED: r3", "Hanya 2 request pertama yang diizinkan."),
                    ("1\nreqA reqB", "ACCEPTED: reqA\nREJECTED: reqB", "Batas kuota 1 hanya meloloskan reqA.")
                },
                starterCode: @"using System;
using System.Threading;

public class Solution
{
    private static int _requestCount = 0;
    public static void Main()
    {
        // TODO: Gunakan Interlocked.Increment untuk membatasi kuota request.
    }
}",
                solutionCode: @"using System;
using System.Threading;

public class Solution
{
    private static int _requestCount = 0;

    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2)) return;

        int limit = int.Parse(l1.Trim());
        var requests = l2.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var req in requests)
        {
            int current = Interlocked.Increment(ref _requestCount);
            if (current <= limit)
            {
                Console.WriteLine($""ACCEPTED: {req}"");
            }
            else
            {
                Console.WriteLine($""REJECTED: {req}"");
            }
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Limit 2)", "2\nr1 r2 r3", "ACCEPTED: r1\nACCEPTED: r2\nREJECTED: r3", false, "Example"),
                    ("Semua Diterima", "5\na b c", "ACCEPTED: a\nACCEPTED: b\nACCEPTED: c", false, "Normal"),
                    ("Limit 1", "1\nx y", "ACCEPTED: x\nREJECTED: y", false, "Normal"),
                    ("Banyak Request", "3\n1 2 3 4 5", "ACCEPTED: 1\nACCEPTED: 2\nACCEPTED: 3\nREJECTED: 4\nREJECTED: 5", true, "EdgeCase")
                },
                hints: new[] {
                    "Interlocked.Increment mengembalikan nilai integer setelah ditambahkan secara atomik.",
                    "Bandingkan nilai kembalian langsung dengan batas limit."
                },
                explanation: "Rate limiting berbasis Interlocked memberikan performa ultra-tinggi dalam skala jutaan RPS tanpa lock contention pada API Gateway."
            );

            // lesson-12-04: Channels Producer-Consumer
            dict["lesson-12-04"] = ChallengeBuilder.Create(
                id: "cc-12-04",
                title: "Simulasi Bounded Channel Producer-Consumer",
                difficulty: "Intermediate",
                problemStatement: "Simulasikan bounded queue berkapasitas tetap K dengan antrian pesan FIFO.\nBaca dua baris dari stdin:\nBaris 1: KapasitasK (int)\nBaris 2: Operasi dipisahkan koma: 'PUB:<pesan>' (publish) atau 'SUB' (consume).\n- 'PUB:<pesan>': jika antrian belum penuh (< K), tambahkan ke antrian dan cetak 'ENQUEUED: <pesan>'. Jika penuh, cetak 'DROPPED: <pesan>'.\n- 'SUB': jika ada pesan di antrian, ambil pesan paling depan dan cetak 'CONSUMED: <pesan>'. Jika kosong, cetak 'QUEUE_EMPTY'.",
                inputFormat: "Dua baris: baris 1 kapasitas antrian, baris 2 deretan operasi.",
                outputFormat: "Status ENQUEUED, DROPPED, CONSUMED, atau QUEUE_EMPTY per operasi.",
                constraints: "Kapasitas K >= 1.",
                examples: new[] {
                    ("2\nPUB:M1, PUB:M2, PUB:M3, SUB", "ENQUEUED: M1\nENQUEUED: M2\nDROPPED: M3\nCONSUMED: M1", "Pesan M3 di-drop karena kapasitas 2 penuh. SUB mengambil M1."),
                    ("1\nSUB, PUB:Data, SUB", "QUEUE_EMPTY\nENQUEUED: Data\nCONSUMED: Data", "SUB awal kosong, lalu enqueue data dan consume.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        // TODO: Simulasikan antrian BoundedChannel dengan Queue<string>.
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2)) return;

        int capacity = int.Parse(l1.Trim());
        var ops = l2.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var queue = new Queue<string>();

        foreach (var op in ops)
        {
            string t = op.Trim();
            if (t.StartsWith(""PUB:"", StringComparison.OrdinalIgnoreCase))
            {
                string msg = t.Substring(4).Trim();
                if (queue.Count < capacity)
                {
                    queue.Enqueue(msg);
                    Console.WriteLine($""ENQUEUED: {msg}"");
                }
                else
                {
                    Console.WriteLine($""DROPPED: {msg}"");
                }
            }
            else if (t.Equals(""SUB"", StringComparison.OrdinalIgnoreCase))
            {
                if (queue.Count > 0)
                {
                    string consumed = queue.Dequeue();
                    Console.WriteLine($""CONSUMED: {consumed}"");
                }
                else
                {
                    Console.WriteLine(""QUEUE_EMPTY"");
                }
            }
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Drop & Consume)", "2\nPUB:M1, PUB:M2, PUB:M3, SUB", "ENQUEUED: M1\nENQUEUED: M2\nDROPPED: M3\nCONSUMED: M1", false, "Example"),
                    ("Queue Kosong", "1\nSUB", "QUEUE_EMPTY", false, "Normal"),
                    ("Kapasitas Penuh Lalu Kosong", "1\nPUB:A, SUB, PUB:B", "ENQUEUED: A\nCONSUMED: A\nENQUEUED: B", false, "Normal"),
                    ("Banyak Publish", "2\nPUB:1, PUB:2, PUB:3, PUB:4", "ENQUEUED: 1\nENQUEUED: 2\nDROPPED: 3\nDROPPED: 4", true, "EdgeCase")
                },
                hints: new[] {
                    "System.Threading.Channels.Channel.CreateBounded<T>(capacity) menyediakan antrian producer-consumer dengan backpressure.",
                    "Gunakan Queue<string> untuk menyimulasikan bounded queue berkapasitas K."
                },
                explanation: "Channels di C# menyediakan primitif komunikasi asinkron antar thread/task berkinerja tinggi dengan kebijakan backpressure terstruktur saat consumer lebih lambat dari producer."
            );

            // lesson-13-03: Tactical DDD Aggregate Root
            dict["lesson-13-03"] = ChallengeBuilder.Create(
                id: "cc-13-03",
                title: "Penegakan Invariant Bisnis pada Aggregate Root (Tactical DDD)",
                difficulty: "Intermediate",
                problemStatement: "Buat Aggregate Root `OrderAggregate` dengan aturan invariant:\n1. Status awal adalah 'Draft'.\n2. Hanya order berstatus 'Draft' yang boleh menambahkan item (`AddItem(string name, decimal price)`).\n3. Transisi status: `Confirm()` hanya valid jika total item > 0 dan total harga > 0 (mengubah status ke 'Confirmed').\nBaca perintah dari stdin dipisahkan koma: 'ADD:ItemA:50000, ADD:ItemB:25000, CONFIRM'.\nCetak status akhir dan total harga: 'STATUS: <Status>, TOTAL: <Total>'.\nJika aturan invariant dilanggar, cetak 'INVARIANT_VIOLATION'.",
                inputFormat: "Satu baris perintah dipisahkan koma.",
                outputFormat: "Satu baris status dan total, atau 'INVARIANT_VIOLATION'.",
                constraints: "Price decimal >= 0.",
                examples: new[] {
                    ("ADD:Laptop:10000, CONFIRM", "STATUS: Confirmed, TOTAL: 10000", "Item ditambahkan saat Draft, lalu di-confirm."),
                    ("CONFIRM", "INVARIANT_VIOLATION", "Order kosong tidak boleh di-confirm.")
                },
                starterCode: @"using System;
using System.Globalization;

public class Solution
{
    public static void Main()
    {
        // TODO: Implementasikan Aggregate Root dengan invariant validation.
        Console.WriteLine(""STATUS: Draft, TOTAL: 0"");
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
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""STATUS: Draft, TOTAL: 0""); return; }

        string status = ""Draft"";
        decimal total = 0;
        int itemCount = 0;

        var ops = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var op in ops)
        {
            string t = op.Trim();
            if (t.StartsWith(""ADD:"", StringComparison.OrdinalIgnoreCase))
            {
                if (status != ""Draft"")
                {
                    Console.WriteLine(""INVARIANT_VIOLATION"");
                    return;
                }
                var p = t.Substring(4).Split(':');
                if (p.Length == 2 && decimal.TryParse(p[1], CultureInfo.InvariantCulture, out decimal price))
                {
                    itemCount++;
                    total += price;
                }
            }
            else if (t.Equals(""CONFIRM"", StringComparison.OrdinalIgnoreCase))
            {
                if (status != ""Draft"" || itemCount == 0 || total <= 0)
                {
                    Console.WriteLine(""INVARIANT_VIOLATION"");
                    return;
                }
                status = ""Confirmed"";
            }
        }

        Console.WriteLine($""STATUS: {status}, TOTAL: {total}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Valid Confirm)", "ADD:Laptop:10000, CONFIRM", "STATUS: Confirmed, TOTAL: 10000", false, "Example"),
                    ("Contoh 2 (Confirm Kosong)", "CONFIRM", "INVARIANT_VIOLATION", false, "Example"),
                    ("Add Setelah Confirm", "ADD:Item:100, CONFIRM, ADD:Extra:50", "INVARIANT_VIOLATION", false, "Normal"),
                    ("Draft Saja", "ADD:Book:5000", "STATUS: Draft, TOTAL: 5000", true, "EdgeCase")
                },
                hints: new[] {
                    "Dalam DDD, Aggregate Root adalah batas konsistensi transaksional.",
                    "Seluruh mutasi data harus melalui method publik Aggregate Root yang memvalidasi invariant bisnis."
                },
                explanation: "Tactical Domain-Driven Design memastikan setiap state transition pada aggregate root terlindungi oleh invariant bisnis yang ketat."
            );

            // lesson-14-04: Circuit Breaker State Machine
            dict["lesson-14-04"] = ChallengeBuilder.Create(
                id: "cc-14-04",
                title: "Simulasi State Machine Circuit Breaker (Polly Pattern)",
                difficulty: "Intermediate",
                problemStatement: "Simulasikan Circuit Breaker dengan 3 state: 'Closed', 'Open', dan 'HalfOpen'.\nAturan:\n- Awal: 'Closed', counter gagal = 0.\n- Jika state 'Closed':\n  - 'FAIL': counter gagal bertambah 1. Jika counter gagal >= 3, transisi ke 'Open'.\n  - 'SUCCESS': reset counter gagal ke 0.\n- Jika state 'Open': seluruh panggilan otomatis ditolak dan menghasilkan 'FAST_FAIL'. Jika menerima perintah 'RESET', transisi ke 'HalfOpen'.\n- Jika state 'HalfOpen':\n  - 'SUCCESS': transisi ke 'Closed', counter gagal = 0.\n  - 'FAIL': langsung transisi kembali ke 'Open'.\nBaca urutan event dari stdin dipisahkan koma: 'FAIL, FAIL, FAIL, CALL, RESET, SUCCESS'.\nCetak status state akhir Circuit Breaker: 'CIRCUIT_STATE: <State>'.",
                inputFormat: "Satu baris deretan event dipisahkan koma.",
                outputFormat: "Satu baris: 'CIRCUIT_STATE: <State>'.",
                constraints: "Event: FAIL, SUCCESS, CALL, RESET.",
                examples: new[] {
                    ("FAIL, FAIL, FAIL", "CIRCUIT_STATE: Open", "3 kali gagal berturut-turut membuka circuit breaker."),
                    ("FAIL, FAIL, FAIL, RESET, SUCCESS", "CIRCUIT_STATE: Closed", "Reset ke HalfOpen lalu Success menutup kembali circuit.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Simulasikan state machine Circuit Breaker.
        Console.WriteLine(""CIRCUIT_STATE: Closed"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""CIRCUIT_STATE: Closed""); return; }

        string state = ""Closed"";
        int failures = 0;

        var events = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var ev in events)
        {
            string e = ev.Trim().ToUpperInvariant();
            if (state == ""Closed"")
            {
                if (e == ""FAIL"")
                {
                    failures++;
                    if (failures >= 3) state = ""Open"";
                }
                else if (e == ""SUCCESS"")
                {
                    failures = 0;
                }
            }
            else if (state == ""Open"")
            {
                if (e == ""RESET"")
                {
                    state = ""HalfOpen"";
                }
            }
            else if (state == ""HalfOpen"")
            {
                if (e == ""SUCCESS"")
                {
                    state = ""Closed"";
                    failures = 0;
                }
                else if (e == ""FAIL"")
                {
                    state = ""Open"";
                }
            }
        }

        Console.WriteLine($""CIRCUIT_STATE: {state}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Open)", "FAIL, FAIL, FAIL", "CIRCUIT_STATE: Open", false, "Example"),
                    ("Contoh 2 (Pulih ke Closed)", "FAIL, FAIL, FAIL, RESET, SUCCESS", "CIRCUIT_STATE: Closed", false, "Example"),
                    ("Dua Kali Gagal Saja", "FAIL, FAIL, SUCCESS", "CIRCUIT_STATE: Closed", false, "Normal"),
                    ("HalfOpen Gagal Kembali Open", "FAIL, FAIL, FAIL, RESET, FAIL", "CIRCUIT_STATE: Open", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola Circuit Breaker mencegah aplikasi membanjiri downstream service yang sedang down dengan request yang pasti gagal.",
                    "Gunakan variabel string state dan switch/if kondisional per event."
                },
                explanation: "Circuit Breaker pattern (seperti yang disediakan library Polly di .NET) memutus aliran trafik saat downstream mengalami gangguan berulang, melindungi ketersediaan seluruh sistem."
            );

            // lesson-15-02: OAuth PKCE Code Challenge Generator
            dict["lesson-15-02"] = ChallengeBuilder.Create(
                id: "cc-15-02",
                title: "Implementasi Perhitungan Code Challenge PKCE SHA-256",
                difficulty: "Intermediate",
                problemStatement: "Pada OAuth 2.0 PKCE, klien menghasilkan `code_verifier` acak lalu menghitung `code_challenge` dengan rumus:\n`code_challenge = BASE64URL(SHA256(ASCII(code_verifier)))`.\nBase64Url: ubah Base64 standar dengan mengganti '+' menjadi '-', '/' menjadi '_', dan buang padding '=' di ujung.\nBaca satu string `code_verifier` dari stdin dan cetak `code_challenge` hasil komputasi.",
                inputFormat: "Satu baris string code_verifier.",
                outputFormat: "Satu baris string code_challenge Base64Url.",
                constraints: "Panjang verifier 43 s/d 128 karakter.",
                examples: new[] {
                    ("dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk", "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM", "Contoh resmi RFC 7636 Appendix B."),
                    ("secret_code_verifier_123456789012345678901234567890", "3eXAg7maz5PXVTxXXDeivvtOJe49_pQ8o0Qx6Kl6YYs", "Code verifier dengan padding angka.")
                },
                starterCode: @"using System;
using System.Security.Cryptography;
using System.Text;

public class Solution
{
    public static void Main()
    {
        string? verifier = Console.ReadLine();
        // TODO: Hitung SHA256 lalu ubah ke Base64Url encoding.
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;
using System.Security.Cryptography;
using System.Text;

public class Solution
{
    public static void Main()
    {
        string? verifier = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(verifier)) return;

        byte[] bytes = Encoding.ASCII.GetBytes(verifier);
        byte[] hash = SHA256.HashData(bytes);

        string base64 = Convert.ToBase64String(hash);
        string base64Url = base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');

        Console.WriteLine(base64Url);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (RFC 7636)", "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk", "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM", false, "Example"),
                    ("Verifier Sederhana", "secret_code_verifier_123456789012345678901234567890", "3eXAg7maz5PXVTxXXDeivvtOJe49_pQ8o0Qx6Kl6YYs", false, "Normal"),
                    ("Verifier Alfabet", "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJK", "cM46ZXqu670yDlJk6RaT8xj8sUvsVmyx2vplpDHbtiY", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan SHA256.HashData(bytes) di .NET 8.",
                    "Ubah Base64 standar: TrimEnd('='), Replace('+', '-'), Replace('/', '_')."
                },
                explanation: "PKCE (Proof Key for Code Exchange) RFC 7636 memitigasi serangan interception authorization code pada Single Page Application (SPA) dan mobile client."
            );

            // lesson-15-03: SSRF Private Network Guard
            dict["lesson-15-03"] = ChallengeBuilder.Create(
                id: "cc-15-03",
                title: "Pencegah Serangan Server-Side Request Forgery (SSRF Private IP Guard)",
                difficulty: "Intermediate",
                problemStatement: "Buat firewall pemvalidasi alamat IP target outgoing HTTP request untuk mencegah serangan SSRF.\nBaca satu alamat IPv4 dari stdin (misal '127.0.0.1' atau '8.8.8.8').\nBlokir ('BLOCKED_SSRF') jika IP termasuk dalam rentang jaringan privat atau loopback:\n- Loopback: `127.0.0.0/8` (dimulai 127.)\n- Private Class A: `10.0.0.0/8` (dimulai 10.)\n- Private Class B: `172.16.0.0/12` (172.16.x.x s/d 172.31.x.x)\n- Private Class C: `192.168.0.0/16` (dimulai 192.168.)\n- Link-Local: `169.254.0.0/16` (dimulai 169.254.)\nJika di luar rentang di atas, cetak 'ALLOWED_PUBLIC'.",
                inputFormat: "Satu baris alamat string IPv4.",
                outputFormat: "Satu baris: 'BLOCKED_SSRF' atau 'ALLOWED_PUBLIC'.",
                constraints: "Format IPv4 4 oktet integer.",
                examples: new[] {
                    ("127.0.0.1", "BLOCKED_SSRF", "Loopback internal dilarang diakses."),
                    ("93.184.216.34", "ALLOWED_PUBLIC", "IP publik aman diakses.")
                },
                starterCode: @"using System;
using System.Net;

public class Solution
{
    public static void Main()
    {
        string? ipStr = Console.ReadLine();
        // TODO: Periksa subnet privat dan loopback IPv4.
        Console.WriteLine(""BLOCKED_SSRF"");
    }
}",
                solutionCode: @"using System;
using System.Net;

public class Solution
{
    public static void Main()
    {
        string? ipStr = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(ipStr) || !IPAddress.TryParse(ipStr, out var ip))
        {
            Console.WriteLine(""BLOCKED_SSRF"");
            return;
        }

        byte[] b = ip.GetAddressBytes();
        if (b.Length != 4)
        {
            Console.WriteLine(""BLOCKED_SSRF"");
            return;
        }

        bool isPrivate =
            b[0] == 127 ||
            b[0] == 10 ||
            (b[0] == 172 && b[1] >= 16 && b[1] <= 31) ||
            (b[0] == 192 && b[1] == 168) ||
            (b[0] == 169 && b[1] == 254) ||
            b[0] == 0;

        Console.WriteLine(isPrivate ? ""BLOCKED_SSRF"" : ""ALLOWED_PUBLIC"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Loopback)", "127.0.0.1", "BLOCKED_SSRF", false, "Example"),
                    ("Contoh 2 (Publik)", "93.184.216.34", "ALLOWED_PUBLIC", false, "Example"),
                    ("Private Class A", "10.0.1.5", "BLOCKED_SSRF", false, "Normal"),
                    ("Private Class B", "172.20.0.1", "BLOCKED_SSRF", false, "Normal"),
                    ("Private Class C", "192.168.1.1", "BLOCKED_SSRF", false, "Normal"),
                    ("Public Google DNS", "8.8.8.8", "ALLOWED_PUBLIC", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan IPAddress.GetAddressBytes() untuk mengecek 4 byte oktet IP.",
                    "Uji oktet: b[0] == 127 (loopback), b[0] == 10, (b[0] == 172 && b[1] >= 16 && b[1] <= 31), dll."
                },
                explanation: "Proteksi SSRF wajib diterapkan pada backend yang melakukan panggilan HTTP keluar berdasarkan input URL pengguna untuk mencegah akses tidak sah ke metadata server cloud (169.254.169.254) atau intranet database internal."
            );

            // lesson-16-04: Kubernetes Probe Evaluator
            dict["lesson-16-04"] = ChallengeBuilder.Create(
                id: "cc-16-04",
                title: "Implementasi Evaluasi Probe K8s (Liveness & Readiness)",
                difficulty: "Intermediate",
                problemStatement: "Sebuah microservice mengevaluasi status kesehatan sistem untuk endpoint Kubernetes Probe:\nBaca status 3 komponen dari stdin dipisahkan spasi: DB (OK/FAIL), Cache (OK/FAIL), dan MemoryPressure (NORMAL/CRITICAL).\nAturan K8s:\n- Liveness Probe: 'LIVENESS_OK' selama MemoryPressure == 'NORMAL' (jika CRITICAL, cetak 'LIVENESS_FAIL' agar pod direstart).\n- Readiness Probe: 'READINESS_OK' jika Liveness == OK DAN DB == 'OK' (Cache boleh FAIL). Jika DB FAIL, cetak 'READINESS_FAIL'.\nCetak dua baris: baris 1 status Liveness, baris 2 status Readiness.",
                inputFormat: "Satu baris: '<DB> <Cache> <MemoryPressure>'.",
                outputFormat: "Dua baris hasil evaluasi Liveness dan Readiness.",
                constraints: "Status string token standar.",
                examples: new[] {
                    ("OK OK NORMAL", "LIVENESS_OK\nREADINESS_OK", "Semua komponen sehat, pod siap menerima trafik."),
                    ("FAIL OK NORMAL", "LIVENESS_OK\nREADINESS_FAIL", "DB gagal: pod tetap hidup tapi tidak menerima trafik.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Evaluasi Liveness dan Readiness Probe Kubernetes.
        Console.WriteLine(""LIVENESS_OK\nREADINESS_OK"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 3) return;

        string db = p[0].ToUpperInvariant();
        string cache = p[1].ToUpperInvariant();
        string mem = p[2].ToUpperInvariant();

        bool liveness = (mem == ""NORMAL"");
        bool readiness = liveness && (db == ""OK"");

        Console.WriteLine(liveness ? ""LIVENESS_OK"" : ""LIVENESS_FAIL"");
        Console.WriteLine(readiness ? ""READINESS_OK"" : ""READINESS_FAIL"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Sehat Semua)", "OK OK NORMAL", "LIVENESS_OK\nREADINESS_OK", false, "Example"),
                    ("Contoh 2 (DB Gangguan)", "FAIL OK NORMAL", "LIVENESS_OK\nREADINESS_FAIL", false, "Example"),
                    ("Memory Critical", "OK OK CRITICAL", "LIVENESS_FAIL\nREADINESS_FAIL", false, "Normal"),
                    ("Cache Saja yang Gangguan", "OK FAIL NORMAL", "LIVENESS_OK\nREADINESS_OK", true, "EdgeCase")
                },
                hints: new[] {
                    "Liveness probe menentukan apakah pod masih responsif atau perlu di-restart oleh kubelet.",
                    "Readiness probe menentukan apakah pod siap menerima request trafik dari Ingress/Service."
                },
                explanation: "Memisahkan Liveness Probe dari Readiness Probe sangat krusial: dependensi eksternal (DB) yang bermasalah tidak boleh memicu restart pod (liveness), melainkan cukup mencopot pod dari routing traffic (readiness)."
            );

            // lesson-18-03: Strangler Fig Routing Table
            dict["lesson-18-03"] = ChallengeBuilder.Create(
                id: "cc-17-06",
                title: "Implementasi Router Strangler Fig Pattern",
                difficulty: "Intermediate",
                problemStatement: "Pada migrasi sistem monolitik ke microservice, Strangler Fig router mengarahkan endpoint yang sudah dimigrasikan ke microservice baru dan sisanya ke legacy monolit.\nBaca daftar rute microservice dari baris 1 (dipisahkan koma, misal '/api/orders, /api/payments').\nBaca path request yang masuk di baris 2.\n- Jika path request diawali salah satu rute microservice yang telah dimigrasi, cetak 'ROUTE: MICROSERVICE'\n- Jika tidak cocok, cetak 'ROUTE: LEGACY_MONOLITH'.",
                inputFormat: "Dua baris stdin: baris 1 rute microservice, baris 2 request path.",
                outputFormat: "Satu baris: 'ROUTE: MICROSERVICE' atau 'ROUTE: LEGACY_MONOLITH'.",
                constraints: "Path diawali '/'.",
                examples: new[] {
                    ("/api/orders, /api/payments\n/api/orders/123", "ROUTE: MICROSERVICE", "Path /api/orders telah dimigrasi ke microservice baru."),
                    ("/api/orders, /api/payments\n/api/users/profile", "ROUTE: LEGACY_MONOLITH", "Path /api/users masih dilayani monolit.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Evaluasi rute Strangler Fig pattern.
        Console.WriteLine(""ROUTE: LEGACY_MONOLITH"");
    }
}",
                solutionCode: @"using System;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2)) return;

        var routes = l1.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToList();

        string path = l2.Trim();
        bool isMigrated = routes.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine(isMigrated ? ""ROUTE: MICROSERVICE"" : ""ROUTE: LEGACY_MONOLITH"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Microservice)", "/api/orders, /api/payments\n/api/orders/123", "ROUTE: MICROSERVICE", false, "Example"),
                    ("Contoh 2 (Monolith)", "/api/orders, /api/payments\n/api/users/profile", "ROUTE: LEGACY_MONOLITH", false, "Example"),
                    ("Cocok Tepat Rute", "/api/v2/catalog\n/api/v2/catalog", "ROUTE: MICROSERVICE", false, "Normal"),
                    ("Rute Root", "/api/v2\n/api/v1/legacy", "ROUTE: LEGACY_MONOLITH", true, "EdgeCase")
                },
                hints: new[] {
                    "Pola Strangler Fig secara bertahap mencekik monolit lama dengan memindahkan fitur per rute ke layanan baru.",
                    "Gunakan path.StartsWith(route, StringComparison.OrdinalIgnoreCase)."
                },
                explanation: "Strangler Fig Pattern adalah pola arsitektur utama untuk modernisasi sistem legacy tanpa downtime berisiko tinggi (big bang release)."
            );

            // lesson-18-04: Architecture Decision Record (ADR) Formatter & Validator
            dict["lesson-18-04"] = ChallengeBuilder.Create(
                id: "cc-17-07",
                title: "Validator dan Generator Format Architecture Decision Record (ADR)",
                difficulty: "Intermediate",
                problemStatement: "Sebuah Architecture Decision Record (ADR) enterprise memuat metadata keputusan arsitektur.\nBaca empat baris dari stdin:\nBaris 1: Nomor ADR (int)\nBaris 2: Judul Keputusan\nBaris 3: Status ('Proposed', 'Accepted', 'Superseded')\nBaris 4: Konteks & Keputusan ringkas.\nValidasi status: jika bukan salah satu dari ketiga status tersebut, jadikan 'Proposed'.\nCetak dokumen ADR format markdown:\n# ADR-{Nomor:D4}: {Judul}\nStatus: {Status}\n{Konteks}",
                inputFormat: "Empat baris stdin berturut-turut.",
                outputFormat: "Tiga baris string format ADR Markdown.",
                constraints: "Nomor ADR >= 1.",
                examples: new[] {
                    ("1\nAdopsi PostgreSQL sebagai RDBMS Utama\nAccepted\nMigrasi dari SQL Server untuk menghemat biaya lisensi.",
                     "# ADR-0001: Adopsi PostgreSQL sebagai RDBMS Utama\nStatus: Accepted\nMigrasi dari SQL Server untuk menghemat biaya lisensi.",
                     "Format ADR resmi dengan penomoran 4 digit."),
                    ("2\nImplementasi Event Sourcing\nDraft\nMemerlukan audit trail keuangan.",
                     "# ADR-0002: Implementasi Event Sourcing\nStatus: Proposed\nMemerlukan audit trail keuangan.",
                     "Status di luar Proposed/Accepted/Superseded dinormalisasi menjadi Proposed.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Format string ADR Markdown dengan nomor 4 digit (D4).
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? title = Console.ReadLine()?.Trim();
        string? status = Console.ReadLine()?.Trim();
        string? ctx = Console.ReadLine()?.Trim();

        if (!int.TryParse(l1, out int num)) return;

        string normStatus = status?.ToLowerInvariant() switch
        {
            ""accepted"" => ""Accepted"",
            ""superseded"" => ""Superseded"",
            _ => ""Proposed""
        };

        Console.WriteLine($""# ADR-{num:D4}: {title}"");
        Console.WriteLine($""Status: {normStatus}"");
        Console.WriteLine(ctx);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Postgres Accepted)",
                     "1\nAdopsi PostgreSQL sebagai RDBMS Utama\nAccepted\nMigrasi dari SQL Server untuk menghemat biaya lisensi.",
                     "# ADR-0001: Adopsi PostgreSQL sebagai RDBMS Utama\nStatus: Accepted\nMigrasi dari SQL Server untuk menghemat biaya lisensi.",
                     false, "Example"),
                    ("Status Invalid Menjadi Proposed",
                     "42\nPenggunaan Kafka\nDraft\nPembahasan awal message broker.",
                     "# ADR-0042: Penggunaan Kafka\nStatus: Proposed\nPembahasan awal message broker.",
                     false, "Normal"),
                    ("Nomor 3 Digit Format D4",
                     "120\nClean Architecture Migration\nsuperseded\nDigantikan oleh Vertical Slice.",
                     "# ADR-0120: Clean Architecture Migration\nStatus: Superseded\nDigantikan oleh Vertical Slice.",
                     false, "Normal"),
                    ("Nomor 4 Digit",
                     "1000\nZero Trust Network\nAccepted\nImplementasi mTLS antar service.",
                     "# ADR-1000: Zero Trust Network\nStatus: Accepted\nImplementasi mTLS antar service.",
                     true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan format string `{num:D4}` untuk menghasilkan angka dengan padding 4 digit (misal 1 -> 0001).",
                    "Gunakan switch expression untuk normalisasi status."
                },
                explanation: "Architecture Decision Record (ADR) adalah dokumen penting yang merekam konteks, konsekuensi, dan alasan teknis di balik keputusan penting arsitektur sistem organisasi."
            );

            return dict;
        }
    }
}
