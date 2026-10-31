using System.Collections.Generic;
using Academy.Api.Models;

namespace Academy.Api.Data.Curriculum
{
    public static class ProjectsDataRepository
    {
        public static List<ProjectSpecificationDto> GetAllProjects()
        {
            return new List<ProjectSpecificationDto>
            {
                new ProjectSpecificationDto
                {
                    Id = "proj-01",
                    Order = 1,
                    Title = "PROJECT 01 — Enterprise Console Calculator & Expression Evaluator",
                    Subtitle = "Variables, Operators, Control Flow, and Exception Handling in C#",
                    Difficulty = "Beginner",
                    Overview = "Membangun aplikasi CLI evaluator ekspresi matematika dengan parser operator precedence, variabel memori sementara, dan penanganan kesalahan divide-by-zero serta overflow yang aman.",
                    FunctionalRequirements = new List<string>
                    {
                        "Mendukung operasi aritmatika dasar (+, -, *, /, %, ^) dengan urutan prioritas matematis (BODMAS).",
                        "Mendukung tanda kurung (parentheses) berlapis untuk overriding precedence.",
                        "Menyimpan nilai ke variabel sementara (misal: MEM = 42, lalu MEM * 2).",
                        "Menangani input tidak valid, sintaks parsing error, dan pembagian dengan nol tanpa crash."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Zero-crash policy: seluruh exception tertangkap dengan pesan kesalahan yang ramah dalam Bahasa Indonesia.",
                        "Clean code architecture: pemisahan tokeniser, parser, dan evaluator.",
                        "Unit test coverage minimal 90% untuk operasi aritmatika dan edge cases."
                    },
                    ArchitectureOverview = "Layered CLI Architecture: Input Reader -> Tokenizer -> Reverse Polish Notation (Shunting-Yard) Parser -> Stack-Based Evaluator -> Output Formatter.",
                    DatabaseSchema = "In-memory Dictionary<string, double> untuk variabel state session.",
                    ApiSpecification = "CLI REPL Interface: menerima baris perintah, mengevaluasi, mencetak 'Result: <val>', atau mencetak pesan kesalahan terstruktur.",
                    StarterCode = @"using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(""=== ENTERPRISE CONSOLE CALCULATOR ==="");
        Console.WriteLine(""Ketik ekspresi (misal: 12 + 4 * 2) atau 'exit' untuk keluar."");
        
        // TODO: Implementasikan Shunting-Yard algorithm & Stack evaluator
        while (true)
        {
            Console.Write(""> "");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == ""exit"") break;
            
            try
            {
                double result = Evaluate(input);
                Console.WriteLine($""Hasil: {result}"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($""Error: {ex.Message}"");
            }
        }
    }

    public static double Evaluate(string expr)
    {
        // Placeholder implementasi
        return 0.0;
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Buat Tokenizer untuk memecah string input menjadi Number, Operator, dan Parenthesis tokens.",
                        "Langkah 2: Terapkan algoritma Dijkstra Shunting-Yard untuk mengonversi Infix ke Postfix (Reverse Polish Notation).",
                        "Langkah 3: Gunakan Stack<double> untuk mengevaluasi Postfix expression.",
                        "Langkah 4: Tambahkan pengecekan double.IsInfinity dan double.IsNaN untuk menangani DivideByZero.",
                        "Langkah 5: Tulis unit test untuk verifikasi operator precedence."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Ekspresi '2 + 3 * 4' menghasilkan 14 (bukan 20).",
                        "Ekspresi '(2 + 3) * 4' menghasilkan 20.",
                        "Ekspresi '10 / 0' menghasilkan pesan error 'Pembagian dengan nol tidak diizinkan' tanpa unhandled exception.",
                        "Semua test cases xUnit lolos 100%."
                    },
                    SecurityReview = "Validasi panjang ekspresi maksimum (maks 500 karakter) untuk mencegah stack overflow / denial of service.",
                    PerformanceReview = "Gunakan ReadOnlySpan<char> untuk tokenizing tanpa alokasi string berulang pada memori Heap."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-02",
                    Order = 2,
                    Title = "PROJECT 02 — Student Information & Academic Grading Engine",
                    Subtitle = "OOP, Encapsulation, Polymorphism, Collections & LINQ",
                    Difficulty = "Beginner",
                    Overview = "Sistem manajemen data akademik mahasiswa enterprise dengan perhitungan IPK otomatis, validasi SKS, enkapsulasi data nilai, dan pencarian menggunakan LINQ queries.",
                    FunctionalRequirements = new List<string>
                    {
                        "Pendaftaran mahasiswa dengan NIM unik, nama lengkap, dan program studi.",
                        "Pencatatan mata kuliah, bobot SKS, dan nilai numerik (0-100) serta konversi ke Grade huruf (A, B, C, D, E).",
                        "Kalkulasi Indeks Prestasi Kumulatif (IPK / GPA) berbobot SKS secara otomatis.",
                        "Pencarian mahasiswa dengan LINQ: Filter IPK tertinggi, mahasiswa berstatus 'Dean's List', dan mahasiswa butuh bimbingan."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Data Invariants dijaga melalui private setters dan validasi pada constructor.",
                        "Immutability diterapkan pada record CourseGrade.",
                        "Format tampilan tabel rapi pada console."
                    },
                    ArchitectureOverview = "Domain Model Layer: Student Aggregate, Course Entity, Enrollment Value Object, AcademicReportService.",
                    DatabaseSchema = "In-memory Generic Collections: List<Student>, Dictionary<string, Course>.",
                    ApiSpecification = "Domain Service Methods: RegisterStudent(nim, name), EnrollCourse(nim, courseId), RecordScore(nim, courseId, score), GenerateTranscript(nim).",
                    StarterCode = @"using System;
using System.Collections.Generic;
using System.Linq;

public record GradeRecord(string CourseCode, string CourseName, int Credits, double Score)
{
    public string LetterGrade => Score switch
    {
        >= 85 => ""A"",
        >= 75 => ""B"",
        >= 65 => ""C"",
        >= 50 => ""D"",
        _ => ""E""
    };

    public double GradePoint => LetterGrade switch
    {
        ""A"" => 4.0,
        ""B"" => 3.0,
        ""C"" => 2.0,
        ""D"" => 1.0,
        _ => 0.0
    };
}

public class Student
{
    public string Nim { get; }
    public string Name { get; private set; }
    private readonly List<GradeRecord> _grades = new();

    public Student(string nim, string name)
    {
        if (string.IsNullOrWhiteSpace(nim)) throw new ArgumentException(""NIM wajib diisi"");
        Nim = nim;
        Name = name;
    }

    public void AddGrade(GradeRecord grade) => _grades.Add(grade);
    public IReadOnlyList<GradeRecord> Grades => _grades.AsReadOnly();

    public double CalculateGpa()
    {
        if (!_grades.Any()) return 0.0;
        int totalCredits = _grades.Sum(g => g.Credits);
        if (totalCredits == 0) return 0.0;
        double totalWeightedPoints = _grades.Sum(g => g.GradePoint * g.Credits);
        return Math.Round(totalWeightedPoints / totalCredits, 2);
    }
}

public class Program
{
    public static void Main()
    {
        var s = new Student(""13520001"", ""Galih Wicaksono"");
        s.AddGrade(new GradeRecord(""CS101"", ""Dasar Pemrograman"", 3, 90));
        s.AddGrade(new GradeRecord(""CS102"", ""Struktur Data"", 4, 86));
        Console.WriteLine($""Mahasiswa: {s.Name} (NIM: {s.Nim}) - IPK: {s.CalculateGpa()}"");
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Rancang class Course dan record GradeRecord dengan perhitungan point nilai.",
                        "Langkah 2: Buat aggregate Student dengan enkapsulasi List nilai.",
                        "Langkah 3: Terapkan LINQ queries untuk pencarian dan agregasi statistik kelas (Rata-rata kelas, Mahasiswa terbaik).",
                        "Langkah 4: Tulis unit test untuk validasi perhitungan IPK berbobot SKS."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Mahasiswa dengan 3 SKS nilai 90 (Grade A = 4.0) dan 4 SKS nilai 86 (Grade A = 4.0) menghasilkan IPK 4.0.",
                        "Pencatatan SKS tidak boleh negatif atau nol.",
                        "LINQ query mampu mengembalikan daftar 5 mahasiswa teratas berdasarkan IPK."
                    },
                    SecurityReview = "Sanitasi nama mahasiswa untuk menghindari karakter kontrol yang merusak formatting.",
                    PerformanceReview = "Gunakan `IReadOnlyList<T>` daripada mengekspos `List<T>` langsung untuk mencegah modifikasi state tidak terkontrol."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-03",
                    Order = 3,
                    Title = "PROJECT 03 — High-Performance Log File Analyzer & Aggregator CLI",
                    Subtitle = "File I/O Streams, Span<T>, Regular Expressions & Custom Data Structures",
                    Difficulty = "Beginner",
                    Overview = "Membangun engine pemroses log server berskala gigabyte menggunakan streaming I/O rendah memori (`StreamReader`, `ReadOnlySpan<char>`) untuk mendeteksi anomali, menghitung distribusi kode status HTTP, dan mengidentifikasi endpoint paling lambat.",
                    FunctionalRequirements = new List<string>
                    {
                        "Membaca baris log berformat standar (Timestamp, Level, Endpoint, StatusCode, LatencyMs) secara streaming.",
                        "Menghitung persentase error rate (5xx dan 4xx) serta persentil latency (p50, p95, p99).",
                        "Mendeteksi IP address dengan frekuensi request mencurigakan di atas ambang batas.",
                        "Mengekspor laporan ringkasan ke format JSON terstruktur."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Alokasi memori konstan O(1) terhadap ukuran file (tidak memuat seluruh file ke RAM sekaligus).",
                        "Kecepatan parsing minimal 100.000 baris per detik menggunakan slicing `ReadOnlySpan<char>`.",
                        "Penanganan baris log korup/malformed tanpa menghentikan proses agregasi."
                    },
                    ArchitectureOverview = "Streaming Pipeline: Buffered File Stream -> Zero-Allocation Span Parser -> In-Memory Histogram Aggregator -> JSON Report Writer.",
                    DatabaseSchema = "In-memory Dictionary<string, EndpointMetrics> dan PriorityQueue<string, long> untuk Top-K slowest endpoints.",
                    ApiSpecification = "CLI Command: log-analyzer --input access.log --threshold-ms 500 --output report.json",
                    StarterCode = @"using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        string[] sampleLogs = {
            ""2026-10-31T08:00:01Z|INFO|/api/orders|200|18"",
            ""2026-10-31T08:00:02Z|ERROR|/api/payments|503|1420"",
            ""2026-10-31T08:00:03Z|INFO|/api/orders|200|24"",
            ""2026-10-31T08:00:04Z|WARN|/api/auth|429|5""
        };

        int errorCount = 0;
        long totalLatency = 0;

        foreach (string line in sampleLogs)
        {
            ReadOnlySpan<char> span = line.AsSpan();
            if (line.Contains(""|ERROR|"")) errorCount++;
            int lastPipe = span.LastIndexOf('|');
            if (lastPipe > 0 && long.TryParse(span.Slice(lastPipe + 1), out long ms))
            {
                totalLatency += ms;
            }
        }

        Console.WriteLine($""Total Log: {sampleLogs.Length} | Error 5xx: {errorCount} | Rata-rata Latency: {totalLatency / sampleLogs.Length} ms"");
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Gunakan `ReadOnlySpan<char>` untuk memecah kolom log berdasarkan delimiter `|` tanpa `string.Split`.",
                        "Langkah 2: Akumulasikan metrik per endpoint ke dalam struktur `Dictionary<string, EndpointStats>`.",
                        "Langkah 3: Hitung persentil p95/p99 dan cetak ringkasan ke konsol."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Log korup dilewati dan dicatat pada counter `MalformedLines` tanpa melempar exception.",
                        "Perhitungan jumlah error 5xx dan rata-rata latency akurat 100%."
                    },
                    SecurityReview = "Cegah Path Traversal pada argumen path file input dan batasi akses hanya pada direktori log yang diizinkan.",
                    PerformanceReview = "Hindari `string.Split('|')` pada loop jutaan baris karena akan memicu alokasi Gen 0 GC berlebih."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-04",
                    Order = 4,
                    Title = "PROJECT 04 — Enterprise Inventory Management REST API",
                    Subtitle = "ASP.NET Core 8, Minimal APIs, Entity Framework Core & SQLite/PostgreSQL",
                    Difficulty = "Intermediate",
                    Overview = "RESTful API manajemen inventaris gudang enterprise dengan pelacakan stok produk, mutasi barang (stock in / stock out), pagination, concurrency conflict prevention, dan OpenAPI/Swagger documentation.",
                    FunctionalRequirements = new List<string>
                    {
                        "CRUD Produk (SKU, Nama, Kategori, Harga, Minimum Stok, Jumlah Stok).",
                        "Operasi Stock In (Penerimaan barang dari supplier) dan Stock Out (Pengiriman pesanan).",
                        "Filter, Sorting, dan Keyset/Offset Pagination pada katalog produk.",
                        "Peringatan otomatis untuk produk yang berada di bawah level Minimum Stock."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Pencegahan race condition pada pengurangan stok (Concurrency Token / RowVersion).",
                        "Standar respon error menggunakan RFC 7807 ProblemDetails.",
                        "Response time endpoint di bawah 50ms untuk operasi read katalog."
                    },
                    ArchitectureOverview = "Clean Web API Architecture: Controllers/Endpoints -> Application Services -> EF Core Repository -> Database Engine.",
                    DatabaseSchema = "Tabel Products (Id, Sku, Name, Price, Quantity, RowVersion), StockTransactions (Id, ProductId, Type, Quantity, Timestamp, Reason).",
                    ApiSpecification = "GET /api/products, POST /api/products, GET /api/products/{id}, POST /api/products/{id}/adjust-stock.",
                    StarterCode = @"using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<InventoryDb>(opt => opt.UseSqlite(""Data Source=inventory.db""));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet(""/api/products"", async (InventoryDb db) => 
    await db.Products.AsNoTracking().ToListAsync());

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public byte[]? RowVersion { get; set; }
}

public class InventoryDb : DbContext
{
    public InventoryDb(DbContextOptions<InventoryDb> options) : base(options) {}
    public DbSet<Product> Products => Set<Product>();
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Setup project ASP.NET Core dengan EF Core SQLite.",
                        "Langkah 2: Definisikan relasi Product dan StockTransaction.",
                        "Langkah 3: Konfigurasikan concurrency token pada kolom RowVersion/Timestamp.",
                        "Langkah 4: Buat endpoint adjust-stock dengan blok transaksi database terisolasi.",
                        "Langkah 5: Tulis integration test menggunakan WebApplicationFactory."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Dua request pengurangan stok yang terjadi bersamaan tidak boleh membuat stok negatif.",
                        "Respon 409 Conflict dikembalikan ketika terjadi concurrency collision.",
                        "Swagger UI dapat dibuka dan dieksekusi dengan baik."
                    },
                    SecurityReview = "Validasi SKU unik dan batasi ukuran input string untuk mencegah payload berlebih.",
                    PerformanceReview = "Gunakan AsNoTracking() pada semua endpoint read query untuk meniadakan alokasi EF Core change tracker."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-05",
                    Order = 5,
                    Title = "PROJECT 05 — Centralized Identity, JWT & RBAC Authentication Server",
                    Subtitle = "ASP.NET Core Security, Argon2/BCrypt Hashing, JWT Access/Refresh Tokens & RBAC",
                    Difficulty = "Intermediate",
                    Overview = "Membangun layanan autentikasi dan otorisasi terpusat dengan rotasi Refresh Token, pencabutan token (revocation), password hashing aman, serta Role-Based & Policy-Based Access Control.",
                    FunctionalRequirements = new List<string>
                    {
                        "Registrasi pengguna dengan validasi kekuatan password dan hashing salted.",
                        "Login menghasilkan short-lived JWT Access Token (15 menit) dan Refresh Token terenkripsi.",
                        "Rotasi Refresh Token otomatis dengan deteksi penggunaan ulang (Token Replay Detection).",
                        "Otorisasi endpoint berbasis Role (Admin, Auditor, Customer) dan Custom Claims Policy."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Password tidak pernah disimpan dalam bentuk plain-text atau hash lemah (MD5/SHA1 dilarang).",
                        "Pencegahan Brute-Force melalui Account Lockout setelah 5 kali gagal login berturut-turut.",
                        "Audit log untuk setiap peristiwa login sukses, gagal, dan pencabutan token."
                    },
                    ArchitectureOverview = "Security Pipeline: Rate Limiter -> Auth Controller -> Identity Domain Service -> Token Provider (HMAC-SHA256 / RSA) -> Token Store.",
                    DatabaseSchema = "Users (Id, Email, PasswordHash, Role, FailedLoginCount, LockoutEnd), RefreshTokens (Id, UserId, TokenHash, ExpiresAt, RevokedAt).",
                    ApiSpecification = "POST /api/auth/register, POST /api/auth/login, POST /api/auth/refresh, POST /api/auth/revoke.",
                    StarterCode = @"using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main()
    {
        string password = ""EnterpriseSecret#2026"";
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations: 100_000,
            HashAlgorithmName.SHA256,
            outputLength: 32);

