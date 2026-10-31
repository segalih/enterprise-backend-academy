import React, { useState } from 'react';
import { Trash2, Plus } from 'lucide-react';

interface GCObject {
  id: string;
  name: string;
  gen: 0 | 1 | 2;
  isLoh: boolean;
  hasRoot: boolean;
  sizeKb: number;
}

export const GCSimulator: React.FC = () => {
  const [objects, setObjects] = useState<GCObject[]>([
    { id: '1', name: 'req_ctx_1', gen: 0, isLoh: false, hasRoot: true, sizeKb: 12 },
    { id: '2', name: 'dto_temp', gen: 0, isLoh: false, hasRoot: false, sizeKb: 8 },
    { id: '3', name: 'user_session', gen: 1, isLoh: false, hasRoot: true, sizeKb: 45 },
    { id: '4', name: 'db_cache_singleton', gen: 2, isLoh: false, hasRoot: true, sizeKb: 250 },
    { id: '5', name: 'large_pdf_buffer', gen: 0, isLoh: true, hasRoot: false, sizeKb: 92 }
  ]);

  const [gcLogs, setGcLogs] = useState<string[]>([
    'Runtime CLR siap. Objek dialokasikan di Generation 0 dan LOH (>85,000 bytes).'
  ]);

  const addObject = () => {
    const id = Date.now().toString().slice(-4);
    const newObj: GCObject = {
      id,
      name: `obj_${id}`,
      gen: 0,
      isLoh: false,
      hasRoot: true,
      sizeKb: Math.floor(Math.random() * 20) + 4
    };
    setObjects(prev => [...prev, newObj]);
    setGcLogs(prev => [`[ALLOC] Objek baru '${newObj.name}' dialokasikan di Gen 0 Heap.`, ...prev.slice(0, 5)]);
  };

  const toggleRoot = (id: string) => {
    const target = objects.find(o => o.id === id);
    if (!target) return;
    const nextRoot = !target.hasRoot;
    setObjects(prev => prev.map(obj => (obj.id === id ? { ...obj, hasRoot: nextRoot } : obj)));
    setGcLogs(l => [
      `[GC ROOT] '${target.name}' root diubah menjadi: ${nextRoot ? 'Aktif (Reachable)' : 'Putus (Unreachable -> Eligible for GC)'}`,
      ...l.slice(0, 5)
    ]);
  };

  const triggerGC = (targetGen: 0 | 1 | 2) => {
    let collectedCount = 0;
    const surviving: GCObject[] = [];

    objects.forEach(obj => {
      // In .NET CoreCLR, LOH is ONLY collected during Gen 2 (Full GC)
      const isEligible = obj.isLoh ? targetGen === 2 : obj.gen <= targetGen;
      if (isEligible) {
        if (obj.hasRoot) {
          // LOH objects are never promoted between generations
          const nextGen = obj.isLoh ? obj.gen : ((Math.min(2, obj.gen + 1)) as 0 | 1 | 2);
          surviving.push({ ...obj, gen: nextGen });
        } else {
          collectedCount++;
        }
      } else {
        surviving.push(obj);
      }
    });

    setObjects(surviving);
    setGcLogs(l => [
      `[GC CYCLE] GC Gen ${targetGen} dijalankan! ${collectedCount} objek tak bertuan dibersihkan.${targetGen === 2 ? ' Termasuk pembersihan LOH.' : ''} Objek SOH yang bertahan dipromosikan.`,
      ...l.slice(0, 5)
    ]);
  };

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-6 border border-slate-800 shadow-xl my-6">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-red-600/20 text-red-400 rounded-lg">
            <Trash2 className="w-5 h-5" />
          </div>
          <div>
            <h3 className="font-semibold text-lg text-white">Garbage Collector Generations Simulator (Gen 0, 1, 2, LOH)</h3>
            <p className="text-xs text-slate-400">Putuskan GC Root objek, lalu picu GC cycle untuk melihat Mark-Sweep dan Promosi Generasi.</p>
          </div>
        </div>

        <div className="flex space-x-2">
          <button
            onClick={addObject}
            className="px-3 py-1.5 text-xs bg-blue-600 hover:bg-blue-500 text-white rounded font-medium flex items-center space-x-1 shadow"
          >
            <Plus className="w-3.5 h-3.5" />
            <span>Alokasikan Objek</span>
          </button>
          <button
            onClick={() => triggerGC(0)}
            className="px-3 py-1.5 text-xs bg-amber-600 hover:bg-amber-500 text-white rounded font-medium shadow"
          >
            Picu GC Gen 0
          </button>
          <button
            onClick={() => triggerGC(1)}
            className="px-3 py-1.5 text-xs bg-orange-600 hover:bg-orange-500 text-white rounded font-medium shadow"
          >
            Picu GC Gen 1
          </button>
          <button
            onClick={() => triggerGC(2)}
            className="px-3 py-1.5 text-xs bg-red-600 hover:bg-red-500 text-white rounded font-medium shadow"
          >
            Full GC (Gen 2)
          </button>
        </div>
      </div>

      {/* Generations Columns */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        {/* Gen 0 */}
        <div className="bg-slate-950/80 p-3.5 rounded-xl border border-slate-800">
          <div className="flex justify-between items-center mb-2 pb-1.5 border-b border-slate-800">
            <span className="text-xs font-bold text-emerald-400">Generation 0</span>
            <span className="text-[10px] text-slate-500">Short-Lived (Ephemeric)</span>
          </div>
          <div className="space-y-2 min-h-[160px]">
            {objects.filter(o => o.gen === 0 && !o.isLoh).map(obj => (
              <div
                key={obj.id}
                onClick={() => toggleRoot(obj.id)}
                className={`p-2.5 rounded-lg border font-mono text-xs cursor-pointer transition ${
                  obj.hasRoot
                    ? 'bg-emerald-950/40 border-emerald-600/60 text-emerald-200 hover:bg-emerald-900/40'
                    : 'bg-red-950/40 border-red-600/60 text-red-300 hover:bg-red-900/40 line-through'
                }`}
                title="Klik untuk memutus/menyambung GC Root"
              >
                <div className="flex justify-between items-center">
                  <span className="font-bold">{obj.name}</span>
                  <span className="text-[10px]">{obj.sizeKb} KB</span>
                </div>
                <div className="text-[10px] text-slate-400 mt-1">
                  Root: {obj.hasRoot ? 'Terhubung (Aman)' : 'Putus (Siap di-GC)'}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Gen 1 */}
        <div className="bg-slate-950/80 p-3.5 rounded-xl border border-slate-800">
          <div className="flex justify-between items-center mb-2 pb-1.5 border-b border-slate-800">
            <span className="text-xs font-bold text-amber-400">Generation 1</span>
            <span className="text-[10px] text-slate-500">Buffer Zone</span>
          </div>
          <div className="space-y-2 min-h-[160px]">
            {objects.filter(o => o.gen === 1 && !o.isLoh).map(obj => (
              <div
                key={obj.id}
                onClick={() => toggleRoot(obj.id)}
                className={`p-2.5 rounded-lg border font-mono text-xs cursor-pointer transition ${
                  obj.hasRoot
                    ? 'bg-amber-950/40 border-amber-600/60 text-amber-200 hover:bg-amber-900/40'
                    : 'bg-red-950/40 border-red-600/60 text-red-300 hover:bg-red-900/40 line-through'
                }`}
                title="Klik untuk memutus/menyambung GC Root"
              >
                <div className="flex justify-between items-center">
                  <span className="font-bold">{obj.name}</span>
                  <span className="text-[10px]">{obj.sizeKb} KB</span>
                </div>
                <div className="text-[10px] text-slate-400 mt-1">
                  Root: {obj.hasRoot ? 'Terhubung' : 'Putus (Siap di-GC)'}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Gen 2 */}
        <div className="bg-slate-950/80 p-3.5 rounded-xl border border-slate-800">
          <div className="flex justify-between items-center mb-2 pb-1.5 border-b border-slate-800">
            <span className="text-xs font-bold text-purple-400">Generation 2</span>
            <span className="text-[10px] text-slate-500">Long-Lived (Singletons)</span>
          </div>
          <div className="space-y-2 min-h-[160px]">
            {objects.filter(o => o.gen === 2 && !o.isLoh).map(obj => (
              <div
                key={obj.id}
                onClick={() => toggleRoot(obj.id)}
                className={`p-2.5 rounded-lg border font-mono text-xs cursor-pointer transition ${
                  obj.hasRoot
                    ? 'bg-purple-950/40 border-purple-600/60 text-purple-200 hover:bg-purple-900/40'
                    : 'bg-red-950/40 border-red-600/60 text-red-300 hover:bg-red-900/40 line-through'
                }`}
                title="Klik untuk memutus/menyambung GC Root"
              >
                <div className="flex justify-between items-center">
                  <span className="font-bold">{obj.name}</span>
                  <span className="text-[10px]">{obj.sizeKb} KB</span>
                </div>
                <div className="text-[10px] text-slate-400 mt-1">
                  Root: {obj.hasRoot ? 'Terhubung' : 'Putus (Siap di-GC)'}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* LOH */}
        <div className="bg-slate-950/80 p-3.5 rounded-xl border border-slate-800">
          <div className="flex justify-between items-center mb-2 pb-1.5 border-b border-slate-800">
            <span className="text-xs font-bold text-blue-400">Large Object Heap (LOH)</span>
            <span className="text-[10px] text-slate-500">&gt;= 85,000 bytes</span>
          </div>
          <div className="space-y-2 min-h-[160px]">
            {objects.filter(o => o.isLoh).map(obj => (
              <div
                key={obj.id}
                onClick={() => toggleRoot(obj.id)}
                className={`p-2.5 rounded-lg border font-mono text-xs cursor-pointer transition ${
                  obj.hasRoot
                    ? 'bg-blue-950/40 border-blue-600/60 text-blue-200 hover:bg-blue-900/40'
                    : 'bg-red-950/40 border-red-600/60 text-red-300 hover:bg-red-900/40 line-through'
                }`}
                title="Klik untuk memutus/menyambung GC Root"
              >
                <div className="flex justify-between items-center">
                  <span className="font-bold">{obj.name}</span>
                  <span className="text-[10px]">{obj.sizeKb} KB</span>
                </div>
                <div className="text-[10px] text-slate-400 mt-1">
                  Root: {obj.hasRoot ? 'Terhubung' : 'Putus (Siap di-GC)'}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* GC Console Output */}
      <div className="bg-black/60 p-3 rounded-lg border border-slate-800 font-mono text-xs text-slate-300">
        <div className="text-[10px] text-slate-500 uppercase tracking-wider mb-1">GC Diagnostic Stream (dotnet-trace/EventPipe):</div>
        {gcLogs.map((log, i) => (
          <div key={i} className={i === 0 ? 'text-emerald-400' : 'text-slate-400'}>{log}</div>
        ))}
      </div>
    </div>
  );
};
