import {
  LevelSummary,
  LevelDetail,
  LessonDetail,
  SearchResult,
  ExecuteResult,
  ChallengeResult,
  QuizSubmitResult,
  DashboardData,
  ProjectSpecification
} from '../types/academy';

const API_BASE = '/api';

export const api = {
  // Curriculum
  async getLevels(): Promise<LevelSummary[]> {
    const res = await fetch(`${API_BASE}/curriculum/levels`);
    if (!res.ok) throw new Error('Gagal mengambil daftar level kurikulum');
    return res.json();
  },

  async getLevelDetail(levelId: string): Promise<LevelDetail> {
    const res = await fetch(`${API_BASE}/curriculum/levels/${levelId}`);
    if (!res.ok) throw new Error(`Gagal mengambil detail level ${levelId}`);
    return res.json();
  },

  async getLessonDetail(lessonId: string): Promise<LessonDetail> {
    const res = await fetch(`${API_BASE}/curriculum/lessons/${lessonId}`);
    if (!res.ok) throw new Error(`Gagal mengambil materi lesson ${lessonId}`);
    return res.json();
  },

  async search(query: string): Promise<SearchResult[]> {
    if (!query || query.trim().length < 2) return [];
    const res = await fetch(`${API_BASE}/curriculum/search?q=${encodeURIComponent(query)}`);
    if (!res.ok) return [];
    return res.json();
  },

  // Code Execution
  async executeCSharp(code: string, input?: string): Promise<ExecuteResult> {
    const res = await fetch(`${API_BASE}/execution/csharp`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ code, input })
    });
    return res.json();
  },

  async submitChallenge(
    challengeId: string,
    code: string,
    singleTestIndex?: number,
    customInput?: string
  ): Promise<ChallengeResult> {
    const res = await fetch(`${API_BASE}/execution/challenge`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ challengeId, code, singleTestIndex, customInput })
    });
    return res.json();
  },

  // Terminal Sandbox
  async executeTerminalCommand(commandLine: string, sessionId: string = 'default-session'): Promise<{ exitCode: number; output: string; currentDirectory: string; executionTimeMs: number }> {
    const res = await fetch(`${API_BASE}/terminal/command`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ commandLine, sessionId })
    });
    return res.json();
  },

  async resetTerminal(sessionId: string = 'default-session'): Promise<void> {
    await fetch(`${API_BASE}/terminal/reset`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ sessionId })
    });
  },

  // Quiz
  async submitQuiz(lessonId: string, answers: Record<string, number>): Promise<QuizSubmitResult> {
    const res = await fetch(`${API_BASE}/quiz/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ lessonId, answers })
    });
    return res.json();
  },

  // Progress & Bookmarks
  async getDashboard(): Promise<DashboardData> {
    const res = await fetch(`${API_BASE}/progress/dashboard`);
    if (!res.ok) throw new Error('Gagal mengambil data dashboard');
    return res.json();
  },

  async markLessonCompleted(lessonId: string): Promise<{ success: boolean; completed: boolean }> {
    const res = await fetch(`${API_BASE}/progress/complete/${lessonId}`, { method: 'POST' });
    return res.json();
  },

  async recordVisit(lessonId: string): Promise<void> {
    try {
      await fetch(`${API_BASE}/progress/visit/${lessonId}`, { method: 'POST' });
    } catch {}
  },

  async toggleBookmark(lessonId: string): Promise<{ success: boolean; bookmarked: boolean }> {
    const res = await fetch(`${API_BASE}/progress/bookmark/${lessonId}`, { method: 'POST' });
    return res.json();
  },

  // Projects
  async getProjects(): Promise<ProjectSpecification[]> {
    const res = await fetch(`${API_BASE}/projects`);
    if (!res.ok) throw new Error('Gagal mengambil daftar proyek enterprise');
    return res.json();
  },

  async getProjectDetail(projectId: string): Promise<ProjectSpecification> {
    const res = await fetch(`${API_BASE}/projects/${projectId}`);
    if (!res.ok) throw new Error(`Gagal mengambil proyek ${projectId}`);
    return res.json();
  }
};