        Console.WriteLine(""=== IDENTITY SECURITY ENGINE ==="");
        Console.WriteLine($""Salt (Base64): {Convert.ToBase64String(salt)}"");
        Console.WriteLine($""PBKDF2-SHA256: {Convert.ToBase64String(hash)}"");
        Console.WriteLine(""Verifikasi Konstanta-Waktu: "" + CryptographicOperations.FixedTimeEquals(hash, hash));
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Implementasikan PasswordHasher menggunakan PBKDF2/Argon2 dengan salt acak kriptografis.",
                        "Langkah 2: Buat JwtTokenGenerator dengan klaim `sub`, `role`, `jti`, dan `exp`.",
                        "Langkah 3: Implementasikan penyimpanan dan rotasi Refresh Token di database."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Verifikasi hash menggunakan `CryptographicOperations.FixedTimeEquals` untuk mencegah Timing Attack.",
                        "Refresh token yang sudah dipakai tidak dapat digunakan kedua kalinya."
                    },
                    SecurityReview = "Gunakan `CryptographicOperations.FixedTimeEquals` dan simpan signing key di luar source code (Secret Manager / Vault).",
                    PerformanceReview = "Validasi JWT dilakukan secara stateless di memori tanpa query database pada setiap request."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-06",
                    Order = 6,
                    Title = "PROJECT 06 — Enterprise Order Processing System",
                    Subtitle = "Clean Architecture, CQRS, Domain-Driven Design & Unit of Work",
                    Difficulty = "Advanced",
                    Overview = "Sistem pemrosesan pesanan enterprise menggunakan Clean Architecture, CQRS (MediatR pattern), validasi FluentValidation, dan domain events.",
                    FunctionalRequirements = new List<string>
                    {
                        "Pembuatan pesanan dengan validasi item ketersediaan dan total harga agregat.",
                        "State machine status pesanan: Pending -> Confirmed -> Shipped -> Delivered / Cancelled.",
                        "Pembatalan pesanan mengembalikan alokasi reservasi stok.",
                        "Penerbitan OrderCreatedDomainEvent untuk memicu notifikasi customer."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Domain Layer independen dari framework dan database (POCO entities).",
                        "Idempotency key wajib disertakan pada pembuatan order untuk mencegah duplicate orders.",
                        "Audit logging setiap transisi state pesanan."
                    },
                    ArchitectureOverview = "Clean Architecture 4-Layer: Domain (Aggregates, Events) -> Application (Commands, Queries, Handlers) -> Infrastructure (EF Core, Repositories) -> API (Endpoints).",
                    DatabaseSchema = "Orders, OrderItems, OrderAuditLogs, IdempotencyRecords.",
                    ApiSpecification = "POST /api/orders, GET /api/orders/{id}, POST /api/orders/{id}/cancel.",
                    StarterCode = @"// Domain Aggregate Root
