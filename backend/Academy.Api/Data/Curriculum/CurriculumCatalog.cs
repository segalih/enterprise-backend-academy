using System.Collections.Generic;
using System.Linq;
using Academy.Api.Models;

namespace Academy.Api.Data.Curriculum
{
    public static class CurriculumCatalog
    {
        public static List<LevelDetailDto> GetAllLevels()
        {
            var levels = new List<LevelDetailDto>
            {
                // LEVEL 00
                new LevelDetailDto
                {
                    Id = "level-00",
                    Order = 0,
                    Title = "LEVEL 00 — Computer & Digital Fundamentals",
                    Subtitle = "Arsitektur Komputer, Biner, CPU, Memori & Sistem Operasi",
                    Description = "Fondasi komputasi dari level gerbang logika hingga kernel OS, virtual memory, dan CLI untuk pemula.",
                    Difficulty = "Beginner",
                    EstimatedHours = 18,
                    Icon = "Cpu",
                    KeyTopics = new List<string> { "CPU & ALU", "Cache L1/L2/L3", "Binary & Two's Complement", "Virtual Memory", "Kernel vs User Mode", "CLI & Shell" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-00-01",
                            LevelId = "level-00",
                            Order = 1,
                            Title = "Hardware, CPU & Komponen Komputasi",
                            Description = "Cara kerja CPU, Register, Cache, Siklus Fetch-Decode-Execute, dan Hierarki Memori.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-00-01", LevelId = "level-00", ModuleId = "mod-00-01", Order = 1, Title = "Anatomi Komputer: Dari Input, CPU, Bus, hingga Output", Slug = "computer-anatomy", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = false, SimulatorType = "cpu" },
                                new LessonSummaryDto { Id = "lesson-00-02", LevelId = "level-00", ModuleId = "mod-00-01", Order = 2, Title = "CPU Deep Dive: Cores, Threads, ALU, Control Unit & Registers", Slug = "cpu-deep-dive", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false, SimulatorType = "cpu" },
                                new LessonSummaryDto { Id = "lesson-00-03", LevelId = "level-00", ModuleId = "mod-00-01", Order = 3, Title = "Hierarki Memori: Dari Register, L1/L2/L3 Cache, RAM, hingga NVMe", Slug = "memory-hierarchy", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = false, SimulatorType = "memory" }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-00-02",
                            LevelId = "level-00",
                            Order = 2,
                            Title = "Sistem Bilangan & Representasi Data Digital",
                            Description = "Biner, Hexadesimal, Two's Complement, Floating Point IEEE 754, dan Karakter Encoding (ASCII, UTF-8).",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-00-04", LevelId = "level-00", ModuleId = "mod-00-02", Order = 4, Title = "Sistem Biner & Heksadesimal: Bahasa Dasar Komputer", Slug = "binary-hexadecimal", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = true, SimulatorType = "binary" },
                                new LessonSummaryDto { Id = "lesson-00-05", LevelId = "level-00", ModuleId = "mod-00-02", Order = 5, Title = "Aritmatika Biner & Two's Complement: Representasi Bilangan Negatif", Slug = "binary-arithmetic-twos-complement", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true, SimulatorType = "binary" },
                                new LessonSummaryDto { Id = "lesson-00-06", LevelId = "level-00", ModuleId = "mod-00-02", Order = 6, Title = "Encoding Karakter: ASCII, Unicode, UTF-8, dan Endianness", Slug = "character-encoding-utf8", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-00-03",
                            LevelId = "level-00",
                            Order = 3,
                            Title = "Sistem Operasi, Kernel & Command Line Interface",
                            Description = "Kernel vs User Mode, System Calls, Process & Thread, File System, PowerShell & Bash.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-00-07", LevelId = "level-00", ModuleId = "mod-00-03", Order = 7, Title = "Sistem Operasi & Kernel: User Mode, Kernel Mode, dan System Calls", Slug = "os-kernel-modes", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-00-08", LevelId = "level-00", ModuleId = "mod-00-03", Order = 8, Title = "Virtual Memory, Paging, dan Proteksi Memori Proses", Slug = "virtual-memory-paging", Difficulty = "Intermediate", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false, SimulatorType = "memory" },
                                new LessonSummaryDto { Id = "lesson-00-09", LevelId = "level-00", ModuleId = "mod-00-03", Order = 9, Title = "Mastering Terminal & CLI: Shell, Environment Variables, dan PATH", Slug = "cli-environment-variables", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = false, SimulatorType = "terminal" }
                            }
                        }
                    }
                },

