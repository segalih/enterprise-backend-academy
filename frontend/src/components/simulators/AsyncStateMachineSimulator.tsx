import React, { useState } from 'react';
import { FastForward, ArrowRight, RotateCcw, Cpu, Server, CheckCircle2 } from 'lucide-react';

export const AsyncStateMachineSimulator: React.FC = () => {
  const [phase, setPhase] = useState<number>(0);

  const phases = [
    {
      title: 'Fase 0: Pemanggilan Awal (Synchronous hingga Await)',
      stateValue: -1,
      threadId: 'Thread #4 (HTTP Request Pool)',
      code: 'var task = client.GetStringAsync("https://api.internal/data");',
      status: 'Thread aktif mengeksekusi kode sebelum keyword await',
      hardwareState: 'NIC Jaringan: Siap mengirim paket SYN',
      poolState: 'Thread #4 sibuk',
      explanation: 'Sebelum keyword await, method async berjalan secara biasa (synchronous). State machine diinisialisasi dengan state -1.'
    },
    {
      title: 'Fase 1: Titik Await & Pelepasan Thread (Non-Blocking)',
      stateValue: 0,
      threadId: 'Thread #4 DIKEMBALIKAN KE POOL!',
      code: 'await task; // AwaitUnsafeOnCompleted mendaftarkan callback MoveNext()',
      status: 'State disimpan ke Struct State Machine, thread dilepaskan!',
      hardwareState: 'OS Kernel Socket: Mengirim paket HTTP over TCP, menunggu ACK/response',
      poolState: 'Thread #4 BEBAS melayani request HTTP pengguna lain (Zero Blocked Threads)',
      explanation: 'Inilah keajaiban asinkronus: Thread #4 tidak tidur atau diblokir. Thread dikembalikan ke ThreadPool. Nol CPU idle waste!'
    },
    {
      title: 'Fase 2: I/O Hardware Asynchronous (OS Kernel IOCP/epoll)',
      stateValue: 0,
      threadId: 'TIDAK ADA THREAD C# YANG DIGUNAKAN (0 Threads)',
      code: '// Menunggu sinyal interrupt dari kartu jaringan (NIC)',
      status: 'Hardware I/O murni berjalan di ring-0 kernel',
      hardwareState: 'NIC menerima paket data dari router, memicu Hardware Interrupt ke OS',
      poolState: 'ThreadPool memiliki kapasitas 100% untuk request lain',
      explanation: 'Ribuan request asinkronus dapat menunggu bersamaan tanpa membutuhkan ribuan OS threads. Menghemat gigabytes RAM stack!'
    },
    {
      title: 'Fase 3: I/O Selesai & Sinyal IOCP ke Runtime',
      stateValue: 0,
      threadId: 'OS IOCP memberi tahu .NET Runtime',
      code: 'task.IsCompleted = true; // Sinyal kelanjutan',
      status: 'Runtime menjadwalkan kelanjutan ke ThreadPool',
      hardwareState: 'Paket HTTP 200 OK selesai diterima utuh',
      poolState: 'ThreadPool mencari thread yang sedang nganggur',
      explanation: 'Saat data tiba, OS Completion Port memberi tahu CLR runtime bahwa task telah selesai dan callback dapat dieksekusi.'
    },
    {
      title: 'Fase 4: Continuation Resume di Thread Baru',
      stateValue: -2,
      threadId: 'Thread #9 (ThreadPool Worker Baru)',
      code: 'string result = awaiter.GetResult(); return result.Trim();',
      status: 'MoveNext() melanjutkan eksekusi dari state 0 ke selesai (state -2)',
      hardwareState: 'I/O Socket ditutup / dikembalikan ke socket pool',
      poolState: 'Thread #9 menyelesaikan pengembalian response ke client',
      explanation: 'Eksekusi dilanjutkan oleh thread sembarang yang tersedia di ThreadPool (bisa Thread #9, tidak harus Thread #4 yang memulai awal!).'
    }
  ];

  const cur = phases[phase];

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-6 border border-slate-800 shadow-xl my-6">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-purple-600/20 text-purple-400 rounded-lg">
            <FastForward className="w-5 h-5" />
          </div>
          <div>
            <h3 className="font-semibold text-lg text-white">Async/Await State Machine & Thread Continuation Flow</h3>
            <p className="text-xs text-slate-400">Pahami bagaimana Roslyn compiler dan runtime CLR menangani non-blocking asynchronous I/O.</p>
          </div>
        </div>

        <div className="flex space-x-2">
          <button
            onClick={() => setPhase(0)}
            className="p-1.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition"
            title="Reset"
          >
            <RotateCcw className="w-4 h-4" />
          </button>
          <button
            onClick={() => setPhase(Math.max(0, phase - 1))}
            disabled={phase === 0}
            className="px-3 py-1.5 text-xs bg-slate-800 hover:bg-slate-700 disabled:opacity-40 text-slate-200 rounded border border-slate-700 transition"
          >
            Sebelumnya
          </button>
          <button
            onClick={() => setPhase(Math.min(phases.length - 1, phase + 1))}
            disabled={phase === phases.length - 1}
            className="px-4 py-1.5 text-xs bg-purple-600 hover:bg-purple-500 disabled:opacity-40 text-white font-medium rounded shadow transition flex items-center space-x-1"
          >
            <span>Fase Berikutnya</span>
            <ArrowRight className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      {/* Progress Step Bar */}
      <div className="flex justify-between items-center mb-6 px-2">
        {phases.map((p, idx) => (
          <div key={idx} className="flex items-center flex-1 last:flex-none">
            <div
              onClick={() => setPhase(idx)}
              className={`w-7 h-7 rounded-full flex items-center justify-center font-mono text-xs font-bold cursor-pointer transition ${
                idx === phase
                  ? 'bg-purple-600 text-white ring-4 ring-purple-600/20'
                  : idx < phase
                  ? 'bg-emerald-600 text-white'
                  : 'bg-slate-800 text-slate-400'
              }`}
            >
              {idx < phase ? <CheckCircle2 className="w-4 h-4" /> : idx}
            </div>
            {idx < phases.length - 1 && (
              <div className={`h-1 flex-1 mx-2 rounded ${idx < phase ? 'bg-emerald-600/70' : 'bg-slate-800'}`} />
            )}
          </div>
        ))}
      </div>

      {/* Current Phase Detail */}
      <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 mb-6">
        <div className="flex justify-between items-center mb-2">
          <span className="text-xs font-bold text-purple-400">{cur.title}</span>
          <span className="text-xs font-mono px-2 py-0.5 bg-slate-800 text-purple-300 rounded">
            Struct State: {cur.stateValue}
          </span>
        </div>
        <pre className="font-mono text-xs text-cyan-300 bg-slate-900/90 p-2.5 rounded border border-slate-800 mb-2">
          <code>{cur.code}</code>
        </pre>
        <p className="text-xs text-slate-300 leading-relaxed">{cur.explanation}</p>
      </div>

      {/* System Status Dashboard */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="p-3 bg-slate-950/70 rounded-lg border border-slate-800">
          <div className="text-[11px] text-slate-400 flex items-center space-x-1.5 mb-1">
            <Cpu className="w-3.5 h-3.5 text-blue-400" />
            <span>Active Thread</span>
          </div>
          <div className="font-mono text-xs font-bold text-blue-300">{cur.threadId}</div>
        </div>

        <div className="p-3 bg-slate-950/70 rounded-lg border border-slate-800">
          <div className="text-[11px] text-slate-400 flex items-center space-x-1.5 mb-1">
            <Server className="w-3.5 h-3.5 text-amber-400" />
            <span>ThreadPool Capacity</span>
          </div>
          <div className="font-mono text-xs font-bold text-amber-300">{cur.poolState}</div>
        </div>

        <div className="p-3 bg-slate-950/70 rounded-lg border border-slate-800">
          <div className="text-[11px] text-slate-400 flex items-center space-x-1.5 mb-1">
            <Cpu className="w-3.5 h-3.5 text-emerald-400" />
            <span>Hardware & OS IOCP</span>
          </div>
          <div className="font-mono text-xs font-bold text-emerald-300">{cur.hardwareState}</div>
        </div>
      </div>
    </div>
  );
};
