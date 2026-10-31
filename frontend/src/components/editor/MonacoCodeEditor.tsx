import React, { useState } from 'react';
import Editor from '@monaco-editor/react';
import { Play, RotateCcw, Copy, Download, Check, Terminal, Clock, AlertCircle } from 'lucide-react';
import { api } from '../../services/api';
import { ExecuteResult } from '../../types/academy';

interface MonacoCodeEditorProps {
  initialCode: string;
  expectedOutput?: string;
  onCodeExecuted?: (result: ExecuteResult) => void;
  height?: string;
}

export const MonacoCodeEditor: React.FC<MonacoCodeEditorProps> = ({
  initialCode,
  onCodeExecuted,
  height = '420px'
}) => {
  const [code, setCode] = useState<string>(initialCode);
  const [inputVal, setInputVal] = useState<string>('');
  const [isRunning, setIsRunning] = useState<boolean>(false);
  const [outputResult, setOutputResult] = useState<ExecuteResult | null>(null);
  const [copied, setCopied] = useState<boolean>(false);
  const [showInput, setShowInput] = useState<boolean>(false);

  const handleRun = async () => {
    setIsRunning(true);
    try {
      const res = await api.executeCSharp(code, showInput ? inputVal : undefined);
      setOutputResult(res);
      if (onCodeExecuted) onCodeExecuted(res);
    } catch (err: any) {
      setOutputResult({
        success: false,
        output: '',
        error: `Gagal berkomunikasi dengan execution engine backend: ${err.message}`,
        diagnostics: [],
        executionTimeMs: 0,
        timedOut: false
      });
    } finally {
      setIsRunning(false);
    }
  };

  const handleReset = () => {
    setCode(initialCode);
    setOutputResult(null);
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(code);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleDownload = () => {
    const blob = new Blob([code], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'Program.cs';
    link.click();
    URL.revokeObjectURL(url);
  };

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-xl overflow-hidden shadow-2xl my-6">
      {/* Editor Header Toolbar */}
      <div className="bg-slate-950 px-4 py-2.5 border-b border-slate-800 flex items-center justify-between">
        <div className="flex items-center space-x-2">
          <div className="flex space-x-1.5 mr-2">
            <div className="w-3 h-3 rounded-full bg-red-500/80" />
            <div className="w-3 h-3 rounded-full bg-amber-500/80" />
            <div className="w-3 h-3 rounded-full bg-emerald-500/80" />
          </div>
          <span className="font-mono text-xs text-slate-300 font-semibold flex items-center space-x-1.5">
            <span>Program.cs</span>
            <span className="text-[10px] text-blue-400 font-normal px-1.5 py-0.5 bg-blue-950/60 rounded border border-blue-900/50">C# 12 / .NET 8</span>
          </span>
        </div>

        <div className="flex items-center space-x-2">
          <button
            onClick={() => setShowInput(!showInput)}
            className={`text-xs px-2.5 py-1 rounded border transition ${
              showInput 
                ? 'bg-blue-600/20 border-blue-500 text-blue-300' 
                : 'bg-slate-800 border-slate-700 text-slate-400 hover:text-slate-200'
            }`}
            title="Sediakan input stdin untuk Console.ReadLine()"
          >
            Stdin Input
          </button>

          <button
            onClick={handleCopy}
            className="p-1.5 text-slate-400 hover:text-slate-200 hover:bg-slate-800 rounded transition"
            title="Salin Kode"
          >
            {copied ? <Check className="w-4 h-4 text-emerald-400" /> : <Copy className="w-4 h-4" />}
          </button>

          <button
            onClick={handleDownload}
            className="p-1.5 text-slate-400 hover:text-slate-200 hover:bg-slate-800 rounded transition"
            title="Download Program.cs"
          >
            <Download className="w-4 h-4" />
          </button>

          <button
            onClick={handleReset}
            className="p-1.5 text-slate-400 hover:text-slate-200 hover:bg-slate-800 rounded transition"
            title="Reset ke Starter Code"
          >
            <RotateCcw className="w-4 h-4" />
          </button>

          <button
            onClick={handleRun}
            disabled={isRunning}
            className="px-4 py-1.5 bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white font-medium text-xs rounded-lg shadow-lg shadow-emerald-900/30 flex items-center space-x-1.5 transition active:scale-95"
          >
            {isRunning ? (
              <>
                <div className="w-3.5 h-3.5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                <span>Compiling...</span>
              </>
            ) : (
              <>
                <Play className="w-3.5 h-3.5 fill-current" />
                <span>Run Code (F5)</span>
              </>
            )}
          </button>
        </div>
      </div>

      {/* Stdin Bar */}
      {showInput && (
        <div className="bg-slate-950/90 px-4 py-2 border-b border-slate-800 flex items-center space-x-3">
          <span className="text-xs font-mono text-slate-400 shrink-0">Stdin Input:</span>
          <input
            type="text"
            value={inputVal}
            onChange={e => setInputVal(e.target.value)}
            placeholder="Masukkan teks input untuk dibaca oleh Console.ReadLine()..."
            className="w-full bg-slate-900 border border-slate-700 rounded px-2.5 py-1 text-xs font-mono text-slate-200 focus:outline-none focus:border-blue-500"
          />
        </div>
      )}

      {/* Monaco Editor */}
      <div className="border-b border-slate-800">
        <Editor
          height={height}
          language="csharp"
          theme="vs-dark"
          value={code}
          onChange={(val) => setCode(val || '')}
          options={{
            minimap: { enabled: false },
            fontSize: 13,
            lineNumbers: 'on',
            scrollBeyondLastLine: false,
            automaticLayout: true,
            tabSize: 4,
            padding: { top: 12, bottom: 12 },
            fontFamily: 'ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace'
          }}
        />
      </div>

      {/* Terminal Output Area */}
      <div className="bg-slate-950 p-4">
        <div className="flex items-center justify-between pb-2 mb-2 border-b border-slate-800/80">
          <div className="flex items-center space-x-2 text-xs font-mono">
            <Terminal className="w-3.5 h-3.5 text-blue-400" />
            <span className="text-slate-300 font-semibold">Console Output (Roslyn Sandbox Execution)</span>
          </div>
          {outputResult && (
            <div className="flex items-center space-x-3 text-xs font-mono">
              <span className="flex items-center space-x-1 text-slate-400">
                <Clock className="w-3 h-3 text-slate-500" />
                <span>{outputResult.executionTimeMs} ms</span>
              </span>
              <span className={`px-2 py-0.5 rounded text-[11px] font-bold ${
                outputResult.success 
                  ? 'bg-emerald-950/80 text-emerald-400 border border-emerald-800' 
                  : 'bg-red-950/80 text-red-400 border border-red-800'
              }`}>
                {outputResult.success ? 'Exit Code 0' : 'Exit Code 1 (Error)'}
              </span>
            </div>
          )}
        </div>

        {outputResult ? (
          <div className="space-y-2">
            {outputResult.error && (
              <div className="p-3 bg-red-950/30 border border-red-800/50 rounded-lg text-xs font-mono text-red-300 whitespace-pre-wrap">
                <div className="flex items-center space-x-1 text-red-400 font-bold mb-1">
                  <AlertCircle className="w-3.5 h-3.5" />
                  <span>Kompilasi / Runtime Error:</span>
                </div>
                {outputResult.error}
              </div>
            )}

            {outputResult.output && (
              <pre className="p-3 bg-slate-900/90 border border-slate-800 rounded-lg text-xs font-mono text-emerald-300 whitespace-pre-wrap overflow-x-auto leading-relaxed">
                <code>{outputResult.output}</code>
              </pre>
            )}

            {outputResult.success && !outputResult.output && (
              <div className="text-xs text-slate-500 italic py-2">
                Program selesai dieksekusi tanpa menghasilkan output konsol.
              </div>
            )}
          </div>
        ) : (
          <div className="text-xs text-slate-500 italic py-3">
            Tekan 'Run Code' untuk mengompilasi dan mengeksekusi C# secara langsung di sandbox backend.
          </div>
        )}
      </div>
    </div>
  );
};
