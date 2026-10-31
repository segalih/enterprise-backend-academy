import React, { useState } from 'react';
import Editor from '@monaco-editor/react';
import {
  Award,
  Play,
  CheckCircle2,
  XCircle,
  AlertTriangle,
  Clock,
  Lightbulb,
  Eye,
  EyeOff,
  RotateCcw,
  Copy,
  Terminal,
  ChevronDown,
  ChevronRight,
  Filter,
  Check,
  Code2,
  Sparkles,
  Info
} from 'lucide-react';
import { Challenge, ChallengeResult } from '../../types/academy';
import { api } from '../../services/api';

interface ChallengeViewProps {
  challenge: Challenge;
}

export const ChallengeView: React.FC<ChallengeViewProps> = ({ challenge }) => {
  const [code, setCode] = useState<string>(challenge.starterCode);
  const [isRunning, setIsRunning] = useState<boolean>(false);
  const [runningTestIndex, setRunningTestIndex] = useState<number | null>(null);
  const [result, setResult] = useState<ChallengeResult | null>(null);
  const [failedAttempts, setFailedAttempts] = useState<number>(0);
  const [showSolution, setShowSolution] = useState<boolean>(false);
  const [showHints, setShowHints] = useState<boolean>(false);
  const [showWhitespaceSymbols, setShowWhitespaceSymbols] = useState<boolean>(false);
  const [failedOnlyFilter, setFailedOnlyFilter] = useState<boolean>(false);
  const [expandedTests, setExpandedTests] = useState<Record<number, boolean>>({});

  // Custom input state
  const [showCustomInput, setShowCustomInput] = useState<boolean>(false);
  const [customInputText, setCustomInputText] = useState<string>('');
  const [customRunOutput, setCustomRunOutput] = useState<{ stdout: string; stderr: string; time: number; error?: string } | null>(null);
  const [copiedInputIdx, setCopiedInputIdx] = useState<number | null>(null);

  const handleSubmitAll = async () => {
    setIsRunning(true);
    setRunningTestIndex(null);
    try {
      const res = await api.submitChallenge(challenge.id, code);
      setResult(res);
      if (!res.passed) {
        setFailedAttempts((prev) => prev + 1);
      }
      if (res.testResults && res.testResults.length > 0) {
        const firstFailed = res.testResults.find((t) => !t.passed);
        setExpandedTests({ [firstFailed ? firstFailed.index : 0]: true });
      }
    } catch (err: any) {
      alert(`Gagal menjalankan pengujian: ${err.message}`);
    } finally {
      setIsRunning(false);
    }
  };

  const handleRunSingleTest = async (testIndex: number) => {
    setIsRunning(true);
    setRunningTestIndex(testIndex);
    try {
      const res = await api.submitChallenge(challenge.id, code, testIndex);
      if (result && result.testResults) {
        // Merge single test result into current results
        const updated = [...result.testResults];
        const singleRes = res.testResults[0];
        if (singleRes) {
          const targetPos = updated.findIndex((t) => t.index === testIndex);
          if (targetPos !== -1) {
            updated[targetPos] = singleRes;
          } else {
            updated.push(singleRes);
          }
          const passedCount = updated.filter((t) => t.passed).length;
          setResult({
            ...result,
            testResults: updated,
            testsPassed: passedCount,
            passed: passedCount === challenge.testCases.length,
            executionTimeMs: res.executionTimeMs
          });
        }
      } else {
        setResult(res);
      }
      setExpandedTests((prev) => ({ ...prev, [testIndex]: true }));
    } catch (err: any) {
      alert(`Gagal menjalankan test case #${testIndex + 1}: ${err.message}`);
    } finally {
      setIsRunning(false);
      setRunningTestIndex(null);
    }
  };

  const handleRunCustomInput = async () => {
    setIsRunning(true);
    setRunningTestIndex(null);
    try {
      const res = await api.submitChallenge(challenge.id, code, undefined, customInputText);
      setCustomRunOutput({
        stdout: res.stdout || res.output || '',
        stderr: res.stderr || '',
        time: res.executionTimeMs,
        error: res.error
      });
    } catch (err: any) {
      alert(`Gagal menjalankan input kustom: ${err.message}`);
    } finally {
      setIsRunning(false);
    }
  };

  const handleCopyInputToCustom = (inputStr: string, idx: number) => {
    setCustomInputText(inputStr);
    setShowCustomInput(true);
    setCopiedInputIdx(idx);
    setTimeout(() => setCopiedInputIdx(null), 2000);
  };

  const handleReset = () => {
    if (window.confirm('Reset kode editor kembali ke kerangka kode awal (Starter Code)?')) {
      setCode(challenge.starterCode);
      setResult(null);
      setCustomRunOutput(null);
    }
  };

  const toggleExpand = (idx: number) => {
    setExpandedTests((prev) => ({ ...prev, [idx]: !prev[idx] }));
  };

  const canUnlockSolution = (result && result.passed) || failedAttempts >= 3;

  // Filter test results if filter is active
  const displayedResults = result?.testResults.filter((t) => (!failedOnlyFilter ? true : !t.passed)) || [];

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-2xl p-4 sm:p-6 shadow-2xl my-8 text-slate-200">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between pb-4 border-b border-slate-800 gap-4 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2.5 bg-amber-500/20 text-amber-400 rounded-xl border border-amber-500/30">
            <Award className="w-6 h-6" />
          </div>
          <div>
            <div className="flex items-center space-x-2">
              <h3 className="text-xl font-bold text-white tracking-tight">{challenge.title}</h3>
              <span
                className={`text-[10px] font-semibold uppercase tracking-wider px-2 py-0.5 rounded-full border ${
                  challenge.difficulty === 'Advanced'
                    ? 'bg-purple-950/80 text-purple-300 border-purple-800'
                    : challenge.difficulty === 'Beginner'
                    ? 'bg-emerald-950/80 text-emerald-300 border-emerald-800'
                    : 'bg-blue-950/80 text-blue-300 border-blue-800'
                }`}
              >
                {challenge.difficulty || 'Intermediate'}
              </span>
            </div>
            <p className="text-xs text-slate-400 mt-0.5">
              Tantangan Kode Roslyn C#: Solusi dieksekusi secara deterministik dengan stdin/stdout dan perbandingan presisi.
            </p>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-2">
          {challenge.hints && challenge.hints.length > 0 && (
            <button
              onClick={() => setShowHints(!showHints)}
              className="px-3 py-1.5 text-xs bg-slate-800 hover:bg-slate-700 text-amber-300 rounded-lg border border-slate-700 transition flex items-center space-x-1.5 shadow-sm"
            >
              <Lightbulb className="w-4 h-4 text-amber-400" />
              <span>{showHints ? 'Tutup Petunjuk' : `Petunjuk (${challenge.hints.length})`}</span>
            </button>
          )}

          <button
            onClick={() => {
              if (!canUnlockSolution) {
                alert('Solusi resmi dapat dibuka setelah Anda menyelesaikan tantangan atau setelah minimal 3 kali percobaan gagal.');
                return;
              }
              setShowSolution(!showSolution);
            }}
            className={`px-3 py-1.5 text-xs rounded-lg border transition flex items-center space-x-1.5 shadow-sm ${
              canUnlockSolution
                ? 'bg-slate-800 hover:bg-slate-700 text-purple-300 border-purple-800/60'
                : 'bg-slate-900/60 text-slate-500 border-slate-800 cursor-not-allowed'
            }`}
            title={canUnlockSolution ? 'Buka solusi acuan resmi' : 'Tersedia setelah 3x gagal atau lulus'}
          >
            {showSolution ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
            <span>{showSolution ? 'Tutup Solusi' : 'Solusi Acuan Resmi'}</span>
          </button>
        </div>
      </div>

      {/* Problem Statement, Formats, Constraints, and Examples */}
      <div className="space-y-4 mb-6">
        <div className="bg-slate-950/80 p-4 rounded-xl border border-slate-800 text-xs leading-relaxed">
          <div className="font-semibold text-slate-300 uppercase tracking-wider text-[11px] mb-1.5 flex items-center space-x-1.5">
            <Info className="w-3.5 h-3.5 text-cyan-400" />
            <span>Deskripsi Masalah</span>
          </div>
          <div className="text-slate-200 whitespace-pre-line">{challenge.problemStatement}</div>
        </div>

        {(challenge.inputFormat || challenge.outputFormat || challenge.constraints) && (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-3 text-xs">
            {challenge.inputFormat && (
              <div className="bg-slate-950 p-3.5 rounded-xl border border-slate-800">
                <div className="font-semibold text-cyan-400 text-[11px] uppercase tracking-wider mb-1">Format Input</div>
                <div className="text-slate-300 whitespace-pre-line">{challenge.inputFormat}</div>
              </div>
            )}
            {challenge.outputFormat && (
              <div className="bg-slate-950 p-3.5 rounded-xl border border-slate-800">
                <div className="font-semibold text-emerald-400 text-[11px] uppercase tracking-wider mb-1">Format Output</div>
                <div className="text-slate-300 whitespace-pre-line">{challenge.outputFormat}</div>
              </div>
            )}
            {challenge.constraints && (
              <div className="bg-slate-950 p-3.5 rounded-xl border border-slate-800">
                <div className="font-semibold text-amber-400 text-[11px] uppercase tracking-wider mb-1">Batasan (Constraints)</div>
                <div className="text-slate-300 whitespace-pre-line font-mono text-[11px]">{challenge.constraints}</div>
              </div>
            )}
          </div>
        )}

        {/* Examples */}
        {challenge.examples && challenge.examples.length > 0 && (
          <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 text-xs">
            <div className="font-semibold text-indigo-400 text-[11px] uppercase tracking-wider mb-2.5 flex items-center space-x-1.5">
              <Sparkles className="w-3.5 h-3.5" />
              <span>Contoh Input & Output</span>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
              {challenge.examples.map((ex, i) => (
                <div key={i} className="bg-slate-900/90 p-3 rounded-lg border border-slate-800/80 font-mono text-xs">
                  <div className="text-slate-400 text-[11px] font-sans font-semibold mb-1">Contoh #{i + 1}</div>
                  <div className="mb-1.5">
                    <span className="text-slate-400 select-none">Input: </span>
                    <span className="text-cyan-300 font-bold">{ex.input}</span>
                  </div>
                  <div className="mb-1.5">
                    <span className="text-slate-400 select-none">Output: </span>
                    <span className="text-emerald-300 font-bold">{ex.output}</span>
                  </div>
                  {ex.explanation && (
                    <div className="text-slate-400 text-[11px] font-sans mt-2 pt-1.5 border-t border-slate-800">
                      <span className="text-slate-300 font-medium">Penjelasan:</span> {ex.explanation}
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Hints Panel */}
      {showHints && challenge.hints && (
        <div className="bg-amber-950/20 border border-amber-900/40 p-4 rounded-xl mb-6 text-xs space-y-2">
          <div className="font-bold text-amber-300 flex items-center space-x-1.5 mb-2">
            <Lightbulb className="w-4 h-4" />
            <span>Petunjuk Bertahap:</span>
          </div>
          {challenge.hints.map((h, i) => (
            <div key={i} className="text-amber-200/90 pl-3 border-l-2 border-amber-600/50 py-0.5">
              <span className="font-semibold mr-1">Langkah {i + 1}:</span> {h}
            </div>
          ))}
        </div>
      )}

      {/* Solution Preview */}
      {showSolution && challenge.solutionCode && (
        <div className="bg-slate-950 p-4 rounded-xl border border-purple-800/60 mb-6">
          <div className="text-xs font-bold text-purple-400 mb-2 flex items-center space-x-1.5">
            <Code2 className="w-4 h-4" />
            <span>Solusi Acuan Terverifikasi:</span>
          </div>
          <pre className="p-3 bg-slate-900 border border-slate-800 rounded-lg font-mono text-xs text-emerald-300 overflow-x-auto leading-relaxed">
            <code>{challenge.solutionCode}</code>
          </pre>
          {challenge.explanation && (
            <div className="text-xs text-slate-300 mt-2 p-2 bg-slate-900/60 rounded border border-slate-800">
              <span className="font-semibold text-purple-300">Pembahasan Arsitektur:</span> {challenge.explanation}
            </div>
          )}
        </div>
      )}

      {/* Code Editor */}
      <div className="rounded-xl border border-slate-800 overflow-hidden mb-6 shadow-inner">
        <div className="bg-slate-950 px-4 py-2.5 border-b border-slate-800 flex flex-wrap justify-between items-center text-xs gap-2">
          <div className="flex items-center space-x-2">
            <span className="font-mono text-slate-400">Solution.cs</span>
            <span className="text-[10px] text-slate-500 bg-slate-900 px-2 py-0.5 rounded border border-slate-800">
              C# 12 / .NET 8
            </span>
          </div>

          <div className="flex items-center space-x-2">
            <button
              onClick={() => setShowCustomInput(!showCustomInput)}
              className={`px-3 py-1 rounded-lg border text-xs transition flex items-center space-x-1 ${
                showCustomInput
                  ? 'bg-cyan-950/80 text-cyan-300 border-cyan-800'
                  : 'bg-slate-900 text-slate-300 border-slate-700 hover:bg-slate-800'
              }`}
            >
              <Terminal className="w-3.5 h-3.5" />
              <span>Input Kustom</span>
            </button>

            <button
              onClick={handleReset}
              className="p-1.5 text-slate-400 hover:text-white rounded hover:bg-slate-800 transition"
              title="Reset ke Starter Code"
            >
              <RotateCcw className="w-3.5 h-3.5" />
            </button>

            <button
              onClick={handleSubmitAll}
              disabled={isRunning}
              className="px-4 py-1.5 bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white font-medium rounded-lg shadow-md flex items-center space-x-1.5 transition text-xs"
            >
              <Play className="w-3.5 h-3.5 fill-current" />
              <span>{isRunning && runningTestIndex === null ? 'Menguji Semua Kasus...' : 'Submit & Test (Semua)'}</span>
            </button>
          </div>
        </div>

        <Editor
          height="340px"
          language="csharp"
          theme="vs-dark"
          value={code}
          onChange={(val) => setCode(val || '')}
          options={{
            minimap: { enabled: false },
            fontSize: 13,
            lineHeight: 20,
            automaticLayout: true,
            tabSize: 4,
            scrollBeyondLastLine: false,
            folding: true
          }}
        />
      </div>

      {/* Custom Input Runner Drawer */}
      {showCustomInput && (
        <div className="bg-slate-950 p-4 rounded-xl border border-cyan-800/60 mb-6 text-xs">
          <div className="flex items-center justify-between pb-2 mb-2 border-b border-slate-800">
            <span className="font-semibold text-cyan-300 flex items-center space-x-1.5">
              <Terminal className="w-4 h-4" />
              <span>Uji dengan Input Kustom (Manual Stdin)</span>
            </span>
            <button
              onClick={handleRunCustomInput}
              disabled={isRunning}
              className="px-3 py-1 bg-cyan-700 hover:bg-cyan-600 disabled:opacity-50 text-white rounded text-xs flex items-center space-x-1 font-medium"
            >
              <Play className="w-3 h-3 fill-current" />
              <span>Jalankan</span>
            </button>
          </div>
          <textarea
            value={customInputText}
            onChange={(e) => setCustomInputText(e.target.value)}
            placeholder="Ketik string input (stdin) yang akan dibaca oleh Console.ReadLine()..."
            className="w-full h-20 p-2.5 bg-slate-900 border border-slate-800 rounded-lg font-mono text-xs text-slate-200 placeholder-slate-500 focus:outline-none focus:border-cyan-600"
          />

          {customRunOutput && (
            <div className="mt-3 pt-3 border-t border-slate-800 space-y-2">
              <div className="flex items-center justify-between text-[11px] text-slate-400">
                <span>Output Eksekusi Manual:</span>
                <span>Durasi: {customRunOutput.time}ms</span>
              </div>
              {customRunOutput.stdout && (
                <div className="p-2.5 bg-slate-900 rounded border border-slate-800 font-mono text-xs text-emerald-300 whitespace-pre-wrap">
                  {customRunOutput.stdout}
                </div>
              )}
              {customRunOutput.stderr && (
                <div className="p-2.5 bg-red-950/40 rounded border border-red-900 font-mono text-xs text-red-300 whitespace-pre-wrap">
                  <div className="text-[10px] uppercase font-bold text-red-400 mb-1">Standard Error (stderr):</div>
                  {customRunOutput.stderr}
                </div>
              )}
              {customRunOutput.error && (
                <div className="p-2.5 bg-amber-950/40 rounded border border-amber-900 font-mono text-xs text-amber-300 whitespace-pre-wrap">
                  {customRunOutput.error}
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* Compilation Diagnostics Alert */}
      {result && result.diagnostics && result.diagnostics.length > 0 && (
        <div className="bg-red-950/40 border border-red-800/80 p-4 rounded-xl mb-6 text-xs font-mono space-y-2">
          <div className="font-bold text-red-400 flex items-center space-x-2">
            <AlertTriangle className="w-4 h-4 shrink-0" />
            <span>Kesalahan Kompilasi Roslyn ({result.diagnostics.length} Error/Warning):</span>
          </div>
          <div className="space-y-1">
            {result.diagnostics.map((d, i) => (
              <div key={i} className="text-red-200 pl-3 border-l-2 border-red-600">
                <span className="text-amber-400 font-bold">Baris {d.line}:{d.column}</span> - {d.message}{' '}
                <span className="text-slate-400">({d.id})</span>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Test Results Output */}
      {result && (
        <div className="bg-slate-950 p-4 sm:p-5 rounded-2xl border border-slate-800 shadow-xl">
          {/* Summary Bar */}
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-3 mb-4 border-b border-slate-800 gap-3 text-xs">
            <div className="flex items-center space-x-3">
              <span className="font-bold text-sm text-white">Hasil Automated Testing</span>
              <span
                className={`px-3 py-1 rounded-full font-bold font-mono text-xs border ${
                  result.passed
                    ? 'bg-emerald-950/80 text-emerald-400 border-emerald-800'
                    : 'bg-red-950/80 text-red-400 border-red-800'
                }`}
              >
                {result.testsPassed} / {result.totalTests} Lolos ({Math.round((result.testsPassed / Math.max(1, result.totalTests)) * 100)}%)
              </span>
              <span className="text-slate-500 text-[11px] flex items-center space-x-1">
                <Clock className="w-3 h-3" />
                <span>{result.executionTimeMs}ms</span>
              </span>
            </div>

            <div className="flex items-center space-x-3">
              <button
                onClick={() => setShowWhitespaceSymbols(!showWhitespaceSymbols)}
                className={`px-2.5 py-1 text-xs rounded border transition flex items-center space-x-1 ${
                  showWhitespaceSymbols
                    ? 'bg-amber-950/60 text-amber-300 border-amber-800'
                    : 'bg-slate-900 text-slate-400 border-slate-800 hover:text-white'
                }`}
                title="Tampilkan tanda spasi ␠ dan newline ↵"
              >
                <span>Tanda ␠ & ↵</span>
              </button>

              <button
                onClick={() => setFailedOnlyFilter(!failedOnlyFilter)}
                className={`px-2.5 py-1 text-xs rounded border transition flex items-center space-x-1 ${
                  failedOnlyFilter
                    ? 'bg-red-950/60 text-red-300 border-red-800'
                    : 'bg-slate-900 text-slate-400 border-slate-800 hover:text-white'
                }`}
              >
                <Filter className="w-3 h-3" />
                <span>Hanya yang Gagal</span>
              </button>
            </div>
          </div>

          {/* Test Case Cards */}
          <div className="space-y-3">
            {displayedResults.map((tc) => {
              const isExpanded = !!expandedTests[tc.index];
              return (
                <div
                  key={tc.index}
                  className={`rounded-xl border transition overflow-hidden ${
                    tc.passed
                      ? 'bg-slate-900/40 border-emerald-900/40'
                      : 'bg-red-950/10 border-red-900/60'
                  }`}
                >
                  {/* Card Header */}
                  <div
                    onClick={() => toggleExpand(tc.index)}
                    className="p-3.5 flex items-center justify-between cursor-pointer hover:bg-slate-800/40 transition select-none"
                  >
                    <div className="flex items-center space-x-3">
                      {tc.passed ? (
                        <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0" />
                      ) : tc.status === 'Timeout' ? (
                        <Clock className="w-5 h-5 text-amber-400 shrink-0" />
                      ) : (
                        <XCircle className="w-5 h-5 text-red-400 shrink-0" />
                      )}

                      <div>
                        <div className="flex items-center space-x-2">
                          <span className="font-semibold text-slate-200 text-xs">{tc.name}</span>
                          <span className="text-[10px] px-1.5 py-0.2 rounded bg-slate-800 text-slate-400 border border-slate-700">
                            {tc.category}
                          </span>
                          {tc.isHidden && (
                            <span className="text-[10px] px-1.5 py-0.2 rounded bg-purple-950/80 text-purple-300 border border-purple-800">
                              Tersembunyi
                            </span>
                          )}
                        </div>
                        <p className="text-[11px] text-slate-400 mt-0.5">{tc.description}</p>
                      </div>
                    </div>

                    <div className="flex items-center space-x-2 text-xs">
                      <span className="text-slate-500 font-mono text-[11px]">{tc.executionTimeMs}ms</span>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleRunSingleTest(tc.index);
                        }}
                        disabled={isRunning}
                        className="px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded text-[11px] border border-slate-700 flex items-center space-x-1"
                        title="Jalankan hanya test case ini"
                      >
                        <Play className="w-3 h-3 fill-current text-emerald-400" />
                        <span>Uji Ini Saja</span>
                      </button>
                      {isExpanded ? <ChevronDown className="w-4 h-4 text-slate-400" /> : <ChevronRight className="w-4 h-4 text-slate-400" />}
                    </div>
                  </div>

                  {/* Card Details (Expanded) */}
                  {isExpanded && (
                    <div className="p-4 bg-slate-950/80 border-t border-slate-800 text-xs space-y-3 font-mono">
                      {/* Input */}
                      {tc.input !== null && tc.input !== undefined && (
                        <div>
                          <div className="flex items-center justify-between text-[11px] text-slate-400 font-sans mb-1">
                            <span>Input (stdin):</span>
                            <button
                              onClick={() => handleCopyInputToCustom(tc.input || '', tc.index)}
                              className="text-cyan-400 hover:text-cyan-300 flex items-center space-x-1"
                            >
                              {copiedInputIdx === tc.index ? <Check className="w-3 h-3" /> : <Copy className="w-3 h-3" />}
                              <span>{copiedInputIdx === tc.index ? 'Tersalin ke Input Kustom' : 'Salin ke Input Kustom'}</span>
                            </button>
                          </div>
                          <pre className="p-2.5 bg-slate-900 rounded border border-slate-800 text-cyan-300 overflow-x-auto whitespace-pre-wrap">
                            {showWhitespaceSymbols && tc.input ? tc.input.replace(/ /g, '␠').replace(/\n/g, '↵\n') : tc.input}
                          </pre>
                        </div>
                      )}

                      {/* Error or Timeout Alert */}
                      {tc.error && (
                        <div className="p-2.5 bg-red-950/30 rounded border border-red-900/60 text-red-300 font-sans text-xs flex items-start space-x-2">
                          <AlertTriangle className="w-4 h-4 text-red-400 shrink-0 mt-0.5" />
                          <div>
                            <span className="font-bold">{tc.status === 'Timeout' ? 'Timeout Error' : 'Galat Pengujian'}:</span>{' '}
                            {tc.error}
                          </div>
                        </div>
                      )}

                      {/* Line ending notice */}
                      {tc.lineEndingMismatch && (
                        <div className="p-2 bg-amber-950/30 rounded border border-amber-900 text-amber-300 font-sans text-[11px]">
                          Perhatian: Terdeteksi perbedaan format akhir baris (CRLF vs LF).
                        </div>
                      )}

                      {/* Expected vs Actual */}
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        <div>
                          <div className="text-[11px] text-slate-400 font-sans mb-1">Ekspektasi (Expected):</div>
                          <pre className="p-2.5 bg-slate-900 rounded border border-slate-800 text-emerald-300 overflow-x-auto whitespace-pre-wrap">
                            {showWhitespaceSymbols ? tc.expectedVisual || tc.expected : tc.expected}
                          </pre>
                        </div>
                        <div>
                          <div className="text-[11px] text-slate-400 font-sans mb-1">Output Aktual (Actual):</div>
                          <pre
                            className={`p-2.5 bg-slate-900 rounded border overflow-x-auto whitespace-pre-wrap ${
                              tc.passed ? 'border-slate-800 text-emerald-300' : 'border-red-800 text-red-300'
                            }`}
                          >
                            {showWhitespaceSymbols ? tc.actualVisual || tc.actual : tc.actual}
                          </pre>
                        </div>
                      </div>

                      {/* Diff Line-by-line visualizer */}
                      {tc.hasDiff && tc.diffLines && tc.diffLines.length > 0 && (
                        <div className="mt-3 pt-3 border-t border-slate-800 font-sans">
                          <div className="font-semibold text-red-400 text-[11px] uppercase tracking-wider mb-2 flex items-center space-x-1">
                            <Info className="w-3.5 h-3.5" />
                            <span>Analisis Perbedaan Output (Diff Breakdown)</span>
                          </div>
                          <div className="space-y-1.5 font-mono text-xs">
                            {tc.diffLines.map((dl, dIdx) => (
                              <div
                                key={dIdx}
                                className={`p-2 rounded border ${
                                  dl.type === 'Equal'
                                    ? 'bg-slate-900/50 border-slate-800 text-slate-400'
                                    : dl.type === 'Modified'
                                    ? 'bg-red-950/40 border-red-800/80 text-red-200'
                                    : dl.type === 'Added'
                                    ? 'bg-amber-950/30 border-amber-800/60 text-amber-200'
                                    : 'bg-purple-950/30 border-purple-800/60 text-purple-200'
                                }`}
                              >
                                <div className="flex items-center justify-between text-[11px] font-sans pb-1 mb-1 border-b border-slate-800/50">
                                  <span className="font-bold">Baris #{dl.lineNumber} ({dl.type})</span>
                                  {dl.note && <span className="text-amber-300 text-[11px]">{dl.note}</span>}
                                </div>
                                {dl.expected !== null && (
                                  <div className="text-emerald-300">
                                    <span className="text-slate-500 font-sans select-none">- EXP: </span>
                                    {showWhitespaceSymbols ? dl.expectedVisual : dl.expected}
                                  </div>
                                )}
                                {dl.actual !== null && (
                                  <div className="text-red-300">
                                    <span className="text-slate-500 font-sans select-none">+ ACT: </span>
                                    {showWhitespaceSymbols ? dl.actualVisual : dl.actual}
                                  </div>
                                )}
                              </div>
                            ))}
                          </div>
                        </div>
                      )}

                      {/* Stderr if present */}
                      {tc.stderr && (
                        <div>
                          <div className="text-[11px] text-red-400 font-sans mb-1">Standard Error (stderr):</div>
                          <pre className="p-2.5 bg-red-950/30 rounded border border-red-900 text-red-300 overflow-x-auto whitespace-pre-wrap">
                            {tc.stderr}
                          </pre>
                        </div>
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};
