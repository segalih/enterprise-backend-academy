export interface LevelSummary {
  id: string;
  order: number;
  title: string;
  subtitle: string;
  description: string;
  icon: string;
  moduleCount: number;
  lessonCount: number;
  difficulty: string;
  estimatedHours: number;
  keyTopics: string[];
}

export interface Module {
  id: string;
  levelId: string;
  order: number;
  title: string;
  description: string;
  lessons: LessonSummary[];
}

export interface LevelDetail extends LevelSummary {
  modules: Module[];
}

export interface LessonSummary {
  id: string;
  levelId: string;
  moduleId: string;
  order: number;
  title: string;
  slug: string;
  difficulty: string;
  estimatedMinutes: number;
  hasQuiz: boolean;
  hasExercise: boolean;
  simulatorType?: string;
}

export interface QuizQuestion {
  id: string;
  question: string;
  type: string;
  options: string[];
  correctOptionIndex: number;
  explanation?: string;
  explanationCorrect?: string;
  explanationIncorrect?: string;
  optionExplanations?: string[];
  sectionRef?: string;
  difficulty: string;
  codeSnippet?: string;
}

export interface ChallengeExample {
  input: string;
  output: string;
  explanation: string;
}

export interface ChallengeTestCase {
  name?: string;
  description: string;
  input: string;
  expectedOutput: string;
  isHidden: boolean;
  category?: 'Example' | 'Normal' | 'EdgeCase' | 'Performance';
}

export interface Challenge {
  id: string;
  title: string;
  difficulty?: 'Beginner' | 'Intermediate' | 'Advanced';
  problemStatement: string;
  inputFormat?: string;
  outputFormat?: string;
  constraints?: string;
  examples?: ChallengeExample[];
  starterCode: string;
  solutionCode: string;
  testCases: ChallengeTestCase[];
  hints: string[];
  explanation: string;
}

export interface CrossReference {
  lessonId: string;
  title?: string;
  levelName?: string;
  context?: string;
  reason?: string;
}

export interface LessonDetail extends LessonSummary {
  description: string;
  prerequisites: Array<string | CrossReference>;
  learningObjectives: string[];
  markdownContent: string;
  starterCode: string;
  expectedOutput?: string;
  quizQuestions: QuizQuestion[];
  codingChallenge?: Challenge;
  relatedLessons: CrossReference[];
  officialReferences?: string[];
  references?: Array<{ title: string; url: string; type?: string }>;
  nextLessonId?: string;
  previousLessonId?: string;
}

export interface ExecuteResult {
  success: boolean;
  output: string;
  stdout?: string;
  stderr?: string;
  error?: string;
  diagnostics: Array<{
    line: number;
    column: number;
    message: string;
    severity: string;
    id: string;
  }>;
  executionTimeMs: number;
  timedOut: boolean;
}

export interface DiffLine {
  lineNumber: number;
  type: 'Equal' | 'Modified' | 'Added' | 'Deleted';
  expected?: string | null;
  actual?: string | null;
  expectedVisual?: string | null;
  actualVisual?: string | null;
  note?: string | null;
}

export interface TestCaseResult {
  index: number;
  name: string;
  description: string;
  category: string;
  isHidden: boolean;
  status: 'Passed' | 'Failed' | 'Error' | 'Timeout';
  passed: boolean;
  input?: string | null;
  expected?: string | null;
  actual?: string | null;
  stdout?: string | null;
  stderr?: string | null;
  error?: string | null;
  executionTimeMs: number;
  hasDiff?: boolean;
  lineEndingMismatch?: boolean;
  expectedVisual?: string | null;
  actualVisual?: string | null;
  diffLines?: DiffLine[];
}

export interface ChallengeResult {
  passed: boolean;
  testsPassed: number;
  totalTests: number;
  testResults: TestCaseResult[];
  stdout?: string;
  stderr?: string;
  output: string;
  error?: string;
  diagnostics?: Array<{
    line: number;
    column: number;
    message: string;
    severity: string;
    id: string;
  }>;
  executionTimeMs: number;
  isCustomRun?: boolean;
}

export interface QuizSubmitResult {
  lessonId: string;
  score: number;
  totalQuestions: number;
  correctAnswers: number;
  passed: boolean;
  passingScorePercentage: number;
  xpEarned: number;
  feedback: Array<{
    questionId: string;
    questionText: string;
    isCorrect: boolean;
    selectedOption: number;
    correctOption: number;
    explanation: string;
  }>;
}

export interface DashboardData {
  userId: string;
  displayName: string;
  totalLessonsCompleted: number;
  totalLessons: number;
  overallProgressPercentage: number;
  currentStreak: number;
  bestStreak: number;
  xp: number;
  levelProgress: Array<{
    levelId: string;
    levelTitle: string;
    completedLessons: number;
    totalLessons: number;
    percentage: number;
  }>;
  completedLessonIds: string[];
  bookmarkedLessonIds: string[];
  achievements: Array<{
    code: string;
    title: string;
    description: string;
    icon: string;
    unlocked: boolean;
  }>;
  weakTopics: string[];
  recommendedLessonId: string;
  lastVisitedLessonId?: string;
}

export interface ProjectSpecification {
  id: string;
  order: number;
  title: string;
  subtitle: string;
  difficulty: string;
  overview: string;
  functionalRequirements: string[];
  nonFunctionalRequirements: string[];
  architectureOverview: string;
  databaseSchema: string;
  apiSpecification: string;
  starterCode: string;
  implementationSteps: string[];
  acceptanceCriteria: string[];
  securityReview: string;
  performanceReview: string;
}

export interface SearchResult {
  lessonId: string;
  levelId: string;
  levelTitle: string;
  moduleTitle: string;
  title: string;
  matchContext: string;
  difficulty: string;
}
