import React from 'react';
import { 
  Terminal, 
  Moon, 
  Sun, 
  Flame, 
  Award, 
  BookOpen, 
  Code2, 
  Briefcase, 
  Layers, 
  BarChart3
} from 'lucide-react';

interface HeaderProps {
  currentTab: string;
  onTabChange: (tab: string) => void;
  onSelectLesson?: (lessonId: string) => void;
  darkMode: boolean;
  onToggleDarkMode: () => void;
  streak?: number;
  xp?: number;
}

export const Header: React.FC<HeaderProps> = ({
  currentTab,
  onTabChange,
  darkMode,
  onToggleDarkMode,
  streak = 4,
  xp = 620
}) => {
  const navItems = [
    { id: 'curriculum', label: 'Kurikulum', icon: BookOpen },
    { id: 'playground', label: 'Playground', icon: Code2 },
    { id: 'projects', label: '12 Proyek Enterprise', icon: Briefcase },
    { id: 'simulators', label: 'Visual Simulators', icon: Layers },
    { id: 'terminal', label: 'Sandbox Terminal', icon: Terminal },
    { id: 'dashboard', label: 'Dashboard', icon: BarChart3 },
  ];

  return (
    <header className="sticky top-0 z-50 bg-slate-900/95 dark:bg-slate-950/95 backdrop-blur border-b border-slate-800 text-slate-100">
      <div className="max-w-[1700px] mx-auto px-4 h-16 flex items-center justify-between">
        {/* Logo */}
        <div 
          onClick={() => onTabChange('curriculum')}
          className="flex items-center space-x-2.5 sm:space-x-3 cursor-pointer group min-w-0 sm:shrink-0"
        >
          <div className="w-8 h-8 sm:w-9 sm:h-9 shrink-0 rounded-xl bg-gradient-to-tr from-blue-600 via-indigo-600 to-purple-600 flex items-center justify-center text-white shadow-lg shadow-blue-900/30 font-bold font-mono text-xs sm:text-sm">
            C#
          </div>
          <div className="min-w-0">
            <div className="font-bold text-xs sm:text-sm tracking-tight text-white group-hover:text-blue-400 transition truncate">
              ENTERPRISE BACKEND ACADEMY
            </div>
            <div className="hidden sm:block text-[10px] text-slate-400 font-mono truncate">
              Zero to Enterprise Principal Engineer
            </div>
          </div>
        </div>

        {/* Navigation Tabs */}
        <nav className="hidden lg:flex items-center space-x-1">
          {navItems.map(item => {
            const Icon = item.icon;
            const active = currentTab === item.id;
            return (
              <button
                key={item.id}
                onClick={() => onTabChange(item.id)}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium flex items-center space-x-1.5 transition ${
                  active
                    ? 'bg-blue-600 text-white shadow-md shadow-blue-900/30'
                    : 'text-slate-300 hover:text-white hover:bg-slate-800'
                }`}
              >
                <Icon className="w-3.5 h-3.5" />
                <span>{item.label}</span>
              </button>
            );
          })}
        </nav>

        {/* Right Stats & Theme */}
        <div className="flex items-center space-x-2 sm:space-x-3 shrink-0">
          <div className="hidden sm:flex items-center space-x-1 text-xs font-mono px-2.5 py-1 bg-amber-950/40 border border-amber-900/50 text-amber-300 rounded-lg" title="Learning Streak">
            <Flame className="w-3.5 h-3.5 text-amber-400 fill-current" />
            <span className="font-bold">{streak}d</span>
          </div>

          <div className="hidden sm:flex items-center space-x-1 text-xs font-mono px-2.5 py-1 bg-purple-950/40 border border-purple-900/50 text-purple-300 rounded-lg" title="Total XP">
            <Award className="w-3.5 h-3.5 text-purple-400" />
            <span className="font-bold">{xp} XP</span>
          </div>

          <button
            onClick={onToggleDarkMode}
            className="p-2 text-slate-400 hover:text-white hover:bg-slate-800 rounded-lg transition"
            title="Ganti Mode Gelap / Terang"
          >
            {darkMode ? <Sun className="w-4 h-4 text-amber-400" /> : <Moon className="w-4 h-4" />}
          </button>
        </div>
      </div>
    </header>
  );
};
