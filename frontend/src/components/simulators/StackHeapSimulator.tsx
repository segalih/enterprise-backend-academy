import React, { useState } from 'react';
import { Layers, ArrowRight, RotateCcw, Info, Sparkles, Box } from 'lucide-react';

interface StackItem {
  name: string;
  type: string;
  val: string;
  isRef: boolean;
  pointsTo?: string;
  highlighted?: boolean;
}

interface HeapObject {
  address: string;
  type: string;
  roots: string;
  fields: Record<string, string>;
  isBoxed?: boolean;
  mutated?: boolean;
}

interface SimStep {
  title: string;
  code: string;
  explanation: string;
  conceptNote: string;
  stack: StackItem[];
  heap: HeapObject[];
}

export const StackHeapSimulator: React.FC = () => {
  const [step, setStep] = useState<number>(0);

  const steps: SimStep[] = [
    {
      title: 'Inisialisasi Frame Eksekusi',
      code: '// Thread baru dimulai dengan stack frame 1 MB',
      explanation: 'Stack thread dialokasikan oleh OS (default 1 MB per thread di Windows). Managed Heap dikelola oleh GC runtime .NET dan saat ini belum memiliki alokasi objek.',
      conceptNote: 'Prinsip Dasar: Stack menyimpan variabel lokal dan call stack, sedangkan Heap menyimpan objek dinamis berumur panjang.',
      stack: [],
      heap: []
    },
    {
      title: 'Langkah 1: Deklarasi Value Type Lokal',
      code: 'int a = 10;',
      explanation: 'Variabel primitif `int` (4 byte) dialokasikan langsung di dalam Stack frame thread aktif. Tidak ada alokasi di Managed Heap dan tidak ada beban kerja bagi Garbage Collector.',
      conceptNote: 'Value Type lokal hidup dan mati bersama lifecycle stack frame method pemanggilnya.',
      stack: [{ name: 'a', type: 'int', val: '10', isRef: false }],
      heap: []
    },
    {
      title: 'Langkah 2: Bitwise Value Copy',
      code: 'int b = a;',
      explanation: 'Nilai dari `a` disalin secara bit-per-bit (4 byte) ke slot memori baru untuk `b`. Variabel `a` dan `b` memiliki alamat fisik memori stack yang independen.',
      conceptNote: 'Semantik Salin Nilai: Mengubah salah satu variabel tidak akan pernah berdampak pada variabel lainnya.',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '10', isRef: false }
      ],
      heap: []
    },
    {
      title: 'Langkah 3: Mutasi Value Type Terisolasi',
      code: 'b = 99;',
      explanation: 'Slot memori `b` diubah menjadi 99. Slot memori `a` tetap bernilai 10 tanpa modifikasi atau efek samping tak terduga.',
      conceptNote: 'Isolasi Memori: Keuntungan utama struct/value type adalah prediktabilitas lokal tanpa shared mutable state.',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '99', isRef: false, highlighted: true }
      ],
      heap: []
    },
    {
      title: 'Langkah 4: Alokasi Reference Type & Fakta Struct di Heap',
      code: 'Customer c1 = new Customer { Name = "Alice", Age = 30 };',
      explanation: 'Instansiasi class mengalokasikan memori di Managed Heap (0x2A40). Perhatikan: field `Age: 30` (Value Type) ikut disimpan DI DALAM HEAP sebagai bagian dari memori objek Customer! Variabel `c1` di Stack hanya menyimpan pointer referensi 8-byte ke 0x2A40.',
      conceptNote: 'Mitos Terpecahkan: "Value Type selalu di Stack" adalah SALAH! Value Type yang merupakan field dari class akan selalu dialokasikan di Managed Heap bersama objek pemiliknya.',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '99', isRef: false },
        { name: 'c1', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' }
      ],
      heap: [
        {
          address: '0x2A40',
          type: 'Customer',
          roots: 'c1',
          fields: {
            Name: '"Alice" (ref string)',
            Age: '30 (int di dalam Heap!)'
          }
        }
      ]
    },
    {
      title: 'Langkah 5: Penyalinan Referensi (Pointer Copy)',
      code: 'Customer c2 = c1;',
      explanation: 'Penugasan `c2 = c1` HANYA menyalin pointer alamat memori 8-byte `0x2A40` ke slot stack `c2`. Tidak ada objek Customer baru yang dibuat di heap. Kedua variabel kini menunjuk ke alamat heap yang sama.',
      conceptNote: 'Reference Semantics: Menyalin objek class hanya menyalin referensi, bukan data objek di memori heap.',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '99', isRef: false },
        { name: 'c1', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' },
        { name: 'c2', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40', highlighted: true }
      ],
      heap: [
        {
          address: '0x2A40',
          type: 'Customer',
          roots: 'c1, c2',
          fields: {
            Name: '"Alice"',
            Age: '30'
          }
        }
      ]
    },
    {
      title: 'Langkah 6: Mutasi Shared State melalui Referensi',
      code: 'c2.Name = "Bob";',
      explanation: 'Melalui referensi `c2`, data pada alamat 0x2A40 diubah. Karena `c1` menunjuk ke alamat yang sama, maka `c1.Name` sekarang juga terbaca sebagai "Bob". Inilah yang disebut aliasing/shared mutable state.',
      conceptNote: 'Hati-hati Bug Concurrency: Ketika beberapa thread mengakses referensi objek yang sama tanpa synchronisation, race condition akan terjadi.',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '99', isRef: false },
        { name: 'c1', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' },
        { name: 'c2', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' }
      ],
      heap: [
        {
          address: '0x2A40',
          type: 'Customer',
          roots: 'c1, c2',
          mutated: true,
          fields: {
            Name: '"Bob" (Dimutasi via c2!)',
            Age: '30'
          }
        }
      ]
    },
    {
      title: 'Langkah 7: Boxing (Value Type Dibungkus ke Heap)',
      code: 'object boxed = a; // BOXING ALOKASI HEAP BARU',
      explanation: 'Ketika value type `a` ditugaskan ke tipe `object`, runtime .NET terpaksa melakukan BOXING: mengalokasikan blok memori baru di Managed Heap (0x3F80), menyalin nilai 10 ke dalamnya, dan melampirkan header SyncBlock + TypeHandle (total 24 byte di arsitektur 64-bit).',
      conceptNote: 'Dampak Performa: Boxing membebani GC dengan alokasi heap tak kasat mata di dalam loop hot-path. Gunakan Generics (<T>) untuk mencegah boxing!',
      stack: [
        { name: 'a', type: 'int', val: '10', isRef: false },
        { name: 'b', type: 'int', val: '99', isRef: false },
        { name: 'c1', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' },
        { name: 'c2', type: 'Customer (ref)', val: '0x2A40', isRef: true, pointsTo: '0x2A40' },
        { name: 'boxed', type: 'object (ref)', val: '0x3F80', isRef: true, pointsTo: '0x3F80', highlighted: true }
      ],
      heap: [
        {
          address: '0x2A40',
          type: 'Customer',
          roots: 'c1, c2',
          fields: {
            Name: '"Bob"',
            Age: '30'
          }
        },
        {
          address: '0x3F80',
          type: 'System.Int32 (Boxed Object)',
          roots: 'boxed',
          isBoxed: true,
          fields: {
            Value: '10 (Salinan dari variabel a)'
          }
        }
      ]
    }
  ];

  const current = steps[step];

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-5 sm:p-6 border border-slate-800 shadow-xl my-6" data-testid="stack-heap-simulator">
      {/* Learning Goals Header */}
      <div className="bg-blue-950/40 border border-blue-900/60 rounded-lg p-3 sm:p-4 mb-5 flex items-start space-x-3 text-xs leading-relaxed text-blue-200">
        <Info className="w-5 h-5 text-blue-400 shrink-0 mt-0.5" />
        <div>
          <span className="font-bold text-white block mb-0.5 uppercase tracking-wider text-[11px] font-mono">
            Tujuan Belajar Simulator:
          </span>
          Memahami alokasi riil Value Type vs Reference Type, semantik penyalinan bitwise vs referensi pointer, overhead alokasi <strong>Boxing</strong>, serta membuktikan bahwa <strong>Value Type (struct/int) yang menjadi field class dialokasikan di Managed Heap</strong>.
        </div>
      </div>

      {/* Simulator Toolbar */}
      <div className="flex flex-wrap items-center justify-between pb-4 border-b border-slate-800 mb-4 gap-3">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-emerald-600/20 text-emerald-400 rounded-lg">
            <Layers className="w-5 h-5" />
          </div>
          <div>
            <h3 className="font-bold text-base sm:text-lg text-white">Stack vs Heap Memory Allocation Simulator</h3>
            <p className="text-xs text-slate-400">Visualisasi representasi memori fisik saat kode C# dieksekusi runtime .NET.</p>
          </div>
        </div>
        <div className="flex items-center space-x-2">
          <button
            onClick={() => setStep(0)}
            className="p-1.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-700 transition"
            title="Reset Simulator ke Awal"
          >
            <RotateCcw className="w-4 h-4" />
          </button>
          <button
            onClick={() => setStep(Math.max(0, step - 1))}
            disabled={step === 0}
            className="px-3 py-1.5 text-xs bg-slate-800 hover:bg-slate-700 disabled:opacity-40 text-slate-200 rounded-lg border border-slate-700 transition"
          >
            Kembali
          </button>
          <button
            onClick={() => setStep(Math.min(steps.length - 1, step + 1))}
            disabled={step === steps.length - 1}
            className="px-4 py-1.5 text-xs bg-emerald-600 hover:bg-emerald-500 disabled:opacity-40 text-white font-medium rounded-lg shadow transition flex items-center space-x-1"
          >
            <span>Lanjut</span>
            <ArrowRight className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      {/* Code Bar & Step Info */}
      <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 mb-6">
        <div className="flex items-center justify-between mb-2">
          <span className="text-xs font-semibold text-emerald-400">
            {current.title} (Langkah {step} dari {steps.length - 1})
          </span>
          <span className="text-[11px] font-mono text-slate-500">
            C# 12 / .NET 8 CoreCLR
          </span>
        </div>
        <pre className="font-mono text-xs sm:text-sm text-cyan-300 bg-slate-900/90 p-2.5 rounded-lg border border-slate-800 overflow-x-auto">
          <code>{current.code}</code>
        </pre>
        <p className="text-xs sm:text-sm text-slate-200 mt-2.5 leading-relaxed">
          {current.explanation}
        </p>
        <div className="mt-2.5 pt-2 border-t border-slate-800/80 flex items-center space-x-2 text-[11px] text-amber-300/90 font-medium">
          <Sparkles className="w-3.5 h-3.5 text-amber-400 shrink-0" />
          <span>{current.conceptNote}</span>
        </div>
      </div>

      {/* Memory Visualization Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* The Stack */}
        <div className="bg-slate-950/80 p-4 rounded-xl border border-slate-800 flex flex-col">
          <div className="flex items-center justify-between pb-2 border-b border-slate-800 mb-3">
            <div className="flex items-center space-x-2">
              <span className="w-2.5 h-2.5 rounded-full bg-blue-500"></span>
              <span className="text-sm font-semibold text-slate-200">The Stack (Per-Thread LIFO)</span>
            </div>
            <span className="text-[10px] font-mono text-emerald-400 bg-emerald-950/60 border border-emerald-800/60 px-1.5 py-0.5 rounded">
              Instan / Tanpa GC
            </span>
          </div>

          <div className="space-y-2 min-h-[260px] flex flex-col justify-end">
            {current.stack.length === 0 ? (
              <div className="text-xs text-slate-500 italic text-center py-16">Stack frame kosong</div>
            ) : (
              current.stack.map((item, idx) => (
                <div
                  key={idx}
                  className={`p-3 rounded-lg border flex items-center justify-between font-mono text-xs transition-all duration-300 ${
                    item.highlighted
                      ? 'bg-amber-950/40 border-amber-500/80 text-amber-200 shadow-md shadow-amber-950/30'
                      : item.isRef
                      ? 'bg-blue-950/40 border-blue-600/50 text-blue-200'
                      : 'bg-slate-900 border-slate-800 text-slate-200'
                  }`}
                >
                  <div className="flex items-center space-x-2">
                    <span className="text-slate-400 text-[11px]">{item.type}</span>
                    <span className="font-bold text-white">{item.name}</span>
                  </div>
                  <div className="flex items-center space-x-1">
                    <span className="text-slate-500">=</span>
                    <span className={item.isRef ? 'text-purple-400 font-bold underline' : 'text-emerald-400 font-bold'}>
                      {item.val}
                    </span>
                    {item.isRef && <span className="text-[10px] text-purple-300 ml-1">➔ [Heap]</span>}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {/* The Managed Heap */}
        <div className="bg-slate-950/80 p-4 rounded-xl border border-slate-800 flex flex-col">
          <div className="flex items-center justify-between pb-2 border-b border-slate-800 mb-3">
            <div className="flex items-center space-x-2">
              <span className="w-2.5 h-2.5 rounded-full bg-purple-500"></span>
              <span className="text-sm font-semibold text-slate-200">The Managed Heap (GC Monitored)</span>
            </div>
            <span className="text-[10px] font-mono text-purple-400 bg-purple-950/60 border border-purple-800/60 px-1.5 py-0.5 rounded">
              Dikelola Garbage Collector
            </span>
          </div>

          <div className="space-y-3 min-h-[260px]">
            {current.heap.length === 0 ? (
              <div className="text-xs text-slate-500 italic text-center py-16">
                Belum ada objek dialokasikan di Managed Heap
              </div>
            ) : (
              current.heap.map((obj, idx) => (
                <div
                  key={idx}
                  className={`p-3 rounded-lg border font-mono text-xs transition-all duration-300 ${
                    obj.isBoxed
                      ? 'bg-amber-950/30 border-amber-600/70 shadow-lg shadow-amber-950/20'
                      : obj.mutated
                      ? 'bg-purple-950/60 border-purple-500 shadow-lg shadow-purple-900/30'
                      : 'bg-purple-950/30 border-purple-800/60'
                  }`}
                >
                  <div className="flex justify-between items-center text-[11px] text-purple-300 border-b border-purple-900/50 pb-1 mb-2">
                    <span className="font-bold">Alamat: {obj.address}</span>
                    <span className="flex items-center space-x-1">
                      {obj.isBoxed && <Box className="w-3 h-3 text-amber-400" />}
                      <span>{obj.type} (Roots: {obj.roots})</span>
                    </span>
                  </div>
                  <div className="space-y-1">
                    <div className="text-[10px] text-slate-500 font-mono">
                      [SyncBlock (8B) + TypeHandle (8B) + Object Body]
                    </div>
                    {Object.entries(obj.fields).map(([k, v]) => (
                      <div key={k} className="flex justify-between items-center bg-slate-900/80 p-1.5 rounded">
                        <span className="text-slate-400">.{k}</span>
                        <span className={`font-bold ${obj.mutated ? 'text-amber-300' : 'text-emerald-300'}`}>{v}</span>
                      </div>
                    ))}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
