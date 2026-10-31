import React, { useState } from 'react';
import { Code2 } from 'lucide-react';
import { MonacoCodeEditor } from '../components/editor/MonacoCodeEditor';

export const PlaygroundPage: React.FC = () => {
  const templates: Record<string, { title: string; code: string; desc: string }> = {
    basic: {
      title: 'Console App Dasar',
      desc: 'Struktur dasar program C# modern dengan top-level statements.',
      code: `using System;

Console.WriteLine("=== ENTERPRISE C# PLAYGROUND ===");
Console.WriteLine($"Waktu Server: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
Console.WriteLine($"Versi OS: {Environment.OSVersion}");
Console.WriteLine($"Jumlah CPU Cores: {Environment.ProcessorCount}");`
    },
    linq: {
      title: 'LINQ Query & Agregasi',
      desc: 'Pemfilteran, pengelompokan (GroupBy), dan agregasi data.',
      code: `using System;
using System.Collections.Generic;
using System.Linq;

var txs = new List<Transaction>
{
    new("ACC-01", 150000m, "Food"),
    new("ACC-01", 500000m, "Travel"),
    new("ACC-02", 2000000m, "Investment"),
    new("ACC-01", 75000m, "Food")
};

Console.WriteLine("Total Pengeluaran per Kategori:");
var summary = txs
    .GroupBy(t => t.Category)
    .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) });

foreach (var item in summary)
{
    Console.WriteLine($" - {item.Category,-12}: Rp {item.Total:N0}");
}

record Transaction(string AccountId, decimal Amount, string Category);`
    },
    asyncTask: {
      title: 'Konkurensi Asinkronus (Task.WhenAll)',
      desc: 'Menjalankan beberapa operasi I/O asinkronus secara konkuren tanpa thread blocking.',
      code: `using System;
using System.Threading.Tasks;

Console.WriteLine("Memulai 3 request API eksternal secara konkuren...");

async Task<string> FetchExternalAsync(string serviceName, int delayMs)
{
    await Task.Delay(delayMs);
    return $"[200 OK] Respon dari {serviceName} ({delayMs}ms)";
}

var t1 = FetchExternalAsync("Core Banking Gateway", 300);
var t2 = FetchExternalAsync("Fraud Detection Engine", 150);
var t3 = FetchExternalAsync("Notification Service", 200);

string[] results = await Task.WhenAll(t1, t2, t3);

foreach (var res in results)
{
    Console.WriteLine(res);
}

Console.WriteLine("Seluruh request selesai tanpa memblokir thread!");`
    },
    ledger: {
      title: 'Double-Entry Ledger Invariant',
      desc: 'Simulasi validasi debit dan kredit transaksi perbankan.',
      code: `using System;
using System.Collections.Generic;
using System.Linq;

var entries = new List<Entry>
{
    new("Kas Utama", EntryType.Debit, 1000000m),
    new("Pendapatan Jasa", EntryType.Credit, 1000000m)
};

PostLedger("TX-001", entries);

void PostLedger(string txId, List<Entry> entries)
{
    decimal debit = entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount);
    decimal credit = entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount);

    Console.WriteLine($"Transaksi: {txId}");
    Console.WriteLine($"  Debit : Rp {debit:N2}");
    Console.WriteLine($"  Kredit: Rp {credit:N2}");

    if (debit != credit)
    {
        Console.WriteLine($"  STATUS: GAGAL! Selisih {Math.Abs(debit - credit):N2}");
        return;
    }

    Console.WriteLine("  STATUS: BERHASIL TERPOSTING. Seimbang 100%.");
}

enum EntryType { Debit, Credit }
record Entry(string Account, EntryType Type, decimal Amount);`
    }
  };

  const [selectedKey, setSelectedKey] = useState<string>('basic');

  return (
    <div className="flex-1 max-w-5xl mx-auto px-6 py-8 text-slate-100">
      <div className="flex items-center justify-between mb-6 pb-4 border-b border-slate-800">
        <div className="flex items-center space-x-3">
          <div className="p-2.5 bg-blue-600/20 text-blue-400 rounded-xl">
            <Code2 className="w-6 h-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-white">Interactive C# Playground</h1>
            <p className="text-xs text-slate-400">Lingkungan eksperimen bebas kompilasi C# 12 / .NET 8 dengan Roslyn Sandbox Engine.</p>
          </div>
        </div>
      </div>

      {/* Template Chooser */}
      <div className="mb-6">
        <div className="text-xs font-mono uppercase tracking-wider text-slate-400 font-bold mb-2">
          Pilih Template Praktik:
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3">
          {Object.entries(templates).map(([key, t]) => (
            <button
              key={key}
              onClick={() => setSelectedKey(key)}
              className={`p-3 rounded-xl border text-left transition ${
                selectedKey === key
                  ? 'bg-blue-600/20 border-blue-500 text-white shadow-md'
                  : 'bg-slate-900 border-slate-800 text-slate-400 hover:text-white hover:bg-slate-800'
              }`}
            >
              <div className="font-bold text-xs mb-1 text-slate-200">{t.title}</div>
              <div className="text-[11px] text-slate-400 line-clamp-2 leading-relaxed">{t.desc}</div>
            </button>
          ))}
        </div>
      </div>

      {/* Editor Component */}
      <MonacoCodeEditor
        key={selectedKey}
        initialCode={templates[selectedKey].code}
        height="460px"
      />
    </div>
  );
};
