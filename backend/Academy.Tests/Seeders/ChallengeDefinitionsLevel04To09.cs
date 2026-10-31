using System.Collections.Generic;
using Academy.Api.Models;

namespace Academy.Tests.Seeders
{
    public static class ChallengeDefinitionsLevel04To09
    {
        public static Dictionary<string, ChallengeDto> GetAll()
        {
            var dict = new Dictionary<string, ChallengeDto>();

            // lesson-04-02: Expression Tree / In-Memory LINQ Evaluator
            dict["lesson-04-02"] = ChallengeBuilder.Create(
                id: "ch-04-02",
                title: "Evaluator Kueri In-Memory Simulating LINQ Predicate",
                difficulty: "Intermediate",
                problemStatement: "Baca data produk dari stdin berformat 'Nama:Harga' dipisahkan koma.\nDi baris kedua, baca batas ambang harga minimum (int).\nFilter dan cetak nama produk yang harganya >= ambang batas dalam urutan alfabetis terpisah koma dan spasi.\nJika tidak ada yang memenuhi, cetak 'EMPTY'.",
                inputFormat: "Dua baris stdin: baris 1 daftar produk, baris 2 harga minimum.",
                outputFormat: "Satu baris daftar nama produk urut alfabetis atau 'EMPTY'.",
                constraints: "Harga bilangan bulat positif.",
                examples: new[] {
                    ("Laptop:15000, Mouse:200, Keyboard:800, Monitor:3000\n1000", "Laptop, Monitor", "Produk dengan harga >= 1000 diurutkan alfabetis."),
                    ("Mouse:200, Keyboard:800\n5000", "EMPTY", "Tidak ada produk di atas 5000.")
                },
                starterCode: @"using System;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        // TODO: Gunakan LINQ Where dan OrderBy untuk memfilter dan mengurutkan produk.
        Console.WriteLine(""EMPTY"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || !int.TryParse(l2, out int minPrice))
        {
            Console.WriteLine(""EMPTY"");
            return;
        }

        var items = l1.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().Split(':'))
            .Where(parts => parts.Length == 2 && int.TryParse(parts[1], out _))
            .Select(parts => new { Name = parts[0], Price = int.Parse(parts[1]) });

        var filtered = items
            .Where(p => p.Price >= minPrice)
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(p => p.Name)
            .ToList();

        Console.WriteLine(filtered.Count > 0 ? string.Join("", "", filtered) : ""EMPTY"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Laptop & Monitor)", "Laptop:15000, Mouse:200, Keyboard:800, Monitor:3000\n1000", "Laptop, Monitor", false, "Example"),
                    ("Contoh 2 (Tidak Ada)", "Mouse:200, Keyboard:800\n5000", "EMPTY", false, "Example"),
                    ("Semua Lolos", "A:100, B:200, C:300\n50", "A, B, C", false, "Normal"),
                    ("Satu Produk Sama Persis", "Keyboard:800\n800", "Keyboard", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan LINQ `.Where(p => p.Price >= minPrice)`.",
                    "Gunakan `.OrderBy(p => p.Name)` untuk pengurutan deterministik."
                },
                explanation: "Eksekusi LINQ in-memory (IEnumerable) mengevaluasi query pipeline secara deferred dan terstruktur tanpa memuat overhead memori besar."
            );

            // lesson-04-03: Zero-Allocation Bearer Token Parser Span<char>
            dict["lesson-04-03"] = ChallengeBuilder.Create(
                id: "ch-04-03",
                title: "Parser Header Authorization Bearer Zero-Allocation Span<char>",
                difficulty: "Intermediate",
                problemStatement: "Diberikan string header 'Authorization' dari stdin (misal 'Bearer eyJhbGciOi...').\nEkstrak token autentikasi (bagian setelah 'Bearer ') menggunakan `ReadOnlySpan<char>` tanpa alokasi substring di managed heap.\nJika string tidak diawali 'Bearer ' atau token setelahnya kosong, cetak 'INVALID_HEADER'.\nJika valid, cetak 'TOKEN: <token>'.",
                inputFormat: "Satu baris string header HTTP Authorization.",
                outputFormat: "Satu baris: 'TOKEN: <token>' atau 'INVALID_HEADER'.",
                constraints: "Header tidak null.",
                examples: new[] {
                    ("Bearer eyJhbGciOiJIUzI1NiJ9", "TOKEN: eyJhbGciOiJIUzI1NiJ9", "Token berhasil diekstrak."),
                    ("Basic YWxhZGRpbjpvcGVuc2VzYW1l", "INVALID_HEADER", "Bukan skema Bearer.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? header = Console.ReadLine();
        // TODO: Gunakan ReadOnlySpan<char> dan MemoryExtensions.StartsWith.
        Console.WriteLine(""INVALID_HEADER"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? header = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(header)) { Console.WriteLine(""INVALID_HEADER""); return; }

        ReadOnlySpan<char> span = header.AsSpan().Trim();
        ReadOnlySpan<char> prefix = ""Bearer "".AsSpan();

        if (span.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            ReadOnlySpan<char> tokenSpan = span.Slice(prefix.Length).Trim();
            if (!tokenSpan.IsEmpty)
            {
                Console.WriteLine($""TOKEN: {tokenSpan.ToString()}"");
                return;
            }
        }

        Console.WriteLine(""INVALID_HEADER"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Valid JWT Token)", "Bearer eyJhbGciOiJIUzI1NiJ9", "TOKEN: eyJhbGciOiJIUzI1NiJ9", false, "Example"),
                    ("Contoh 2 (Bukan Bearer)", "Basic YWxhZGRpbjpvcGVuc2VzYW1l", "INVALID_HEADER", false, "Example"),
                    ("Bearer Huruf Kecil", "bearer token12345", "TOKEN: token12345", false, "Normal"),
                    ("Bearer Tanpa Token", "Bearer   ", "INVALID_HEADER", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan header.AsSpan() untuk mendapatkan ReadOnlySpan<char> tanpa alokasi objek string baru.",
                    "Gunakan span.Slice() untuk mengambil potongan token."
                },
                explanation: "ReadOnlySpan<char> memungkinkan slicing dan parsing string pada stack memory tanpa alokasi heap yang memicu tekanan pada Garbage Collector (zero-allocation)."
            );

            // lesson-04-04: Reflection Inspector
            dict["lesson-04-04"] = ChallengeBuilder.Create(
                id: "ch-04-04",
                title: "Inspeksi Properti Objek Dinamis Menggunakan Reflection",
                difficulty: "Intermediate",
                problemStatement: "Buat method refleksi `InspectProperties(object obj)` yang membaca seluruh public instance properties dari sebuah objek.\nUrutkan properti secara alfabetis berdasarkan nama, lalu cetak dalam format: '<NamaProperti>=<Nilai>' dipisahkan tanda pipa '|'.\nJika nilai properti bernilai null, cetak 'NULL'.\nUji dengan kelas DTO yang memuat properti: `Id (int)`, `Name (string)`, `Role (string)` yang dibaca dari stdin.",
                inputFormat: "Satu baris: '<Id> <Name> <Role>'.",
                outputFormat: "Satu baris: properti berurutan dipisahkan '|'.",
                constraints: "Id integer, Name dan Role string.",
                examples: new[] {
                    ("101 Alice Admin", "Id=101|Name=Alice|Role=Admin", "Properti Id, Name, Role diurutkan alfabetis."),
                    ("202 Bob Developer", "Id=202|Name=Bob|Role=Developer", "Inspeksi refleksi berhasil mengekstrak nilai.")
                },
                starterCode: @"using System;
using System.Reflection;

public class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
}

public class Solution
{
    public static void Main()
    {
        // TODO: Gunakan typeof(UserDto).GetProperties() dan PropertyInfo.GetValue(obj).
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
}

public class Solution
{
    public static string InspectProperties(object obj)
    {
        var props = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(p => p.Name, StringComparer.Ordinal);

        var list = new List<string>();
        foreach (var p in props)
        {
            var val = p.GetValue(obj);
            string valStr = val != null ? val.ToString()! : ""NULL"";
            list.Add($""{p.Name}={valStr}"");
        }
        return string.Join(""|"", list);
    }

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3) return;

        var dto = new UserDto
        {
            Id = int.Parse(parts[0]),
            Name = parts[1],
            Role = parts[2]
        };

        Console.WriteLine(InspectProperties(dto));
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Alice)", "101 Alice Admin", "Id=101|Name=Alice|Role=Admin", false, "Example"),
                    ("Contoh 2 (Bob)", "202 Bob Developer", "Id=202|Name=Bob|Role=Developer", false, "Example"),
                    ("Role Kosong/Null", "303 Charlie NULL", "Id=303|Name=Charlie|Role=NULL", false, "Normal"),
                    ("Angka Negatif", "-1 Guest Anonymous", "Id=-1|Name=Guest|Role=Anonymous", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).",
                    "Gunakan OrderBy(p => p.Name) untuk pengurutan alfabetis yang konsisten."
                },
                explanation: "Reflection menyediakan kapabilitas runtime type introspection yang menjadi fondasi ORM (Entity Framework) dan JSON serializer dalam memetakan metadata objek C#."
            );

            // lesson-04-05: System.Text.Json CamelCase Serializer
            dict["lesson-04-05"] = ChallengeBuilder.Create(
                id: "ch-04-05",
                title: "Serialisasi DTO dengan System.Text.Json CamelCase & Enum String",
                difficulty: "Intermediate",
                problemStatement: "Buat program yang membaca data akun dari stdin: '<Id> <Username> <Status>' di mana Status adalah enum `AccountStatus { Active, Inactive, Suspended }`.\nSerialisasikan objek C# tersebut ke format JSON menggunakan `System.Text.Json` dengan aturan:\n1. Naming policy: `JsonNamingPolicy.CamelCase` (sehingga properti Id menjadi id, Username menjadi username)\n2. Nilai enum diserialisasikan sebagai string (bukan integer) menggunakan `JsonStringEnumConverter`.\nCetak string JSON satu baris kompak tanpa indentasi.",
                inputFormat: "Satu baris: '<Id> <Username> <Status>'.",
                outputFormat: "Satu baris string JSON kompak.",
                constraints: "Status salah satu dari Active, Inactive, Suspended.",
                examples: new[] {
                    ("101 john_doe Active", "{\"id\":101,\"username\":\"john_doe\",\"status\":\"Active\"}", "JSON dengan camelCase dan enum sebagai string."),
                    ("202 admin Suspended", "{\"id\":202,\"username\":\"admin\",\"status\":\"Suspended\"}", "Enum Suspended tercetak string.")
                },
                starterCode: @"using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum AccountStatus { Active, Inactive, Suspended }

public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = """";
    public AccountStatus Status { get; set; }
}

public class Solution
{
    public static void Main()
    {
        // TODO: Konfigurasikan JsonSerializerOptions dengan CamelCase dan JsonStringEnumConverter.
        Console.WriteLine(""{}"");
    }
}",
                solutionCode: @"using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum AccountStatus { Active, Inactive, Suspended }

public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = """";
    public AccountStatus Status { get; set; }
}

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length < 3) return;

        int id = int.Parse(p[0]);
        string user = p[1];
        var status = Enum.Parse<AccountStatus>(p[2], true);

        var account = new UserAccount { Id = id, Username = user, Status = status };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new JsonStringEnumConverter());

