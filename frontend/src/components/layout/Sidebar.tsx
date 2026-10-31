import React, { useState } from 'react';
import { 
  ChevronDown, 
  ChevronRight, 
  CheckCircle2, 
  Circle, 
  BookOpen, 
  Cpu, 
  Brain, 
  Terminal, 
  Layers, 
  Zap, 
  Activity, 
  Server, 
  Globe, 
  Database, 
  HardDrive, 
  Network, 
  FastForward, 
  Compass, 
  Share2, 
  ShieldCheck, 
  BarChart2, 
  Award 
} from 'lucide-react';
import { LevelDetail } from '../../types/academy';

interface SidebarProps {
  levels: LevelDetail[];
  currentLessonId: string;
  onSelectLesson: (lessonId: string) => void;
  completedLessonIds: string[];
}

const iconMap: Record<string, React.FC<{ className?: string }>> = {
  Cpu, Brain, Terminal, Layers, Zap, Activity, Server, Globe, Database, HardDrive, Network, FastForward, Compass, Share2, ShieldCheck, CheckCircle2, BarChart2, Award, BookOpen
};

export const Sidebar: React.FC<SidebarProps> = ({
  levels,
  currentLessonId,
  onSelectLesson,
  completedLessonIds
}) => {
  // Keep active level expanded by default
  const [expandedLevels, setExpandedLevels] = useState<Record<string, boolean>>({
    'level-00': true,
    'level-02': true,
    'level-12': true,
    'level-18': true
  });

  const activeLevelMatch = currentLessonId.match(/^lesson-(\d{2})-/);
  const activeLevelId = activeLevelMatch ? `level-${activeLevelMatch[1]}` : '';

  const toggleLevel = (id: string) => {
    setExpandedLevels(prev => {
      const currentVal = prev[id] ?? (id === activeLevelId);
      return { ...prev, [id]: !currentVal };
    });
  };

  return (
    <aside className="hidden lg:block sticky top-16 w-80 shrink-0 bg-slate-950/80 border-r border-slate-800 h-[calc(100vh-4rem)] overflow-y-auto p-3 text-slate-300 select-none">
      <div className="text-[11px] font-mono uppercase tracking-wider text-slate-400 px-3 py-2 font-bold flex items-center justify-between">
        <span>Kurikulum Induk (18 Level)</span>
        <span className="text-[10px] px-2 py-0.5 bg-blue-950 text-blue-300 rounded border border-blue-900/60">
          Zero to Principal
        </span>
      </div>

      <div className="space-y-1 mt-2">
        {levels.map((lvl) => {
          const isExpanded = expandedLevels[lvl.id] ?? (lvl.id === activeLevelId);
          const IconComponent = iconMap[lvl.icon] || BookOpen;
          
          // Calculate completed in this level
          const allLessonIdsInLevel = lvl.modules.flatMap(m => m.lessons.map(l => l.id));
          const completedCount = allLessonIdsInLevel.filter(id => completedLessonIds.includes(id)).length;
          const isLevelFinished = allLessonIdsInLevel.length > 0 && completedCount === allLessonIdsInLevel.length;

          return (
            <div key={lvl.id} className="rounded-lg overflow-hidden">
              {/* Level Accordion Header */}
              <button
                onClick={() => toggleLevel(lvl.id)}
                className={`w-full text-left px-3 py-2.5 rounded-lg flex items-center justify-between transition text-xs font-medium ${
                  isExpanded ? 'bg-slate-900/90 text-white' : 'hover:bg-slate-900/60 text-slate-300'
                }`}
              >
                <div className="flex items-center space-x-2.5 truncate pr-2">
                  <div className={`p-1.5 rounded-md ${isLevelFinished ? 'bg-emerald-600/20 text-emerald-400' : 'bg-slate-800 text-blue-400'}`}>
                    <IconComponent className="w-4 h-4 shrink-0" />
                  </div>
                  <div className="truncate">
                    <div className="font-semibold text-slate-100 truncate text-xs">{lvl.title.split('—')[0].trim()}</div>
                    <div className="text-[10px] text-slate-400 truncate">{lvl.title.split('—')[1]?.trim() || lvl.subtitle}</div>
                  </div>
                </div>

                <div className="flex items-center space-x-1.5 shrink-0">
                  <span className="text-[10px] font-mono text-slate-400">
                    {completedCount}/{allLessonIdsInLevel.length}
                  </span>
                  {isExpanded ? <ChevronDown className="w-3.5 h-3.5 text-slate-400" /> : <ChevronRight className="w-3.5 h-3.5 text-slate-500" />}
                </div>
              </button>

              {/* Modules & Lessons */}
              {isExpanded && (
                <div className="pl-4 pr-1 py-1 space-y-2 mt-1 border-l-2 border-slate-800 ml-5">
                  {lvl.modules.map(mod => (
                    <div key={mod.id} className="space-y-0.5">
                      <div className="text-[10px] font-semibold text-slate-400 uppercase tracking-wider px-2 py-1">
                        {mod.title}
                      </div>
                      <div className="space-y-0.5">
                        {mod.lessons.map(les => {
                          const isCurrent = currentLessonId === les.id;
                          const isDone = completedLessonIds.includes(les.id);

                          return (
                            <button
                              key={les.id}
                              onClick={() => onSelectLesson(les.id)}
                              className={`w-full text-left px-2.5 py-1.5 rounded-md text-xs flex items-center justify-between transition ${
                                isCurrent
                                  ? 'bg-blue-600 text-white font-medium shadow-md shadow-blue-900/40'
                                  : 'hover:bg-slate-900/80 text-slate-300'
                              }`}
                            >
                              <div className="flex items-center space-x-2 truncate pr-1">
                                {isDone ? (
                                  <CheckCircle2 className={`w-3.5 h-3.5 shrink-0 ${isCurrent ? 'text-white' : 'text-emerald-400'}`} />
                                ) : (
                                  <Circle className={`w-3.5 h-3.5 shrink-0 ${isCurrent ? 'text-blue-200' : 'text-slate-600'}`} />
                                )}
                                <span className="truncate">{les.title}</span>
                              </div>

                              {les.simulatorType && (
                                <span className={`text-[9px] px-1 py-0.2 rounded font-mono shrink-0 ml-1 ${
                                  isCurrent ? 'bg-blue-800 text-blue-100' : 'bg-purple-950/80 text-purple-300 border border-purple-900/60'
                                }`}>
                                  Sim
                                </span>
                              )}
                            </button>
                          );
                        })}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </aside>
  );
};