public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();

    private Order() {} // EF Core constructor

    public static Order Create(string customerId, List<OrderItem> items)
    {
        if (items == null || !items.Any()) 
            throw new InvalidOperationException(""Order harus memiliki minimal 1 item"");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending
        };
        order._items.AddRange(items);
        return order;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(""Hanya order Pending yang dapat dikonfirmasi"");
        Status = OrderStatus.Confirmed;
    }
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }
public record OrderItem(string ProductId, int Quantity, decimal UnitPrice);",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Implementasikan Order Aggregate Root dengan invariants bisnis yang ketat.",
                        "Langkah 2: Buat CreateOrderCommand dan CreateOrderCommandHandler di Application layer.",
                        "Langkah 3: Integrasikan Idempotency Filter untuk mencegat request duplikat.",
                        "Langkah 4: Konfigurasikan EF Core Fluent API mapping pada Infrastructure layer.",
                        "Langkah 5: Tulis Unit Test untuk Domain Rules dan Integration Test untuk API."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Tidak mungkin membuat order tanpa item (Domain Invariant dilindungi).",
                        "Pengiriman request pembuatan order dengan Idempotency-Key yang sama mengembalikan order yang sudah ada tanpa menduplikasi data.",
                        "Transisi state dari Delivered ke Cancelled ditolak dengan pesan error yang tepat."
                    },
                    SecurityReview = "Validasi kepemilikan order: Pengguna hanya boleh membaca atau membatalkan order milik mereka sendiri.",
                    PerformanceReview = "Gunakan Dapper untuk query baca (Queries) dan EF Core untuk mutasi tulis (Commands) sesuai prinsip CQRS."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-07",
                    Order = 7,
                    Title = "PROJECT 07 — High-Concurrency Flash Sale Transaction Engine",
                    Subtitle = "Multi-Threading, Race Condition Mitigation, Redis/In-Memory Atomic Locks",
                    Difficulty = "Expert",
                    Overview = "Engine transaksi flash sale berkursi/berstok terbatas yang menangani ribuan request konkuren per detik tanpa overselling dan tanpa race condition.",
                    FunctionalRequirements = new List<string>
                    {
                        "Manajemen stok terbatas (contoh: 100 tiket konser).",
                        "Reservasi tiket dengan lock time 10 menit (TTL).",
                        "Penyelesaian pembayaran untuk mengonfirmasi kepemilikan tiket permanen.",
                        "Pelepasan otomatis tiket yang tidak dibayar setelah TTL habis."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Zero-overselling guarantee: tidak boleh ada tiket ke-101 yang berhasil terjual di bawah load apapun.",
                        "Throughput minimal 2.000 RPS dengan p99 latency di bawah 20ms.",
                        "Bebas dari distributed deadlocks."
                    },
                    ArchitectureOverview = "In-Memory Atomic CAS / Redis Lua Scripting -> Queue Worker -> SQL Ledger Confirmation.",
                    DatabaseSchema = "FlashSaleItems (Id, TotalStock, ReservedStock, SoldStock), Reservations (Id, ItemId, UserId, ExpiresAt, Status).",
                    ApiSpecification = "POST /api/flash-sale/{itemId}/reserve, POST /api/flash-sale/{reservationId}/pay.",
                    StarterCode = @"using System;