                // LEVEL 01
                new LevelDetailDto
                {
                    Id = "level-01",
                    Order = 1,
                    Title = "LEVEL 01 — Programming Logic & Problem Solving",
                    Subtitle = "Logika Pemrograman, Algoritma, Dekomposisi Masalah & Matematika Komputasi",
                    Description = "Membangun cara berpikir algoritmik dari nol sebelum menulis kode konkret dalam C#.",
                    Difficulty = "Beginner",
                    EstimatedHours = 16,
                    Icon = "Brain",
                    KeyTopics = new List<string> { "Compiler vs Interpreter", "Algoritma & Pseudocode", "Alur Kontrol (Branching & Looping)", "Logika Boolean & Truth Tables", "Dekomposisi Masalah" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-01-01",
                            LevelId = "level-01",
                            Order = 1,
                            Title = "Konsep Dasar Pemrograman & Eksekusi Kode",
                            Description = "Source code, Mesin penerjemah, Kompilasi vs Interpretasi, dan Runtime.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-01-01", LevelId = "level-01", ModuleId = "mod-01-01", Order = 1, Title = "Bagaimana Komputer Mengeksekusi Kode: Source Code hingga Machine Code", Slug = "how-code-executes", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-01-02", LevelId = "level-01", ModuleId = "mod-01-01", Order = 2, Title = "Paradigma Pemrograman: Imperatif, Terstruktur, OOP, dan Fungsional", Slug = "programming-paradigms", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-01-02",
                            LevelId = "level-01",
                            Order = 2,
                            Title = "Algoritma & Alur Kontrol Logika",
                            Description = "Sequence, Selection (Branching), Iteration (Looping), dan Rekursi.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-01-03", LevelId = "level-01", ModuleId = "mod-01-02", Order = 3, Title = "Merancang Algoritma dengan Flowchart dan Pseudocode", Slug = "algorithms-pseudocode", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-01-04", LevelId = "level-01", ModuleId = "mod-01-02", Order = 4, Title = "Logika Boolean, Tabel Kebenaran, dan Ekspresi Kondisional", Slug = "boolean-logic-truth-tables", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-01-05", LevelId = "level-01", ModuleId = "mod-01-02", Order = 5, Title = "Dekomposisi Masalah, Pattern Recognition, dan Edge Cases", Slug = "problem-decomposition", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 02
                new LevelDetailDto
                {
                    Id = "level-02",
                    Order = 2,
                    Title = "LEVEL 02 — C# Programming Fundamentals",
                    Subtitle = "Sintaks C#, Tipe Data, Value vs Reference, Kontrol Alur, Method & Koleksi",
                    Description = "Pengenalan lengkap ekosistem modern .NET dan C#, dari console app pertama hingga exception handling dan manipulasi file.",
                    Difficulty = "Beginner",
                    EstimatedHours = 24,
                    Icon = "Terminal",
                    KeyTopics = new List<string> { "CLR & Roslyn", "Value Types vs Reference Types", "Boxing & Unboxing", "Control Flow", "Methods & Ref/Out", "Collections & Exceptions" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-02-01",
                            LevelId = "level-02",
                            Order = 1,
                            Title = "Ekosistem .NET & Anatomi Program C#",
                            Description = "Sejarah .NET, CLR, Roslyn Compiler, .csproj, dotnet CLI, dan Program.cs.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-02-01", LevelId = "level-02", ModuleId = "mod-02-01", Order = 1, Title = "Ekosistem .NET Modern: CLR, Roslyn Compiler, dan dotnet CLI", Slug = "dotnet-ecosystem-clr", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true, SimulatorType = "terminal" },
                                new LessonSummaryDto { Id = "lesson-02-02", LevelId = "level-02", ModuleId = "mod-02-01", Order = 2, Title = "Anatomi Program C#: Top-Level Statements, Namespace, dan Main Method", Slug = "csharp-program-structure", Difficulty = "Beginner", EstimatedMinutes = 35, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-02-02",
                            LevelId = "level-02",
                            Order = 2,
                            Title = "Sistem Tipe C#, Memori & Konversi",
                            Description = "Value Types, Reference Types, Stack vs Heap, Boxing/Unboxing, dan Nullable Types.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-02-03", LevelId = "level-02", ModuleId = "mod-02-02", Order = 3, Title = "Sistem Tipe C#: Integer, Floating Point, Decimal, Boolean, dan Char", Slug = "csharp-primitive-types", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-02-04", LevelId = "level-02", ModuleId = "mod-02-02", Order = 4, Title = "Value Types vs Reference Types: Stack vs Heap Memory Allocation", Slug = "value-vs-reference-types", Difficulty = "Intermediate", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true, SimulatorType = "stack-heap" },
                                new LessonSummaryDto { Id = "lesson-02-05", LevelId = "level-02", ModuleId = "mod-02-02", Order = 5, Title = "Boxing, Unboxing, Type Casting, dan Dampak Performance", Slug = "boxing-unboxing-performance", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true, SimulatorType = "stack-heap" }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-02-03",
                            LevelId = "level-02",
                            Order = 3,
                            Title = "Struktur Kontrol, Methods & Koleksi Data",
                            Description = "Kondisional, Pattern Matching, Perulangan, Method Parameters (ref, out, in), Arrays, List & Dictionary.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-02-06", LevelId = "level-02", ModuleId = "mod-02-03", Order = 6, Title = "Struktur Kontrol: If/Else, Switch Expression, dan Pattern Matching Modern", Slug = "control-flow-pattern-matching", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-02-07", LevelId = "level-02", ModuleId = "mod-02-03", Order = 7, Title = "Methods: Return Types, Overloading, Named & Optional Params, serta ref, out, in", Slug = "methods-parameters-ref-out", Difficulty = "Beginner", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-02-08", LevelId = "level-02", ModuleId = "mod-02-03", Order = 8, Title = "Koleksi Data C#: Array, List<T>, Dictionary<K, V>, HashSet<T>, Stack dan Queue", Slug = "csharp-collections", Difficulty = "Beginner", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-02-09", LevelId = "level-02", ModuleId = "mod-02-03", Order = 9, Title = "Robust Exception Handling: Try/Catch/Finally, Custom Exceptions & Filter", Slug = "exception-handling-robust", Difficulty = "Beginner", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 03
                new LevelDetailDto
                {
                    Id = "level-03",
                    Order = 3,
                    Title = "LEVEL 03 — Object-Oriented Programming (OOP)",
                    Subtitle = "Enkapsulasi, Abstraksi, Pewarisan, Polimorfisme, Struct, Record, Generics & Events",
                    Description = "Memahami paradigma berorientasi objek dalam C# dengan kedalaman arsitektur enterprise.",
                    Difficulty = "Intermediate",
                    EstimatedHours = 22,
                    Icon = "Layers",
                    KeyTopics = new List<string> { "Class vs Struct vs Record", "Polymorphism & Virtual/Override", "Interfaces & Abstraction", "Generics & Constraints", "Delegates & Events" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-03-01",
                            LevelId = "level-03",
                            Order = 1,
                            Title = "Pilar Utama OOP & Anatomi Kelas",
                            Description = "Class, Object, Constructor, Properties, Enkapsulasi, dan Access Modifiers.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-03-01", LevelId = "level-03", ModuleId = "mod-03-01", Order = 1, Title = "Kelas, Objek & Enkapsulasi: Constructor, Init-Only Properties, dan Invariant", Slug = "oop-classes-encapsulation", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true, SimulatorType = "object-ref" },
                                new LessonSummaryDto { Id = "lesson-03-02", LevelId = "level-03", ModuleId = "mod-03-01", Order = 2, Title = "Pewarisan vs Komposisi: Mengapa Enterprise Lebih Memilih Komposisi", Slug = "inheritance-vs-composition", Difficulty = "Intermediate", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-03-02",
                            LevelId = "level-03",
                            Order = 2,
                            Title = "Polimorfisme, Interface & Tipe Modern (Record & Struct)",
                            Description = "Virtual, Override, Abstract Class, Interface, Record Immutability, dan Struct.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-03-03", LevelId = "level-03", ModuleId = "mod-03-02", Order = 3, Title = "Polimorfisme Dinamis: Virtual, Override, Sealed, dan VTable Resolution", Slug = "polymorphism-vtable", Difficulty = "Intermediate", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-03-04", LevelId = "level-03", ModuleId = "mod-03-02", Order = 4, Title = "Interfaces & Kontrak: Desain Abstraksi Bersih dan Loose Coupling", Slug = "interfaces-abstraction-contracts", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-03-05", LevelId = "level-03", ModuleId = "mod-03-02", Order = 5, Title = "Record & Value Immutability: Deep Dive C# Records vs Class vs Struct", Slug = "csharp-records-immutability", Difficulty = "Intermediate", EstimatedMinutes = 40, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-03-03",
                            LevelId = "level-03",
                            Order = 3,
                            Title = "Generics, Delegates & Events",
                            Description = "Generics, Type Constraints, Func, Action, Events, dan Pencegahan Memory Leak.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-03-06", LevelId = "level-03", ModuleId = "mod-03-03", Order = 6, Title = "Mastering C# Generics: Type Constraints, Covariance, dan Contravariance", Slug = "csharp-generics-variance", Difficulty = "Intermediate", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-03-07", LevelId = "level-03", ModuleId = "mod-03-03", Order = 7, Title = "Delegates, Func, Action, Predicate dan Event Memory Leaks", Slug = "delegates-events-memory-leaks", Difficulty = "Intermediate", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 04
                new LevelDetailDto
                {
                    Id = "level-04",
                    Order = 4,
                    Title = "LEVEL 04 — Intermediate & Advanced C#",
                    Subtitle = "LINQ, Deferred Execution, Reflection, Source Generators, Span<T> & Memory<T>",
                    Description = "Teknik tingkat lanjut C# untuk manipulasi data efisien, metaprogramming, dan high-performance memory semantics.",
                    Difficulty = "Advanced",
                    EstimatedHours = 26,
                    Icon = "Zap",
                    KeyTopics = new List<string> { "LINQ & Deferred Execution", "IEnumerable vs IQueryable", "Span<T> & ref struct", "Reflection & Metadata", "Roslyn Source Generators" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-04-01",
                            LevelId = "level-04",
                            Order = 1,
                            Title = "LINQ & Internal Iterasi",
                            Description = "Query vs Method Syntax, Deferred vs Immediate Execution, IEnumerable vs IQueryable, dan Yield Return.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-04-01", LevelId = "level-04", ModuleId = "mod-04-01", Order = 1, Title = "LINQ Deep Dive: Deferred Execution, Yield Return, dan State Machine Generator", Slug = "linq-deferred-execution-yield", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-04-02", LevelId = "level-04", ModuleId = "mod-04-01", Order = 2, Title = "IEnumerable vs IQueryable: Perbedaan Runtime Execution dan Expression Trees", Slug = "ienumerable-vs-iqueryable", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-04-02",
                            LevelId = "level-04",
                            Order = 2,
                            Title = "High-Performance Memory Semantics: Span<T> & Ref Struct",
                            Description = "Span<T>, ReadOnlySpan<T>, Memory<T>, stackalloc, dan zero-allocation parsing.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-04-03", LevelId = "level-04", ModuleId = "mod-04-02", Order = 3, Title = "High-Performance C#: Span<T>, Memory<T>, dan Zero-Allocation Programming", Slug = "span-memory-zero-allocation", Difficulty = "Advanced", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true, SimulatorType = "memory" }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-04-03",
                            LevelId = "level-04",
                            Order = 3,
                            Title = "Metaprogramming & Serialisasi Modern",
                            Description = "Reflection, Custom Attributes, Roslyn Source Generators, dan System.Text.Json Custom Converters.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-04-04", LevelId = "level-04", ModuleId = "mod-04-03", Order = 4, Title = "Reflection & Metadata: Inspeksi Tipe Dinamis dan Kompilasi Emisi", Slug = "reflection-metadata-internals", Difficulty = "Advanced", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-04-05", LevelId = "level-04", ModuleId = "mod-04-03", Order = 5, Title = "Modern Serialization: System.Text.Json, Custom Converters & Versioning", Slug = "json-serialization-versioning", Difficulty = "Advanced", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 05
                new LevelDetailDto
                {
                    Id = "level-05",
                    Order = 5,
                    Title = "LEVEL 05 — Data Structures & Algorithms",
                    Subtitle = "Kompleksitas Asimptotik Big O, Hash Table, Pohon B-Tree/AVL, Graph & Dynamic Programming",
                    Description = "Struktur data dan algoritma fundamental yang diimplementasikan secara konkret dalam C# berorientasi performa enterprise.",
                    Difficulty = "Advanced",
                    EstimatedHours = 28,
                    Icon = "Activity",
                    KeyTopics = new List<string> { "Big O, Omega, Theta", "Hash Tables & Collisions", "Binary Search Trees & B-Trees", "Graph BFS/DFS & Dijkstra", "Dynamic Programming" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-05-01",
                            LevelId = "level-05",
                            Order = 1,
                            Title = "Analisis Algoritma & Kompleksitas Asimptotik",
                            Description = "Big O, Big Omega, Big Theta, Analisis Amortized, Best/Average/Worst Case.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-05-01", LevelId = "level-05", ModuleId = "mod-05-01", Order = 1, Title = "Analisis Kompleksitas Big O: Waktu, Ruang, dan Trade-Off Sistem Skala Besar", Slug = "big-o-complexity-analysis", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-05-02",
                            LevelId = "level-05",
                            Order = 2,
                            Title = "Struktur Data Linear & Hash-Based",
                            Description = "Dynamic Arrays, Linked List, Stack, Queue, Circular Queue, dan Hash Table Internals.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-05-02", LevelId = "level-05", ModuleId = "mod-05-02", Order = 2, Title = "Hash Table Internals: Hash Functions, Bucket Arrays, Chaining vs Open Addressing", Slug = "hash-table-internals", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-05-03",
                            LevelId = "level-05",
                            Order = 3,
                            Title = "Trees, B-Trees & Graph Algorithms",
                            Description = "Binary Search Tree, AVL, Red-Black, B-Tree (Index DB), Graph Traversal, dan Dijkstra.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-05-03", LevelId = "level-05", ModuleId = "mod-05-03", Order = 3, Title = "B-Tree & B+ Tree: Struktur Data Fundamental Database Indexing dan Disk I/O", Slug = "btree-database-index", Difficulty = "Advanced", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true, SimulatorType = "btree" },
                                new LessonSummaryDto { Id = "lesson-05-04", LevelId = "level-05", ModuleId = "mod-05-03", Order = 4, Title = "Graph Algorithms: BFS, DFS, Topological Sorting, dan Shortest Path Dijkstra", Slug = "graph-algorithms-routing", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 06
                new LevelDetailDto
                {
                    Id = "level-06",
                    Order = 6,
                    Title = "LEVEL 06 — Computer Architecture & Operating Systems",
                    Subtitle = "Cache Coherency, Context Switching, Paging, Inter-Process Communication & Thread Lifecycle",
                    Description = "Pemahaman mendalam mengenai interaksi perangkat lunak backend enterprise dengan OS dan hardware CPU modern.",
                    Difficulty = "Advanced",
                    EstimatedHours = 24,
                    Icon = "Server",
                    KeyTopics = new List<string> { "Branch Prediction & Out-of-Order", "Cache Line False Sharing", "Process vs Thread", "Race Conditions & Deadlocks", "Linux vs Windows Internals" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-06-01",
                            LevelId = "level-06",
                            Order = 1,
                            Title = "Mikroarsitektur CPU & Cache Locality",
                            Description = "Pipeline, Branch Prediction, Cache Lines, Spatial/Temporal Locality, dan False Sharing.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-06-01", LevelId = "level-06", ModuleId = "mod-06-01", Order = 1, Title = "CPU Pipeline, Branch Prediction, Cache Locality, dan False Sharing", Slug = "cpu-cache-locality-false-sharing", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-06-02",
                            LevelId = "level-06",
                            Order = 2,
                            Title = "Proses, Thread, Context Switch & Sinkronisasi",
                            Description = "Process Lifecycle, Thread Scheduling, Context Switching Cost, Critical Sections, dan Deadlocks.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-06-02", LevelId = "level-06", ModuleId = "mod-06-02", Order = 2, Title = "Proses vs Thread: Context Switching Overhead, Thread Stack, dan IPC", Slug = "process-thread-context-switch", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-06-03", LevelId = "level-06", ModuleId = "mod-06-02", Order = 3, Title = "Sinkronisasi Rendah: Race Conditions, Deadlocks, Starvation, dan Atomic Ops", Slug = "concurrency-hazards-deadlocks", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true, SimulatorType = "deadlock" }
                            }
                        }
                    }
                },

                // LEVEL 07
                new LevelDetailDto
                {
                    Id = "level-07",
                    Order = 7,
                    Title = "LEVEL 07 — Networking & Internet Engineering",
                    Subtitle = "OSI, TCP/IP, Three-Way Handshake, HTTP/1.1-2-3, TLS 1.3, Reverse Proxy & Load Balancer",
                    Description = "Arsitektur jaringan backend dari paket Ethernet fisik hingga Layer 7 HTTP/3, WebSockets, dan TLS Handshake.",
                    Difficulty = "Advanced",
                    EstimatedHours = 24,
                    Icon = "Globe",
                    KeyTopics = new List<string> { "TCP 3-Way Handshake", "TCP Flow & Congestion Control", "HTTP/1.1 vs HTTP/2 vs HTTP/3 (QUIC)", "TLS 1.3 Handshake & PKI", "Reverse Proxy & L4/L7 Load Balancing" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-07-01",
                            LevelId = "level-07",
                            Order = 1,
                            Title = "Fondasi Jaringan Komputer & Model TCP/IP",
                            Description = "OSI 7 Layers, IP Routing, Subnetting CIDR, TCP vs UDP, dan Socket.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-07-01", LevelId = "level-07", ModuleId = "mod-07-01", Order = 1, Title = "Model OSI & TCP/IP: Enkapsulasi Paket, IP Addressing, dan Port Socket", Slug = "osi-tcp-ip-encapsulation", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-07-02", LevelId = "level-07", ModuleId = "mod-07-01", Order = 2, Title = "TCP Deep Dive: 3-Way Handshake, Flow Control, dan Congestion Window", Slug = "tcp-handshake-congestion-control", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false, SimulatorType = "tcp" }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-07-02",
                            LevelId = "level-07",
                            Order = 2,
                            Title = "Protokol Aplikasi Web & Keamanan Transport",
                            Description = "HTTP/1.1, HTTP/2 (Multiplexing), HTTP/3 (QUIC), TLS 1.3 Handshake, Certificates, PKI.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-07-03", LevelId = "level-07", ModuleId = "mod-07-02", Order = 3, Title = "Evolusi HTTP: Dari HTTP/1.1 Head-of-Line Blocking hingga HTTP/3 QUIC", Slug = "http-evolution-quic", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-07-04", LevelId = "level-07", ModuleId = "mod-07-02", Order = 4, Title = "TLS 1.3 Handshake, Sertifikat Digital, PKI, dan Enkripsi End-to-End", Slug = "tls-handshake-pki", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false, SimulatorType = "tls" },
                                new LessonSummaryDto { Id = "lesson-07-05", LevelId = "level-07", ModuleId = "mod-07-02", Order = 5, Title = "Reverse Proxy, API Gateway, dan Load Balancing L4 vs L7", Slug = "reverse-proxy-load-balancing", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false }
                            }
                        }
                    }
                },

                // LEVEL 08
                new LevelDetailDto
                {
                    Id = "level-08",
                    Order = 8,
                    Title = "LEVEL 08 — .NET Runtime & Internal Engineering",
                    Subtitle = "CoreCLR, JIT Compilation, Tiered PGO, Garbage Collector Gen 0/1/2/LOH & Profiling",
                    Description = "Bedah internal Common Language Runtime, pipeline kompilasi IL ke machine code, memori heap managed, dan garbage collection.",
                    Difficulty = "Expert",
                    EstimatedHours = 26,
                    Icon = "Cpu",
                    KeyTopics = new List<string> { "CoreCLR & IL Execution", "JIT, Dynamic PGO & Native AOT", "GC Gen 0/1/2, LOH, POH", "IDisposable & SafeHandle", "dotnet-trace & dotnet-dump" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-08-01",
                            LevelId = "level-08",
                            Order = 1,
                            Title = "Arsitektur CoreCLR & Pipeline Kompilasi JIT",
                            Description = "Intermediate Language (IL), JIT Compilation, Tiered Compilation, Dynamic PGO, dan Native AOT.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-08-01", LevelId = "level-08", ModuleId = "mod-08-01", Order = 1, Title = "Arsitektur CoreCLR: IL, Metadata, JIT Compilation, dan Dynamic PGO", Slug = "coreclr-jit-dynamic-pgo", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-08-02",
                            LevelId = "level-08",
                            Order = 2,
                            Title = "Managed Memory & Garbage Collector Deep Dive",
                            Description = "Generational GC (Gen 0, 1, 2), Large Object Heap (LOH), Pinned Object Heap (POH), Workstation vs Server GC.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-08-02", LevelId = "level-08", ModuleId = "mod-08-02", Order = 2, Title = "Garbage Collector Internals: Gen 0/1/2, LOH, Mark-Sweep-Compact, dan GC Tuning", Slug = "garbage-collector-internals", Difficulty = "Expert", EstimatedMinutes = 65, HasQuiz = true, HasExercise = true, SimulatorType = "gc" },
                                new LessonSummaryDto { Id = "lesson-08-03", LevelId = "level-08", ModuleId = "mod-08-02", Order = 3, Title = "Pola Resource Cleanup: IDisposable, IAsyncDisposable, dan Finalization Guard", Slug = "idisposable-pattern-unmanaged", Difficulty = "Advanced", EstimatedMinutes = 45, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 09
                new LevelDetailDto
                {
                    Id = "level-09",
                    Order = 9,
                    Title = "LEVEL 09 — Database Engineering",
                    Subtitle = "Relasional vs NoSQL, Index B-Tree, ACID, MVCC, Query Execution Plan & Sharding",
                    Description = "Desain basis data enterprise berskala tinggi, indexing mendalam, transaksi ACID, isolasi MVCC, dan query tuning.",
                    Difficulty = "Advanced",
                    EstimatedHours = 28,
                    Icon = "Database",
                    KeyTopics = new List<string> { "Relational Modeling & Normalization", "B-Tree vs Clustered Indexes", "ACID & Transaction Isolation Levels", "MVCC & Lock Contention", "SARGability & Query Optimization" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-09-01",
                            LevelId = "level-09",
                            Order = 1,
                            Title = "Database Modeling & Relational Schema Design",
                            Description = "Relational Modeling, Normalisasi 1NF-BCNF, Denormalisasi Strategis, Keys & Constraints.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-09-01", LevelId = "level-09", ModuleId = "mod-09-01", Order = 1, Title = "Relational Modeling: Normalisasi 1NF-3NF, Foreign Keys, dan Integritas Relasional", Slug = "relational-modeling-normalization", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-09-02",
                            LevelId = "level-09",
                            Order = 2,
                            Title = "Database Engine Architecture, Indexing & Storage Pages",
                            Description = "Pages & Extents, Buffer Pool, Clustered vs Non-Clustered Indexes, Covering Index, B-Tree Traversal.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-09-02", LevelId = "level-09", ModuleId = "mod-09-02", Order = 2, Title = "Database Storage & Indexing: Clustered Index, Covering Index, dan Scan vs Seek", Slug = "database-indexing-clustered-seek", Difficulty = "Advanced", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true, SimulatorType = "btree" }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-09-03",
                            LevelId = "level-09",
                            Order = 3,
                            Title = "Transaksi, Tingkat Isolasi ACID, Lock & MVCC",
                            Description = "Read Uncommitted, Read Committed, Repeatable Read, Serializable, Snapshot Isolation, MVCC, dan Deadlock.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-09-03", LevelId = "level-09", ModuleId = "mod-09-03", Order = 3, Title = "Transaksi ACID & Isolation Levels: Dirty Read, Lost Update, Phantom Read, dan MVCC", Slug = "acid-isolation-levels-mvcc", Difficulty = "Advanced", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true, SimulatorType = "deadlock" },
                                new LessonSummaryDto { Id = "lesson-09-04", LevelId = "level-09", ModuleId = "mod-09-03", Order = 4, Title = "Query Optimization Enterprise: SARGability, Execution Plans, dan Masalah N+1", Slug = "query-optimization-sargability-n1", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 10
                new LevelDetailDto
                {
                    Id = "level-10",
                    Order = 10,
                    Title = "LEVEL 10 — C# Database Integration",
                    Subtitle = "ADO.NET, Dapper High-Performance Micro-ORM, EF Core 8 & Repository Pattern",
                    Description = "Integrasi data tingkat enterprise di C#: Dari ADO.NET murni, Dapper ultra-fast, hingga Entity Framework Core 8.",
                    Difficulty = "Advanced",
                    EstimatedHours = 24,
                    Icon = "HardDrive",
                    KeyTopics = new List<string> { "ADO.NET DbConnection/Command", "Dapper Micro-ORM", "EF Core Change Tracking", "Migrations & Fluent API", "Repository & Unit of Work" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-10-01",
                            LevelId = "level-10",
                            Order = 1,
                            Title = "Akses Data Tingkat Rendah & Dapper Micro-ORM",
                            Description = "ADO.NET, Parameterized Queries (Anti-SQLi), Dapper Query/Execute, dan Multi-Mapping.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-10-01", LevelId = "level-10", ModuleId = "mod-10-01", Order = 1, Title = "ADO.NET & Parameterized Queries: Pencegahan SQL Injection dan Connection Pooling", Slug = "adonet-connection-pooling-sqli", Difficulty = "Intermediate", EstimatedMinutes = 45, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-10-02", LevelId = "level-10", ModuleId = "mod-10-01", Order = 2, Title = "High-Performance Data Access dengan Dapper: Micro-ORM untuk Skala Besar", Slug = "dapper-micro-orm-performance", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-10-02",
                            LevelId = "level-10",
                            Order = 2,
                            Title = "Entity Framework Core 8 Architecture & Optimasi",
                            Description = "DbContext, Change Tracker, AsNoTracking, Fluent API, Migrations, Eager/Lazy/Explicit Loading.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-10-03", LevelId = "level-10", ModuleId = "mod-10-02", Order = 3, Title = "Entity Framework Core Architecture: DbContext Lifecycle, Change Tracker, dan AsNoTracking", Slug = "efcore-change-tracking-performance", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-10-04", LevelId = "level-10", ModuleId = "mod-10-02", Order = 4, Title = "Pola Akses Data: Repository Pattern, Unit of Work, dan Boundary Transaksional", Slug = "repository-unit-of-work-patterns", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 11
                new LevelDetailDto
                {
                    Id = "level-11",
                    Order = 11,
                    Title = "LEVEL 11 — ASP.NET Core & Backend API Engineering",
                    Subtitle = "Kestrel, Request Pipeline, Middleware, Minimal APIs, Dependency Injection & REST",
                    Description = "Membangun REST API modern, scalable, dan secure menggunakan ASP.NET Core 8 Kestrel dan Generic Host.",
                    Difficulty = "Advanced",
                    EstimatedHours = 26,
                    Icon = "Network",
                    KeyTopics = new List<string> { "Kestrel & Request Pipeline", "Custom Middleware", "Minimal APIs vs Controllers", "Dependency Injection Lifetimes", "Options Pattern & Configuration", "Rate Limiting & Health Checks" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-11-01",
                            LevelId = "level-11",
                            Order = 1,
                            Title = "Arsitektur ASP.NET Core & Middleware Pipeline",
                            Description = "Kestrel Web Server, HttpContext, Middleware Execution Chain, dan Custom Middleware.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-11-01", LevelId = "level-11", ModuleId = "mod-11-01", Order = 1, Title = "Arsitektur ASP.NET Core: Kestrel Server, WebApplication, dan Middleware Pipeline", Slug = "aspnetcore-kestrel-middleware-pipeline", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-11-02", LevelId = "level-11", ModuleId = "mod-11-01", Order = 2, Title = "Minimal APIs vs Controller-Based APIs: Performance, Routing, dan Desain Endpoint", Slug = "minimal-apis-vs-controllers", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-11-02",
                            LevelId = "level-11",
                            Order = 2,
                            Title = "Dependency Injection Container & Konfigurasi Enterprise",
                            Description = "Service Lifetimes (Transient, Scoped, Singleton), Captive Dependencies, Circular References, Options Pattern.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-11-03", LevelId = "level-11", ModuleId = "mod-11-02", Order = 3, Title = "Dependency Injection Deep Dive: Lifetimes, Scope Validation, dan Captive Dependency Trap", Slug = "dependency-injection-captive-dependencies", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-11-04", LevelId = "level-11", ModuleId = "mod-11-02", Order = 4, Title = "Standar REST Enterprise: ProblemDetails, Versioning, Idempotency, dan Pagination", Slug = "rest-enterprise-standards-idempotency", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 12
                new LevelDetailDto
                {
                    Id = "level-12",
                    Order = 12,
                    Title = "LEVEL 12 — Concurrency, Threading & Parallel Programming",
                    Subtitle = "Task, Async/Await State Machine, SemaphoreSlim, Channels & Lock-Free Interlocked",
                    Description = "Pemrograman konkuren dan asinkronus bebas deadlock, high-throughput I/O processing, dan sinkronisasi thread di .NET.",
                    Difficulty = "Expert",
                    EstimatedHours = 28,
                    Icon = "FastForward",
                    KeyTopics = new List<string> { "Async/Await State Machine", "SynchronizationContext & ConfigureAwait", "SemaphoreSlim & Mutex", "System.Threading.Channels", "Lock-Free & Interlocked" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-12-01",
                            LevelId = "level-12",
                            Order = 1,
                            Title = "Model Asinkronus & State Machine C#",
                            Description = "Task-Based Asynchronous Pattern (TAP), State Machine Generation, Task vs ValueTask, CancellationToken.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-12-01", LevelId = "level-12", ModuleId = "mod-12-01", Order = 1, Title = "Internal Async/Await: State Machine Struct, Task Continuations, dan SynchronizationContext", Slug = "async-await-state-machine-internals", Difficulty = "Expert", EstimatedMinutes = 65, HasQuiz = true, HasExercise = false, SimulatorType = "async-flow" },
                                new LessonSummaryDto { Id = "lesson-12-02", LevelId = "level-12", ModuleId = "mod-12-01", Order = 2, Title = "Pola Concurrency Trap: Async Void, Deadlocks, dan Penggunaan CancellationToken yang Benar", Slug = "async-traps-cancellation-tokens", Difficulty = "Expert", EstimatedMinutes = 50, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-12-02",
                            LevelId = "level-12",
                            Order = 2,
                            Title = "Sinkronisasi Thread, Lock & High-Throughput Channels",
                            Description = "Monitor (lock), SemaphoreSlim, ReaderWriterLockSlim, Interlocked CAS, System.Threading.Channels.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-12-03", LevelId = "level-12", ModuleId = "mod-12-02", Order = 3, Title = "Mekanisme Sinkronisasi: Lock, SemaphoreSlim, ReaderWriterLockSlim, dan Interlocked", Slug = "thread-synchronization-locks-interlocked", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true, SimulatorType = "deadlock" },
                                new LessonSummaryDto { Id = "lesson-12-04", LevelId = "level-12", ModuleId = "mod-12-02", Order = 4, Title = "High-Throughput Producer-Consumer dengan System.Threading.Channels dan Backpressure", Slug = "channels-producer-consumer-backpressure", Difficulty = "Expert", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 13
                new LevelDetailDto
                {
                    Id = "level-13",
                    Order = 13,
                    Title = "LEVEL 13 — Software Design Principles & Architecture",
                    Subtitle = "SOLID, Clean Architecture, Vertical Slice, Tactical Domain-Driven Design (DDD) & ADR",
                    Description = "Prinsip rekayasa perangkat lunak enterprise, pemisahan dependensi, domain-driven design, dan arsitektur yang tahan uji masa depan.",
                    Difficulty = "Expert",
                    EstimatedHours = 26,
                    Icon = "Compass",
                    KeyTopics = new List<string> { "SOLID Principles in C#", "Clean vs Onion vs Hexagonal Architecture", "Vertical Slice Architecture", "Tactical DDD (Aggregate, Value Object)", "Architecture Decision Records (ADR)" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-13-01",
                            LevelId = "level-13",
                            Order = 1,
                            Title = "Prinsip Desain SOLID & Pola Desain Enterprise",
                            Description = "Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion, Strategy, Mediator.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-13-01", LevelId = "level-13", ModuleId = "mod-13-01", Order = 1, Title = "Prinsip SOLID dalam Praktik Enterprise C#: Menghindari Over-Engineering dan Code Rot", Slug = "solid-principles-enterprise-csharp", Difficulty = "Advanced", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-13-02",
                            LevelId = "level-13",
                            Order = 2,
                            Title = "Arsitektur Aplikasi Bersih & Domain-Driven Design",
                            Description = "Clean Architecture, Vertical Slice Architecture, Strategic & Tactical DDD (Aggregates, Value Objects, Domain Events).",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-13-02", LevelId = "level-13", ModuleId = "mod-13-02", Order = 2, Title = "Clean Architecture vs Vertical Slice: Dependency Rule, Application Boundaries, dan CQRS", Slug = "clean-architecture-vertical-slice-cqrs", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-13-03", LevelId = "level-13", ModuleId = "mod-13-02", Order = 3, Title = "Tactical Domain-Driven Design (DDD): Entities, Value Objects, Aggregate Roots, dan Invariant", Slug = "domain-driven-design-tactical-aggregates", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 14
                new LevelDetailDto
                {
                    Id = "level-14",
                    Order = 14,
                    Title = "LEVEL 14 — Distributed Systems & Microservices",
                    Subtitle = "CAP Theorem, Event-Driven Architecture, Kafka/RabbitMQ, Saga Pattern & Outbox",
                    Description = "Prinsip sistem terdistribusi, komunikasi pesan asinkronus, transaksi kompensasi Saga, dan ketahanan terhadap kegagalan parsial.",
                    Difficulty = "Expert",
                    EstimatedHours = 30,
                    Icon = "Share2",
                    KeyTopics = new List<string> { "CAP & PACELC Theorems", "Event-Driven Architecture", "Kafka vs RabbitMQ Internals", "Outbox & Inbox Pattern", "Saga Orchestration vs Choreography", "Circuit Breaker & Retry with Jitter" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-14-01",
                            LevelId = "level-14",
                            Order = 1,
                            Title = "Fondasi Sistem Terdistribusi & Konsistensi Data",
                            Description = "Network Partitions, Partial Failure, CAP Theorem, Eventual Consistency, Vector Clocks, Consensus Raft.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-14-01", LevelId = "level-14", ModuleId = "mod-14-01", Order = 1, Title = "Hukum Sistem Terdistribusi: CAP Theorem, PACELC, Kegagalan Parsial dan Eventual Consistency", Slug = "distributed-systems-cap-pacelc", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-14-02",
                            LevelId = "level-14",
                            Order = 2,
                            Title = "Event-Driven Architecture & Message Brokers",
                            Description = "RabbitMQ Exchanges/Queues, Apache Kafka Partitions/Consumer Groups, Message Ordering, Dead Letter Queues.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-14-02", LevelId = "level-14", ModuleId = "mod-14-02", Order = 2, Title = "Event-Driven Architecture: Kafka Partitions, RabbitMQ Exchanges, dan Idempotent Consumers", Slug = "event-driven-kafka-rabbitmq-idempotency", Difficulty = "Expert", EstimatedMinutes = 65, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-14-03",
                            LevelId = "level-14",
                            Order = 3,
                            Title = "Pola Transaksi Terdistribusi & Ketahanan Sistem",
                            Description = "Transactional Outbox Pattern, Saga Pattern (Orchestration vs Choreography), Polly Circuit Breaker, Exponential Backoff.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-14-03", LevelId = "level-14", ModuleId = "mod-14-03", Order = 3, Title = "Transactional Outbox & Saga Pattern: Menghindari Dual-Write dan Menjamin Konsistensi Transaksi", Slug = "outbox-pattern-saga-transactions", Difficulty = "Expert", EstimatedMinutes = 65, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-14-04", LevelId = "level-14", ModuleId = "mod-14-03", Order = 4, Title = "Resilience Engineering: Circuit Breaker, Bulkhead, Retry dengan Jitter, dan Fallback", Slug = "resilience-circuit-breaker-polly", Difficulty = "Expert", EstimatedMinutes = 55, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 15
                new LevelDetailDto
                {
                    Id = "level-15",
                    Order = 15,
                    Title = "LEVEL 15 — Security Engineering",
                    Subtitle = "Zero Trust, OAuth 2.0 / OIDC, JWT Hardening, OWASP Top 10 & Kriptografi Modern",
                    Description = "Keamanan backend enterprise: Otentikasi, otorisasi berbasis klaim, enkripsi data at rest & transit, dan mitigasi kerentanan tingkat sistem.",
                    Difficulty = "Expert",
                    EstimatedHours = 26,
                    Icon = "ShieldCheck",
                    KeyTopics = new List<string> { "Zero Trust & Least Privilege", "JWT Signing, Claims & Rotation", "OAuth 2.0 Authorization Code + PKCE", "OWASP Top 10 Backend Defense", "AES-256-GCM & RSA Cryptography" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-15-01",
                            LevelId = "level-15",
                            Order = 1,
                            Title = "Identitas, Otentikasi & Otorisasi Modern",
                            Description = "Password Hashing (Argon2id, BCrypt), JWT Architecture, Refresh Token Rotation, Policy-Based Authorization.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-15-01", LevelId = "level-15", ModuleId = "mod-15-01", Order = 1, Title = "Otentikasi Aman: Password Hashing (Argon2id), JWT Token Hardening, dan Refresh Token Rotation", Slug = "authentication-argon2-jwt-hardening", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-15-02", LevelId = "level-15", ModuleId = "mod-15-01", Order = 2, Title = "OAuth 2.0 & OpenID Connect: Authorization Code Flow dengan PKCE untuk Backend Enterprise", Slug = "oauth2-oidc-pkce-architecture", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-15-02",
                            LevelId = "level-15",
                            Order = 2,
                            Title = "Pertahanan OWASP Top 10 & Kriptografi Backend",
                            Description = "SQL Injection, SSRF, IDOR, Mass Assignment, AES Enkripsi Simetris, RSA Asimetris, Audit Trail.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-15-03", LevelId = "level-15", ModuleId = "mod-15-02", Order = 3, Title = "OWASP Top 10 Backend Engineering: Mitigasi SQLi, SSRF, IDOR, dan Insecure Deserialization", Slug = "owasp-top-10-backend-defense", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true },
                                new LessonSummaryDto { Id = "lesson-15-04", LevelId = "level-15", ModuleId = "mod-15-02", Order = 4, Title = "Kriptografi Terapan: AES-256 Enkripsi Data at Rest, Hashing HMAC, dan Secrets Management", Slug = "applied-cryptography-aes-secrets", Difficulty = "Expert", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false }
                            }
                        }
                    }
                },

                // LEVEL 16
                new LevelDetailDto
                {
                    Id = "level-16",
                    Order = 16,
                    Title = "LEVEL 16 — Testing, Quality Engineering & DevOps",
                    Subtitle = "Piramida Tes, xUnit, WebApplicationFactory, Docker Multi-Stage, CI/CD & Kubernetes",
                    Description = "Otomasi kualitas perangkat lunak dari unit test, integration test database kontainer, hingga pipeline CI/CD dan deployment Kubernetes.",
                    Difficulty = "Expert",
                    EstimatedHours = 26,
                    Icon = "CheckCircle2",
                    KeyTopics = new List<string> { "Test Pyramid & TDD", "xUnit & WebApplicationFactory", "Docker Multi-Stage Builds", "GitHub Actions CI/CD Pipeline", "Kubernetes Pods, Deployments & Probes" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-16-01",
                            LevelId = "level-16",
                            Order = 1,
                            Title = "Strategi Pengujian Backend & Otomasi xUnit",
                            Description = "Unit Test vs Integration Test, xUnit Theories/Facts, WebApplicationFactory, Mocking vs Faking.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-16-01", LevelId = "level-16", ModuleId = "mod-16-01", Order = 1, Title = "Strategi Pengujian xUnit: Unit Testing Komprehensif, Mocking, dan Assertion Patterns", Slug = "xunit-testing-strategies-mocking", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-16-02", LevelId = "level-16", ModuleId = "mod-16-01", Order = 2, Title = "Integration Testing ASP.NET Core: WebApplicationFactory dan Database Kontainer", Slug = "integration-testing-webapplicationfactory", Difficulty = "Expert", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-16-02",
                            LevelId = "level-16",
                            Order = 2,
                            Title = "Konteinerisasi Docker, CI/CD & Kubernetes",
                            Description = "Dockerfile Multi-stage aman, Docker Compose, GitHub Actions CI/CD, Kubernetes Pods, Deployments, Readiness/Liveness Probes.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-16-03", LevelId = "level-16", ModuleId = "mod-16-02", Order = 3, Title = "Docker Multi-Stage Build untuk .NET: Minimal Image Size, Non-Root User, dan Container Security", Slug = "docker-multistage-dotnet-security", Difficulty = "Advanced", EstimatedMinutes = 50, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-16-04", LevelId = "level-16", ModuleId = "mod-16-02", Order = 4, Title = "CI/CD Pipeline & Kubernetes Deployment: Automated Testing, Zero-Downtime Rolling Update", Slug = "cicd-kubernetes-zero-downtime", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                },

                // LEVEL 17
                new LevelDetailDto
                {
                    Id = "level-17",
                    Order = 17,
                    Title = "LEVEL 17 — Production, Performance & Reliability Engineering (SRE)",
                    Subtitle = "Observability, OpenTelemetry, BenchmarkDotNet, SLI/SLO, Postmortem & GC Tuning",
                    Description = "Mengoperasikan backend di produksi dengan keandalan 99.99%: Observability terdistribusi, profiling performa mikrodetik, dan SRE incident response.",
                    Difficulty = "Expert",
                    EstimatedHours = 28,
                    Icon = "BarChart2",
                    KeyTopics = new List<string> { "Distributed Tracing with OpenTelemetry", "Structured Logging & Correlation IDs", "SLI, SLO & Error Budgets", "BenchmarkDotNet Hot Path Tuning", "Production Memory Leak Investigation" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-17-01",
                            LevelId = "level-17",
                            Order = 1,
                            Title = "Observability Terdistribusi & Monitoring",
                            Description = "Structured Logging, Distributed Tracing dengan OpenTelemetry, Correlation ID propagation, Prometheus & Grafana.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-17-01", LevelId = "level-17", ModuleId = "mod-17-01", Order = 1, Title = "Observability Terdistribusi: OpenTelemetry Tracing, Correlation IDs, dan W3C TraceContext", Slug = "opentelemetry-distributed-tracing", Difficulty = "Expert", EstimatedMinutes = 60, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-17-02",
                            LevelId = "level-17",
                            Order = 2,
                            Title = "Site Reliability Engineering (SRE) & Incident Management",
                            Description = "SLI, SLO, SLA, Error Budget, Blameless Postmortem, On-Call Playbooks, Graceful Degradation.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-17-02", LevelId = "level-17", ModuleId = "mod-17-02", Order = 2, Title = "SRE Fundamentals: Mendesain SLI/SLO, Mengelola Error Budget, dan Blameless Postmortems", Slug = "sre-sli-slo-error-budget-postmortem", Difficulty = "Expert", EstimatedMinutes = 55, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-17-03",
                            LevelId = "level-17",
                            Order = 3,
                            Title = "Investigasi & Optimasi Performa Produksi",
                            Description = "BenchmarkDotNet, dotnet-dump, dotnet-trace, Analisis High CPU & Memory Leak di Produksi.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-17-03", LevelId = "level-17", ModuleId = "mod-17-03", Order = 3, Title = "Investigasi Masalah Produksi: Diagnosa High CPU, Memory Leaks, dan Thread Pool Starvation", Slug = "production-debugging-dumps-traces", Difficulty = "Expert", EstimatedMinutes = 65, HasQuiz = true, HasExercise = false }
                            }
                        }
                    }
                },

                // LEVEL 18
                new LevelDetailDto
                {
                    Id = "level-18",
                    Order = 18,
                    Title = "LEVEL 18 — Enterprise Architecture & Principal Engineering",
                    Subtitle = "Double-Entry Ledger, Financial Transactions, Legacy Modernization & Kepemimpinan Teknik",
                    Description = "Tingkat tertinggi rekayasa backend: Arsitektur perbankan dan transaksi finansial, buku besar double-entry, tata kelola arsitektur, dan kepemimpinan engineering organisasi.",
                    Difficulty = "Principal",
                    EstimatedHours = 32,
                    Icon = "Award",
                    KeyTopics = new List<string> { "Double-Entry Ledger Architecture", "Idempotent Financial Settlement", "Multi-Tenant & Multi-Region", "Legacy System Modernization", "Architecture Review & Technical Leadership" },
                    Modules = new List<ModuleDto>
                    {
                        new ModuleDto
                        {
                            Id = "mod-18-01",
                            LevelId = "level-18",
                            Order = 1,
                            Title = "Arsitektur Transaksi Finansial & Buku Besar Double-Entry",
                            Description = "Prinsip Akuntansi Finansial, Double-Entry Ledger, Debit/Kredit Invariants, Settlement, Idempotency, dan Reversal.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-18-01", LevelId = "level-18", ModuleId = "mod-18-01", Order = 1, Title = "Arsitektur Buku Besar Finansial (Double-Entry Ledger): Invariant Debit-Kredit, Audit Trail, dan Idempotent Settlement", Slug = "double-entry-ledger-architecture", Difficulty = "Principal", EstimatedMinutes = 70, HasQuiz = true, HasExercise = false },
                                new LessonSummaryDto { Id = "lesson-18-02", LevelId = "level-18", ModuleId = "mod-18-01", Order = 2, Title = "Pemrosesan Pembayaran Skala Tinggi: Penanganan Transaksi Gagal, Rekonsiliasi Otomatis, dan Reversal", Slug = "high-volume-financial-reconciliation", Difficulty = "Principal", EstimatedMinutes = 65, HasQuiz = true, HasExercise = false }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-18-02",
                            LevelId = "level-18",
                            Order = 2,
                            Title = "Arsitektur Enterprise Skala Global & Modernisasi Sistem",
                            Description = "Multi-Tenant Isolation, Multi-Region Active-Active, Zero-Downtime Database Migration, Strangler Fig Pattern.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-18-03", LevelId = "level-18", ModuleId = "mod-18-02", Order = 3, Title = "Modernisasi Sistem Warisan (Legacy): Strangler Fig Pattern, CDC, dan Zero-Downtime Data Migration", Slug = "legacy-modernization-strangler-cdc", Difficulty = "Principal", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        },
                        new ModuleDto
                        {
                            Id = "mod-18-03",
                            LevelId = "level-18",
                            Order = 3,
                            Title = "Kepemimpinan Teknik & Peran Principal Engineer",
                            Description = "Architecture Governance, Architecture Decision Records (ADR), Mentoring, Technical Vision, dan Risk Management.",
                            Lessons = new List<LessonSummaryDto>
                            {
                                new LessonSummaryDto { Id = "lesson-18-04", LevelId = "level-18", ModuleId = "mod-18-03", Order = 4, Title = "Peran Principal Engineer: Technical Vision, ADR, Architecture Review Board, dan Manajemen Risiko Skala Organisasi", Slug = "principal-engineering-leadership-vision", Difficulty = "Principal", EstimatedMinutes = 60, HasQuiz = true, HasExercise = true }
                            }
                        }
                    }
                }
            };

            // Calculate module and lesson counts for summaries
            foreach (var lvl in levels)
            {
                lvl.ModuleCount = lvl.Modules.Count;
                lvl.LessonCount = lvl.Modules.Sum(m => m.Lessons.Count);
            }

            return levels;
        }

        public static LevelSummaryDto ToSummary(LevelDetailDto level)
        {
            return new LevelSummaryDto
            {
                Id = level.Id,
                Order = level.Order,
                Title = level.Title,
                Subtitle = level.Subtitle,
                Description = level.Description,
                Icon = level.Icon,
                Difficulty = level.Difficulty,
                EstimatedHours = level.EstimatedHours,
                KeyTopics = level.KeyTopics,
                ModuleCount = level.Modules.Count,
                LessonCount = level.Modules.Sum(m => m.Lessons.Count)
            };
        }
    }
}
