# ENTERPRISE BACKEND ENGINEERING ACADEMY
> **Zero to Enterprise Principal Engineer — Modern C# & .NET 8**

Sebuah platform edukasi teknis interaktif, full-text technical documentation, structured learning platform, dan enterprise knowledge base yang dirancang khusus untuk membawa seseorang dari nol (tanpa latar belakang coding) hingga menguasai perancangan sistem backend enterprise berskala besar menggunakan C# dan .NET modern.

---

## 🌟 Fitur Utama Platform

### 1. Master Curriculum (18 Learning Levels)
Mencakup kurikulum lengkap berstandar industri dengan materi mendalam dalam **Bahasa Indonesia** dan istilah teknis standar industri:
- **LEVEL 00** — Computer & Digital Fundamentals (CPU, Register, Cache, Two's Complement, OS, Kernel, Virtual Memory, CLI)
- **LEVEL 01** — Programming Logic & Problem Solving (Compiler vs Interpreter, Algoritma, Flowchart, Logika Boolean, Rekursi)
- **LEVEL 02** — C# Programming Fundamentals (CLR, Roslyn, Value vs Reference Types, Boxing/Unboxing, Control Flow, Collections)
- **LEVEL 03** — Object-Oriented Programming (Class, Struct, Record, Virtual/Override, Generics, Variance, Events)
- **LEVEL 04** — Intermediate & Advanced C# (LINQ Deferred Execution, Span<T>, Memory<T>, Reflection, Source Generators)
- **LEVEL 05** — Data Structures & Algorithms (Big O, Hash Table, B-Tree, BST, Graph, Dijkstra, Dynamic Programming)
- **LEVEL 06** — Computer Architecture & Operating Systems (Branch Prediction, Cache Locality, False Sharing, Context Switch)
- **LEVEL 07** — Networking & Internet Engineering (OSI, TCP 3-Way Handshake, Flow Control, HTTP/1.1-2-3, TLS 1.3, Reverse Proxy)
- **LEVEL 08** — .NET Runtime & Internal Engineering (CoreCLR, JIT, Tiered PGO, Native AOT, GC Gen 0/1/2/LOH/POH)
- **LEVEL 09** — Database Engineering (Relational Modeling, Normalisasi, B-Tree Index, ACID, MVCC, Lock Contention, SARGability)
- **LEVEL 10** — C# Database Integration (ADO.NET, Dapper Micro-ORM, EF Core 8, Change Tracking, Repository & Unit of Work)
- **LEVEL 11** — ASP.NET Core & Backend API Engineering (Kestrel, Middleware Pipeline, Minimal APIs, Dependency Injection, REST)
- **LEVEL 12** — Concurrency, Threading & Parallel Programming (Async/Await State Machine, SemaphoreSlim, Channels, Interlocked)
- **LEVEL 13** — Software Design Principles & Architecture (SOLID, Clean Architecture, Vertical Slice, Tactical DDD)
- **LEVEL 14** — Distributed Systems & Microservices (CAP/PACELC, Kafka/RabbitMQ, Transactional Outbox, Saga, Circuit Breaker)
- **LEVEL 15** — Security Engineering (Zero Trust, Argon2, JWT Hardening, OAuth 2.0 PKCE, OWASP Top 10, Kriptografi AES-256)
- **LEVEL 16** — Testing, Quality Engineering & DevOps (xUnit, WebApplicationFactory, Docker Multi-Stage, CI/CD, Kubernetes)
- **LEVEL 17** — Production, Performance & Reliability Engineering (OpenTelemetry Tracing, SRE, SLI/SLO, Postmortem, GC Tuning)
- **LEVEL 18** — Enterprise Architecture & Principal Engineering (Double-Entry Ledger, Financial Settlements, Legacy Modernization, ADR)

---

### 2. Interactive C# Code Editor (Monaco Editor)
- Monaco Editor bertenaga C# syntax highlighting.
- **In-Memory Roslyn Code Execution Engine** di backend: Mengompilasi dan mengeksekusi C# secara instan (<200ms) dengan redirect `Console.Out` dan `Console.Error`.
- Penanganan execution timeout (4000ms max) untuk mencegah infinite loops.
- Error highlighting dan pelaporan diagnostik baris/kolom.
- Mendukung input standar (`Console.ReadLine()`).
- Salin kode, download file `.cs`, dan reset ke kode awal.

---

### 3. Interactive Visual Simulators
Simulator visual interaktif yang membantu memvisualisasikan konsep abstrak:
1. **8-Bit Binary & Two's Complement Simulator**: Toggle bit interaktif, konversi unsigned/signed, kalkulasi heksadesimal, dan MSB sign bit.
2. **Stack vs Heap Memory Simulator**: Visualisasi langkah demi langkah alokasi Value Type di Stack frame vs Reference Type pointer ke Managed Heap.
3. **Garbage Collector Generations Simulator**: Alokasi objek, mutasi GC Roots, mark-sweep collection, dan promosi generasi (Gen 0 ➔ Gen 1 ➔ Gen 2 & LOH).
4. **Async/Await State Machine Flow Simulator**: Alur transformasi Roslyn struct `IAsyncStateMachine`, pelepasan thread ke ThreadPool, dan sinyal OS IOCP.
5. **Database B+ Tree Index Seek Simulator**: Perbandingan visual antara Clustered Index Seek (3 disk I/Os) vs Full Table Scan (10.000 pages read).
6. **Concurrency Deadlock & Lock Ordering Simulator**: Skenario circular wait dua transaksi dan pembuktian matematis pencegahan deadlock dengan ordered locking.

---

### 4. Interactive Dotnet CLI Sandbox Terminal
- Terminal CLI terisolasi untuk melatih perintah `dotnet`.
- Mendukung: `dotnet --version`, `dotnet --info`, `dotnet build`, `dotnet run`, `ls`, `cat Program.cs`, `pwd`, `clear`, `reset`.
- Terisolasi di workspace sesi sementara dengan proteksi path traversal dan pembatasan perintah berbahaya.

---

### 5. Quiz & Coding Challenge Engine
- Kuis pilihan ganda, analisis kode, dan prediksi output di setiap materi.
- Penilaian instan dengan batas kelulusan 80%.
- **Penjelasan Mendalam**: Membedah mengapa jawaban benar adalah tepat DAN mengapa opsi lain salah.
- Coding Challenge dengan automated grading terhadap unit test cases.

---

### 6. 12 Enterprise Projects
Spesifikasi lengkap 12 proyek bertingkat:
1. **PROJECT 01** — Console Calculator & Expression Evaluator
2. **PROJECT 02** — Student Information & Academic Grading Engine
3. **PROJECT 03** — Employee Directory & Generic Repository System
4. **PROJECT 04** — Enterprise Inventory Management REST API
5. **PROJECT 05** — Enterprise Authentication & JWT Service
6. **PROJECT 06** — Enterprise Order Processing System (Clean Architecture & CQRS)
7. **PROJECT 07** — High-Concurrency Flash Sale Engine (Atomic CAS & Lock-Free)
8. **PROJECT 08** — Distributed Background Job Processor (Channels & Workers)
9. **PROJECT 09** — Event-Driven Payment Settlement (Saga & Outbox Pattern)
10. **PROJECT 10** — Microservices Platform & Distributed Tracing
11. **PROJECT 11** — Enterprise Document Management & Audit Trail
12. **PROJECT 12** — Core Enterprise Banking Backend (Double-Entry Ledger, Idempotent Transfers, Immutable Audit Trail, Reversal)

---

## 🛠️ Technology Stack

| Layer | Komponen & Library |
| :--- | :--- |
| **Frontend** | React 19, TypeScript, Vite, Tailwind CSS v4, Monaco Editor (`@monaco-editor/react`), Lucide React, React Markdown |
| **Backend** | ASP.NET Core 8 Web API, C# 12, Roslyn Compiler (`Microsoft.CodeAnalysis.CSharp`), Entity Framework Core 8 |
| **Database** | SQLite (out-of-the-box zero config), EF Core Migrations |
| **Testing** | xUnit, .NET Test Runner |
| **Architecture** | REST API, Clean Architecture, Roslyn In-Memory Sandboxed Runner |

---

## 🚀 Panduan Menjalankan Aplikasi

### Prasyarat
- **.NET 8 SDK** (`dotnet --version` >= 8.0)
- **Node.js** (`node -v` >= 18.0) dan **npm**

### 1. Menjalankan Backend API
```bash
# Dari root direktori learn-cs
dotnet run --project backend/Academy.Api/Academy.Api.csproj --urls "http://localhost:5100"
```
Backend akan aktif di `http://localhost:5100`. Database SQLite `academy.db` akan dibuat dan di-seed secara otomatis pada startup pertama kali.
Swagger API Explorer dapat diakses di: `http://localhost:5100/swagger`.

### 2. Menjalankan Frontend Development Server
```bash
# Buka terminal baru di root direktori
cd frontend
npm run dev
```
Frontend development server akan aktif di `http://localhost:3000` dengan reverse proxy otomatis ke backend di port `5100`.

### 3. Menjalankan Automated Tests
```bash
# Menjalankan seluruh test suite xUnit (Roslyn runner, kurikulum, sandbox, dsb)
dotnet test backend/Academy.Tests/Academy.Tests.csproj
```

### 4. Build Production Standalone
```bash
# Build frontend
cd frontend
npm run build
cd ..

# Build backend
dotnet build backend/Academy.Api/Academy.Api.csproj -c Release

# Jalankan server production (menyajikan Frontend SPA & API sekaligus)
dotnet run --project backend/Academy.Api/Academy.Api.csproj --urls "http://localhost:5100"
```
Saat frontend di-build ke `frontend/dist`, backend ASP.NET Core secara otomatis menyajikan SPA frontend langsung dari port `5100` tanpa perlu server web terpisah.

---

## 📁 Struktur Direktori Repository

```
learn-cs/
├── backend/
│   ├── Academy.Api/
│   │   ├── Controllers/          # API Endpoints (Curriculum, Execution, Quiz, Progress, Terminal, Projects)
│   │   ├── Data/
│   │   │   ├── Curriculum/       # Master 18 Levels Catalog, Content Provider, Projects Repository
│   │   │   ├── Entities/         # Learner, Progress, QuizAttempt, Bookmark, Submission Entities
│   │   │   └── AcademyDbContext.cs # EF Core DbContext
│   │   ├── Models/               # DTOs & Execution Contracts
│   │   ├── Services/             # Roslyn CodeExecutionService, TerminalSandboxService, CurriculumService
│   │   └── Program.cs            # ASP.NET Core Startup & Pipeline
│   └── Academy.Tests/            # xUnit Test Suite (10 Automated Tests)
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   │   ├── editor/           # Monaco Editor dengan stdin, execution time, stdout/stderr
│   │   │   ├── layout/           # Header (Global Search, Dark Mode), Sidebar (18 Levels), RightToc
│   │   │   ├── quiz/             # Quiz Runner & Explanations Engine
│   │   │   ├── challenge/        # Coding Challenge Runner & Test Cases Table
│   │   │   ├── simulators/       # 6 Interactive Simulators (Binary, Stack/Heap, GC, Async, B-Tree, Deadlock)
│   │   │   └── terminal/         # Interactive Dotnet CLI Terminal
│   │   ├── pages/                # LessonPage, DashboardPage, PlaygroundPage, ProjectsPage, SimulatorsPage, TerminalPage
│   │   ├── services/             # API Client
│   │   └── types/                # TypeScript Contracts
│   └── vite.config.ts
└── README.md
```

---

## 🔒 Kebijakan Keamanan (Security Policy)
1. **Isolated Code Execution**: Kode C# dieksekusi dalam `AssemblyLoadContext` terisolasi dengan timeout 4000ms dan pembatasan alokasi thread.
2. **Path Traversal Protection**: Sandbox terminal membatasi seluruh manipulasi file ke dalam direktori sesi sementara.
3. **Data Integrity**: Menjamin prinsip immutable ledger dan audit trail kriptografis pada implementasi transaksi finansial.

---
Dikembangkan untuk mencetak Principal Backend Engineer kelas dunia.