using System.Threading;
using System.Threading.Tasks;

public class AtomicStockManager
{
    private int _remainingStock;

    public AtomicStockManager(int initialStock)
    {
        _remainingStock = initialStock;
    }

    public bool TryAcquireItem()
    {
        while (true)
        {
            int current = Volatile.Read(ref _remainingStock);
            if (current <= 0) return false; // Habis

            // Compare-And-Swap (CAS) atomic operation
            int original = Interlocked.CompareExchange(ref _remainingStock, current - 1, current);
            if (original == current)
            {
                return true; // Berhasil mendapatkan barang secara thread-safe
            }
        }
    }

    public int CurrentStock => Volatile.Read(ref _remainingStock);
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Implementasikan AtomicStockManager menggunakan `Interlocked.CompareExchange`.",
                        "Langkah 2: Buat load test simulator dengan Parallel.For untuk 10.000 thread berebut 100 stok.",
                        "Langkah 3: Integrasikan Timer / BackgroundService untuk expire reservasi yang kadaluarsa.",
                        "Langkah 4: Simpan hasil pembelian yang berhasil ke database dengan transaksi ACID."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Ketika 10.000 user memperebutkan 100 stok secara serentak, tepat 100 user berhasil dan stok akhir tepat 0.",
                        "Tidak terjadi deadlock atau thread starvation.",
                        "Log audit mencatat urutan timestamp setiap transaksi secara presisi."
                    },
                    SecurityReview = "Terapkan per-user rate limiting (token bucket) untuk mencegah bot farming.",
                    PerformanceReview = "Gunakan atomic lock-free CAS primitives untuk hot path alokasi stok daripada lock mutex berat."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-08",
                    Order = 8,
                    Title = "PROJECT 08 — Reliable Transactional Outbox & Event-Driven Dispatcher",
                    Subtitle = "Outbox Pattern, System.Threading.Channels, At-Least-Once Delivery & Idempotent Consumers",
                    Difficulty = "Advanced",
                    Overview = "Membangun infrastruktur pengiriman pesan asinkronus bebas kehilangan data (Zero Dual-Write Problem) menggunakan pola Transactional Outbox, antrean berkecepatan tinggi `Channel<T>`, dan mekanisme Exponential Backoff dengan Dead-Letter Queue (DLQ).",
                    FunctionalRequirements = new List<string>
                    {
                        "Menyimpan mutasi bisnis dan event OutboxMessage dalam satu transaksi database ACID yang sama.",
                        "Background worker mem-polling OutboxMessage yang belum terkirim dan mempublikasikannya ke broker.",
                        "Retry otomatis dengan Exponential Backoff + Jitter ketika broker sedang gangguan.",
                        "Idempotent Consumer yang mendeteksi dan mengabaikan pengiriman pesan duplikat berdasarkan MessageId."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Jaminan pengiriman At-Least-Once dengan efek pemrosesan Exactly-Once di sisi konsumen.",
                        "Lag pengiriman pesan di bawah 100ms pada kondisi normal.",
                        "Pesan yang gagal lebih dari 5 kali dipindahkan ke Dead-Letter Queue (DLQ) untuk inspeksi."
                    },
                    ArchitectureOverview = "Transactional DB Write -> Outbox Table -> Polling Relay Worker -> System.Threading.Channels -> Idempotent Event Handler.",
                    DatabaseSchema = "OutboxMessages (Id, EventType, PayloadJson, CreatedAt, ProcessedAt, RetryCount, LastError), InboxProcessedMessages (MessageId, ProcessedAt).",
                    ApiSpecification = "POST /api/events/publish, GET /api/outbox/pending, POST /api/outbox/retry-dlq.",
                    StarterCode = @"using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

