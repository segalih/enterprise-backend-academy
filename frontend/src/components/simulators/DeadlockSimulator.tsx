import React, { useState } from 'react';
import { AlertTriangle, Lock, ShieldCheck, RotateCcw } from 'lucide-react';

export const DeadlockSimulator: React.FC = () => {
  const [strategy, setStrategy] = useState<'naive' | 'ordered'>('naive');
  const [step, setStep] = useState<number>(0);

  const naiveSteps = [
    {
      title: 'Kondisi Normal',
      t1: 'Menunggu transfer Rp 500rb dari Akun A ke Akun B',
      t2: 'Menunggu transfer Rp 300rb dari Akun B ke Akun A',
      lockA: 'Tersedia',
      lockB: 'Tersedia',
      status: 'Ready'
    },
    {
      title: 'Langkah 1: T1 mengunci Akun A & T2 mengunci Akun B',
      t1: 'Mengunci Akun A (Berhasil!) -> Mencoba mengunci Akun B...',
      t2: 'Mengunci Akun B (Berhasil!) -> Mencoba mengunci Akun A...',
      lockA: 'Dikunci oleh T1',
      lockB: 'Dikunci oleh T2',
      status: 'Partial Lock'
    },
    {
      title: 'Langkah 2: CIRCULAR WAIT -> DEADLOCK!',
      t1: 'BLOCKED: Menunggu Akun B dilepaskan oleh T2',
      t2: 'BLOCKED: Menunggu Akun A dilepaskan oleh T1',
      lockA: 'Dikunci T1 (Menunggu T2)',
      lockB: 'Dikunci T2 (Menunggu T1)',
      status: 'DEADLOCK'
    }
  ];

  const orderedSteps = [
    {
      title: 'Kondisi Normal (Pencegahan Deadlock dengan Ordered Locking)',
      t1: 'Transfer A -> B (Urutan lock: Min(A,B) lalu Max(A,B))',
      t2: 'Transfer B -> A (Urutan lock: Min(A,B) lalu Max(A,B))',
      lockA: 'Tersedia',
      lockB: 'Tersedia',
      status: 'Ready'
    },
    {
      title: 'Langkah 1: Kedua Transaksi mengantri mengunci Akun A lebih dulu!',
      t1: 'Mengunci Akun A (Berhasil!)',
      t2: 'Menunggu antrean Akun A (Tertib di antrean, belum menyentuh Akun B)',
      lockA: 'Dikunci oleh T1',
      lockB: 'Tersedia (Belum disentuh)',
      status: 'Deterministic Order'
    },
    {
      title: 'Langkah 2: T1 mengunci B, menyelesaikan transfer, lalu T2 berjalan lancar!',
      t1: 'Mengunci B -> Selesai -> Melepaskan A dan B',
      t2: 'Mendapatkan kunci A -> Mengunci B -> Selesai!',
      lockA: 'Bebas',
      lockB: 'Bebas',
      status: 'SUCCESS'
    }
  ];

  const activeSteps = strategy === 'naive' ? naiveSteps : orderedSteps;
  const current = activeSteps[step];

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-6 border border-slate-800 shadow-xl my-6">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className={`p-2 rounded-lg ${strategy === 'naive' ? 'bg-red-600/20 text-red-400' : 'bg-emerald-600/20 text-emerald-400'}`}>
            {strategy === 'naive' ? <AlertTriangle className="w-5 h-5" /> : <ShieldCheck className="w-5 h-5" />}
          </div>
          <div>
            <h3 className="font-semibold text-lg text-white">Concurrency Deadlock & Lock Ordering Simulator</h3>
            <p className="text-xs text-slate-400">Pahami skenario Deadlock dua arah dan bagaimana Enterprise Locking Order membasminya.</p>
          </div>
        </div>

        <div className="flex space-x-2">
          <button
            onClick={() => { setStrategy('naive'); setStep(0); }}
            className={`px-3 py-1.5 text-xs rounded font-medium transition ${
              strategy === 'naive' ? 'bg-red-600 text-white shadow' : 'bg-slate-800 text-slate-300 hover:bg-slate-700'
            }`}
          >
            Skenario Deadlock
          </button>
          <button
            onClick={() => { setStrategy('ordered'); setStep(0); }}
            className={`px-3 py-1.5 text-xs rounded font-medium transition ${
              strategy === 'ordered' ? 'bg-emerald-600 text-white shadow' : 'bg-slate-800 text-slate-300 hover:bg-slate-700'
            }`}
          >
            Solusi: Ordered Locking
          </button>
        </div>
      </div>

      {/* Simulator Stepper */}
      <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 mb-6">
        <div className="flex justify-between items-center mb-2">
          <span className="text-xs font-bold text-slate-200">{current.title}</span>
          <div className="flex space-x-2">
            <button
              onClick={() => setStep(0)}
              className="p-1 text-slate-400 hover:text-white"
              title="Reset"
            >
              <RotateCcw className="w-4 h-4" />
            </button>
            <button
              onClick={() => setStep(Math.min(activeSteps.length - 1, step + 1))}
              disabled={step === activeSteps.length - 1}
              className="px-3 py-1 text-xs bg-blue-600 hover:bg-blue-500 disabled:opacity-40 text-white rounded font-medium shadow"
            >
              Langkah Berikutnya
            </button>
          </div>
        </div>

        {current.status === 'DEADLOCK' && (
          <div className="p-3 bg-red-950/60 border border-red-600/70 rounded-lg text-xs text-red-200 font-semibold mb-3 flex items-center space-x-2 animate-pulse">
            <AlertTriangle className="w-4 h-4 text-red-400 shrink-0" />
            <span>CRITICAL DEADLOCK DETECTED: Siklus saling menunggu (Circular Dependency) terjadi antara Transaksi 1 dan Transaksi 2. Database engine akan membunuh salah satu transaksi sebagai Deadlock Victim!</span>
          </div>
        )}

        {current.status === 'SUCCESS' && (
          <div className="p-3 bg-emerald-950/60 border border-emerald-600/70 rounded-lg text-xs text-emerald-200 font-semibold mb-3 flex items-center space-x-2">
            <ShieldCheck className="w-4 h-4 text-emerald-400 shrink-0" />
            <span>DEADLOCK-FREE: Dengan mengurutkan ID akun sebelum mengunci (misal: selalu lock ID terendah lebih dulu), circular dependency menjadi mustahil secara matematis!</span>
          </div>
        )}
      </div>

      {/* Transactions & Resources Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Transaction 1 */}
        <div className="p-4 bg-slate-950/70 rounded-xl border border-slate-800 font-mono text-xs">
          <div className="flex justify-between items-center pb-2 border-b border-slate-800 mb-2">
            <span className="font-bold text-blue-400">Thread #1 / Transaksi 1 (A ➔ B)</span>
            <Lock className="w-3.5 h-3.5 text-blue-400" />
          </div>
          <p className="text-slate-300">{current.t1}</p>
        </div>

        {/* Transaction 2 */}
        <div className="p-4 bg-slate-950/70 rounded-xl border border-slate-800 font-mono text-xs">
          <div className="flex justify-between items-center pb-2 border-b border-slate-800 mb-2">
            <span className="font-bold text-purple-400">Thread #2 / Transaksi 2 (B ➔ A)</span>
            <Lock className="w-3.5 h-3.5 text-purple-400" />
          </div>
          <p className="text-slate-300">{current.t2}</p>
        </div>
      </div>
    </div>
  );
};
