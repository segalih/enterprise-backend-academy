import React, { useState } from 'react';
import { Database, Search } from 'lucide-react';

export const BTreeSimulator: React.FC = () => {
  const [searchTarget, setSearchTarget] = useState<number>(73);
  const [activePath, setActivePath] = useState<string[]>([]);
  const [mode, setMode] = useState<'seek' | 'scan'>('seek');
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [searchStats, setSearchStats] = useState<{ pagesRead: number; timeMs: number } | null>(null);

  const runIndexSeek = (target: number) => {
    setIsSearching(true);
    setMode('seek');
    setActivePath(['root']);

    setTimeout(() => {
      // Determine branch
      let branch = target < 50 ? 'branch-left' : 'branch-right';
      setActivePath(['root', branch]);

      setTimeout(() => {
        let leaf = target < 25 ? 'leaf-1' : target < 50 ? 'leaf-2' : target < 75 ? 'leaf-3' : 'leaf-4';
        setActivePath(['root', branch, leaf]);
        setIsSearching(false);
        setSearchStats({ pagesRead: 3, timeMs: 0.2 });
      }, 400);
    }, 400);
  };

  const runTableScan = () => {
    setIsSearching(true);
    setMode('scan');
    setActivePath(['leaf-1', 'leaf-2', 'leaf-3', 'leaf-4']);
    setTimeout(() => {
      setIsSearching(false);
      setSearchStats({ pagesRead: 10000, timeMs: 145.8 });
    }, 600);
  };

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-6 border border-slate-800 shadow-xl my-6">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-indigo-600/20 text-indigo-400 rounded-lg">
            <Database className="w-5 h-5" />
          </div>
          <div>
            <h3 className="font-semibold text-lg text-white">Database B+ Tree Index Simulator: Clustered Index Seek vs Table Scan</h3>
            <p className="text-xs text-slate-400">Lihat mengapa indeks database mereduksi pembacaan disk I/O dari ribuan page menjadi 3 lompatan.</p>
          </div>
        </div>

        <div className="flex items-center space-x-2">
          <input
            type="number"
            min="1"
            max="100"
            value={searchTarget}
            onChange={e => setSearchTarget(Number(e.target.value))}
            className="w-20 px-2.5 py-1 text-xs bg-slate-950 border border-slate-700 rounded font-mono text-white text-center"
          />
          <button
            onClick={() => runIndexSeek(searchTarget)}
            disabled={isSearching}
            className="px-3 py-1.5 text-xs bg-indigo-600 hover:bg-indigo-500 disabled:opacity-40 text-white font-medium rounded shadow flex items-center space-x-1"
          >
            <Search className="w-3.5 h-3.5" />
            <span>Index Seek</span>
          </button>
          <button
            onClick={runTableScan}
            disabled={isSearching}
            className="px-3 py-1.5 text-xs bg-amber-600 hover:bg-amber-500 disabled:opacity-40 text-white font-medium rounded shadow"
          >
            Table Scan (No Index)
          </button>
        </div>
      </div>

      {/* Visual Tree */}
      <div className="bg-slate-950 p-6 rounded-xl border border-slate-800 flex flex-col items-center space-y-6 mb-6">
        {/* Root Node */}
        <div className="flex flex-col items-center">
          <div className="text-[10px] text-slate-400 mb-1 font-mono">LEVEL 0: ROOT INDEX PAGE</div>
          <div
            className={`px-6 py-2.5 rounded-lg border font-mono text-xs font-bold transition-all duration-300 ${
              activePath.includes('root')
                ? 'bg-indigo-950/80 border-indigo-400 text-indigo-200 ring-4 ring-indigo-500/20 shadow-lg'
                : 'bg-slate-900 border-slate-700 text-slate-300'
            }`}
          >
            Key Threshold: [ 50 ]
          </div>
        </div>

        <div className="w-48 h-4 border-b border-slate-700 relative">
          <div className="absolute left-0 top-2 w-px h-2 bg-slate-700" />
          <div className="absolute right-0 top-2 w-px h-2 bg-slate-700" />
        </div>

        {/* Level 1: Intermediate Nodes */}
        <div className="flex justify-center space-x-16 sm:space-x-32">
          <div
            className={`px-4 py-2 rounded-lg border font-mono text-xs transition-all duration-300 ${
              activePath.includes('branch-left')
                ? 'bg-indigo-950/80 border-indigo-400 text-indigo-200 ring-4 ring-indigo-500/20'
                : 'bg-slate-900 border-slate-700 text-slate-400'
            }`}
          >
            <div className="text-[10px] text-slate-500">Key &lt; 50</div>
            <div className="font-bold">Split: [ 25 ]</div>
          </div>

          <div
            className={`px-4 py-2 rounded-lg border font-mono text-xs transition-all duration-300 ${
              activePath.includes('branch-right')
                ? 'bg-indigo-950/80 border-indigo-400 text-indigo-200 ring-4 ring-indigo-500/20'
                : 'bg-slate-900 border-slate-700 text-slate-400'
            }`}
          >
            <div className="text-[10px] text-slate-500">Key &gt;= 50</div>
            <div className="font-bold">Split: [ 75 ]</div>
          </div>
        </div>

        {/* Level 2: Leaf Data Pages */}
        <div className="w-full pt-2">
          <div className="text-[10px] text-slate-400 mb-2 font-mono text-center">LEVEL 2: LEAF DATA PAGES (8 KB Disk Storage Pages)</div>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
            {[
              { id: 'leaf-1', label: 'Page #101', range: 'Keys 1 - 24' },
              { id: 'leaf-2', label: 'Page #102', range: 'Keys 25 - 49' },
              { id: 'leaf-3', label: 'Page #103', range: 'Keys 50 - 74' },
              { id: 'leaf-4', label: 'Page #104', range: 'Keys 75 - 100' }
            ].map(leaf => (
              <div
                key={leaf.id}
                className={`p-3 rounded-lg border font-mono text-xs text-center transition-all duration-300 ${
                  activePath.includes(leaf.id)
                    ? mode === 'seek'
                      ? 'bg-emerald-950/70 border-emerald-400 text-emerald-200 ring-4 ring-emerald-500/20 shadow-lg'
                      : 'bg-amber-950/60 border-amber-500 text-amber-200'
                    : 'bg-slate-900 border-slate-800 text-slate-400'
                }`}
              >
                <div className="font-bold">{leaf.label}</div>
                <div className="text-[10px] text-slate-400 mt-1">{leaf.range}</div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Query Stats Comparison */}
      {searchStats && (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div className={`p-4 rounded-lg border ${mode === 'seek' ? 'bg-emerald-950/30 border-emerald-600/50' : 'bg-amber-950/30 border-amber-600/50'}`}>
            <div className="text-xs text-slate-400 uppercase tracking-wider font-semibold">Tipe Eksekusi Terpilih</div>
            <div className="text-xl font-bold font-mono mt-1 text-white">
              {mode === 'seek' ? 'Clustered Index Seek (B+ Tree Traversal)' : 'Full Table Scan (Tanpa Indeks)'}
            </div>
            <div className="text-xs text-slate-300 mt-2">
              {mode === 'seek'
                ? 'Hanya membaca 3 page disk (Root -> Branch -> Leaf). Kompleksitas waktu O(log N).'
                : 'Membaca seluruh page tabel dari awal hingga akhir. Kompleksitas waktu O(N), menghabiskan CPU dan disk bandwidth.'}
            </div>
          </div>

          <div className="p-4 rounded-lg border bg-slate-950/80 border-slate-800 flex justify-around items-center">
            <div className="text-center">
              <div className="text-xs text-slate-400">Pages Read (Disk I/O)</div>
              <div className={`text-2xl font-mono font-bold mt-1 ${mode === 'seek' ? 'text-emerald-400' : 'text-red-400'}`}>
                {searchStats.pagesRead.toLocaleString()} pages
              </div>
            </div>
            <div className="text-center">
              <div className="text-xs text-slate-400">Estimasi Eksekusi</div>
              <div className="text-2xl font-mono font-bold mt-1 text-cyan-400">
                {searchStats.timeMs} ms
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
