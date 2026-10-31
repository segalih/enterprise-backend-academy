import React, { useState } from 'react';
import { Layers, Binary, Trash2, FastForward, Database, AlertTriangle } from 'lucide-react';
import { BinarySimulator } from '../components/simulators/BinarySimulator';
import { StackHeapSimulator } from '../components/simulators/StackHeapSimulator';
import { GCSimulator } from '../components/simulators/GCSimulator';
import { AsyncStateMachineSimulator } from '../components/simulators/AsyncStateMachineSimulator';
import { BTreeSimulator } from '../components/simulators/BTreeSimulator';
import { DeadlockSimulator } from '../components/simulators/DeadlockSimulator';

export const SimulatorsPage: React.FC = () => {
  const [activeSim, setActiveSim] = useState<string>('stack-heap');

  const tabs = [
    { id: 'stack-heap', label: 'Stack vs Heap Memory', icon: Layers, desc: 'Alokasi Value Type vs Reference Type, pointer, dan shared mutable state.' },
    { id: 'binary', label: '8-Bit Binary & Two\'s Complement', icon: Binary, desc: 'Konversi biner, desimal, heksadesimal, dan representasi bilangan bertanda.' },
    { id: 'gc', label: 'Garbage Collector (Gen 0/1/2)', icon: Trash2, desc: 'Siklus hidup objek, GC roots, mark-sweep, dan promosi generasi memori.' },
    { id: 'async', label: 'Async/Await State Machine', icon: FastForward, desc: 'Mekanisme Roslyn struct state machine, IOCP hardware interrupt, dan non-blocking I/O.' },
    { id: 'btree', label: 'Database B+ Tree Index Seek', icon: Database, desc: 'Struktur indeks database clustered, traversal pohon, dan efisiensi disk I/O.' },
    { id: 'deadlock', label: 'Deadlock & Lock Ordering', icon: AlertTriangle, desc: 'Skenario mutual exclusion circular wait dan teknik pencegahan ordered locking.' }
  ];

  return (
    <div className="flex-1 max-w-5xl mx-auto px-6 py-8 text-slate-100">
      <div className="flex items-center space-x-3 mb-6 pb-4 border-b border-slate-800">
        <div className="p-2.5 bg-purple-600/20 text-purple-400 rounded-xl">
          <Layers className="w-6 h-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-white">Interactive Visual Learning Simulators</h1>
          <p className="text-xs text-slate-400">Visualisasi dinamis konsep-konsep teknik komputer tingkat rendah dan arsitektur sistem backend.</p>
        </div>
      </div>

      {/* Simulator Selection Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 gap-3 mb-8">
        {tabs.map(tab => {
          const Icon = tab.icon;
          const active = activeSim === tab.id;
          return (
            <button
              key={tab.id}
              onClick={() => setActiveSim(tab.id)}
              className={`p-3.5 rounded-xl border text-left transition ${
                active
                  ? 'bg-purple-600/20 border-purple-500 text-white shadow-lg shadow-purple-950/40'
                  : 'bg-slate-900 border-slate-800 text-slate-400 hover:text-white hover:bg-slate-800'
              }`}
            >
              <div className="flex items-center space-x-2 mb-1.5">
                <Icon className={`w-4 h-4 ${active ? 'text-purple-400' : 'text-slate-400'}`} />
                <span className="font-bold text-xs truncate text-slate-200">{tab.label}</span>
              </div>
              <p className="text-[11px] text-slate-400 line-clamp-2 leading-relaxed">{tab.desc}</p>
            </button>
          );
        })}
      </div>

      {/* Active Simulator Display */}
      <div>
        {activeSim === 'stack-heap' && <StackHeapSimulator />}
        {activeSim === 'binary' && <BinarySimulator />}
        {activeSim === 'gc' && <GCSimulator />}
        {activeSim === 'async' && <AsyncStateMachineSimulator />}
        {activeSim === 'btree' && <BTreeSimulator />}
        {activeSim === 'deadlock' && <DeadlockSimulator />}
      </div>
    </div>
  );
};
