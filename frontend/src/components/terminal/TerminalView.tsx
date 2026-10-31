import React, { useState, useRef, useEffect } from 'react';
import { Terminal, RefreshCw } from 'lucide-react';
import { api } from '../../services/api';

export const TerminalView: React.FC = () => {
  const [history, setHistory] = useState<Array<{ command?: string; output: string; dir: string; isError?: boolean }>>([
    {
      output: 'Enterprise Backend Engineering Academy — Dotnet Sandbox Terminal v1.0\nKetik \'help\' untuk melihat daftar perintah yang didukung atau \'dotnet --version\'.',
      dir: '~/workspace'
    }
  ]);
  const [currentInput, setCurrentInput] = useState<string>('');
  const [commandHistory, setCommandHistory] = useState<string[]>([]);
  const [historyIdx, setHistoryIdx] = useState<number>(-1);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [currentDir, setCurrentDir] = useState<string>('~/workspace');

  const bottomRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [history]);

  const executeCommand = async (rawCommand: string) => {
    const cmd = rawCommand.trim();
    if (!cmd) return;

    setCommandHistory(prev => [...prev, cmd]);
    setHistoryIdx(-1);
    setCurrentInput('');
    setIsLoading(true);

    try {
      const res = await api.executeTerminalCommand(cmd);
      if (res.output === '__CLEAR__') {
        setHistory([]);
      } else {
        setHistory(prev => [
          ...prev,
          { command: cmd, output: res.output, dir: currentDir, isError: res.exitCode !== 0 }
        ]);
      }
      if (res.currentDirectory) setCurrentDir(res.currentDirectory);
    } catch (err: any) {
      setHistory(prev => [
        ...prev,
        { command: cmd, output: `Terminal error: ${err.message}`, dir: currentDir, isError: true }
      ]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      await executeCommand(currentInput);
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      if (commandHistory.length === 0) return;
      const nextIdx = historyIdx === -1 ? commandHistory.length - 1 : Math.max(0, historyIdx - 1);
      setHistoryIdx(nextIdx);
      setCurrentInput(commandHistory[nextIdx]);
    } else if (e.key === 'ArrowDown') {
      e.preventDefault();
      if (historyIdx === -1) return;
      const nextIdx = historyIdx + 1;
      if (nextIdx >= commandHistory.length) {
        setHistoryIdx(-1);
        setCurrentInput('');
      } else {
        setHistoryIdx(nextIdx);
        setCurrentInput(commandHistory[nextIdx]);
      }
    }
  };

  const handleReset = async () => {
    try {
      await api.resetTerminal();
      setHistory([
        { output: 'Sandbox workspace di-reset. Project baru disiapkan dengan Program.cs dan App.csproj.', dir: '~/workspace' }
      ]);
      setCurrentDir('~/workspace');
    } catch (err: any) {
      alert(`Gagal me-reset terminal: ${err.message}`);
    }
  };

  return (
    <div className="bg-slate-950 border border-slate-800 rounded-xl overflow-hidden shadow-2xl flex flex-col h-[520px]">
      {/* Terminal Title Bar */}
      <div className="bg-slate-900 px-4 py-2.5 border-b border-slate-800 flex items-center justify-between">
        <div className="flex items-center space-x-2">
          <div className="flex space-x-1.5 mr-2">
            <div className="w-3 h-3 rounded-full bg-red-500/80" />
            <div className="w-3 h-3 rounded-full bg-amber-500/80" />
            <div className="w-3 h-3 rounded-full bg-emerald-500/80" />
          </div>
          <span className="font-mono text-xs text-slate-300 font-semibold flex items-center space-x-1.5">
            <Terminal className="w-3.5 h-3.5 text-blue-400" />
            <span>bash / dotnet-cli-sandbox</span>
          </span>
        </div>

        <div className="flex items-center space-x-2">
          <button
            onClick={() => executeCommand('dotnet --version')}
            className="text-xs px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition"
          >
            dotnet --version
          </button>
          <button
            onClick={handleReset}
            className="text-xs px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition flex items-center space-x-1"
          >
            <RefreshCw className="w-3 h-3" />
            <span>Reset Workspace</span>
          </button>
        </div>
      </div>

      {/* Terminal Body */}
      <div 
        className="flex-1 p-4 font-mono text-xs overflow-y-auto space-y-3 cursor-text text-slate-200"
        onClick={() => inputRef.current?.focus()}
      >
        {history.map((h, i) => (
          <div key={i} className="space-y-1">
            {h.command && (
              <div className="flex items-center space-x-2 text-slate-400">
                <span className="text-emerald-400 font-bold">learner@academy</span>
                <span className="text-slate-500">:</span>
                <span className="text-blue-400">{h.dir}</span>
                <span className="text-slate-500">$</span>
                <span className="text-white font-semibold">{h.command}</span>
              </div>
            )}
            {h.output && (
              <pre className={`whitespace-pre-wrap leading-relaxed pl-2 border-l border-slate-800 ${
                h.isError ? 'text-red-400' : 'text-slate-300'
              }`}>
                {h.output}
              </pre>
            )}
          </div>
        ))}

        {/* Active Command Prompt Input */}
        <div className="flex items-center space-x-2">
          <span className="text-emerald-400 font-bold">learner@academy</span>
          <span className="text-slate-500">:</span>
          <span className="text-blue-400">{currentDir}</span>
          <span className="text-slate-500">$</span>
          <input
            ref={inputRef}
            type="text"
            value={currentInput}
            onChange={e => setCurrentInput(e.target.value)}
            onKeyDown={handleKeyDown}
            disabled={isLoading}
            className="flex-1 bg-transparent border-none outline-none text-white font-mono text-xs focus:ring-0"
            autoFocus
          />
          {isLoading && (
            <div className="w-3 h-3 border-2 border-blue-400 border-t-transparent rounded-full animate-spin" />
          )}
        </div>
        <div ref={bottomRef} />
      </div>
    </div>
  );
};