public record OutboxEvent(Guid MessageId, string EventType, string Payload);

public class Program
{
    public static async Task Main()
    {
        var channel = Channel.CreateBounded<OutboxEvent>(100);
        var processedIds = new HashSet<Guid>();

        var msgId = Guid.NewGuid();
        await channel.Writer.WriteAsync(new OutboxEvent(msgId, ""OrderPaid"", ""{ \""OrderId\"": 101 }""));
        await channel.Writer.WriteAsync(new OutboxEvent(msgId, ""OrderPaid"", ""{ \""OrderId\"": 101 }"")); // Duplicate
        channel.Writer.Complete();

        await foreach (var evt in channel.Reader.ReadAllAsync())
        {
            if (!processedIds.Add(evt.MessageId))
            {
                Console.WriteLine($""[IDEMPOTENT SKIP] Pesan duplikat diabaikan: {evt.MessageId}"");
                continue;
            }
            Console.WriteLine($""[DISPATCHED] {evt.EventType}: {evt.Payload}"");
        }
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Buat entitas OutboxMessage yang disimpan bersama entitas domain dalam satu `SaveChangesAsync()`.",
                        "Langkah 2: Bangun BackgroundService yang memproses antrean menggunakan `Channel<OutboxEvent>`.",
                        "Langkah 3: Terapkan pengecekan `InboxProcessedMessages` untuk menjamin idempotensi konsumen."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Tidak ada event yang hilang meskipun message broker mati saat transaksi bisnis berlangsung.",
                        "Pengiriman pesan duplikat hanya dieksekusi satu kali oleh konsumen."
                    },
                    SecurityReview = "Enkripsi field sensitif pada `PayloadJson` di tabel Outbox jika memuat informasi PII.",
                    PerformanceReview = "Gunakan batch update dan index terfilter (`WHERE ProcessedAt IS NULL`) pada tabel Outbox."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-09",
                    Order = 9,
                    Title = "PROJECT 09 — Distributed API Gateway, Rate Limiter & Circuit Breaker",
                    Subtitle = "Sliding Window Rate Limiting, Token Bucket, Polly Resilience & Reverse Proxy",
                    Difficulty = "Advanced",
                    Overview = "Membangun gateway pengaman trafik backend enterprise yang dilengkapi algoritma Token Bucket / Sliding Window Rate Limiter per API Key, Circuit Breaker otomatis saat layanan downstream melambat, serta korelasi request terdistribusi.",
                    FunctionalRequirements = new List<string>
                    {
                        "Pembatasan kuota request per klien (misal: 100 request per menit untuk tier Standard, 1000 untuk Enterprise).",
                        "Mengembalikan status HTTP 429 Too Many Requests beserta header `Retry-After` yang presisi.",
                        "Circuit Breaker otomatis membuka jalur (Open State) jika rasio kegagalan downstream melewati 50%.",
                        "Injeksi header `X-Correlation-Id` pada setiap request yang diteruskan."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Overhead pemeriksaan rate limit di bawah 1 milidetik.",
                        "Thread-safe di bawah ribuan request paralel per detik.",
                        "Fallback response terstruktur saat Circuit Breaker sedang berstatus Open."
                    },
                    ArchitectureOverview = "Client Request -> Correlation Middleware -> Token Bucket Rate Limiter -> Circuit Breaker Guard -> Upstream Service.",
                    DatabaseSchema = "In-Memory ConcurrentDictionary<string, TokenBucketState> / Redis Atomic Counters.",
                    ApiSpecification = "ANY /gateway/{service}/{**path} dengan Header X-Api-Key.",
                    StarterCode = @"using System;

public class TokenBucketLimiter
{
    private readonly int _capacity;
    private double _tokens;
    private DateTime _lastRefill;
    private readonly object _sync = new();

