import React, { useState, useEffect } from 'react';
import { Header } from './components/layout/Header';
import { Sidebar } from './components/layout/Sidebar';
import { RightToc } from './components/layout/RightToc';
import { LessonPage } from './pages/LessonPage';
import { DashboardPage } from './pages/DashboardPage';
import { PlaygroundPage } from './pages/PlaygroundPage';
import { ProjectsPage } from './pages/ProjectsPage';
import { SimulatorsPage } from './pages/SimulatorsPage';
import { TerminalPage } from './pages/TerminalPage';
import { api } from './services/api';
import { LevelDetail, LessonDetail, DashboardData } from './types/academy';

export const App: React.FC = () => {
  const [currentTab, setCurrentTab] = useState<string>('curriculum');
  const [levels, setLevels] = useState<LevelDetail[]>([]);
  const [currentLessonId, setCurrentLessonId] = useState<string>('lesson-02-04');
  const [lessonDetail, setLessonDetail] = useState<LessonDetail | null>(null);
  const [dashboard, setDashboard] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [darkMode, setDarkMode] = useState<boolean>(() => {
    return localStorage.getItem('academy-theme') !== 'light';
  });

  // Load initial levels and dashboard
  useEffect(() => {
    const initialize = async () => {
      try {
        const [lvlSummaries, dash] = await Promise.all([
          api.getLevels(),
          api.getDashboard().catch(err => {
            console.error('Dashboard fetch warning:', err);
            return null;
          })
        ]);

        // Fetch detail for all levels to build full tree
        const fullLevels = await Promise.all(
          lvlSummaries.map(l => api.getLevelDetail(l.id))
        );

        setLevels(fullLevels);
        if (dash) setDashboard(dash);

        // Load last visited history lesson from localStorage or backend dashboard
        const storedHistory = localStorage.getItem('academy-last-lesson');
        const initialId = storedHistory || dash?.lastVisitedLessonId || dash?.recommendedLessonId || 'lesson-00-01';
        setCurrentLessonId(initialId);
        try {
          const lDetail = await api.getLessonDetail(initialId);
          setLessonDetail(lDetail);
        } catch {
          const fallbackDetail = await api.getLessonDetail('lesson-00-01');
          setCurrentLessonId('lesson-00-01');
          setLessonDetail(fallbackDetail);
        }
      } catch (err) {
        console.error('Initialization error:', err);
      } finally {
        setLoading(false);
      }
    };

    initialize();
  }, []);

  // Theme effect
  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('academy-theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('academy-theme', 'light');
    }
  }, [darkMode]);

  const handleSelectLesson = async (lessonId: string) => {
    setCurrentLessonId(lessonId);
    localStorage.setItem('academy-last-lesson', lessonId);
    api.recordVisit(lessonId);
    try {
      const detail = await api.getLessonDetail(lessonId);
      setLessonDetail(detail);
      setCurrentTab('curriculum');
    } catch (err) {
      console.error(err);
    }
  };

  const handleToggleComplete = async (lessonId: string) => {
    try {
      await api.markLessonCompleted(lessonId);
      const dash = await api.getDashboard();
      setDashboard(dash);
    } catch (err) {
      console.error(err);
    }
  };

  const handleRefreshDashboard = async () => {
    try {
      const dash = await api.getDashboard();
      setDashboard(dash);
    } catch (err) {
      console.error(err);
    }
  };

  const handleToggleBookmark = async (lessonId: string) => {
    try {
      await api.toggleBookmark(lessonId);
      const dash = await api.getDashboard();
      setDashboard(dash);
    } catch (err) {
      console.error(err);
    }
  };

  // Extract Markdown Headings for RightToc
  const extractHeadings = (markdown?: string) => {
    if (!markdown) return [];
    const lines = markdown.split('\n');
    const headings: Array<{ id: string; text: string; level: number }> = [];
    let inFence = false;

    const cleanInlineMarkdown = (raw: string) =>
      raw.replace(/[`*_~]/g, '').trim();

    const toSlug = (raw: string) =>
      cleanInlineMarkdown(raw)
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '');

    lines.forEach(line => {
      if (line.trimStart().startsWith('```')) {
        inFence = !inFence;
        return;
      }
      if (inFence) return;

      if (line.startsWith('## ')) {
        const text = cleanInlineMarkdown(line.replace('## ', ''));
        const id = toSlug(text);
        headings.push({ id, text, level: 2 });
      } else if (line.startsWith('### ')) {
        const text = cleanInlineMarkdown(line.replace('### ', ''));
        const id = toSlug(text);
        headings.push({ id, text, level: 3 });
      }
    });

    return headings;
  };

  const headings = React.useMemo(() => extractHeadings(lessonDetail?.markdownContent), [lessonDetail?.markdownContent]);

  const completedLessonIds = dashboard?.completedLessonIds || [];
  const isCurrentCompleted = completedLessonIds.includes(currentLessonId);
  const isCurrentBookmarked = (dashboard?.bookmarkedLessonIds || []).includes(currentLessonId);

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center text-slate-400">
        <div className="w-10 h-10 border-2 border-blue-500 border-t-transparent rounded-full animate-spin mb-4" />
        <div className="font-mono text-sm">Memuat Enterprise Backend Engineering Academy...</div>
      </div>
    );
  }

  return (
    <div className="h-screen overflow-hidden bg-slate-950 text-slate-100 flex flex-col font-sans">
      {/* Header */}
      <Header
        currentTab={currentTab}
        onTabChange={setCurrentTab}
        onSelectLesson={handleSelectLesson}
        darkMode={darkMode}
        onToggleDarkMode={() => setDarkMode(!darkMode)}
        streak={dashboard?.currentStreak}
        xp={dashboard?.xp}
      />

      {/* Main Container */}
      <div className="flex-1 flex overflow-hidden">
        {/* Left Sidebar (visible when in Curriculum tab) */}
        {currentTab === 'curriculum' && (
          <Sidebar
            levels={levels}
            currentLessonId={currentLessonId}
            onSelectLesson={handleSelectLesson}
            completedLessonIds={completedLessonIds}
          />
        )}

        {/* Central View Area */}
        <main id="main-scroll-container" className="flex-1 min-w-0 overflow-y-auto bg-slate-950">
          {currentTab === 'curriculum' && lessonDetail && (
            <LessonPage
              key={lessonDetail.id}
              lesson={lessonDetail}
              onNavigateLesson={handleSelectLesson}
              onLessonCompleted={handleRefreshDashboard}
            />
          )}

          {currentTab === 'dashboard' && (
            <DashboardPage onNavigateLesson={handleSelectLesson} />
          )}

          {currentTab === 'playground' && (
            <PlaygroundPage />
          )}

          {currentTab === 'projects' && (
            <ProjectsPage />
          )}

          {currentTab === 'simulators' && (
            <SimulatorsPage />
          )}

          {currentTab === 'terminal' && (
            <TerminalPage />
          )}
        </main>

        {/* Right TOC (visible when reading a lesson in curriculum) */}
        {currentTab === 'curriculum' && lessonDetail && (
          <RightToc
            headings={headings}
            isBookmarked={isCurrentBookmarked}
            onToggleBookmark={() => handleToggleBookmark(currentLessonId)}
            isCompleted={isCurrentCompleted}
            onToggleComplete={() => handleToggleComplete(currentLessonId)}
            relatedLessons={lessonDetail.relatedLessons || []}
            onSelectLesson={handleSelectLesson}
            officialReferences={(lessonDetail.references && lessonDetail.references.length > 0) ? lessonDetail.references : (lessonDetail.officialReferences || [])}
          />
        )}
      </div>
    </div>
  );
};

export default App;
