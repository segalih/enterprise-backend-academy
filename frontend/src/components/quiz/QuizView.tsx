import React, { useState } from 'react';
import confetti from 'canvas-confetti';
import { HelpCircle, CheckCircle, XCircle, Award, RotateCcw, ArrowRight } from 'lucide-react';
import { QuizQuestion, QuizSubmitResult } from '../../types/academy';
import { api } from '../../services/api';

interface QuizViewProps {
  lessonId: string;
  questions: QuizQuestion[];
  onQuizCompleted?: (result: QuizSubmitResult) => void;
}

export const QuizView: React.FC<QuizViewProps> = ({ lessonId, questions, onQuizCompleted }) => {
  const [answers, setAnswers] = useState<Record<string, number>>({});
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [result, setResult] = useState<QuizSubmitResult | null>(null);

  const selectOption = (questionId: string, optionIndex: number) => {
    if (result) return; // Prevent changing after submission
    setAnswers(prev => ({ ...prev, [questionId]: optionIndex }));
  };

  const handleSubmit = async () => {
    setSubmitting(true);
    try {
      const res = await api.submitQuiz(lessonId, answers);
      setResult(res);
      if (res.passed) {
        confetti({
          particleCount: 100,
          spread: 70,
          origin: { y: 0.6 }
        });
      }
      if (onQuizCompleted) onQuizCompleted(res);
    } catch (err: any) {
      alert(`Gagal mengirimkan kuis: ${err.message}`);
    } finally {
      setSubmitting(false);
    }
  };

  const handleRetry = () => {
    setResult(null);
    setAnswers({});
  };

  const allAnswered = questions.length > 0 && questions.every(q => answers[q.id] !== undefined);

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-xl my-8">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-blue-600/20 text-blue-400 rounded-lg">
            <HelpCircle className="w-5 h-5" />
          </div>
          <div>
            <h3 className="text-lg font-semibold text-white">Evaluasi & Kuis Pemahaman Konsep</h3>
            <p className="text-xs text-slate-400">Ambang kelulusan: 80%. Uji kemampuan analisis dan penerapan arsitektur Anda.</p>
          </div>
        </div>

        {result && (
          <button
            onClick={handleRetry}
            className="px-3 py-1.5 text-xs bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition flex items-center space-x-1"
          >
            <RotateCcw className="w-3.5 h-3.5" />
            <span>Ulangi Kuis</span>
          </button>
        )}
      </div>

      {/* Score Banner when completed */}
      {result && (
        <div className={`p-4 rounded-xl border mb-6 flex items-center justify-between ${
          result.passed
            ? 'bg-emerald-950/40 border-emerald-500/60 text-emerald-200'
            : 'bg-red-950/40 border-red-500/60 text-red-200'
        }`}>
          <div className="flex items-center space-x-3">
            {result.passed ? (
              <Award className="w-8 h-8 text-emerald-400" />
            ) : (
              <XCircle className="w-8 h-8 text-red-400" />
            )}
            <div>
              <div className="text-base font-bold text-white">
                {result.passed ? 'LULUS! Pemahaman Konsep Sangat Baik' : 'Belum Mencapai Nilai Ambang Batas (80%)'}
              </div>
              <div className="text-xs text-slate-300 mt-0.5">
                Skor Anda: <span className="font-bold text-white">{result.score}%</span> ({result.correctAnswers} dari {result.totalQuestions} Benar)
                {result.passed && ` • Mendapatkan +${result.xpEarned} XP!`}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Questions List */}
      <div className="space-y-6">
        {questions.map((q, qIndex) => {
          const qId = q.id;
          const selected = answers[qId];
          const feedback = result?.feedback?.find(f => f.questionId === qId);

          return (
            <div key={qId} className="bg-slate-950 p-5 rounded-xl border border-slate-800/80">
              <div className="flex items-start justify-between mb-3">
                <span className="text-xs font-mono font-bold text-blue-400">
                  PERTANYAAN #{qIndex + 1}
                </span>
                <span className="text-[10px] px-2 py-0.5 rounded bg-slate-800 text-slate-400 font-medium">
                  {q.difficulty || 'Intermediate'}
                </span>
              </div>

              <h4 className="text-sm font-medium text-slate-100 mb-3 leading-relaxed">
                {q.question}
              </h4>

              {q.codeSnippet && (
                <pre className="p-3 bg-slate-900 border border-slate-800 rounded font-mono text-xs text-cyan-300 mb-4 overflow-x-auto">
                  <code>{q.codeSnippet}</code>
                </pre>
              )}

              {/* Options */}
              <div className="space-y-2">
                {q.options.map((opt, optIndex) => {
                  let optStyle = 'bg-slate-900 border-slate-800 text-slate-300 hover:bg-slate-800/80';
                  
                  if (result) {
                    if (optIndex === q.correctOptionIndex) {
                      optStyle = 'bg-emerald-950/60 border-emerald-500/80 text-emerald-200 font-semibold';
                    } else if (selected === optIndex && !feedback?.isCorrect) {
                      optStyle = 'bg-red-950/60 border-red-500/80 text-red-200';
                    } else {
                      optStyle = 'bg-slate-900/40 border-slate-800/50 text-slate-500 opacity-60';
                    }
                  } else if (selected === optIndex) {
                    optStyle = 'bg-blue-600/20 border-blue-500 text-blue-200 font-medium';
                  }

                  return (
                    <button
                      key={optIndex}
                      onClick={() => selectOption(qId, optIndex)}
                      disabled={result !== null}
                      className={`w-full text-left p-3 rounded-lg border text-xs transition flex items-center justify-between ${optStyle}`}
                    >
                      <div className="flex items-center space-x-3">
                        <span className="w-5 h-5 rounded-full flex items-center justify-center font-mono text-[11px] font-bold border border-slate-700 bg-slate-800 text-slate-300">
                          {String.fromCharCode(65 + optIndex)}
                        </span>
                        <span>{opt}</span>
                      </div>
                      {result && optIndex === q.correctOptionIndex && (
                        <CheckCircle className="w-4 h-4 text-emerald-400 shrink-0" />
                      )}
                      {result && selected === optIndex && !feedback?.isCorrect && (
                        <XCircle className="w-4 h-4 text-red-400 shrink-0" />
                      )}
                    </button>
                  );
                })}
              </div>

              {/* Feedback Explanation */}
              {feedback && (
                <div className={`mt-4 p-3.5 rounded-lg border text-xs leading-relaxed ${
                  feedback.isCorrect
                    ? 'bg-emerald-950/20 border-emerald-800/40 text-emerald-300'
                    : 'bg-red-950/20 border-red-800/40 text-red-300'
                }`}>
                  <div className="font-semibold mb-1 flex items-center space-x-1.5">
                    {feedback.isCorrect ? (
                      <>
                        <CheckCircle className="w-3.5 h-3.5 text-emerald-400" />
                        <span>Penjelasan Jawaban Benar:</span>
                      </>
                    ) : (
                      <>
                        <XCircle className="w-3.5 h-3.5 text-red-400" />
                        <span>Pembahasan Analisis Kesalahan:</span>
                      </>
                    )}
                  </div>
                  <p>{feedback.explanation}</p>
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Submit Button */}
      {!result && (
        <div className="mt-6 flex justify-end">
          <button
            onClick={handleSubmit}
            disabled={!allAnswered || submitting}
            className="px-6 py-2.5 bg-blue-600 hover:bg-blue-500 disabled:opacity-50 text-white font-medium text-xs rounded-lg shadow-lg shadow-blue-900/30 flex items-center space-x-2 transition"
          >
            {submitting ? (
              <span>Menilai Jawaban...</span>
            ) : (
              <>
                <span>Kirim Jawaban Kuis</span>
                <ArrowRight className="w-4 h-4" />
              </>
            )}
          </button>
        </div>
      )}
    </div>
  );
};