    public TokenBucketLimiter(int capacity)
    {
        _capacity = capacity;
        _tokens = capacity;
        _lastRefill = DateTime.UtcNow;
    }

    public bool TryConsume()
    {
        lock (_sync)
        {
            if (_tokens >= 1.0)
            {
                _tokens -= 1.0;
                return true;
            }
            return false;
        }
    }
}

public class Program
{
    public static void Main()
    {
        var limiter = new TokenBucketLimiter(capacity: 3);
        for (int i = 1; i <= 5; i++)
        {
            bool allowed = limiter.TryConsume();
            Console.WriteLine($""Request #{i}: {(allowed ? ""200 OK (Allowed)"" : ""429 Too Many Requests"")}"");
        }
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Implementasikan algoritma Token Bucket yang thread-safe.",
                        "Langkah 2: Buat state machine Circuit Breaker (Closed -> Open -> Half-Open).",
                        "Langkah 3: Integrasikan sebagai middleware pipeline ASP.NET Core."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Request yang melebihi kapasitas bucket langsung ditolak dengan status 429.",
                        "Circuit Breaker mencegah cascading failure ke service downstream yang sedang mengalami gangguan."
                    },
                    SecurityReview = "Gunakan IP + API Key fingerprinting untuk memitigasi serangan DDoS terdistribusi.",
                    PerformanceReview = "Hindari alokasi objek baru di hot-path middleware untuk menjaga tekanan GC tetap rendah."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-10",
                    Order = 10,
                    Title = "PROJECT 10 — Multi-Tenant SaaS Billing & Subscription Metering Engine",
                    Subtitle = "Multi-Tenancy Isolation, Global Query Filters, Usage Metering & Invoice Generation",
                    Difficulty = "Expert",
                    Overview = "Merancang dan membangun mesin tagihan SaaS multi-tenant dengan isolasi data tingkat baris (`TenantId` Global Query Filter), pencatatan pemakaian kuota API secara real-time, dan pembuatan invoice bulanan otomatis.",
                    FunctionalRequirements = new List<string>
                    {
                        "Isolasi data ketat antar-tenant menggunakan EF Core Global Query Filter berdasarkan konteks `TenantId`.",
                        "Pencatatan metrik pemakaian (API calls, penyimpanan GB, jumlah kursi aktif) per tenant.",
                        "Kalkulasi tagihan berjenjang (Tiered Pricing) beserta pajak dan diskon prorata.",
                        "Pencegahan kebocoran data lintas-tenant (Cross-Tenant Data Leak Zero Tolerance)."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Setiap query database wajib secara otomatis terfilter oleh `TenantId` aktif.",
                        "Perhitungan mata uang menggunakan tipe `decimal` dengan pembulatan finansial `MidpointRounding.ToEven`.",
                        "Audit log pembuatan dan perubahan status invoice."
                    },
                    ArchitectureOverview = "Tenant Resolution Middleware -> ITenantContext -> EF Core DbContext (Global Query Filter) -> Billing Calculation Engine.",
                    DatabaseSchema = "Tenants (Id, Name, PlanTier), UsageRecords (Id, TenantId, MetricName, Quantity, RecordedAt), Invoices (Id, TenantId, Subtotal, Tax, Total, Status).",
                    ApiSpecification = "POST /api/billing/usage, POST /api/billing/invoices/generate, GET /api/billing/invoices.",
                    StarterCode = @"using System;
using System.Collections.Generic;
using System.Linq;

public record UsageItem(string TenantId, int ApiCalls);

public class Program
{
    public static void Main()
    {
        var records = new List<UsageItem>
        {
            new(""tenant-alpha"", 12_500),
            new(""tenant-beta"", 3_200),
            new(""tenant-alpha"", 7_500)
        };

        string currentTenant = ""tenant-alpha"";
        int totalCalls = records
            .Where(r => r.TenantId == currentTenant)
            .Sum(r => r.ApiCalls);

        decimal invoiceAmount = totalCalls * 15m; // Rp 15 per call
        Console.WriteLine($""Tenant: {currentTenant} | Total Pemakaian: {totalCalls:N0} calls | Tagihan: Rp {invoiceAmount:N2}"");
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Buat `ITenantProvider` yang mengekstrak `TenantId` dari klaim JWT atau header.",
                        "Langkah 2: Terapkan `modelBuilder.Entity<T>().HasQueryFilter(e => e.TenantId == _tenantId)`.",
                        "Langkah 3: Bangun kalkulator invoice berjenjang dengan pengujian isolasi antar-tenant."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Tenant A tidak pernah dapat membaca atau mengubah data milik Tenant B dalam kondisi apa pun.",
                        "Perhitungan prorata dan pajak akurat tanpa galat floating-point."
                    },
                    SecurityReview = "Validasi `TenantId` pada operasi Insert/Update di `SaveChangesAsync` agar entitas tidak bisa disuntikkan ke tenant lain.",
                    PerformanceReview = "Sertakan `TenantId` sebagai kolom pertama pada seluruh composite index tabel multi-tenant."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-11",
                    Order = 11,
                    Title = "PROJECT 11 — Enterprise Observability, OpenTelemetry & Health Telemetry Collector",
                    Subtitle = "Distributed Tracing (ActivitySource), Metrics (Meter), Structured Logging & HealthChecks",
                    Difficulty = "Expert",
                    Overview = "Membangun pipeline observabilitas produksi menggunakan standar OpenTelemetry (.NET `ActivitySource` dan `Meter`), korelasi W3C TraceContext lintas microservice, pendeteksian bottleneck database, dan endpoint Liveness/Readiness Health Checks.",
                    FunctionalRequirements = new List<string>
                    {
                        "Instrumentasi Distributed Tracing menggunakan `System.Diagnostics.ActivitySource` dengan propagasi `traceparent`.",
                        "Pencatatan metrik Counter, UpDownCounter, dan Histogram menggunakan `System.Diagnostics.Metrics.Meter`.",
                        "Structured JSON logging yang menyertakan `TraceId`, `SpanId`, dan `CorrelationId`.",
                        "Health Check terpisah untuk `/health/live` (Liveness) dan `/health/ready` (Readiness database & cache)."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Overhead telemetri di bawah 2% utilisasi CPU melalui mekanisme sampling.",
                        "Redaksi otomatis data sensitif (Password, Token, Nomor Kartu) sebelum log ditulis.",
                        "Kompatibel penuh dengan standar W3C TraceContext."
                    },
                    ArchitectureOverview = "Instrumented Service (ActivitySource + Meter + ILogger) -> PII Redaction Processor -> OpenTelemetry Exporter -> Telemetry Dashboard.",
                    DatabaseSchema = "Time-series Trace Spans & Metric Buckets (In-Memory Ring Buffer / OTLP Collector).",
                    ApiSpecification = "GET /health/live, GET /health/ready, GET /metrics/snapshot.",
                    StarterCode = @"using System;
using System.Diagnostics;

public class Program
{
    private static readonly ActivitySource Source = new(""Enterprise.Academy.Telemetry"", ""1.0.0"");

    public static void Main()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == ""Enterprise.Academy.Telemetry"",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = a => Console.WriteLine($""[SPAN] {a.OperationName} | TraceId: {a.TraceId} | Duration: {a.Duration.TotalMilliseconds:F2}ms"")
        };
        ActivitySource.AddActivityListener(listener);

        using (var activity = Source.StartActivity(""ProcessSettlementTransaction""))
        {
            activity?.SetTag(""payment.currency"", ""IDR"");
            Console.WriteLine(""Menjalankan transaksi dengan korelasi W3C TraceContext..."");
        }
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Daftarkan `ActivitySource` dan `Meter` khusus domain aplikasi.",
                        "Langkah 2: Buat log enricher dan PII redactor untuk menyensor field rahasia.",
                        "Langkah 3: Konfigurasikan endpoint `/health/live` dan `/health/ready`."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Setiap span mencatat `TraceId`, `SpanId`, dan durasi eksekusi secara akurat.",
                        "Data sensitif seperti password atau token tidak pernah muncul di dalam output log."
                    },
                    SecurityReview = "Pastikan endpoint `/metrics` dan `/health/ready` hanya dapat diakses dari jaringan internal cluster.",
                    PerformanceReview = "Gunakan `LoggerMessage` source generator untuk menghindari boxing parameter pada saat logging."
                },

