import React, { useState, useEffect } from 'react';
import { 
  BarChart3, 
  Flame, 
  Award, 
  BookOpen, 
  ArrowRight, 
  AlertCircle, 
  ShieldCheck 
} from 'lucide-react';
import { api } from '../services/api';
import { DashboardData } from '../types/academy';

interface DashboardPageProps {
  onNavigateLesson: (lessonId: string) => void;
}

export const DashboardPage: React.FC<DashboardPageProps> = ({ onNavigateLesson }) => {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const res = await api.getDashboard();
        setData(res);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, []);

  if (loading || !data) {
    return (
      <div className="flex-1 p-8 text-center text-slate-400">
        <div className="w-8 h-8 border-2 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto mb-3" />
        <div>Memuat ringkasan performa pembelajaran...</div>
      </div>
    );
  }

  return (
    <div className="flex-1 max-w-5xl mx-auto px-6 py-8 text-slate-100">
      {/* Welcome Banner */}
      <div className="bg-gradient-to-r from-blue-900/40 via-indigo-900/30 to-purple-900/30 border border-blue-800/40 rounded-2xl p-6 sm:p-8 mb-8 relative overflow-hidden">
        <div className="relative z-10 max-w-2xl">
          <span className="text-xs font-mono uppercase tracking-wider text-blue-400 font-bold mb-1 block">
            Enterprise Backend Learning Journey
          </span>
          <h1 className="text-2xl sm:text-3xl font-bold text-white mb-2">
            Selamat Datang, {data.displayName}!
          </h1>
          <p className="text-sm text-slate-300 leading-relaxed mb-6">
            Anda sedang menempuh jalur kurikulum dari pemula absolut hingga Enterprise Principal Engineer. Teruskan ritme belajar Anda hari ini.
          </p>
          <button
            onClick={() => onNavigateLesson(data.recommendedLessonId)}
            className="px-5 py-2.5 bg-blue-600 hover:bg-blue-500 text-white font-medium text-xs rounded-xl shadow-lg shadow-blue-900/40 flex items-center space-x-2 transition active:scale-95"
          >
            <span>Lanjutkan Pembelajaran ({data.recommendedLessonId})</span>
            <ArrowRight className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Metrics Row */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-8">
        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4">
          <div className="flex items-center space-x-2 text-slate-400 text-xs mb-1">
            <BookOpen className="w-4 h-4 text-blue-400" />
            <span>Materi Selesai</span>
          </div>
          <div className="text-2xl font-mono font-bold text-white">
            {data.totalLessonsCompleted} <span className="text-xs text-slate-500 font-normal">/ {data.totalLessons}</span>
          </div>
          <div className="text-[11px] text-blue-400 font-semibold mt-1">
            {data.overallProgressPercentage}% Kurikulum
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4">
          <div className="flex items-center space-x-2 text-slate-400 text-xs mb-1">
            <Flame className="w-4 h-4 text-amber-400 fill-current" />
            <span>Learning Streak</span>
          </div>
          <div className="text-2xl font-mono font-bold text-amber-300">
            {data.currentStreak} <span className="text-xs text-slate-500 font-normal">Hari</span>
          </div>
          <div className="text-[11px] text-slate-400 mt-1">
            Rekor Terbaik: {data.bestStreak} Hari
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4">
          <div className="flex items-center space-x-2 text-slate-400 text-xs mb-1">
            <Award className="w-4 h-4 text-purple-400" />
            <span>Total XP Diperoleh</span>
          </div>
          <div className="text-2xl font-mono font-bold text-purple-300">
            {data.xp}
          </div>
          <div className="text-[11px] text-purple-400 mt-1">
            Tingkat: Mid-Senior Engineer
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4">
          <div className="flex items-center space-x-2 text-slate-400 text-xs mb-1">
            <ShieldCheck className="w-4 h-4 text-emerald-400" />
            <span>Badges & Prestasi</span>
          </div>
          <div className="text-2xl font-mono font-bold text-emerald-300">
            {data.achievements.filter(a => a.unlocked).length} <span className="text-xs text-slate-500 font-normal">/ {data.achievements.length}</span>
          </div>
          <div className="text-[11px] text-emerald-400 mt-1">
            Unlocked Masteries
          </div>
        </div>
      </div>

      {/* Level Breakdown Grid */}
      <div className="mb-8">
        <h3 className="text-base font-bold text-white mb-4 flex items-center space-x-2">
          <BarChart3 className="w-4 h-4 text-blue-400" />
          <span>Kemajuan per Level Pembelajaran</span>
        </h3>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
          {data.levelProgress.map((lvl) => {
            const levelNum = lvl.levelId.replace('level-', '');
            const firstLessonInLevel = `lesson-${levelNum}-01`;
            return (
            <div
              key={lvl.levelId}
              onClick={() => onNavigateLesson(firstLessonInLevel)}
              className="bg-slate-900/90 border border-slate-800/90 hover:border-slate-700 p-4 rounded-xl cursor-pointer transition group"
            >
              <div className="flex justify-between items-start mb-2">
                <span className="text-xs font-mono font-bold text-blue-400 group-hover:text-blue-300 transition">
                  {lvl.levelTitle.split('—')[0].trim()}
                </span>
                <span className="text-[11px] font-mono text-slate-400">
                  {lvl.completedLessons}/{lvl.totalLessons}
                </span>
              </div>
              <div className="text-xs font-semibold text-slate-200 mb-3 truncate">
                {lvl.levelTitle.split('—')[1]?.trim()}
              </div>

              {/* Progress Bar */}
              <div className="w-full h-1.5 bg-slate-800 rounded-full overflow-hidden">
                <div
                  className="h-full bg-blue-500 transition-all duration-500"
                  style={{ width: `${lvl.percentage}%` }}
                />
              </div>
            </div>
            );
          })}
        </div>
      </div>

      {/* Achievements Section */}
      <div className="mb-8">
        <h3 className="text-base font-bold text-white mb-4 flex items-center space-x-2">
          <Award className="w-4 h-4 text-purple-400" />
          <span>Lencana Pencapaian Teknik (Achievements)</span>
        </h3>

        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
          {data.achievements.map((ach) => (
            <div
              key={ach.code}
              className={`p-4 rounded-xl border transition ${
                ach.unlocked
                  ? 'bg-slate-900 border-purple-800/50 shadow-lg shadow-purple-950/40'
                  : 'bg-slate-950/50 border-slate-800/60 opacity-40'
              }`}
            >
              <div className="w-9 h-9 rounded-lg bg-purple-950/80 border border-purple-800 text-purple-300 flex items-center justify-center mb-3">
                <Award className="w-5 h-5" />
              </div>
              <div className="font-bold text-xs text-white mb-1">{ach.title}</div>
              <div className="text-[11px] text-slate-400 leading-relaxed">{ach.description}</div>
            </div>
          ))}
        </div>
      </div>

      {/* Weak Topics Warning */}
      {data.weakTopics && data.weakTopics.length > 0 && (
        <div className="p-4 bg-amber-950/20 border border-amber-900/40 rounded-xl text-xs flex items-start space-x-3">
          <AlertCircle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
          <div>
            <span className="font-bold text-amber-300 block mb-1">Rekomendasi Penguatan Topik:</span>
            <p className="text-slate-300 mb-2">
              Berdasarkan hasil kuis sebelumnya, Anda disarankan mengulang konsep berikut untuk memperkuat pemahaman arsitektur:
            </p>
            <div className="flex flex-wrap gap-2">
              {data.weakTopics.map((topic, i) => (
                <span key={i} className="px-2 py-0.5 bg-amber-900/40 text-amber-200 rounded border border-amber-800/50 text-[11px]">
                  {topic}
                </span>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
