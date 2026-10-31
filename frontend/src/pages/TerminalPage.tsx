import React from 'react';
import { Terminal, ShieldAlert, Cpu } from 'lucide-react';
import { TerminalView } from '../components/terminal/TerminalView';

export const TerminalPage: React.FC = () => {
  return (
    <div className="flex-1 max-w-5xl mx-auto px-6 py-8 text-slate-100">
      <div className="flex items-center space-x-3 mb-6 pb-4 border-b border-slate-800">
        <div className="p-2.5 bg-blue-600/20 text-blue-400 rounded-xl">
          <Terminal className="w-6 h-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-white">Interactive Dotnet CLI Sandbox Terminal</h1>
          <p className="text-xs text-slate-400">Lingkungan command line terisolasi untuk melatih perintah dotnet CLI, build, run, dan manipulasi project.</p>
        </div>
      </div>

      <div className="mb-6">
        <TerminalView />
      </div>

      {/* Guide Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="p-4 bg-slate-900 border border-slate-800 rounded-xl text-xs">
          <div className="font-bold text-blue-400 mb-2 flex items-center space-x-1.5">
            <Cpu className="w-4 h-4" />
            <span>Perintah Praktik yang Disarankan:</span>
          </div>
          <ul className="space-y-1.5 font-mono text-slate-300">
            <li><span className="text-emerald-400 font-bold">dotnet --version</span> : Cek versi SDK</li>
            <li><span className="text-emerald-400 font-bold">dotnet --info</span> : Detail environment runtime .NET</li>
            <li><span className="text-emerald-400 font-bold">ls</span> : Melihat daftar file dalam direktori sesi</li>
            <li><span className="text-emerald-400 font-bold">cat Program.cs</span> : Membaca kode C# yang ada di project</li>
            <li><span className="text-emerald-400 font-bold">dotnet build</span> : Mengompilasi project sandbox</li>
            <li><span className="text-emerald-400 font-bold">dotnet run</span> : Menjalankan aplikasi console</li>
          </ul>
        </div>

        <div className="p-4 bg-slate-900 border border-slate-800 rounded-xl text-xs">
          <div className="font-bold text-amber-400 mb-2 flex items-center space-x-1.5">
            <ShieldAlert className="w-4 h-4" />
            <span>Keamanan & Batasan Sandbox:</span>
          </div>
          <p className="text-slate-300 leading-relaxed">
            Terminal ini berjalan dalam folder sesi sementara yang terisolasi. Perintah yang berpotensi membahayakan sistem operasi (seperti penghapusan direktori induk atau skrip tidak dikenal) diblokir secara otomatis oleh policy backend.
          </p>
        </div>
      </div>
    </div>
  );
};