                new ProjectSpecificationDto
                {
                    Id = "proj-12",
                    Order = 12,
                    Title = "PROJECT 12 — Core Enterprise Banking Backend & Double-Entry Ledger Engine",
                    Subtitle = "Double-Entry Accounting, Idempotent Settlement, Audit Trail, High Availability & Reversal",
                    Difficulty = "Principal",
                    Overview = "Sistem backend perbankan enterprise berskala industri: Manajemen rekening nasabah, mesin buku besar entri ganda (Double-Entry General Ledger), transfer dana antar-rekening, audit trail permanen yang tidak dapat diubah (immutable), penanganan reversal transaksi, dan rekonsiliasi otomatis.",
                    FunctionalRequirements = new List<string>
                    {
                        "Manajemen Rekening (Nomor Rekening, Tipe: Aset/Liabilitas/Modal, Saldo, Mata Uang, Status: Active/Frozen/Closed).",
                        "Buku Besar Entri Ganda (Double-Entry Ledger): Setiap transaksi finansial WAJIB memiliki minimal satu jurnal Debit dan satu jurnal Kredit dengan jumlah nominal seimbang (Total Debit == Total Kredit).",
                        "Transfer Dana Antar-Rekening: Operasi atomik pendebitan rekening pengirim dan pengkreditan rekening penerima.",
                        "Idempotency Guarantee: Setiap request transfer finansial wajib memiliki UUID Idempotency-Key. Pengiriman ulang request dengan key yang sama tidak boleh mendebit ulang saldo nasabah.",
                        "Transaction Reversal (Pembalikan Transaksi): Transaksi yang sudah terposting tidak boleh di-DELETE atau di-UPDATE; pembatalan hanya boleh dilakukan dengan membuat entri jurnal pembalik (Compensating Journal Entry).",
                        "Pencatatan Audit Trail Kriptografis: Hash SHA-256 berantai (Merkle/Blockchain-like chain) untuk mendeteksi manipulasi data buku besar."
                    },
                    NonFunctionalRequirements = new List<string>
                    {
                        "Strict Zero-Data-Loss dan Zero-Balance-Discrepancy (Integritas Saldo 100%).",
                        "Tingkat isolasi transaksi Serializable atau Snapshot Isolation untuk mencegah Phantom Read dan Lost Update pada saldo.",
                        "Response time transfer di bawah 80ms pada volume 1.000 TPS.",
                        "Audit Trail Immutable: Larangan hard-delete pada seluruh tabel jurnal finansial."
                    },
                    ArchitectureOverview = "Enterprise Core Banking Architecture: API Gateway -> Idempotency Guard -> Banking Application Service -> Double-Entry Ledger Engine -> Pessimistic/Optimistic Locking Repository -> Event Outbox Publisher -> Anti-Money Laundering (AML) Worker.",
                    DatabaseSchema = @"Tabel Accounts (Id, AccountNumber, CustomerId, Type, Balance, Currency, Status, RowVersion);
Tabel LedgerTransactions (Id, IdempotencyKey, ReferenceNumber, Description, PostingDate, Status, Hash);
Tabel JournalEntries (Id, TransactionId, AccountId, EntryType [Debit/Credit], Amount, Currency, CreatedAt);
Tabel AuditLogs (Id, EntityName, EntityId, Action, PayloadJson, CreatedBy, CreatedAt).",
                    ApiSpecification = @"POST /api/banking/accounts (Buka Rekening);
GET /api/banking/accounts/{accountNumber}/balance (Cek Saldo);
POST /api/banking/transfers (Transfer Dana Antar-Rekening dengan Header Idempotency-Key);
POST /api/banking/transfers/{ref}/reverse (Pembalikan Transaksi);
GET /api/banking/ledger/statements/{accountNumber} (Mutasi Buku Besar).",
                    StarterCode = @"using System;
using System.Collections.Generic;
using System.Linq;

public enum EntryType { Debit, Credit }
public enum AccountType { Asset, Liability, Equity, Revenue, Expense }

public record JournalEntry(string AccountNumber, EntryType Type, decimal Amount);

public class LedgerTransaction
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string IdempotencyKey { get; }
    public string Description { get; }
    public DateTime PostingDate { get; } = DateTime.UtcNow;
    public List<JournalEntry> Entries { get; } = new();

    public LedgerTransaction(string idempotencyKey, string description)
    {
        IdempotencyKey = idempotencyKey;
        Description = description;
    }

    public void AddEntry(string accountNumber, EntryType type, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException(""Nominal transaksi harus lebih besar dari nol."");
        Entries.Add(new JournalEntry(accountNumber, type, amount));
    }

    public void ValidateInvariants()
    {
        if (Entries.Count < 2) 
            throw new InvalidOperationException(""Buku besar entri ganda memerlukan minimal 2 entri jurnal."");

        decimal totalDebit = Entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount);
        decimal totalCredit = Entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount);