        string json = JsonSerializer.Serialize(account, options);
        Console.WriteLine(json);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Active)", "101 john_doe Active", "{\"id\":101,\"username\":\"john_doe\",\"status\":\"Active\"}", false, "Example"),
                    ("Contoh 2 (Suspended)", "202 admin Suspended", "{\"id\":202,\"username\":\"admin\",\"status\":\"Suspended\"}", false, "Example"),
                    ("Inactive Huruf Kecil", "303 guest inactive", "{\"id\":303,\"username\":\"guest\",\"status\":\"Inactive\"}", false, "Normal"),
                    ("Id Nol", "0 root Active", "{\"id\":0,\"username\":\"root\",\"status\":\"Active\"}", true, "EdgeCase")
                },
                hints: new[] {
                    "Atur options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase.",
                    "Tambahkan options.Converters.Add(new JsonStringEnumConverter())."
                },
                explanation: "System.Text.Json adalah serializer modern berkecepatan tinggi di .NET dengan alokasi memori minimal, menggantikan Newtonsoft.Json pada aplikasi web enterprise."
            );

            // lesson-05-02: Hash Table Word Frequency
            dict["lesson-05-02"] = ChallengeBuilder.Create(
                id: "ch-05-02",
                title: "Penghitung Frekuensi Kata Berbasis Hash Table Dictionary",
                difficulty: "Beginner",
                problemStatement: "Baca satu baris kalimat dari stdin.\nHitung frekuensi kemunculan setiap kata (case-insensitive, abaikan spasi berlebih).\nCetak kata dan jumlahnya dengan format '<Kata>:<Frekuensi>' dipisahkan koma dan spasi, diurutkan secara alfabetis berdasarkan kata.\nJika kalimat kosong, cetak 'EMPTY'.",
                inputFormat: "Satu baris teks kalimat.",
                outputFormat: "Satu baris pasangan kata dan frekuensi.",
                constraints: "Panjang kalimat <= 1000 karakter.",
                examples: new[] {
                    ("Saya suka CSharp dan saya suka backend", "backend:1, csharp:1, dan:1, saya:2, suka:2", "Semua kata dinormalisasi huruf kecil dan dihitung."),
                    ("API API api", "api:3", "Kata 'api' muncul 3 kali.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? text = Console.ReadLine();
        // TODO: Gunakan Dictionary<string, int> untuk menghitung frekuensi kata.
        Console.WriteLine(""EMPTY"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        string? text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text)) { Console.WriteLine(""EMPTY""); return; }

        var words = text.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var w in words)
        {
            string lower = w.ToLowerInvariant();
            if (freq.TryGetValue(lower, out int count))
            {
                freq[lower] = count + 1;
            }
            else
            {
                freq[lower] = 1;
            }
        }

        var result = freq.OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => $""{kv.Key}:{kv.Value}"");

        Console.WriteLine(string.Join("", "", result));
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Kalimat)", "Saya suka CSharp dan saya suka backend", "backend:1, csharp:1, dan:1, saya:2, suka:2", false, "Example"),
                    ("Contoh 2 (Kata Sama)", "API API api", "api:3", false, "Example"),
                    ("Satu Kata", "Microservices", "microservices:1", false, "Normal"),
                    ("Teks Kosong", "", "EMPTY", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan Dictionary<string, int> dengan TryGetValue untuk lookup cepat O(1).",
                    "Gunakan OrderBy(kv => kv.Key) untuk pengurutan alfabetis."
                },
                explanation: "Dictionary<TKey, TValue> di C# diimplementasikan menggunakan hash table dengan bucketing dan probing, memberikan kompleksitas waktu amortized O(1) untuk pencarian dan pembaruan frekuensi."
            );

            // lesson-05-03: B-Tree Binary Search Node Navigation
            dict["lesson-05-03"] = ChallengeBuilder.Create(
                id: "ch-05-03",
                title: "Pencari Slot Kunci Navigasi Simpul B-Tree (Binary Search)",
                difficulty: "Intermediate",
                problemStatement: "Sebuah node internal B-Tree menyimpan daftar kunci terurut (sorted keys). Diberikan daftar kunci dari baris pertama (dipisahkan spasi) dan kunci target pencarian di baris kedua.\nGunakan algoritma Binary Search untuk menentukan slot navigasi:\n- Jika target ada di simpul, cetak 'EXACT_MATCH: Index=<idx>'\n- Jika target tidak ada, cetak 'CHILD_SLOT: Index=<idx>' di mana idx adalah posisi cabang anak yang harus ditelusuri (0 <= idx <= N).",
                inputFormat: "Dua baris: baris 1 daftar kunci terurut, baris 2 target pencarian.",
                outputFormat: "Satu baris status slot B-Tree.",
                constraints: "Kunci integer unik terurut menaik.",
                examples: new[] {
                    ("10 20 30 40 50\n20", "EXACT_MATCH: Index=1", "Kunci 20 ditemukan tepat di indeks 1 simpul."),
                    ("10 20 30 40 50\n25", "CHILD_SLOT: Index=2", "Kunci 25 berada di antara 20 dan 30, navigasi ke anak indeks 2.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Gunakan Array.BinarySearch untuk navigasi simpul B-Tree.
        Console.WriteLine("""");
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
        if (string.IsNullOrWhiteSpace(l1) || !int.TryParse(l2, out int target)) return;

        var keys = l1.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        int result = Array.BinarySearch(keys, target);
        if (result >= 0)
        {
            Console.WriteLine($""EXACT_MATCH: Index={result}"");
        }
        else
        {
            int slot = ~result;
            Console.WriteLine($""CHILD_SLOT: Index={slot}"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Match)", "10 20 30 40 50\n20", "EXACT_MATCH: Index=1", false, "Example"),
                    ("Contoh 2 (Child Slot)", "10 20 30 40 50\n25", "CHILD_SLOT: Index=2", false, "Example"),
                    ("Slot Paling Depan", "10 20 30 40 50\n5", "CHILD_SLOT: Index=0", false, "Normal"),
                    ("Slot Paling Belakang", "10 20 30 40 50\n60", "CHILD_SLOT: Index=5", true, "EdgeCase")
                },
                hints: new[] {
                    "Array.BinarySearch mengembalikan indeks negatif bernilai bitwise complement (~index) jika target tidak ditemukan.",
                    "Gunakan operator bitwise NOT `~result` untuk mendapatkan slot penyisipan/pointer anak."
                },
                explanation: "Mesin penyimpanan database (seperti SQL Server atau SQLite) menggunakan Binary Search di dalam page memory B-Tree untuk secara instan menemukan pointer node anak disk selanjutnya."
            );

            // lesson-05-04: Graph Circular Dependency Check
            dict["lesson-05-04"] = ChallengeBuilder.Create(
                id: "ch-05-04",
                title: "Deteksi Ketergantungan Siklus Modul Sistem (Cycle Detection)",
                difficulty: "Intermediate",
                problemStatement: "Diberikan daftar dependensi antar modul sistem dari stdin dalam format 'A->B, B->C, ...' (artinya modul A bergantung pada B).\nTentukan apakah grafik dependensi memiliki ketergantungan siklus (circular dependency / dead lock loop).\nCetak 'CIRCULAR_DEPENDENCY' jika terdeteksi siklus, atau 'DAG_VALID' jika grafik merupakan Directed Acyclic Graph yang aman.",
                inputFormat: "Satu baris string dependensi dipisahkan koma.",
                outputFormat: "Satu baris: 'CIRCULAR_DEPENDENCY' atau 'DAG_VALID'.",
                constraints: "Jumlah relasi dependensi <= 50.",
                examples: new[] {
                    ("A->B, B->C, C->A", "CIRCULAR_DEPENDENCY", "A bergantung B, B bergantung C, C bergantung A membentuk siklus."),
                    ("A->B, A->C, B->D, C->D", "DAG_VALID", "Grafik asiklik tanpa siklus tertutup.")
                },
                starterCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Gunakan DFS dengan 3-state coloring (Unvisited, Visiting, Visited).
        Console.WriteLine(""DAG_VALID"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""DAG_VALID""); return; }

        var adj = new Dictionary<string, List<string>>();
        var nodes = new HashSet<string>();

        var edges = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var e in edges)
        {
            var p = e.Trim().Split(new[] { ""->"" }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 2)
            {
                string u = p[0].Trim();
                string v = p[1].Trim();
                nodes.Add(u);
                nodes.Add(v);
                if (!adj.ContainsKey(u)) adj[u] = new List<string>();
                adj[u].Add(v);
            }
        }

        // 0: Unvisited, 1: Visiting, 2: Visited
        var state = new Dictionary<string, int>();
        foreach (var n in nodes) state[n] = 0;

        bool hasCycle = false;
        bool Dfs(string u)
        {
            state[u] = 1; // Visiting
            if (adj.TryGetValue(u, out var neighbors))
            {
                foreach (var v in neighbors)
                {
                    if (state[v] == 1) return true; // Cycle detected
                    if (state[v] == 0 && Dfs(v)) return true;
                }
            }
            state[u] = 2; // Visited
            return false;
        }

        foreach (var n in nodes)
        {
            if (state[n] == 0)
            {
                if (Dfs(n)) { hasCycle = true; break; }
            }
        }

        Console.WriteLine(hasCycle ? ""CIRCULAR_DEPENDENCY"" : ""DAG_VALID"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Siklus 3 Simpul)", "A->B, B->C, C->A", "CIRCULAR_DEPENDENCY", false, "Example"),
                    ("Contoh 2 (Valid DAG Diamond)", "A->B, A->C, B->D, C->D", "DAG_VALID", false, "Example"),
                    ("Dua Komponen Lepas Aman", "A->B, C->D", "DAG_VALID", false, "Normal"),
                    ("Self Loop Siklus", "ServiceA->ServiceA", "CIRCULAR_DEPENDENCY", true, "EdgeCase")
                },
                hints: new[] {
                    "Gunakan algoritma pewarnaan 3 status (Depth-First Search): 0 = Unvisited, 1 = Visiting (sedang dalam recursion call stack), 2 = Visited.",
                    "Jika simpul tetangga berstatus 1 (Visiting), maka terdapat siklus (back-edge)."
                },
                explanation: "Dependency Injection container (seperti Microsoft.Extensions.DependencyInjection) melakukan cycle detection saat kompilasi graf layanan guna mencegah stack overflow saat service resolution."
            );

            // lesson-06-02: Thread Stack & Heap Memory Sizing
            dict["lesson-06-02"] = ChallengeBuilder.Create(
                id: "ch-06-02",
                title: "Kalkulator Estimasi Kebutuhan Memori Thread Stack",
                difficulty: "Beginner",
                problemStatement: "Pada arsitektur 64-bit OS, setiap native thread di .NET secara default dialokasikan stack memory sebesar 1 MB (1024 KB).\nBaca dua angka dari stdin: JumlahThread (int) dan AlokasiHeapMb (int).\nHitung total estimasi kebutuhan memori proses (dalam Megabytes / MB) dengan rumus:\nTotalMemori = (JumlahThread * 1 MB) + AlokasiHeapMb.\nCetak dalam format: 'TOTAL_MEMORI: <Total> MB'.",
                inputFormat: "Satu baris memuat JumlahThread dan AlokasiHeapMb dipisahkan spasi.",
                outputFormat: "Satu baris: 'TOTAL_MEMORI: <Total> MB'.",
                constraints: "JumlahThread >= 1, AlokasiHeapMb >= 0.",
                examples: new[] {
                    ("100 500", "TOTAL_MEMORI: 600 MB", "100 thread * 1 MB = 100 MB stack + 500 MB heap = 600 MB."),
                    ("2000 1024", "TOTAL_MEMORI: 3024 MB", "2000 MB stack + 1024 MB heap = 3024 MB.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Hitung memori stack 1 MB per thread + memori heap.
        Console.WriteLine(""TOTAL_MEMORI: 0 MB"");
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
        if (p.Length < 2) return;

        long threads = long.Parse(p[0]);
        long heapMb = long.Parse(p[1]);

        long total = (threads * 1) + heapMb;
        Console.WriteLine($""TOTAL_MEMORI: {total} MB"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (100 Thread)", "100 500", "TOTAL_MEMORI: 600 MB", false, "Example"),
                    ("Contoh 2 (2000 Thread)", "2000 1024", "TOTAL_MEMORI: 3024 MB", false, "Example"),
                    ("Satu Thread", "1 100", "TOTAL_MEMORI: 101 MB", false, "Normal"),
                    ("Skala Besar", "10000 8192", "TOTAL_MEMORI: 18192 MB", true, "EdgeCase")
                },
                hints: new[] {
                    "Setiap thread OS memiliki overhead stack fisik 1 MB di lingkungan 64-bit.",
                    "Inilah alasan arsitektur asinkron (async/await) jauh lebih hemat memori dibanding alokasi thread per request."
                },
                explanation: "Membuat ribuan thread fisik memboroskan memori virtual stack dan memicu context-switching overhead, mendorong pemanfaatan ThreadPool dan Task async."
            );

            // lesson-06-03: Lock-Free Interlocked Counter
            dict["lesson-06-03"] = ChallengeBuilder.Create(
                id: "ch-06-03",
                title: "Penghitung Metrik Transaksi Bebas Lock (Lock-Free Interlocked)",
                difficulty: "Intermediate",
                problemStatement: "Simulasikan penghitung transaksi atomik bebas lock menggunakan `System.Threading.Interlocked`.\nBaca daftar operasi dari stdin yang dipisahkan spasi:\n- 'INC' (menambah 1 dengan Interlocked.Increment)\n- 'DEC' (mengurangi 1 dengan Interlocked.Decrement)\n- 'ADD:<value>' (menambah nilai dengan Interlocked.Add)\nNilai awal adalah 0.\nCetak nilai akhir counter: 'COUNTER_AKHIR: <Value>'.",
                inputFormat: "Satu baris perintah dipisahkan spasi.",
                outputFormat: "Satu baris: 'COUNTER_AKHIR: <Value>'.",
                constraints: "Operasi <= 1000 item.",
                examples: new[] {
                    ("INC INC ADD:10 DEC", "COUNTER_AKHIR: 11", "0 + 1 + 1 + 10 - 1 = 11."),
                    ("DEC DEC", "COUNTER_AKHIR: -2", "0 - 1 - 1 = -2.")
                },
                starterCode: @"using System;
using System.Threading;

public class Solution
{
    private static int _counter = 0;
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Gunakan Interlocked.Increment, Decrement, dan Add.
        Console.WriteLine(""COUNTER_AKHIR: 0"");
    }
}",
                solutionCode: @"using System;
using System.Threading;

public class Solution
{
    private static int _counter = 0;

    public static void Main()
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine(""COUNTER_AKHIR: 0""); return; }

        var ops = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var op in ops)
        {
            if (op.Equals(""INC"", StringComparison.OrdinalIgnoreCase))
            {
                Interlocked.Increment(ref _counter);
            }
            else if (op.Equals(""DEC"", StringComparison.OrdinalIgnoreCase))
            {
                Interlocked.Decrement(ref _counter);
            }
            else if (op.StartsWith(""ADD:"", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(op.Substring(4), out int val))
                {
                    Interlocked.Add(ref _counter, val);
                }
            }
        }
        Console.WriteLine($""COUNTER_AKHIR: {_counter}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Campuran)", "INC INC ADD:10 DEC", "COUNTER_AKHIR: 11", false, "Example"),
                    ("Contoh 2 (Pengurangan)", "DEC DEC", "COUNTER_AKHIR: -2", false, "Example"),
                    ("Operasi Nol", "INC DEC", "COUNTER_AKHIR: 0", false, "Normal"),
                    ("Add Angka Besar", "ADD:1000 ADD:2500 DEC", "COUNTER_AKHIR: 3499", true, "EdgeCase")
                },
                hints: new[] {
                    "System.Threading.Interlocked menyediakan instruksi CPU level hardware atomik tanpa lock kernel.",
                    "Gunakan ref _counter pada pemanggilan method Interlocked."
                },
                explanation: "Interlocked menggunakan instruksi atomik CPU (seperti LOCK CMPXCHG pada x86/x64) yang tidak memblokir thread dan bebas dari deadlock."
            );

            // lesson-08-01: Runtime Feature Flag Config Parser
            dict["lesson-08-01"] = ChallengeBuilder.Create(
                id: "ch-08-01",
                title: "Pemeriksa Status Konfigurasi Dynamic PGO Runtime",
                difficulty: "Beginner",
                problemStatement: "Baca satu baris string konfigurasi environment .NET runtime dari stdin (format 'Key1=Val1;Key2=Val2;...').\nCari konfigurasi `DOTNET_TieredPGO`.\n- Jika nilainya '1' atau 'true': cetak 'DYNAMIC_PGO: ENABLED'\n- Jika nilainya '0' atau 'false': cetak 'DYNAMIC_PGO: DISABLED'\n- Jika tidak ditemukan: cetak 'DYNAMIC_PGO: NOT_CONFIGURED'.",
                inputFormat: "Satu baris string konfigurasi dipisahkan titik koma (';').",
                outputFormat: "Satu baris status Dynamic PGO.",
                constraints: "Case-insensitive.",
                examples: new[] {
                    ("DOTNET_TieredPGO=1;DOTNET_TC_QuickJit=1", "DYNAMIC_PGO: ENABLED", "TieredPGO diaktifkan dengan nilai 1."),
                    ("ASPNETCORE_ENVIRONMENT=Production", "DYNAMIC_PGO: NOT_CONFIGURED", "Kunci tidak ditemukan.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? config = Console.ReadLine();
        // TODO: Parse key-value pair dan evaluasi DOTNET_TieredPGO.
        Console.WriteLine(""DYNAMIC_PGO: NOT_CONFIGURED"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? config = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(config)) { Console.WriteLine(""DYNAMIC_PGO: NOT_CONFIGURED""); return; }

        var pairs = config.Split(';', StringSplitOptions.RemoveEmptyEntries);
        string status = ""NOT_CONFIGURED"";

        foreach (var p in pairs)
        {
            var kv = p.Trim().Split('=');
            if (kv.Length == 2 && kv[0].Trim().Equals(""DOTNET_TieredPGO"", StringComparison.OrdinalIgnoreCase))
            {
                string val = kv[1].Trim().ToLowerInvariant();
                if (val == ""1"" || val == ""true"") status = ""ENABLED"";
                else if (val == ""0"" || val == ""false"") status = ""DISABLED"";
                break;
            }
        }
        Console.WriteLine($""DYNAMIC_PGO: {status}"");
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Enabled Nilai 1)", "DOTNET_TieredPGO=1;DOTNET_TC_QuickJit=1", "DYNAMIC_PGO: ENABLED", false, "Example"),
                    ("Contoh 2 (Not Configured)", "ASPNETCORE_ENVIRONMENT=Production", "DYNAMIC_PGO: NOT_CONFIGURED", false, "Example"),
                    ("Disabled Nilai False", "DOTNET_TieredPGO=false", "DYNAMIC_PGO: DISABLED", false, "Normal"),
                    ("Enabled Huruf Besar True", "DOTNET_TieredPGO=TRUE", "DYNAMIC_PGO: ENABLED", true, "EdgeCase")
                },
                hints: new[] {
                    "Pecah string dengan Split(';').",
                    "Gunakan StringComparison.OrdinalIgnoreCase saat memeriksa nama konfigurasi."
                },
                explanation: "Dynamic Profile-Guided Optimization (PGO) di .NET 8 mengumpulkan profil eksekusi kode saat runtime untuk mengompilasi ulang hot path menjadi instruksi mesin paling optimal."
            );

            // lesson-08-02: LOH 85,000 Byte Threshold Router
            dict["lesson-08-02"] = ChallengeBuilder.Create(
                id: "ch-08-02",
                title: "Pemeriksa Ambang Batas Alokasi Large Object Heap (LOH)",
                difficulty: "Beginner",
                problemStatement: "Di .NET CLR, objek berukuran >= 85.000 byte secara default langsung dialokasikan ke Large Object Heap (LOH) alih-alih Small Object Heap (Gen 0).\nBaca ukuran alokasi array (dalam satuan bytes) dari stdin.\n- Jika ukuran >= 85000: cetak 'HEAP: LOH (Direct Allocation)'\n- Jika ukuran < 85000: cetak 'HEAP: SOH (Generation 0)'.",
                inputFormat: "Satu baris bilangan bulat positif ukuran bytes.",
                outputFormat: "Satu baris lokasi heap CLR.",
                constraints: "Ukuran >= 0.",
                examples: new[] {
                    ("85000", "HEAP: LOH (Direct Allocation)", "Tepat di ambang batas 85.000 byte langsung dialokasikan ke LOH."),
                    ("1024", "HEAP: SOH (Generation 0)", "1 KB dialokasikan ke Small Object Heap Gen 0.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        // TODO: Evaluasi ambang batas 85.000 byte CLR.
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? line = Console.ReadLine();
        if (!long.TryParse(line, out long bytes)) return;

        if (bytes >= 85000)
        {
            Console.WriteLine(""HEAP: LOH (Direct Allocation)"");
        }
        else
        {
            Console.WriteLine(""HEAP: SOH (Generation 0)"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Tepat 85000 LOH)", "85000", "HEAP: LOH (Direct Allocation)", false, "Example"),
                    ("Contoh 2 (1024 SOH)", "1024", "HEAP: SOH (Generation 0)", false, "Example"),
                    ("Tepat di Bawah Batas 84999", "84999", "HEAP: SOH (Generation 0)", false, "Normal"),
                    ("Alokasi Sangat Besar", "10485760", "HEAP: LOH (Direct Allocation)", true, "EdgeCase")
                },
                hints: new[] {
                    "Ambang batas resmi CLR LOH adalah 85.000 byte.",
                    "LOH tidak dipadatkan (compacted) secara default untuk menghemat CPU overhead."
                },
                explanation: "Memahami batas 85.000 byte penting dalam arsitektur backend .NET untuk mencegah fragmentasi memori akibat alokasi berulang pada Large Object Heap."
            );

            // lesson-08-03: IDisposable Scope Execution Timer
            dict["lesson-08-03"] = ChallengeBuilder.Create(
                id: "ch-08-03",
                title: "Implementasi IDisposable pada Pelacak Waktu Operasi (Scope Timer)",
                difficulty: "Intermediate",
                problemStatement: "Buat kelas `ScopeTimer : IDisposable` yang menerima nama operasi (string) di konstruktornya.\nSaat diinisialisasi (konstruktor), cetak 'START: <OperationName>'.\nSaat dispose dipanggil (method `Dispose()`), cetak 'STOP: <OperationName>'.\nBaca nama operasi dari stdin dan jalankan dalam blok `using (new ScopeTimer(op)) { Console.WriteLine(\"EXECUTING: ...\"); }`.",
                inputFormat: "Satu baris nama operasi (misal 'DatabaseQuery').",
                outputFormat: "Tiga baris: START, EXECUTING, STOP.",
                constraints: "Nama operasi tidak kosong.",
                examples: new[] {
                    ("ProcessPayment", "START: ProcessPayment\nEXECUTING: ProcessPayment\nSTOP: ProcessPayment", "Pola using menjamin Dispose dipanggil otomatis di akhir blok."),
                    ("AuditLogFlush", "START: AuditLogFlush\nEXECUTING: AuditLogFlush\nSTOP: AuditLogFlush", "Operasi audit flush dieksekusi dalam scope.")
                },
                starterCode: @"using System;

public class ScopeTimer : IDisposable
{
    // TODO: Implementasikan konstruktor dan Dispose().
    public void Dispose() {}
}

public class Solution
{
    public static void Main()
    {
        string? op = Console.ReadLine();
        // TODO: Bungkus dalam using blok.
    }
}",
                solutionCode: @"using System;

public class ScopeTimer : IDisposable
{
    private readonly string _name;
    private bool _disposed = false;

    public ScopeTimer(string name)
    {
        _name = name;
        Console.WriteLine($""START: {_name}"");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($""STOP: {_name}"");
            _disposed = true;
        }
    }
}

public class Solution
{
    public static void Main()
    {
        string? op = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(op)) return;

        using (new ScopeTimer(op))
        {
            Console.WriteLine($""EXECUTING: {op}"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Payment)", "ProcessPayment", "START: ProcessPayment\nEXECUTING: ProcessPayment\nSTOP: ProcessPayment", false, "Example"),
                    ("Operasi Query", "QueryOrderDb", "START: QueryOrderDb\nEXECUTING: QueryOrderDb\nSTOP: QueryOrderDb", false, "Normal"),
                    ("Operasi Simbol", "Task#123", "START: Task#123\nEXECUTING: Task#123\nSTOP: Task#123", true, "EdgeCase")
                },
                hints: new[] {
                    "Blok `using` secara otomatis diterjemahkan oleh compiler C# menjadi blok `try ... finally` yang memanggil `Dispose()`.",
                    "Pola ini umum digunakan untuk mengukur durasi atau mengelola scope resource unmanaged."
                },
                explanation: "Antarmuka IDisposable menyediakan mekanisme pembersihan resource deterministik di C# yang dieksekusi seketika saat keluar dari lexical scope blok using."
            );

            // lesson-09-02: Query Plan Seek vs Scan
            dict["lesson-09-02"] = ChallengeBuilder.Create(
                id: "ch-09-02",
                title: "Simulator Evaluasi Operator RDBMS: Index Seek vs Table Scan",
                difficulty: "Intermediate",
                problemStatement: "Baca dua baris dari stdin:\nBaris 1: Kolom tabel yang memiliki indeks ('Id, Email, CreatedAt')\nBaris 2: Kolom yang difilter pada klausa WHERE ('WHERE Email = ...' atau 'WHERE Name = ...').\nJika kolom yang difilter ada di dalam daftar kolom berindeks, cetak 'OPERATOR: INDEX_SEEK'.\nJika kolom tidak berindeks, cetak 'OPERATOR: TABLE_SCAN'.",
                inputFormat: "Dua baris stdin: baris 1 indeks kolom, baris 2 klausa WHERE.",
                outputFormat: "Satu baris operator kueri.",
                constraints: "Case-insensitive.",
                examples: new[] {
                    ("Id, Email, CreatedAt\nWHERE Email = 'alice@example.com'", "OPERATOR: INDEX_SEEK", "Kolom Email memiliki indeks."),
                    ("Id, Email\nWHERE FullName = 'Bob'", "OPERATOR: TABLE_SCAN", "Kolom FullName tidak berindeks.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        // TODO: Evaluasi keberadaan indeks pada kolom klausa WHERE.
        Console.WriteLine(""OPERATOR: TABLE_SCAN"");
    }
}",
                solutionCode: @"using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        string? l1 = Console.ReadLine();
        string? l2 = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2)) return;

        var indexedCols = new HashSet<string>(
            l1.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()),
            StringComparer.OrdinalIgnoreCase
        );

        string whereClause = l2.Trim();
        if (whereClause.StartsWith(""WHERE "", StringComparison.OrdinalIgnoreCase))
        {
            whereClause = whereClause.Substring(6).Trim();
        }

        var tokens = whereClause.Split(new[] { ' ', '=', '<', '>', '!' }, StringSplitOptions.RemoveEmptyEntries);
        string filterCol = tokens.Length > 0 ? tokens[0].Trim() : """";

        if (indexedCols.Contains(filterCol))
        {
            Console.WriteLine(""OPERATOR: INDEX_SEEK"");
        }
        else
        {
            Console.WriteLine(""OPERATOR: TABLE_SCAN"");
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (Seek)", "Id, Email, CreatedAt\nWHERE Email = 'alice@example.com'", "OPERATOR: INDEX_SEEK", false, "Example"),
                    ("Contoh 2 (Scan)", "Id, Email\nWHERE FullName = 'Bob'", "OPERATOR: TABLE_SCAN", false, "Example"),
                    ("Id Kolom Pertama", "Id, Name\nWHERE Id = 10", "OPERATOR: INDEX_SEEK", false, "Normal"),
                    ("Filter Bukan Indeks", "CreatedAt\nWHERE DeletedAt IS NULL", "OPERATOR: TABLE_SCAN", true, "EdgeCase")
                },
                hints: new[] {
                    "Ekstrak nama kolom pertama pada klausa WHERE.",
                    "Uji apakah nama kolom tersebut terdapat di dalam koleksi indexedCols."
                },
                explanation: "Index Seek menjelajahi pohon B-Tree secara logaritmik O(log N) langsung ke baris yang dicari, sedangkan Table Scan membaca seluruh page tabel O(N) dari disk."
            );

            // lesson-09-03: Transaction Anomaly Detector
            dict["lesson-09-03"] = ChallengeBuilder.Create(
                id: "ch-09-03",
                title: "Pemvalidasi Kompatibilitas Tingkat Isolasi Transaksi ACID",
                difficulty: "Intermediate",
                problemStatement: "Diberikan nama tingkat isolasi transaksi RDBMS dari stdin ('ReadUncommitted', 'ReadCommitted', 'RepeatableRead', 'Serializable').\nTentukan anomali konkurensi paling parah yang masih MUNGKIN terjadi pada tingkat isolasi tersebut:\n- ReadUncommitted: 'DIRTY_READ'\n- ReadCommitted: 'NON_REPEATABLE_READ'\n- RepeatableRead: 'PHANTOM_READ'\n- Serializable: 'NONE (ANOMALY_FREE)'\nJika input tidak valid, cetak 'INVALID_ISOLATION_LEVEL'.",
                inputFormat: "Satu baris nama tingkat isolasi transaksi.",
                outputFormat: "Satu baris nama anomali atau status aman.",
                constraints: "Case-insensitive.",
                examples: new[] {
                    ("ReadCommitted", "NON_REPEATABLE_READ", "ReadCommitted mencegah Dirty Read tetapi masih memungkinkan Non-Repeatable Read."),
                    ("Serializable", "NONE (ANOMALY_FREE)", "Serializable menjamin kebebasan mutlak dari semua anomali.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? level = Console.ReadLine();
        // TODO: Evaluasi anomali transaksi ACID per tingkat isolasi.
        Console.WriteLine(""NONE (ANOMALY_FREE)"");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? level = Console.ReadLine()?.Trim().ToLowerInvariant();

        string result = level switch
        {
            ""readuncommitted"" => ""DIRTY_READ"",
            ""readcommitted"" => ""NON_REPEATABLE_READ"",
            ""repeatableread"" => ""PHANTOM_READ"",
            ""serializable"" => ""NONE (ANOMALY_FREE)"",
            _ => ""INVALID_ISOLATION_LEVEL""
        };

        Console.WriteLine(result);
    }
}",
                testCases: new[] {
                    ("Contoh 1 (ReadCommitted)", "ReadCommitted", "NON_REPEATABLE_READ", false, "Example"),
                    ("Contoh 2 (Serializable)", "Serializable", "NONE (ANOMALY_FREE)", false, "Example"),
                    ("ReadUncommitted", "readuncommitted", "DIRTY_READ", false, "Normal"),
                    ("RepeatableRead", "RepeatableRead", "PHANTOM_READ", false, "Normal"),
                    ("Invalid Level", "SnapshotSuper", "INVALID_ISOLATION_LEVEL", true, "EdgeCase")
                },
                hints: new[] {
                    "Standar ANSI SQL mendefinisikan 4 level isolasi transaksi.",
                    "Gunakan switch expression untuk pemetaan langsung."
                },
                explanation: "Tingkat isolasi transaksi mengatur trade-off antara throughput konkurensi data dan pencegahan anomali konkurensi (Dirty Read, Non-Repeatable Read, Phantom Read)."
            );

            // lesson-09-04: SARGable Query Transformer
            dict["lesson-09-04"] = ChallengeBuilder.Create(
                id: "ch-09-04",
                title: "Pengubah Kueri Non-SARGable Menjadi SARGable Range Predicate",
                difficulty: "Intermediate",
                problemStatement: "Fungsi pada kolom seperti `YEAR(OrderDate) = 2024` bersifat non-SARGable sehingga indeks kolom tidak dapat digunakan (memicu Full Table Scan).\nBaca string pola non-SARGable dari stdin dalam format 'YEAR(Column) = YYYY'.\nUbah menjadi predikat rentang SARGable dengan format persis:\n'Column >= YYYY-01-01 AND Column < (YYYY+1)-01-01'.",
                inputFormat: "Satu baris: 'YEAR(<Column>) = <YYYY>'.",
                outputFormat: "Satu baris predikat rentang SARGable.",
                constraints: "YYYY adalah tahun integer 4 digit.",
                examples: new[] {
                    ("YEAR(OrderDate) = 2024", "OrderDate >= 2024-01-01 AND OrderDate < 2025-01-01", "Fungsi YEAR diubah menjadi rentang batas tanggal."),
                    ("YEAR(CreatedAt) = 2023", "CreatedAt >= 2023-01-01 AND CreatedAt < 2024-01-01", "Tahun 2023 menjadi rentang 2023 s/d 2024.")
                },
                starterCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? query = Console.ReadLine();
        // TODO: Ekstrak nama kolom dan tahun, lalu bentuk ekspresi SARGable.
        Console.WriteLine("""");
    }
}",
                solutionCode: @"using System;

public class Solution
{
    public static void Main()
    {
        string? query = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(query)) return;

        // Pattern: YEAR(Col) = YYYY
        int openParen = query.IndexOf('(');
        int closeParen = query.IndexOf(')');
        int equalsIdx = query.IndexOf('=');

        if (openParen >= 0 && closeParen > openParen && equalsIdx > closeParen)
        {
            string col = query.Substring(openParen + 1, closeParen - openParen - 1).Trim();
            string yearStr = query.Substring(equalsIdx + 1).Trim();

            if (int.TryParse(yearStr, out int year))
            {
                int nextYear = year + 1;
                Console.WriteLine($""{col} >= {year}-01-01 AND {col} < {nextYear}-01-01"");
            }
        }
    }
}",
                testCases: new[] {
                    ("Contoh 1 (OrderDate 2024)", "YEAR(OrderDate) = 2024", "OrderDate >= 2024-01-01 AND OrderDate < 2025-01-01", false, "Example"),
                    ("Contoh 2 (CreatedAt 2023)", "YEAR(CreatedAt) = 2023", "CreatedAt >= 2023-01-01 AND CreatedAt < 2024-01-01", false, "Example"),
                    ("Tahun 2030", "YEAR(TransDate) = 2030", "TransDate >= 2030-01-01 AND TransDate < 2031-01-01", false, "Normal"),
                    ("Spasi Banyak", "YEAR(  DeliveryDate  )   =   2025", "DeliveryDate >= 2025-01-01 AND DeliveryDate < 2026-01-01", true, "EdgeCase")
                },
                hints: new[] {
                    "Predikat SARGable (Search Argument Able) memungkinkan Query Optimizer database memanfaatkan B-Tree Index Seek.",
                    "Mengubah fungsi YEAR menjadi rentang tanggal >= dan < memungkinkan engine melakukan range seek."
                },
                explanation: "Membungkus kolom terindeks dalam pemanggilan fungsi (seperti YEAR(Col)) membatalkan kemampuan database menggunakan Index Seek, memaksa Table Scan pada seluruh baris tabel."
            );

            return dict;
        }
    }
}