        if (totalDebit != totalCredit)
        {
            throw new InvalidOperationException($""Ketidakseimbangan Buku Besar! Total Debit: {totalDebit} != Total Kredit: {totalCredit}"");
        }
    }
}",
                    ImplementationSteps = new List<string>
                    {
                        "Langkah 1: Bangun domain model Account dan LedgerTransaction dengan validasi invariant Debit == Credit.",
                        "Langkah 2: Terapkan tabel IdempotencyKeys dengan status PENDING, COMPLETED, dan FAILED.",
                        "Langkah 3: Terapkan Pessimistic Locking (SELECT FOR UPDATE) atau Optimistic Concurrency pada update saldo akun.",
                        "Langkah 4: Bangun modul Reversal yang menerbitkan transaksi cermin berlawanan (Debit dibalik menjadi Kredit).",
                        "Langkah 5: Tulis pengujian concurrency multi-thread untuk memverifikasi tidak adanya kebocoran saldo uang saat ribuan transfer serentak."
                    },
                    AcceptanceCriteria = new List<string>
                    {
                        "Total debit selalu sama persis dengan total kredit pada setiap transaksi.",
                        "Pengiriman 10 request transfer serentak dengan Idempotency-Key yang sama HANYA mengeksekusi 1 transfer.",
                        "Saldo rekening pengirim tidak pernah berkurang menjadi negatif jika saldo tidak mencukupi (Insufficient Funds).",
                        "Audit log dan riwayat transaksi immutable lolos verifikasi integritas."
                    },
                    SecurityReview = "Terapkan Mutual TLS (mTLS), Token Scoping, dan enkripsi field nomor rekening (PII).",
                    PerformanceReview = "Gunakan partitioned index pada kolom PostingDate dan AccountNumber untuk mempercepat kueri mutasi rekening historis."
                }
            };
        }
    }
}
